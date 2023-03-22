using System.Collections.Generic;

namespace FTK.UIToolkit.Util
{
    public class MappingList<KeyType, ValueType>
    {
        private readonly List<KeyType> keys = new List<KeyType>();
        private readonly Dictionary<KeyType, ValueType> values = new Dictionary<KeyType, ValueType>();
        public int Count { get => keys.Count; }
        public Dictionary<KeyType, ValueType>.KeyCollection Keys { get => values.Keys; }
        public Dictionary<KeyType, ValueType>.ValueCollection Values { get => values.Values; }
        public ValueType this[KeyType key]
        {
            get => values[key];
            set
            {
                if (values.ContainsKey(key))
                    values[key] = value;
                else
                {
                    keys.Add(key);
                    values.Add(key, value);
                }
            }
        }
        public ValueType GetAt(int index) { return values[keys[index]]; }
        public KeyType IdAt(int index) { return keys[index]; }
        public void SetAt(int index, ValueType value) { values[keys[index]] = value; }
        public void Remove(KeyType key)
        {
            keys.Remove(key);
            values.Remove(key);
        }
        public void RemoveAt(int index)
        {
            values.Remove(keys[index]);
            keys.RemoveAt(index);
        }
        public bool Contains(KeyType key) { return values.ContainsKey(key); }
    }
}

