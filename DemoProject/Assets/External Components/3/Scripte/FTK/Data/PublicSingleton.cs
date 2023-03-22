
namespace Sandbox.Damian.FTK.Data
{
    public abstract class PublicSingleton<T> : Singleton<T>
        where T : PublicSingleton<T>
    {
        public static new T Instance { get { return Singleton<T>.Instance; } }
        public static new bool TryGetInstance(out T instance) => Singleton<T>.TryGetInstance(out instance);
    }

}
