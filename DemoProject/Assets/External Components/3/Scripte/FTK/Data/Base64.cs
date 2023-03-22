#region IMPORTS
using NeoSmart.Utils;
using System;
using System.Text;
#endregion

namespace Sandbox.Damian.FTK.Data

{
    /// <summary>
    /// This is an utility-class to help work with base64-encoded data.
    /// </summary>
    public static class Base64
    {

        /// <summary>
        /// Encodes an UTF-8 string in base64.
        /// </summary>
        /// <param name="plainData">UTF-8 string to encode</param>
        /// <returns>Base64-encoded string</returns>
        #region Encode
        public static string Encode(string plainData)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainData));
        }
        #endregion


        /// <summary>
        /// Decodes a base64 string to UTF-8.
        /// </summary>
        /// <param name="base64">Base64 string to decode</param>
        /// <returns>Decoded UTF-8 string</returns>
        #region Decode
        public static string Decode(string base64)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }
        #endregion


        /// <summary>
        /// Converts a byte-array to a base64 string.
        /// </summary>
        /// <param name="data">The byte-array to convert</param>
        /// <returns>The resulting base64 string</returns>
        #region FromBytes
        public static string FromBytes(byte[] data)
        {
            return Convert.ToBase64String(data);
        }
        #endregion


        /// <summary>
        /// Converts a base64 string to a byte-array.
        /// </summary>
        /// <param name="base64">The base64 string to convert</param>
        /// <returns>The resulting byte-array</returns>
        #region ToBytes
        public static byte[] ToBytes(string base64)
        {
            return Convert.FromBase64String(base64);
        }
        #endregion


        #region URLEncode
        /// <summary>
        /// Encodes a byte-array in Base64-URL.
        /// </summary>
        /// <param name="data">The byte-array to convert</param>
        /// <returns>The resulting Base64-URL representation</returns>
        public static string URLEncode(byte[] data)
        {
            return UrlBase64.Encode(data);
        }


        /// <summary>
        /// Encodes a string in Base64-URL.
        /// </summary>
        /// <param name="data">The string to encode</param>
        /// <returns>The resulting Base64-URL representation</returns>
        public static string URLEncode(string data)
        {
            return UrlBase64.Encode(Encoding.UTF8.GetBytes(data));
        }
        #endregion


        /// <summary>
        /// Encodes a string in Base64-URL.
        /// </summary>
        /// <param name="data">The string to encode</param>
        /// <returns>The resulting Base64-URL representation as a byte-array</returns>
        #region URLEncodeToBytes
        public static byte[] URLEncodeToBytes(string data)
        {
            return Encoding.UTF8.GetBytes(URLEncode(data));
        }
        #endregion


        /// <summary>
        /// Decodes a Base64-URL string into a byte-array.
        /// </summary>
        /// <param name="base64url">The Base64-URL string to decode</param>
        /// <returns>The resulting byte-array</returns>
        #region URLDecode
        public static byte[] URLDecode(string base64url)
        {
            return UrlBase64.Decode(base64url);
        }
        #endregion


        /// <summary>
        /// Decodes a Base64-URL string.
        /// </summary>
        /// <param name="base64url">The Base64-URL string to decode</param>
        /// <returns>The decodes string</returns>
        #region URLDecodeToString
        public static string URLDecodeToString(string base64url)
        {
            return Encoding.UTF8.GetString(URLDecode(base64url));
        }
        #endregion

    }
}