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
using Sapl.Internal.PEP;
using Sapl.Internal.PEP.Factory;
using csharp.sapl.pdp.api;
using Newtonsoft.Json.Linq;
using Sapl.Components;
using Sapl.Internal.Enums;
using Sapl.Internal.Interfaces;
using Sapl.Internal.Registry;
using Sapl.Internal.Structs;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sapl.Internal
{
    ///<summary>
    /// Base class for Sapl components.   
    ///</summary>
    public abstract class EnforcementBase : MonoBehaviour, IInspectorProperties
    {
#nullable enable
        private protected AbstractPep? pep;
        private protected SaplRegistry? registry;
        private protected JToken? currentGameObjectSubject;
        EnvironmentPoint? currentEnvironmentPoint;

        private protected Decision currentDecision = Decision.INDETERMINATE;
        /// <summary>
        /// Gets or sets the current <see cref="Decision"/> of <see cref="EnforcementBase"/>. 
        /// </summary>
        protected virtual Decision CurrentDecision
        {
            get => currentDecision;
            set
            {
                if (currentDecision != value)
                {
                    currentDecision = value;
                }
            }
        }

        //Fields

        [Tooltip("Type of Decision: once or stream")]
        [SerializeField]
        private DecisionTypeEnum DecisionType = DecisionTypeEnum.STREAM;

        /// <summary>Only for use in unity-inspector. Dont use this in code.</summary>
        [Tooltip("The Subject of the SAPL Authorization Subscription.")]
        [SerializeField]
        private string? subjectString;
        /// <summary>Use this to set the SubjectString in code.</summary>
        public string? SubjectString {
            get => subjectString;
            set
            {
                if (subjectString != value)
                {
                    subjectString = value;
                    if(pep != null) GetDecision();
                }
            }
        }

        /// <summary>Only for use in unity-inspector. Dont use this in code.</summary>
        [Tooltip("GameObject as Subject")]
        [SerializeField]
        private GameObject? subjectGameObject;
        /// <summary>Use this to set the SubjectGameObject in code.</summary>
        public GameObject? SubjectGameObject
        {
            get => subjectGameObject;

            set
            {
                if ((value != null && currentGameObjectSubject == null) || (value != null && subjectGameObject != value)) //sonst keine initiale Belegung von currentSubject
                {//sonst keine initiale Belegung von currentSubject

                    GameObjectSubjectStruct str = new (value.name, value.tag);
#pragma warning disable S4275 // subjectGameObject is set in inspector; this setter is used to serialize it
                    currentGameObjectSubject = JToken.FromObject(str);
#pragma warning restore S4275 // Getters and setters should access the expected fields
                    if (pep != null) GetDecision();
                }
            }
        }

        /// <summary>Only for use in unity-inspector. Dont use this in code.</summary>
        [Tooltip("The Action of the SAPL Authorization Subscription.")]
        [SerializeField]
        private string? action;
        /// <summary>Use this to set the action in code.</summary>
        public string? Action {
            get => action;
            set
            {
                if (action != value)
                {
                    action = value;
                    if (pep != null) GetDecision();
                }
            }
        }

        /// <summary>Only for use in unity-inspector. Dont use this in code.</summary>
        [field: Tooltip("The Resource of the SAPL Authorization Subscription.")]
        [field: SerializeField]
        private string? resource;
        /// <summary>Use this to set the Resource in code.</summary>
        public string? Resource {
            get => resource;
            set
            {
                if (resource != value)
                {
                    resource = value;
                    if (pep != null) GetDecision();
                }
            }
        }

        /// <summary>Only for use in unity-inspector. Dont use this in code.</summary>
        [field: Tooltip("The Environment of the SAPL Authorization Subscription.")]
        [field: SerializeField]
        private string? environment;
        /// <summary>Use this to set the environment in code.</summary>
        public string? Environment {
            get => environment;
            set
            {
                if (environment != value)
                {
                    environment = value;
                    if (pep != null) GetDecision();
                }
            }
        }
      
        // Start is called before the first frame update
        protected void Start()
        {
            if (Application.isPlaying)
            {
                registry = SaplRegistry.GetRegistry();
                if (registry == null)
                {
                    Debug.LogWarning("SaplRegistry not found");
                }
                else
                {
                    registry.SubjectChanged -= OnPropertyChanged;
                    registry.EnvironmentChanged -= OnPropertyChanged;
                    registry.SubjectChanged += OnPropertyChanged;
                    registry.EnvironmentChanged += OnPropertyChanged;
                }
            }

            AbstractFactory factory = new SaplPEPFactory();
            pep = factory.CreatePEP(gameObject);
            GetDecision();
        }

        protected void OnDestroy()
        {
            if (Application.isPlaying && registry != null)
            {
                registry.SubjectChanged -= OnPropertyChanged;
                registry.EnvironmentChanged -= OnPropertyChanged;
            }
            if (currentEnvironmentPoint != null)
                currentEnvironmentPoint.EnvironmentChanged -= OnPropertyChanged;

            if(pep != null) 
                pep.Unsubscribe();
        }

        internal void SubscribeEnvironmentPoint()
        {
            var newEnvPoint = EnvironmentPoint.GetEnvironmentPoint(gameObject); //get current valid EnvironmentPoint

            if(currentEnvironmentPoint != null)
            {
                if (!currentEnvironmentPoint.Equals(newEnvPoint)) //new EnvironmentPoint instantiated
                {
                    currentEnvironmentPoint.EnvironmentChanged -= OnPropertyChanged;
                    currentEnvironmentPoint = newEnvPoint;
                    if (currentEnvironmentPoint != null)
                        currentEnvironmentPoint.EnvironmentChanged += OnPropertyChanged;
                }
            }
            else
            {
                currentEnvironmentPoint = newEnvPoint;
                if (currentEnvironmentPoint != null)
                    currentEnvironmentPoint.EnvironmentChanged += OnPropertyChanged;
            }
            GetDecision();
        }
        
        internal void UnsubscribeEnvironmentPoint(EnvironmentPoint envPoint)
        {
            envPoint!.EnvironmentChanged -= OnPropertyChanged;
            currentEnvironmentPoint = null;          
            SubscribeEnvironmentPoint();
            //get new valid EnvironmentPoint in case the old was set disabled
        }

        ///<summary>
        /// Routine to check all on-execution-constraints.
        /// Calls <see cref="PolicyEnforcementPoint.HandleOnExecutionBundle"/>
        ///</summary>
        protected virtual void HandleOnExecutionConstraints()
        {
            pep!.HandleOnExecutionBundle();
        }

        ///<summary>
        /// Gets the set resource
        ///</summary>
        ///<returns>The resource for subscriptions</returns>
        protected virtual string GetResource()
        {
            string res = gameObject.name;
            if (!String.IsNullOrEmpty(Resource) && !String.IsNullOrWhiteSpace(Resource)) //prb. override with Resource from inspector
            {
                res = Resource;
            }
            return res;
        }

        ///<summary>
        /// Gets the set action
        ///</summary>
        ///<returns>The action for subscriptions</returns>
        protected abstract string GetAction();

        private void GetDecision()
        {
            if (this.enabled && pep != null) //nullpointerexception if component is not enabled
            {
                pep.GetDecision(DecisionType, GetSubject(), GetAction(), GetResource(), GetEnvironment());
                pep.ExecuteOnDecisionChanged = (authDecision) =>
                {
                    CurrentDecision = authDecision.Decision;
                };
            }
        }

        private string GetEnvironment()
        {
            string env = SceneManager.GetActiveScene().name;
            if (registry != null && registry.CurrentEnvironment != null)
                env = registry.CurrentEnvironment;

            //prb. override with Environment from EnvironmentPoint
            if (currentEnvironmentPoint != null && currentEnvironmentPoint.enabled && !String.IsNullOrEmpty(currentEnvironmentPoint.Environment) && !String.IsNullOrWhiteSpace(currentEnvironmentPoint.Environment))
            {
                env = currentEnvironmentPoint.Environment;
            }
            //prb. override with Environment from inspector
            if (!String.IsNullOrEmpty(Environment) && !String.IsNullOrWhiteSpace(Environment)) 
            { 
                env = Environment;
            }
                return env;
        }

        private JToken GetSubject()
        {
            //if GameObjectSubject is set at startup-time, setter is called too late
            if (subjectGameObject != null && currentGameObjectSubject == null) SubjectGameObject = subjectGameObject;
        
            JToken? subject = null;
            string? sub = null;
            if (registry != null)
                subject = registry.CurrentSubject;

            if (!String.IsNullOrEmpty(SubjectString) && !String.IsNullOrWhiteSpace(SubjectString)) //prb. override with SubjectString from inspector 
                sub = SubjectString;

            if (currentGameObjectSubject != null)//prb. override with GameObjectSubject from inspector
                subject = currentGameObjectSubject;

            if(sub != null)
                subject = JToken.FromObject(sub);

            else if (subject == null && sub == null)
                subject = JToken.FromObject("No Subject found");

            return subject!;
#nullable disable
        }

        private void OnPropertyChanged(string value)
        {
             if(pep != null) GetDecision();
        }

    }
}
