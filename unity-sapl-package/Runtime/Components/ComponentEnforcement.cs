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
using Sapl.Interfaces;
using Sapl.Internal;
using System;
using UnityEngine;
using System.Linq;

namespace Sapl.Components
{
    ///<summary>
    /// Uses the <see cref="CurrentDecision">last Decision</see> to set Components enabled/disabled   
    /// which implement <see cref="IComponentEnforcement"/>    
    ///</summary>
    [AddComponentMenu("Sapl/ComponentEnforcement")]
    public class ComponentEnforcement: EnforcementBase
    {
        /// <summary>Gets or sets the current <see cref="Decision" />.</summary>
        protected override Decision CurrentDecision
        {
            get => currentDecision;
            set
            {
                if (currentDecision != value)
                {
                    currentDecision = value;
                    Execute();
                }
            }
        }

        new void Start()
        {
            base.Start();
            Execute();
        }

        /// <summary>Gets the set action. Default is "Enable Component". Can be overriden by value in inspector</summary>
        /// <returns>The action for subscriptions</returns>
        protected override string GetAction()
        {           
            string act = "Enable Component";

            if (!String.IsNullOrEmpty(Action) && !String.IsNullOrWhiteSpace(Action)) //ggf. überschreiben mit Wert aus inspector
            {
                act = Action;
            }
            return act;
        }

        private void Execute()
        {
            var components = gameObject.GetComponents<IComponentEnforcement>();

            if (components.Length > 0 && CurrentDecision == csharp.sapl.pdp.api.Decision.PERMIT)
            {
                foreach (var c in from component in components
                                  where component is MonoBehaviour
                                  let c = component as MonoBehaviour
                                  select c)
                {
                    c.enabled = true;
                }
            }
            else if(components.Length > 0)
            {
                foreach (var c in from component in components
                                  where component is MonoBehaviour
                                  let c = component as MonoBehaviour
                                  select c)
                {
                    c.enabled = false;
                }
            }
        }
    }
}
