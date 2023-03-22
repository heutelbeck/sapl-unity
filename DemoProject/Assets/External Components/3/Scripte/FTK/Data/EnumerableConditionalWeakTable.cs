using System;
using System.Collections;
using System.Collections.Generic;

namespace Sandbox.Damian.FTK.Data
{
    /// <summary>
    /// Acts like a System.Runtime.CompilerServices.ConditionalWeakTable<TKey, TValue> but also implements IEnumerable<KeyValuePair<TKey, TValue>>.
    /// "Enables compilers to dynamically attach object fields to managed objects."
    /// Most actions are O(n), so this table is highly inefficient for a large amount of entries.
    /// If you like to improve it, please go study some black magic.
    /// </summary>
    /// <typeparam name="TKey">The reference type to which the field is attached.</typeparam>
    /// <typeparam name="TValue">The field's type. This must be a reference type.</typeparam>
    public class EnumerableConditionalWeakTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly List<KeyValuePair<WeakReference, TValue>> baseList = new List<KeyValuePair<WeakReference, TValue>>();

        /// <summary>
        /// Adds a key with it's value to the table, iff previously not in it.
        /// Overrides the key's value, iff the key was previously stored.
        /// </summary>
        /// <param name="key">The key to add. key represents the object to which the property is attached.</param>
        /// <param name="value">The key's property value.</param>
        public void Put(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("Key is not allowed to be null!");
            Purge();
            if (TryFindKey(key, out var index))
            {
                var kvp = baseList[index];
                baseList[index] = new KeyValuePair<WeakReference, TValue>(kvp.Key, value);
            } else
            {
                baseList.Add(new KeyValuePair<WeakReference, TValue>(new WeakReference(key), value));
            }
        }

        /// <summary>
        /// Searches for a specified key in the table and returns the corresponding value. If the key does not exist in the table, the method invokes a callback method to create a value that is bound to the specified key.
        /// </summary>
        /// <param name="key">The key to search for. key represents the object to which the property is attached.</param>
        /// <param name="createValueCallback">A delegate to a method that can create a value for the given key. It has a single parameter of type TKey, and returns a value of type TValue.</param>
        /// <returns>The value attached to key, if key already exists in the table; otherwise, the new value returned by the createValueCallback delegate.</returns>
        public TValue GetValue(TKey key, CreateValueCallback createValueCallback)
        {
            if (key == null)
                throw new ArgumentNullException("Key is not allowed to be null!");
            if (createValueCallback == null)
                throw new ArgumentNullException("CreateValueCallback is not allowed to be null!");
            if (TryGetValue(key, out TValue value))
                return value;
            value = createValueCallback(key);
            Put(key, value);
            return value;
        }

        /// <summary>
        /// Removes a key and its value from the table.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>true if the key is found and removed; otherwise, false.</returns>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("Key is not allowed to be null!");
            Purge();
            if (TryFindKey(key, out var index))
            {
                baseList.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears the table.
        /// </summary>
        public void Clear() => baseList.Clear();

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key that represents an object with an attached property.</param>
        /// <param name="value">When this method returns, contains the attached property value. If key is not found, value contains the default value.</param>
        /// <returns>true if key is found; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("Key is not allowed to be null!");
            Purge();
            if (TryFindKey(key, out var index))
            {
                value = baseList[index].Value;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the EnumerableConditionalWeakTable.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the EnumerableConditionalWeakTable.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(new List<KeyValuePair<WeakReference, TValue>>(baseList));

        /// <summary>
        /// Represents a method that creates a non-default value to add as part of a key/value pair to a EnumerableConditionalWeakTable<TKey,TValue> object.
        /// </summary>
        /// <param name="key">The key that belongs to the value to create.</param>
        /// <returns>An instance of a reference type that represents the value to attach to the specified key.</returns>
        public delegate TValue CreateValueCallback(TKey key);

        IEnumerator IEnumerable.GetEnumerator() => _GetEnumerator();
        private IEnumerator _GetEnumerator() => this.GetEnumerator();
        private void Purge()
        {
            for (var index = 0; index < baseList.Count; index++)
            {
                var kvp = baseList[index];
                if (!kvp.Key.IsAlive)
                {
                    baseList.RemoveAt(index);
                    index--;
                }
            }
        }
        private bool TryFindKey(TKey key, out int index)
        {
            for (index = 0; index < baseList.Count; index++)
            {
                var kvp = baseList[index];
                if (kvp.Key.IsAlive)
                {
                    var keyCandidate = (TKey)kvp.Key.Target;
                    if (key.Equals(keyCandidate))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Supports a simple iteration over a EnumerableConditionalWeakTable.
        /// </summary>
        private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly List<KeyValuePair<WeakReference, TValue>> baseList;
            private int index = -1;
            private KeyValuePair<TKey, TValue> current = default;
            internal Enumerator(List<KeyValuePair<WeakReference, TValue>> baseList) => this.baseList = baseList;

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public KeyValuePair<TKey, TValue> Current => current;

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() => baseList.Clear();

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                index++;
                IncremetalPurge();
                if (index < baseList.Count)
                {
                    var kvp = baseList[index];
                    current = new KeyValuePair<TKey, TValue>((TKey)kvp.Key.Target, kvp.Value);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset() => index = -1;

            object IEnumerator.Current => _Current;
            private object _Current => this.Current;
            private void IncremetalPurge()
            {
                var increment = true;
                do
                {
                    increment = index < baseList.Count && !baseList[index].Key.IsAlive;
                    if (increment) baseList.RemoveAt(index);
                }
                while (increment);
            }
        }
    }
}