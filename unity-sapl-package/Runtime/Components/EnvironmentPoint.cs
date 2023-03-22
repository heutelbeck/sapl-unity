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
using Sapl.Internal;
using System;
using UnityEngine;
namespace Sapl.Components
{
    ///<summary>
    /// Used to set a common Environment for all 
    /// Sapl Components on an equal or lower level in
    /// Game Object hierarchy. Can be overridden in Sapl Component.
    ///</summary>
    [AddComponentMenu("Sapl/EnvironmentPoint")]
    public class EnvironmentPoint : MonoBehaviour
    {
        /// <summary>Is raised if the environment changed.</summary>
        public Action<string> EnvironmentChanged { get; set; }

#nullable enable
        [field: Tooltip("The Environment of the SAPL Authorization Subscription.")]
        [field: SerializeField]
        /// <summary>Only for use in unity-inspector. Dont use this in code.</summary>
        private string? environment;
        /// <summary>Use this to set the environment in code.</summary>
        public string? Environment { 
            get => environment; 
            set 
            {   
                if (environment != value && this.enabled)
                {
                    environment = value;
                    EnvironmentChanged.Invoke(environment);
                } 
            } 
        }

        private void Start()
        {
            if(String.IsNullOrEmpty(environment) || String.IsNullOrWhiteSpace(environment))
                Environment = gameObject.name;
        }

        private void OnEnable()
        {
            //register components initially 
            RegisterComponents();
        }

        private void OnDisable()
        {
            UnregisterComponents();
        }

        /// <summary>Gets the responsible environment point for a <see cref="GameObject"/>.</summary>
        /// <param name="go">The <see cref="GameObject"/>.</param>
        /// <returns>
        /// The <see cref="EnvironmentPoint"/> or null if not found.
        /// </returns>
        public static EnvironmentPoint? GetEnvironmentPoint(GameObject go)
        {
            EnvironmentPoint? envPoint = go.GetComponent<EnvironmentPoint>();
            if (envPoint != null && envPoint.enabled)
                return envPoint;

            Transform? parent = go.transform.parent;
            while (parent != null && (envPoint == null || !envPoint.enabled))
            {
                envPoint = parent.GetComponent<EnvironmentPoint>();
                if (envPoint != null && envPoint.enabled)
                    return envPoint;
                parent = parent.parent;
            }
            return null;
        }

        private void RegisterComponents()
        {
            var c = gameObject.GetComponentsInChildren<EnforcementBase>(true);
            foreach (EnforcementBase eb in c)
            {
                eb.SubscribeEnvironmentPoint();
            }
        }

        private void UnregisterComponents()
        {
            var c = gameObject.GetComponentsInChildren<EnforcementBase>(true);
            foreach (EnforcementBase eb in c)
            {
                eb.UnsubscribeEnvironmentPoint(this);
            }
        }
    }
}