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
using csharp.sapl.constraint.api;
using csharp.sapl.pdp.api;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Security.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Sapl.Components
{
    [AddComponentMenu("Sapl/Enforcement")]
    public class Enforcement : MonoBehaviour
    {
        [SerializeField]
        Subscription[] subscriptions;

        public AuthorizationProvider provider;
        private RemotePolicyDecisionPoint pdp;
        private ConstraintEnforcementService constraintEnforcementService;
#nullable enable
        private OnDecisionConstraintHandlerBundle? onDecisionBundle;
        private OnExecutionConstraintHandlerBundle? onExecutionBundle;
#nullable disable
        // Start is called before the first frame update
        void Start()
        {
            if (pdp == null)
            {
                pdp = new RemotePolicyDecisionPoint();
            }
            foreach (Subscription subscription in subscriptions)
            {
                var authSubscription = new AuthorizationSubscription((JValue)JToken.FromObject(subscription.subject.subjectName), JValue.CreateString(subscription.action.actionName), JValue.CreateString(subscription.resource.resourceName), JValue.CreateString(subscription.environment.environName));
                AuthorizationPublisher publisher = (AuthorizationPublisher)pdp.Decide(authSubscription);
                Debug.Log("Start");
                if (provider == null)
                {
                    provider = publisher.Provider;
                }
                publisher.PropertyChanged += AuthorizationDecisionOnPropertyChanged;
                publisher.AuthorizationDecision.PropertyChanged +=
                    (sender, args) => DecisionOnPropertyChanged(sender, args, subscription, publisher);
            }
			//RemotePolicyDecisionPoint pdp = new RemotePolicyDecisionPoint();
			//Debug.Log("PDP for " + gameObject.name + " -- Subject: " + subjectName + " - Action: " + actionName + " - Resource: " + resource + " - Environment: " + environment);
			//var authSubscription = new AuthorizationSubscription((JValue)JToken.FromObject(subjectName.subjectName), JValue.CreateString(actionName.actionName), JValue.CreateString(resource.resource), JValue.CreateString(environment.environment));
			//AuthorizationPublisher publisher = (AuthorizationPublisher)pdp.Decide(authSubscription);

			//provider = ((AuthorizationPublisher)publisher).Provider;
			//publisher.PropertyChanged += AuthorizationDecisionOnPropertyChanged;


			List<IRunnableConstraintHandlerProvider> runnableProviderList = new();
            runnableProviderList = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IRunnableConstraintHandlerProvider>().ToList();

			List<IGameObjectConsumerConstraintHandlerProvider> gameObjectConsumerProviderList = new();
            gameObjectConsumerProviderList = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IGameObjectConsumerConstraintHandlerProvider>().ToList();

             List<IJsonConsumerConstraintHandlerProvider> jsonConsumerProviderList = new();
            jsonConsumerProviderList = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IJsonConsumerConstraintHandlerProvider>().ToList();

            constraintEnforcementService = new ConstraintEnforcementService(runnableProviderList, gameObjectConsumerProviderList,
                                    jsonConsumerProviderList);
        }



        private void DecisionOnPropertyChanged(object sender, PropertyChangedEventArgs e, Subscription subscription, AuthorizationPublisher publisher)
        {
            if (sender is AuthorizationDecision decision && e.PropertyName.Equals(nameof(decision.Decision)) && subscription != null)
            {
                foreach (GameObject resource in subscription.resource.resources)
                {
                    subscription.action.Enforce(resource, decision.Decision);
                    subscription.action.EnforceOnAction(publisher);
                }
            }
        }

        private void AuthorizationDecisionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is AuthorizationPublisher publisher && e.PropertyName.Equals(nameof(publisher.AuthorizationDecision)))
            {
                // Obligation Handling
                try
                {
                    constraintEnforcementService.BundleFor(publisher!.AuthorizationDecision, out onDecisionBundle, out onExecutionBundle);


                    // To-Do: Unterscheiden zwischen Obligations und Advices!!!
                    onDecisionBundle.HandleRunnableHandlers();
                    onDecisionBundle.HandleGameObjectConsumerHandlers(gameObject);

                    if (publisher!.AuthorizationDecision.Obligations != null)
                    {
                        onDecisionBundle?.HandleJsonConsumerHandlers(publisher!.AuthorizationDecision.Obligations);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("XX Obligation Excecution failed: " + ex.Message);
                    publisher!.AuthorizationDecision.Decision = Decision.DENY; //has to be shifted to PEP/Publisher
                }

                // Advice Handling
                try
                {
                    if (publisher!.AuthorizationDecision.Advice != null)
                    {
                        onDecisionBundle?.HandleJsonConsumerHandlers(publisher!.AuthorizationDecision.Advice);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("XX Advice Excecution failed: " + ex.Message);
                }

            }
        }
    } 
}
