/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using csharp.sapl.pdp.api;
using Sapl.Internal;
using System;
using UnityEngine;
using Sapl.Internal.Tools;

namespace Sapl.Components
{
    ///<summary>
    /// Used to secure methods or events when they are called.      
    ///</summary>
    [AddComponentMenu("Sapl/EventMethodEnforcement")]
    public class EventMethodEnforcement : EnforcementBase
    {
        /// <summary>
        /// Gets or sets the current <see cref="Decision"/>. 
        /// </summary>
        protected override Decision CurrentDecision
        {
            get => currentDecision;
            set
            {
                if (currentDecision != value)
                {
                    currentDecision = value;
                    if (ExecuteOnDecisionChanged.GetPersistentEventCount() > 0) ExecuteOnDecisionChanged.Invoke(currentDecision);
                }
            }
        }

        //UnityEvents  

        [Header("Executed if last Decision is PERMIT")]
        [SerializeField]
        private OnPermit executeOnPermit = new();
        /// <summary>Method to be executed if last Decision is PERMIT.
        /// Add your event handler here.
        /// </summary>
        public OnPermit ExecuteOnPermit { get => executeOnPermit; }

        
        [Header("Executed if last Decision is NOT PERMIT and no other Methods are specified")]
        [SerializeField]
        private OnNotPermitted executeOnNotPermitted = new();
        /// <summary>Method to be executed if last Decision is NOT PERMIT.</summary>
        public OnNotPermitted ExecuteOnNotPermitted { get => executeOnNotPermitted; }
        
        [Header("Executed if Decision has changed. Returns csharp.sapl.pdp.api.Decision")]
        [SerializeField]
        private OnDecisionChanged executeOnDecisionChanged = new();
        /// <summary>Method to be executed if Decision has changed.</summary>
        /// <returns><see cref="csharp.sapl.pdp.api.Decision"/></returns>
        public OnDecisionChanged ExecuteOnDecisionChanged { get => executeOnDecisionChanged; }
        
        ///<summary>
        /// Uses the <see cref="CurrentDecision">last Decision</see> to decide if <see cref="ExecuteOnPermit"/> or <see cref="ExecuteOnNotPermitted"/>
        /// will be executed. Use this as event handler in your control or call manually.       
        ///</summary>
        public void ExecuteMethod()
        {
            if (!this.enabled) return;

            HandleOnExecutionConstraints();

            if (currentDecision == Decision.PERMIT && (ExecuteOnPermit.GetListenerCount() > 0 || ExecuteOnPermit.GetPersistentEventCount() > 0))
                ExecuteOnPermit.Invoke();

            else if (currentDecision != Decision.PERMIT && (ExecuteOnNotPermitted.GetListenerCount() > 0 || ExecuteOnNotPermitted.GetPersistentEventCount() > 0))
                ExecuteOnNotPermitted.Invoke();

            else if (
                    (ExecuteOnPermit.GetListenerCount() == 0 && ExecuteOnNotPermitted.GetListenerCount() == 0 && ExecuteOnDecisionChanged.GetListenerCount() == 0)
                    &&
                    (ExecuteOnPermit.GetPersistentEventCount() == 0 && ExecuteOnNotPermitted.GetPersistentEventCount() == 0 && ExecuteOnDecisionChanged.GetPersistentEventCount() == 0)
                )
                OnNoHandlersFound();
        }

        ///<summary>
        /// Adds the given tuple of subscription ID and related authorization decision to
        /// this multi-decision.
        /// </summary>
        /// <param name="onPermit"> 
        ///  The Action which is called if last Decision was PERMIT</param>
        /// <param name="onNotPermit">  
        ///  The Action which is called if last Decision was NOT PERMIT.
        ///  Can be null.</param>
        ///  <param name="args">  
        ///  The argument(s) which is/are provided for both methods.
        ///  Can be null.</param>
        public void ExecuteMethodParam(Action<object> onPermit, Action<object> onNotPermit = null, object args = null)
        {
            if (!this.enabled) return;

            HandleOnExecutionConstraints();

            if (currentDecision == Decision.PERMIT)
                onPermit.Invoke(args);

            else if (onNotPermit != null)
                onNotPermit.Invoke(args);

            else
                OnNoHandlersFound();
        }

        ///<summary>
        /// Gets the set action
        ///</summary>
        ///<returns>The action for subscriptions</returns>
        protected override string GetAction()
        {
            string act = "not specified";

            if (ExecuteOnPermit.GetListenerCount() > 0)
                act = "RuntimeAction";

            if (ExecuteOnPermit.GetPersistentEventCount() > 0)
                act = ExecuteOnPermit.GetPersistentMethodName(0);

            if (!String.IsNullOrEmpty(Action) && !String.IsNullOrWhiteSpace(Action)) //ggf. überschreiben mit Wert aus inspector
                act = Action;

            return act;
        }

        private void OnNoHandlersFound()
        {
            Debug.LogWarning(this.GetType().Name + "for " + gameObject.name + ": no handlers found for " + currentDecision);
        }

    }
}