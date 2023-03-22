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
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Sapl.Components
{
    public static class MonoBehaviourExtension
    {
#nullable enable

        #region SingleAuthorization

        /// <summary>
        /// Authorize one action and a bool value as resource
        /// the decision is made only one time (no stream)
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="resource"></param>
        /// <param name="subject"></param>
        /// <param name="environment"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task AuthorizeOnceAsync(this MonoBehaviour monoBehaviour, string resource, [CanBeNull] string? subject = null, [CanBeNull] string? environment = null, [CallerMemberName] string? action = null)
        {
            if (string.IsNullOrEmpty(subject))
            {
                subject = nameof(subject);
            }
            if (string.IsNullOrEmpty(action))
            {
                action = nameof(action);
            }
            if (string.IsNullOrEmpty(resource))
            {
                resource = nameof(resource);
            }
            if (string.IsNullOrEmpty(environment))
            {
                environment = nameof(environment);
            }
            AuthorizationSubscription authorizationSubscription = new AuthorizationSubscription(
                JValue.CreateString(subject), JValue.CreateString(action), JValue.CreateString(resource), JValue.CreateString(environment)
            );

            RemotePolicyDecisionPoint pdp = new RemotePolicyDecisionPoint();
            Debug.Log($"Task for {action} started");
            var result = await pdp.DecideOnceAsync(authorizationSubscription);
            SubmitDecision(monoBehaviour, result, resource);
        }

        private static void SubmitDecision(MonoBehaviour monoBehaviour, AuthorizationDecision decision, string resource)
        {
            try
            {
                FieldInfo prop = monoBehaviour.GetType().GetField(resource, BindingFlags.NonPublic | BindingFlags.Instance);
                if (null == prop)
                {
                    prop = monoBehaviour.GetType().GetField(resource, BindingFlags.Public | BindingFlags.Instance);
                }
                if (prop != null)
                {
                    Debug.Log($"Field {resource} found in the script {monoBehaviour.GetType().FullName}");
                    prop.SetValue(monoBehaviour, IsPermitted(monoBehaviour, decision));
                }
                else
                {
                    Debug.Log($"No field {resource} found in the script {monoBehaviour.GetType().FullName}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new InvalidDataException($"The {nameof(resource)}: {resource} is no Member of the script {monoBehaviour.GetType().FullName}");
            }

        }

#nullable disable

        /// <summary>
        /// Checks if obligations can be raised and the decision is PERMIT
        /// Raises the obligations
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="decision"></param>
        /// <returns>true if all requirements are fulfilled </returns>
        public static bool IsPermitted(MonoBehaviour monoBehaviour, AuthorizationDecision decision)
        {
            Debug.Log($"Task completed decision is {decision.Decision.ToString()}");
            if (decision.Decision == Decision.INDETERMINATE || decision.Decision == Decision.NOT_APPLICABLE)
            {
                return false;
            }
            if (CanRaiseObligations(monoBehaviour, decision.Obligations))
            {
                if (decision.Obligations != null && decision.Obligations.Any())
                {
                    RaiseObligations(monoBehaviour, decision.Obligations);
                }

                return decision.Decision == Decision.PERMIT;
            }
            return false;
        }


#nullable enable

        /// <summary>
        /// Authorize one action and a bool value as resource
        /// the decision is made every time the policies change (observed stream)
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="resource"></param>
        /// <param name="subject"></param>
        /// <param name="environment"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static AuthorizationPublisher AuthorizeAsStream(this MonoBehaviour monoBehaviour, string resource, [CanBeNull] string? subject = null, [CanBeNull] string? environment = null, [CallerMemberName] string? action = null)
        {
            if (string.IsNullOrEmpty(subject))
            {
                subject = "ANNA";
            }
            if (string.IsNullOrEmpty(environment))
            {
                environment = nameof(environment);
            }
            AuthorizationSubscription authorizationSubscription = new AuthorizationSubscription(
                JValue.CreateString(subject), JValue.CreateString(action), JValue.CreateString(resource), JValue.CreateString(environment)
            );
            RemotePolicyDecisionPoint pdp = new RemotePolicyDecisionPoint();
            var publisher = (AuthorizationPublisher)pdp.Decide(authorizationSubscription);
            publisher.PropertyChanged += ((sender, args) =>
                AuthorizationDecisionOnPropertyChanged(monoBehaviour, sender, args));
            return publisher;
        }
#nullable disable
        private static void AuthorizationDecisionOnPropertyChanged(this MonoBehaviour monoBehaviour, object sender, PropertyChangedEventArgs e)
        {
            if (sender is AuthorizationPublisher publisher && e.PropertyName.Equals(nameof(publisher.AuthorizationDecision)))
            {
                FieldInfo prop = monoBehaviour.GetType().GetField(publisher.Subscription.Resource .ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
                if (null == prop)
                {
                    prop = monoBehaviour.GetType().GetField(publisher.Subscription.Resource.Value<string>(), BindingFlags.Public | BindingFlags.Instance);
                }
                //Try to execute the Obligations
                if (publisher.AuthorizationDecision.Obligations != null && CanRaiseObligations(monoBehaviour, publisher.AuthorizationDecision.Obligations) && publisher.AuthorizationDecision.Decision != Decision.INDETERMINATE)
                {

                    RaiseObligations(monoBehaviour, publisher.AuthorizationDecision.Obligations);
                    prop.SetValue(monoBehaviour, publisher.AuthorizationDecision.Decision == Decision.PERMIT);
                    return;
                }
                if (publisher.AuthorizationDecision.Obligations != null && !CanRaiseObligations(monoBehaviour, publisher.AuthorizationDecision.Obligations))
                {
                    prop.SetValue(monoBehaviour, false);
                    return;
                }
                if (publisher.AuthorizationDecision.Obligations == null)
                {
                    prop.SetValue(monoBehaviour, publisher.AuthorizationDecision.Decision == Decision.PERMIT);
                    return;
                }
                prop.SetValue(monoBehaviour, publisher.AuthorizationDecision.Decision == Decision.PERMIT);

            }
        }

        #endregion

        #region Multauthorization

        public static void MultiAuthorizationDecisionOnPropertyChanged(this MonoBehaviour monoBehaviour, object sender, PropertyChangedEventArgs e)
        {
            if (sender is MultiAuthorizationPublisher publisher && e.PropertyName.Equals(nameof(publisher.MultiAuthorizationDecision)))
            {
                foreach (var subscription in publisher.MultiAuthzSubscription.SubscriptionsWithId)
                {
                    if (!string.IsNullOrEmpty(subscription.Key))
                    {
                        if (publisher.MultiAuthorizationDecision.AuthorizationDecisions.Any(k => k.Key.Equals(subscription.Key)))
                        {
                            var decision =
                                publisher.MultiAuthorizationDecision.GetAuthorizationDecisionForSubscriptionWithId(
                                    subscription.Key);
                            var authorizationSubscription =
                                publisher.MultiAuthzSubscription.GetAuthorizationSubscriptionWithId(subscription.Key);
                            var action = authorizationSubscription.Action.Value<string>();
                            var resource = authorizationSubscription.Resource.Value<string>(); ;


                            FieldInfo prop = monoBehaviour.GetType().GetField(resource, BindingFlags.NonPublic | BindingFlags.Instance);
                            if (null == prop)
                            {
                                prop = monoBehaviour.GetType().GetField(resource, BindingFlags.Public | BindingFlags.Instance);
                            }
                            // prop.SetValue(monoBehaviour, decision.Decision == Decision.PERMIT);

                            //Try to execute the Obligations
                            if (decision.Obligations != null && CanRaiseObligations(monoBehaviour, decision.Obligations))
                            {

                                RaiseObligations(monoBehaviour, decision.Obligations);
                                prop.SetValue(monoBehaviour, decision.Decision == Decision.PERMIT);
                                continue;
                            }
                            if (decision.Obligations != null && !CanRaiseObligations(monoBehaviour, decision.Obligations))
                            {
                                prop.SetValue(monoBehaviour, false);
                                continue;
                            }
                            if (decision.Obligations == null)
                            {
                                prop.SetValue(monoBehaviour, decision.Decision == Decision.PERMIT);
                                continue;
                            }
                        }


                    }
                }
            }
        }




        /// <summary>
        /// Creates a MultiAuthorizationSubscription to subscribe many actions at one time
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <returns></returns>
        public static MultiAuthorizationSubscription MultiAuthorizationSubscription(this MonoBehaviour monoBehaviour)
        {
            MultiAuthorizationSubscription authorizationSubscription = new MultiAuthorizationSubscription();
            return authorizationSubscription;
        }

        /// <summary>
        /// Subcribe to a Stream of decisions
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="authorizationSubscription"></param>
        /// <returns></returns>
        public static MultiAuthorizationPublisher AuthorizeAsStream(this MonoBehaviour monoBehaviour, MultiAuthorizationSubscription authorizationSubscription)
        {
            RemotePolicyDecisionPoint decisionPoint = new RemotePolicyDecisionPoint();
            var authorizationPublisher = (MultiAuthorizationPublisher)decisionPoint.DecideAll(authorizationSubscription);
            authorizationPublisher.PropertyChanged += (((sender, args) =>
                MultiAuthorizationDecisionOnPropertyChanged(monoBehaviour, sender, args)));
            return authorizationPublisher;
        }

        #endregion

        #region Obligationhandling


        /// <summary>
        /// Check if obligations are a part of the script
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="obligations"></param>
        /// <returns></returns>
        public static bool CanRaiseObligations(MonoBehaviour monoBehaviour, JArray obligations)
        {
            if (obligations == null || !obligations.Any())
            {
                return true;
            }
            MethodInfo[] methods = monoBehaviour.GetType().GetMethods();
            foreach (JToken obligationToken in obligations)
            {
                var obligation = obligationToken.ToObject<JObject>();
                JToken methodName;
                if (obligation.TryGetValue("method", out methodName))
                {
                    //If Obligations invoke a Method with parameters
                    JToken parameterType;
                    if (obligation.TryGetValue("parameterType", out parameterType))
                    {
                        JToken parameterValue;
                        if (obligation.TryGetValue("parameterValue", out parameterValue))
                        {
                            foreach (MethodInfo method in methods)
                            {
                                if (method.Name.Equals(methodName.ToString()))
                                {
                                    var parameters = method.GetParameters();
                                    if (parameters.Length > 0)
                                    {
                                        foreach (ParameterInfo parameterInfo in parameters)
                                        {
                                            if (parameterInfo.ParameterType.FullName != null && parameterInfo.ParameterType.FullName.Contains(parameterType.ToString(), StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                return true;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    //Method without parameters
                    else
                    {
                        foreach (MethodInfo method in methods)
                        {
                            if (method.Name.Equals(methodName.ToString()))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Executes the obligations on script
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="obligations"></param>
        public static void RaiseObligations(MonoBehaviour monoBehaviour, JArray obligations)
        {
            MethodInfo[] methods = monoBehaviour.GetType().GetMethods();
            foreach (JToken obligationToken in obligations)
            {
                var obligation = obligationToken.ToObject<JObject>();
                JToken methodName;
                if (obligation.TryGetValue("method", out methodName))
                {
                    JToken parameterType;
                    if (obligation.TryGetValue("parameterType", out parameterType))
                    {
                        JToken parameterValue;
                        if (obligation.TryGetValue("parameterValue", out parameterValue))
                        {
                            foreach (MethodInfo method in methods)
                            {
                                if (method.Name.Equals(methodName.ToString()))
                                {
                                    method.Invoke(monoBehaviour, new object[] { parameterValue.ToString() });
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (MethodInfo method in methods)
                        {
                            if (method.Name.Equals(methodName.ToString()))
                            {
                                method.Invoke(monoBehaviour, null);
                            }
                        }
                    }
                }
            }
        }

#nullable enable
        private static bool TryRaiseObligations(this MonoBehaviour monoBehaviour, MultiAuthorizationPublisher multiAuthorizationPublisher, [CallerMemberName] string? action = null)
        {
            try
            {
                foreach (var subscription in multiAuthorizationPublisher.MultiAuthzSubscription.SubscriptionsWithId)
                {
                    if (!string.IsNullOrEmpty(subscription.Key))
                    {
                        if (multiAuthorizationPublisher.MultiAuthorizationDecision.AuthorizationDecisions.Any(k => k.Key.Equals(subscription.Key)))
                        {
                            var decision =
                                multiAuthorizationPublisher.MultiAuthorizationDecision.GetAuthorizationDecisionForSubscriptionWithId(
                                    subscription.Key);
                            var authorizationSubscription =
                                multiAuthorizationPublisher.MultiAuthzSubscription.GetAuthorizationSubscriptionWithId(subscription.Key);
                            if (authorizationSubscription != null && authorizationSubscription.Action != null && authorizationSubscription.Action.Values<string>() != null)
                            {
                                var actionS = authorizationSubscription.Action.Value<string>();

                                if (actionS != null && actionS.Equals(action))
                                {
                                    //Try to execute the Obligations
                                    if (decision.Obligations != null &&
                                        CanRaiseObligations(monoBehaviour, decision.Obligations))
                                    {

                                        RaiseObligations(monoBehaviour, decision.Obligations);
                                        return true;
                                    }

                                    if (decision.Obligations != null &&
                                        !CanRaiseObligations(monoBehaviour, decision.Obligations))
                                    {
                                        return false;
                                    }

                                    if (decision.Obligations == null)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return false;
        }

        private static bool TryRaiseObligations(this MonoBehaviour monoBehaviour, AuthorizationPublisher authorizationPublisher, [CallerMemberName] string? action = null)
        {
            try
            {
                if (authorizationPublisher.AuthorizationDecision.Obligations != null && CanRaiseObligations(monoBehaviour, authorizationPublisher.AuthorizationDecision.Obligations))
                {
                    RaiseObligations(monoBehaviour, authorizationPublisher.AuthorizationDecision.Obligations);
                    return true;
                }
                if (authorizationPublisher.AuthorizationDecision.Obligations != null && !CanRaiseObligations(monoBehaviour, authorizationPublisher.AuthorizationDecision.Obligations))
                {
                    return false;
                }
                if (authorizationPublisher.AuthorizationDecision.Obligations == null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return false;
        }
        #endregion

#nullable disable
    }
}