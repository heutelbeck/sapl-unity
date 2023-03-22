namespace Sandbox.Damian.FTK.Data.Serialization
{
    /// <summary>
    /// Interface for JSON-serializable data
    /// </summary>
    public interface IJSONSerializable : ISerializable
    {
        /// <summary>
        /// Encodes the object as JSON.
        /// </summary>
        /// <returns>The corresponding JSON-string</returns>
        string ToJSONString();
    }
}
