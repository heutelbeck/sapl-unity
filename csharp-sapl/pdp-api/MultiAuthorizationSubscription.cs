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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace csharp.sapl.pdp.api
{
	/// <summary>
	/// A multi-subscription holds a list of subjects, a list of actions, a list of resources,
	/// a list of environments (which are the elements of a {@link AuthorizationSubscription
	/// SAPL authorization subscription}) and a map holding subscription IDs and corresponding
	/// {@link AuthorizationSubscriptionElements authorization subscription elements}. It
	/// provides methods to
	/// {@link #addAuthorizationSubscription(String, Object, Object, Object, Object) add}
	/// single authorization subscriptions and to {@link #iterator() iterate} over all the
	/// authorization subscriptions.
	///
	/// <see cref="AuthorizationSubscription"/> 
	/// 
	/// </summary>
	public class MultiAuthorizationSubscription
	{
		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		List<object> subjects = new List<object>();

		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		List<Object> actions = new List<object>();

		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		List<Object> resources = new List<object>();

		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		List<Object> environments = new List<object>();

		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		Dictionary<string, AuthorizationSubscriptionElements> authorizationSubscriptions = new Dictionary<string, AuthorizationSubscriptionElements>();


		public ReadOnlyDictionary<string, AuthorizationSubscriptionElements> SubscriptionsWithId
		{
			get
			{
				return new ReadOnlyDictionary<string, AuthorizationSubscriptionElements>(authorizationSubscriptions);
			}
		}


		public MultiAuthorizationSubscription() { }

		/// <summary>
		/// Convenience method to add an authorization subscription without environment data.
		/// Calls {@code addAuthorizationSubscription(String, Object, Object, Object)
		/// addAuthorizationSubscription(subscriptionId, subject, action, resource, null)}.
		/// @param subscriptionId the id identifying the authorization subscription to be
		/// added.
		/// @param subject the subject of the authorization subscription to be added.
		/// @param action the action of the authorization subscription to be added.
		/// @param resource the resource of the authorization subscription to be added.
		/// @return this {@code MultiAuthorizationSubscription} instance to support chaining of
		/// multiple calls to {@code addAuthorizationSubscription}.
		/// <(summary>
		//public MultiAuthorizationSubscription AddAuthorizationSubscription(String subscriptionId, Object subject,
		//        Object action, Object resource)
		//{
		//    return AddAuthorizationSubscription(subscriptionId, subject, action, resource, null);
		//}

		/// <summary>
		///  Convenience method to add an authorization subscription without environment data.
		/// Calls {@link #addAuthorizationSubscription(String, Object, Object, Object)
		/// addAuthorizationSubscription(subscriptionId, subject, action, resource, null)}.
		/// @param subscriptionId the id identifying the authorization subscription to be
		/// added.
		/// @param subscription an authorization subscription.
		/// @return this {@code MultiAuthorizationSubscription} instance to support chaining of
		/// multiple calls to {@code addAuthorizationSubscription}.
		/// <summary>
		//public MultiAuthorizationSubscription AddAuthorizationSubscription(String subscriptionId,
		//        AuthorizationSubscription subscription)
		//{
		//    return AddAuthorizationSubscription(subscriptionId, subscription.Subject, subscription.Action,
		//            subscription.Resource, subscription.Environment);
		//}

		/// <summary>
		/// Adds the authorization subscription defined by the given subject, action, resource
		/// and environment. The given {@code subscriptionId} is associated with the according
		/// decision to allow the recipient of the PDP decision to correlate
		/// subscription/decision pairs.
		/// @param subscriptionId the id identifying the authorization subscription to be
		/// added.
		/// @param subject the subject of the authorization subscription to be added.
		/// @param action the action of the authorization subscription to be added.
		/// @param resource the resource of the authorization subscription to be added.
		/// @param environment the environment of the authorization subscription to be added.
		/// @return this {@code MultiAuthorizationSubscription} instance to support chaining of
		/// multiple calls to {@code addAuthorizationSubscription}.
		/// </summary>
#nullable enable
		public MultiAuthorizationSubscription AddAuthorizationSubscription(String subscriptionId, Object subject,
				 Object action, Object resource, Object? environment)
		{

			if (authorizationSubscriptions.ContainsKey(subscriptionId))
				throw new ArgumentException("Cannot add two subscriptions with the same ID: " + subscriptionId);

			var subjectId = EnsureIsElementOfListAndReturnIndex(subject, subjects);
			var actionId = EnsureIsElementOfListAndReturnIndex(action, actions);
			var resourceId = EnsureIsElementOfListAndReturnIndex(resource, resources);
			var environmentId = EnsureIsElementOfListAndReturnIndex(environment!, environments);

			authorizationSubscriptions.Add(subscriptionId,
				   new AuthorizationSubscriptionElements(subjectId, actionId, resourceId, environmentId));
			return this;
		}

		private int? EnsureIsElementOfListAndReturnIndex(Object element, List<Object> list)
		{
			if (element == null)
				return null;

			int index = list.IndexOf(element);
			if (index == -1)
			{
				index = list.Count;
				list.Add(element);
			}
			return index;
		}
#nullable disable
		/// <summary>
		/// @return {@code true} if this multi-subscription holds at least one authorization
		/// subscription, {@code false} otherwise.
		/// </summary>
		public bool HasAuthorizationSubscriptions()
		{
			return !(authorizationSubscriptions.Count == 0);
		}

		/// <summary>
		/// Returns the authorization subscription related to the given ID or {@code null} if
		/// this multi-subscription contains no such ID.
		/// @param subscriptionId the ID of the authorization subscription to be returned.
		/// @return the authorization subscription related to the given ID or {@code null}.
		/// </summary>
		public AuthorizationSubscription GetAuthorizationSubscriptionWithId(String subscriptionId)
		{
			AuthorizationSubscriptionElements subscriptionElements = authorizationSubscriptions[subscriptionId];
			if (subscriptionElements != null)
			{
				return ToAuthzSubscription(subscriptionElements);
			}
			return null;
		}


		private AuthorizationSubscription ToAuthzSubscription(AuthorizationSubscriptionElements subscriptionElements)
		{
			Object subject = subjects[subscriptionElements.SubjectId.Value];
			Object action = actions[subscriptionElements.ActionId.Value];
			Object resource = resources[subscriptionElements.ResourceId.Value];
#nullable enable
			Object? environment = subscriptionElements.EnvironmentId != null ? environments[subscriptionElements.EnvironmentId.Value] : null;
			//return new AuthorizationSubscription(MAPPER.valueToTree(subject), MAPPER.valueToTree(action),
			//        MAPPER.valueToTree(resource), environment == null ? null : MAPPER.valueToTree(environment));
			//JValue? e = null;
#nullable disable
			return new AuthorizationSubscription((JValue)JToken.FromObject(subject), (JValue)JToken.FromObject(action),
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
					(JValue)JToken.FromObject(resource), environment == null ? null : (JValue)JToken.FromObject(environment));
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
		}

	}
}
