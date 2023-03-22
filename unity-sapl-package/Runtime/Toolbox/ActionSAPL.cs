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
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using csharp.sapl.constraint;
using UnityEngine;

namespace Sapl.Components
{
    [AddComponentMenu("Sapl/Action")]
    public class ActionSAPL : MonoBehaviour
    {

        [CanBeNull]
        [SerializeField]
        public string actionName;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public virtual void Enforce(GameObject gameObject, Decision decision)
        {
            return;
        }

        public virtual void EnforceOnAction(AuthorizationPublisher publisher)
        {
            return;
        }
        
    } 
}