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
using csharp.sapl.constraint;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace csharp.sapl.pdp.api
{
	///<summary>
	/// A multi-decision holds a map of authorization subscription IDs and
	/// corresponding <see cref="AuthorizationDecision"> authorization decisions</see>. It
	/// provides methods to
	/// <see cref="MultiAuthorizationDecision.SetAuthorizationDecisionForSubscriptionWithId(String, AuthorizationDecision)">
	/// add single authorization decisions related to an authorization subscriptionID</see>, 
	/// to <see cref="MultiAuthorizationDecision.GetAuthorizationDecisionForSubscriptionWithId(String)"> get a
	/// single authorization decision for a given authorization subscription ID</see> and
	/// to <see cref="IEnumerator"> iterate</see> over all the authorization decisions.
	///</summary>
	public class MultiAuthorizationDecision : IEnumerable<IdentifiableAuthorizationDecision>, INotifyDecisionChanged
	{
		private Dictionary<string, AuthorizationDecision> authorizationDecisions;

		//public List<>

		private JToken decisionBundle;

		public Dictionary<string, AuthorizationDecision> AuthorizationDecisions

		{
			get
			{
				if (authorizationDecisions == null)
				{
					authorizationDecisions = new Dictionary<string, AuthorizationDecision>();
				}
				if (decisionBundle != null)
				{
					authorizationDecisions.Clear();
					JToken decisionCollection = decisionBundle.Children<JProperty>().FirstOrDefault(x => x.Name == "authorizationDecisions")?.Value;

					foreach (JToken item in decisionCollection.Children())
					{
						JEnumerable<JToken> itemProperties = item.Children<JToken>();

						foreach (JToken itemProperty in itemProperties)
						{
							var property = item.ToObject<JProperty>();
							var decision = itemProperty.ToObject<AuthorizationDecision>();
							if (decision != null && property != null)
							{
								authorizationDecisions.Add(property.Name, decision);
							}
						}
					}
				}

				return authorizationDecisions;
			}
		}

		public MultiAuthorizationDecision(JToken decisionBundle)
		{
			this.decisionBundle = decisionBundle;
			UpdateDecisions();
		}


		public MultiAuthorizationDecision() { }

		///<returns>
		///A simple INDETERMINATE decision.
		///</returns>
		public static MultiAuthorizationDecision Indeterminate()
		{
			MultiAuthorizationDecision multiAuthzDecision = new MultiAuthorizationDecision();
			multiAuthzDecision.SetAuthorizationDecisionForSubscriptionWithId("", AuthorizationDecision.INDETERMINATE);
			return multiAuthzDecision;
		}

		public string DecisionString
		{
			get
			{
				string s = String.Empty;
				foreach (var ad in AuthorizationDecisions)
				{
					s += "ID: " + ad.Key.ToString();
					s += "Decision: " + ad.Value.DecisionString;
					s += "\n";
				}
				return s;

			}
			set { }

		}

		///<returns>
		///the number of <see cref="AuthorizationDecision"/>s
		///</returns>
		public int Size()
		{
			return AuthorizationDecisions.Count;
		}


		/// <summary>
		/// Adds the given tuple of subscription ID and related authorization decision to
		/// this multi-decision. 
		/// </summary>
		/// <param name="subscriptionId">the ID of the authorization subscription related to the
		/// given authorization decision.</param>
		/// <param name="authzDecision">the authorization decision related to the authorization
		/// subscription with the given ID.</param>
		public void SetAuthorizationDecisionForSubscriptionWithId(String subscriptionId,
				 AuthorizationDecision authzDecision)
		{
			AuthorizationDecisions.Add(subscriptionId, authzDecision);
			UpdateDecisions();
		}

		/// <summary>
		/// Retrieves the authorization decision related to the subscription with the
		/// given ID. 
		/// </summary>
		/// <param name="subscriptionId">subscriptionId the ID of the subscription for which the related
		/// authorization decision has to be returned.</param>
		/// <returns>the authorization decision related to the subscription with the given ID.</returns>
		public AuthorizationDecision GetAuthorizationDecisionForSubscriptionWithId(String subscriptionId)
		{
			return AuthorizationDecisions[subscriptionId];
		}

		/// <summary>
		/// Retrieves the decision related to the authorization subscription with the
		/// given ID.
		/// </summary>
		/// <param name="subscriptionId">subscriptionId the ID of the authorization subscription for which the
		/// related decision has to be returned.</param>
		/// <returns>the decision related to the authorization subscription with the given
		/// ID. Returns null if not present.</returns>
		public Decision? GetDecisionForSubscriptionWithId(String subscriptionId)
		{
			var decision = AuthorizationDecisions[subscriptionId];
			if (decision == null) return null;
			return decision.Decision;

		}

		/// <summary>
		/// Returns {@code true} if the decision related to the authorization
		/// subscription with the given ID is {@link Decision#PERMIT}, {@code false}
		/// otherwise.
		/// @param subscriptionId 
		/// @return 
		/// </summary>
		/// <param name="subscriptionId">the ID of the authorization subscription for which the
		/// related flag indicating whether the decision was PERMIT
		/// or not has to be returned.</param>
		/// <returns>trueif the decision related to the authorization
		/// subscription with the given ID is <see cref="Decision.PERMIT"/>,
		/// false otherwise.</returns>
		public bool IsAccessPermittedForSubscriptionWithId(String subscriptionId)
		{
			var decision = AuthorizationDecisions[subscriptionId];
			return decision != null && decision.Decision == Decision.PERMIT;
		}


		private List<IdentifiableAuthorizationDecision> GetIdentifiableAuthorizationDecisions()
		{
			List<IdentifiableAuthorizationDecision> list = new List<IdentifiableAuthorizationDecision>();
			foreach (var ad in AuthorizationDecisions)
			{
				list.Add(new IdentifiableAuthorizationDecision(ad.Key, ad.Value));
			}
			return list;
		}


		/// <summary>Converts to string.</summary>
		/// <returns>A string that represents the current Multi Authorization Decision.</returns>
		public override String ToString() //testen: null-references?
		{
			StringBuilder sb = new StringBuilder("MultiAuthorizationDecision {");
			foreach (var iad in GetIdentifiableAuthorizationDecisions())
			{

				sb.Append("\n\t[").Append("SUBSCRIPTION-ID: ").Append(iad.AuthorizationSubscriptionId).Append(" | ")
						.Append("DECISION: ").Append(iad.AuthorizationDecision == null ? String.Empty : iad.AuthorizationDecision.Decision.ToString()).Append(" | ")
						.Append("RESOURCE: ").Append(iad.AuthorizationDecision == null ? String.Empty : iad.AuthorizationDecision.Resource.ToString()).Append(" | ")
						.Append("OBLIGATIONS: ").Append(iad.AuthorizationDecision == null ? String.Empty : iad.AuthorizationDecision.Obligations.ToString()).Append(" | ")
						.Append("ADVICE: ").Append(iad.AuthorizationDecision.Advice).Append(']');
			}


			sb.Append("\n}");

			return sb.ToString();
		}

		///<returns>
		/// an <see cref="IEnumerator"/> providing access to the
		/// <see cref="IdentifiableAuthorizationDecision">identifiable authorization
		/// decisions</see> created from the data held by this multi-decision.
		///</returns>
		public IEnumerator<IdentifiableAuthorizationDecision> GetEnumerator()
		{
			foreach (KeyValuePair<string, AuthorizationDecision> authorizationDecision in AuthorizationDecisions)
			{
				yield return new IdentifiableAuthorizationDecision(authorizationDecision.Key,
					authorizationDecision.Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#region implements INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
#nullable enable
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
#nullable disable
		#endregion

		/// <summary>Updates the decisions.</summary>
		public void UpdateDecisions()
		{
			OnPropertyChanged(nameof(AuthorizationDecisions));
			OnPropertyChanged(nameof(DecisionString));
		}
	}
}
