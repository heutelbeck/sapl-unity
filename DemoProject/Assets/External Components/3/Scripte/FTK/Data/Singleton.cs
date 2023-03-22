#region IMPORTS
using System;
#endregion

namespace Sandbox.Damian.FTK.Data
{
    /// <summary>
    /// This is a base class for classes that adhere to the Singleton pattern.
    /// </summary>
    /// <typeparam name="T">The type of the derived singleton class</typeparam>
    public abstract class Singleton<T>
        where T : Singleton<T>
    {

        /// <summary>
        /// The Singleton instance of type T.
        /// </summary>
        #region Instance
        protected static T Instance { get { return _instance.Value; } }
        #endregion

        /// <summary>
        /// Tries to get the instance of type T.
        /// </summary>
        /// <param name="instance">The instance of type T, iff it exists.</param>
        /// <returns>Whether the instance of type T exists.</returns>
        #region TryGetInstance
        protected static bool TryGetInstance(out T instance)
        {
            instance = default;
            try
            {
                if (_instance != null && _instance.Value != null)
                {
                    instance = _instance.Value;
                    return true;
                }
            }
            catch { }
            return false;
        }
        #endregion

        #region PRIVATE
        private static readonly Lazy<T> _instance = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
        #endregion

    }
}