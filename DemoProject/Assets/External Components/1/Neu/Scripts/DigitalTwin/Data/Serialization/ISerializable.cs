namespace FTK.SturmMixedReality.Data.Serialization {
    /// <summary>
    /// An interface for serializable data.
    /// </summary>
    public interface ISerializable {
        /// <summary>
        /// Serializes the object into a byte-array.
        /// </summary>
        /// <returns>The corresponding byte-array</returns>
        byte[] SerializeToBytes();
    }
}