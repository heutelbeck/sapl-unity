using System;
using System.Linq;
using UnityEngine;

namespace FTK.UIToolkit.Util
{
    /// <summary>
    /// An extension-class for providing further functionality to Unity's matricies.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Tries to parse the supplied transformation-string into a matrix.
        /// Valid inputs are strings consiting of 12 or 16 whitespace-separated real numbers.
        /// This method fails iff the input is malformed.
        /// </summary>
        /// <param name="matrix">The Matrix to write the transformation to.</param>
        /// <param name="threeMFTransformString">The string encoding the transformation.</param>
        /// <returns>whether parsing the input was successful.</returns>
        #region TryApplyThreeMFTransformString
        public static bool TryApplyThreeMFTransformString(this Matrix4x4 matrix, string threeMFTransformString)
        {
            var stringComponents = threeMFTransformString.Split(WHITESPACE, StringSplitOptions.RemoveEmptyEntries);
            if (!stringComponents.All(str => float.TryParse(str, out _)))
                return false;
            var components = stringComponents.Select(float.Parse).ToArray();
            //evtl transponieren.
            if (components.Length == 12)
            {
                matrix.m00 = components[0];
                matrix.m01 = components[3];
                matrix.m02 = components[6];
                matrix.m03 = components[9]; //0
                matrix.m10 = components[1];
                matrix.m11 = components[4];
                matrix.m12 = components[7];
                matrix.m13 = components[10]; //0
                matrix.m20 = components[2];
                matrix.m21 = components[5];
                matrix.m22 = components[8];
                matrix.m23 = components[11]; //0
                matrix.m30 = 0f;
                matrix.m31 = 0f;
                matrix.m32 = 0f;
                matrix.m33 = 1f; //1
                return true;
            }
            if (components.Length == 16)
            {
                matrix.m00 = components[0];
                matrix.m01 = components[4];
                matrix.m02 = components[8];
                matrix.m03 = components[12];
                matrix.m10 = components[1];
                matrix.m11 = components[5];
                matrix.m12 = components[9];
                matrix.m13 = components[13];
                matrix.m20 = components[2];
                matrix.m21 = components[6];
                matrix.m22 = components[10];
                matrix.m23 = components[14];
                matrix.m30 = components[3];
                matrix.m31 = components[7];
                matrix.m32 = components[11];
                matrix.m33 = components[15];
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Creates a copy of the current Matrix.
        /// </summary>
        /// <param name="matrix">The Matrix to copy.</param>
        /// <returns>a copy of the Matrix.</returns>
        public static Matrix4x4 Copy(this Matrix4x4 matrix) => new Matrix4x4(matrix.GetColumn(0), matrix.GetColumn(1), matrix.GetColumn(2), matrix.GetColumn(3));

        /// <summary>
        /// Extracts the translation/offset from this Matrix.
        /// </summary>
        /// <param name="matrix">The Matrix to extract the translation from.</param>
        /// <returns>the translation components of the Matrix.</returns>
        #region Translation
        public static Vector3 Translation(this Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }
        #endregion

        #region Internal
        private static readonly char[] WHITESPACE = new char[] { '\n', '\t', ' ', '\r' };
        #endregion
    }
}