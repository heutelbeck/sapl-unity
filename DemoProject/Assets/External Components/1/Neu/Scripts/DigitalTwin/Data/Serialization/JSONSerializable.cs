#region IMPORTS
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
#endregion

namespace FTK.SturmMixedReality.Data.Serialization {
    /// <summary>
    /// class with methods for Serialization and Deserialization of JSON Data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JSONSerializable<T> where T : JSONSerializable<T>, IJSONSerializable {
        /// <summary>
        /// Serializes Objects to JSON
        /// </summary>
        /// <returns></returns>
        #region ToJSONString
        public string ToJSONString() => JsonConvert.SerializeObject(this, typeof(T), null);
        #endregion

        /// <summary>
        /// Deserializes JSON to Objects
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        #region FromJSONString
        public static T FromJSONString(string json) => JsonConvert.DeserializeObject<T>(json);
        #endregion

        /// <summary>
        /// Serializes Bytes toJSON 
        /// </summary>
        /// <returns></returns>
        #region SerializeToBytes
        public byte[] SerializeToBytes() => Encoding.UTF8.GetBytes(ToJSONString());
        #endregion

        /// <summary>
        /// Deserializes JSON to Collection
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        #region FromJSONCollection
        public static Collection<T> FromJSONCollection(string json) => JsonConvert.DeserializeObject<Collection<T>>(json);
        #endregion

    }
}