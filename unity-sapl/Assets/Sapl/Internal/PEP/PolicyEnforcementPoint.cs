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
using Sapl.Internal.Enums;
using Sapl.Internal.Security.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sapl.Internal.PEP
{
    /// <summary>Handles the Connection to Sapl-backend.</summary>
    class PolicyEnforcementPoint : AbstractPep
    {

#nullable enable
        private readonly GameObject gameObject;
        private readonly ConstraintEnforcementService? constraintEnforcementService;
        private JArray constraintsOfLastDecision = new();

       

        internal PolicyEnforcementPoint(GameObject gameObject):base()
        {
            this.gameObject = gameObject;

			List<IRunnableConstraintHandlerProvider> runnableProviderList = 
                    UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IRunnableConstraintHandlerProvider>().ToList();
			List<IGameObjectConsumerConstraintHandlerProvider> gameObjectConsumerProviderList =
                    UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IGameObjectConsumerConstraintHandlerProvider>().ToList();
			List<IJsonConsumerConstraintHandlerProvider> jsonConsumerProviderList =
                    UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IJsonConsumerConstraintHandlerProvider>().ToList();
			List<IBoolFunctionConstraintHandlerProvider> boolFunctionProviderList =
                    UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IBoolFunctionConstraintHandlerProvider>().ToList();

            constraintEnforcementService = new(runnableProviderList, gameObjectConsumerProviderList, jsonConsumerProviderList, boolFunctionProviderList);
        }

        /// <summary>Creates a <see cref="AuthorizationSubscription"/> and gets the <see cref="AuthorizationDecision"/>.</summary>
        /// <param name="endpoint"><see cref="DecisionTypeEnum.ONCE"/> or <see cref="DecisionTypeEnum.STREAM"/></param>
        /// <param name="subject">The subject.</param>
        /// <param name="action">The action.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="environment">The environment.</param>
        internal override void GetDecision(DecisionTypeEnum endpoint, JToken subject, string action,
            string resource, string environment = "")
        {
            var authSubscription = new AuthorizationSubscription(subject, JToken.FromObject(action),
                JToken.FromObject(resource), JToken.FromObject(environment));

            if (authorizationPublisher != null)
            {
                var oldSub = authorizationPublisher.Subscription;
                if (oldSub.Subject.Equals(authSubscription.Subject) && oldSub.Action.Equals(authSubscription.Action)
                    && oldSub.Resource.Equals(authSubscription.Resource) && oldSub.Environment.Equals(authSubscription.Environment))
                        return;

                authorizationPublisher.Unsubscribe();
            }

            Debug.Log(gameObject.name + ": SaplConnection is trying to get a decision: " + "Subject: " + subject +
                " - Action: " + action + " - Resource: " + resource + " - Environment: " + environment);

            switch (endpoint)
            {
                case DecisionTypeEnum.STREAM:
                    {
                        authorizationPublisher = (AuthorizationPublisher)pdp.Decide(authSubscription);
                        break;
                    }
                case DecisionTypeEnum.ONCE:
                    {
                        authorizationPublisher = (AuthorizationPublisher)pdp.DecideOnce(authSubscription);
                        break;
                    }
            }
            authorizationPublisher!.PropertyChanged += AuthDecisionPublisher_PropertyChanged;
        }

        ///<summary>
        /// Routine to check all on-execution-constraints
        ///</summary>
        internal override void HandleOnExecutionBundle()
        {
            Decision decisionbeforeConstraintHandling = authorizationPublisher!.AuthorizationDecision.Decision;
            // Obligation/Advice Handling on Execution
            try
            {
                onExecutionBundle?.HandleRunnableHandlers();
                onExecutionBundle?.HandleGameObjectConsumerHandlers(gameObject);
                onExecutionBundle?.HandleJsonConsumerHandlers(constraintsOfLastDecision);

                if (onExecutionBundle?.HandleBoolFunctionHandlers() == false)
                {
                    authorizationPublisher!.AuthorizationDecision.Decision = Decision.DENY;
                }
            }
            catch (ObligationExecutionFailedException ex)
            {
                Debug.Log(ex.Message);
                authorizationPublisher!.AuthorizationDecision.Decision = Decision.DENY; 
            }
            catch (AdviceExecutionFailedException ex)
            {
                Debug.Log(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                authorizationPublisher!.AuthorizationDecision.Decision = Decision.DENY; 
            }

            if (!(decisionbeforeConstraintHandling.Equals(authorizationPublisher!.AuthorizationDecision.Decision)))
            {
                ExecuteOnDecisionChanged?.Invoke(authorizationPublisher!.AuthorizationDecision);
            }
        }

        private void AuthDecisionPublisher_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Obligation/Advice Handling on decision
            try
            {
                constraintEnforcementService!.BundleFor(authorizationPublisher!.AuthorizationDecision, out onDecisionBundle, out onExecutionBundle);

                constraintsOfLastDecision = new JArray();
                if (authorizationPublisher!.AuthorizationDecision.Obligations != null)
                {
                    constraintsOfLastDecision.Merge(authorizationPublisher!.AuthorizationDecision.Obligations);
                }
                if (authorizationPublisher!.AuthorizationDecision.Advice != null)
                {
                    constraintsOfLastDecision.Merge(authorizationPublisher!.AuthorizationDecision.Advice);
                }

                onDecisionBundle.HandleRunnableHandlers();
                onDecisionBundle.HandleGameObjectConsumerHandlers(gameObject);
                onDecisionBundle?.HandleJsonConsumerHandlers(constraintsOfLastDecision);

                if (onDecisionBundle?.HandleBoolFunctionHandlers() == false)
                {
                    authorizationPublisher!.AuthorizationDecision.Decision = Decision.DENY;
                }
            }
            catch (ObligationExecutionFailedException ex)
            {
                Debug.Log(ex.Message);
                authorizationPublisher!.AuthorizationDecision.Decision = Decision.DENY;
            }
            catch (AdviceExecutionFailedException ex)
            {
                Debug.Log(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                authorizationPublisher!.AuthorizationDecision.Decision = Decision.DENY;
            }

            Debug.Log(gameObject.name + ": SaplConnection received " + authorizationPublisher!.AuthorizationDecision.DecisionString + " for: " + "Subject: " + authorizationPublisher!.Subscription.Subject
                + " - Action: " + authorizationPublisher!.Subscription.Action + " - Resource: " + authorizationPublisher!.Subscription.Resource + " - Environment: "
                + authorizationPublisher!.Subscription.Environment);

            ExecuteOnDecisionChanged?.Invoke(authorizationPublisher!.AuthorizationDecision);
        }

    }
}

