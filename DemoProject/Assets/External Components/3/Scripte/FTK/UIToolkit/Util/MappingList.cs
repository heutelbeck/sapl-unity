/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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

