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
using Sapl.Internal.Enums;
using Sapl.Internal.Tools;
using System;
using UnityEngine;

namespace Sapl.Components
{
    ///<summary>
    /// Used to secure whole <see cref="GameObject"/>s.   
    ///</summary>
    [AddComponentMenu("Sapl/GameObjectEnforcement")]
    public class GameObjectEnforcement : Internal.EnforcementBase
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

        /// <summary>Method to be executed if last Decision is PERMIT. To be set in inspector.</summary>
        [Header("Executed if last Decision is PERMIT")]
        [SerializeField]
        private OnPermitMethodsEnum OnPermit = OnPermitMethodsEnum.SetActive;

        /// <summary>Method to be executed if last Decision is NOT PERMIT. To be set in inspector.</summary>
        [Header("Executed if last Decision is NOT PERMIT")]
        [SerializeField]
        private OnNotPermitMethodsEnum OnNotPermit = OnNotPermitMethodsEnum.SetInactive;

        new void Start()
        {
            base.Start();
            Execute();
        }

        ///<summary>
        /// Gets the set action
        ///</summary>
        ///<returns>The action for subscriptions</returns>
        protected override string GetAction()
        {
            string act =  OnPermit.ToString();
            if (!String.IsNullOrEmpty(Action) && !String.IsNullOrWhiteSpace(Action)) //ggf. überschreiben mit Wert aus inspector
                act = Action;

            return act;
        }

        private void Execute()
        {
            if (CurrentDecision == csharp.sapl.pdp.api.Decision.PERMIT)
            {
                if (OnPermit == OnPermitMethodsEnum.DoNothing) 
                    return;

                if (OnPermit != OnPermitMethodsEnum.CustomOnPermit)
                {
                    var onPermit = typeof(EnforcementComponentHelper).GetMethod(OnPermit.ToString(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (onPermit != null) onPermit.Invoke(this,new object[] { gameObject, this.GetType() });
                }
                else
                {
                    var scripts = EnforcementComponentHelper.GetScripts(gameObject);
                    if (scripts.Length > 0)
                    {
                        foreach (var script in scripts)
                            script.OnPermit();
                    }        
                }
            }
            else
            {
                if (OnNotPermit == OnNotPermitMethodsEnum.DoNothing)
                    return;

                if (OnNotPermit != OnNotPermitMethodsEnum.CustomOnNotPermit)
                {
                    var onNotPermit = typeof(EnforcementComponentHelper).GetMethod(OnNotPermit.ToString(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if(onNotPermit != null) onNotPermit.Invoke(this, new object[] {gameObject, this.GetType()});
                }
                else
                {
                    var scripts = EnforcementComponentHelper.GetScripts(gameObject);
                    if (scripts.Length > 0)
                    {
                        foreach (var script in scripts)
                            script.OnNotPermit(); 
                    }                       
                }
            }
        }
    }
}