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
using System;

namespace csharp.sapl.pdp.api
{
	///<summary>
	/// Holds an <see cref="csharp.sapl.pdp.api.AuthorizationDecision">  SAPL authorization decision</see> together with the ID
	/// of the corresponding <see cref="AuthorizationSubscription"> SAPL authorization subscription</see>.
	/// 
	/// <see cref="csharp.sapl.pdp.api.AuthorizationDecision"/>
	/// <see cref="IdentifiableAuthorizationSubscription"/>
	///</summary>
	public class IdentifiableAuthorizationDecision
	{
#nullable enable
		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		public String? AuthorizationSubscriptionId { get; set; }

		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		public AuthorizationDecision? AuthorizationDecision { get; set; }
#nullable disable

		public IdentifiableAuthorizationDecision() { }

#nullable enable
		public IdentifiableAuthorizationDecision(String? authorizationSubscriptionId, AuthorizationDecision authorizationDecision)
		{
			this.AuthorizationSubscriptionId = authorizationSubscriptionId;
			this.AuthorizationDecision = authorizationDecision;
		}
#nullable disable
		///<summary>
		/// A simple INDETERMINATE decision.
		///</summary>
		public readonly static IdentifiableAuthorizationDecision INDETERMINATE = new IdentifiableAuthorizationDecision(null,
				AuthorizationDecision.INDETERMINATE);
	}
}
