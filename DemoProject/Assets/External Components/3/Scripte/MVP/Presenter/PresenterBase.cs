using MVP.Model;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MVP.Presenter
{
    public abstract class PresenterBase : MonoBehaviour
    {

        public abstract bool Initialized { get; }

        public abstract void ApplyCondition(Condition condition);

        public void ApplyCondition(string condition) => ApplyCondition(Condition.GetConditionInstance(condition));

        public void SubPresenterClosed(SubPresenter closedPresenter) { }

        public bool TryInvokeMethod(string methodName, params object[] args)
        {
            if (TryGetMethod(methodName, out MethodInfo methodInfo))
            {
                try
                {
                    methodInfo.Invoke(this, parameters: args);
                    return true;
                }
                catch { }
            }
            return false;
        }

        public bool TryInvokeMethod<ReturnType>(string methodName, out ReturnType returnValue, params object[] args)
        {
            if (TryGetMethod(methodName, out MethodInfo methodInfo) && methodInfo.ReturnType == typeof(ReturnType))
            {
                try
                {
                    returnValue = (ReturnType)methodInfo.Invoke(this, parameters: args);
                    return true;
                }
                catch { }
            }
            returnValue = default(ReturnType);
            return false;
        }

        public bool TryGetMethod(string methodName, out MethodInfo methodInfo)
        {
            methodInfo = GetType().GetMethod(methodName);
            return methodInfo != null;
        }

        public MethodInfo[] GetMethods()
        {
            return GetType().GetMethods();
        }

        public string[] GetMethodNames()
        {
            return (from method in GetMethods()
                    select method.Name).ToArray();
        }

        protected void ScheduleOnUpdate(Action action) => scheduledActions.Enqueue(action);

        private ConcurrentQueue<Action> scheduledActions = new ConcurrentQueue<Action>();

        private void Update()
        {
            var count = scheduledActions.Count;
            for (int c = 0; c < count; c++)
                if (scheduledActions.TryDequeue(out var action))
                    action.Invoke();
        }
    }

}