using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sandbox.Damian.FTK.Data
{
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static T m_Instance;
        private static Queue<OnUpdateAction> consumerQueue = new Queue<OnUpdateAction>();

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_ShuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed. Returning null.");
                    return null;
                }

                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {
                        // Search for existing instance.
                        m_Instance = (T)FindObjectOfType(typeof(T));

                        // Create new instance if one doesn't already exist.
                        if (m_Instance == null)
                        {
                            // Need to create a new GameObject to attach the singleton to.
                            var singletonObject = new GameObject();
                            m_Instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return m_Instance;
                }
            }
        }

        public delegate void DelegateConsumer(T instance, params object[] args);

        public static void GetInstanceOnUpdate(DelegateConsumer callback, params object[] args)
        {
            Debug.Log("enqueueing...");
            lock (m_Lock)
            {
                consumerQueue.Enqueue(new OnUpdateAction(callback, parameters:args));
            }
        }

        public static void GetInstanceOnUpdate(Action<T> callback)
        {
            Debug.Log("enqueueing...");
            lock (m_Lock)
            {
                consumerQueue.Enqueue(new OnUpdateAction(callback));
            }
        }

        public static Task<T> GetInstanceOnUpdateAsync()
        {
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            GetInstanceOnUpdate(result => completionSource.SetResult(result));
            return completionSource.Task;
        }
        private void Update()
        {
            while (true)
            {
                if (ConsumersInQueue())
                {
                    Debug.Log("dequeueing...");
                    OnUpdateAction act;
                    lock (m_Lock)
                        act = consumerQueue.Dequeue();
                    if (act.IsAction) act.Action.Invoke(Instance);
                    else act.Consumer(Instance, args:act.Args);
                }
                else return;
            }
        }

        private bool ConsumersInQueue()
        {
            lock (m_Lock)
                return consumerQueue.Count > 1;
        }

        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }


        private void OnDestroy()
        {
            m_ShuttingDown = true;
        }

        private readonly struct OnUpdateAction
        {
            public OnUpdateAction(Action<T> action)
            {
                Consumer = null;
                Args = null;
                Action = action;
                IsAction = true;
            }
            public OnUpdateAction(DelegateConsumer consumer, params object[] parameters)
            {
                Consumer = consumer;
                Args = parameters;
                Action = null;
                IsAction = false;
            }
            public readonly bool IsAction;
            public readonly Action<T> Action;
            public readonly DelegateConsumer Consumer;
            public readonly object[] Args;
        }
    }
}