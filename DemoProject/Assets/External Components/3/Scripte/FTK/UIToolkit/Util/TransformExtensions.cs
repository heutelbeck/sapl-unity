using UnityEngine;

namespace FTK.UIToolkit.Util
{
    /// <summary>
    /// An extension-class providing further functionality to the Unity Transform
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Attempts to extract the local position, rotation and scale from the supplied matrix and overrides the local Transform.
        /// Although the localScale of the Transform is overridden, the function might not be able to accuratly recreate the local scale from the supplied matrix, if e.g. the matrix encodes sheering or projection.
        /// </summary>
        /// <param name="transform">The Transform to override.</param>
        /// <param name="matrix">The matrix to extract the new local Transform from.</param>
        #region ApplyLocalMatrix
        public static void ApplyLocalMatrix(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.lossyScale;
            transform.rotation = matrix.rotation;
            transform.position = matrix.Translation();
        }
        #endregion

        /// <summary>
        /// Converts the local Transform into a matrix.
        /// The resulting matrix losslessly encodes the local scale of the Transform and can be used to recreate the local Transform using the extension ApplyLocalMatrix().
        /// </summary>
        /// <param name="transform">The Transform from which to extract it's local matrix.</param>
        /// <returns>a matrix encoding the local position, local rotation and local scale of the Transform.</returns>
        #region GetLocalMatrix
        public static Matrix4x4 GetLocalMatrix(this Transform transform)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(transform.localPosition, transform.localRotation, transform.localScale);
            return matrix;
        }
        #endregion
    }
}