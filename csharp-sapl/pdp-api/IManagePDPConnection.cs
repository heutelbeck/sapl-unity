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
using System;
using System.Threading.Tasks;

namespace csharp.sapl.pdp.api
{
	public interface IManagePDPConnection
	{

		public Task SubscribeDecision(AuthorizationSubscription authzSubscription,
			IObserver<AuthorizationDecision> observer, string relativeUri);

		public Task SubscribeAllDecisions(MultiAuthorizationSubscription authzSubscription,
			MultiAuthorizationPublisher observer, string relativeUri);

		public Task SubscribeAllIdentifiableDecisions(MultiAuthorizationSubscription authzSubscription,
			MultiAuthorizationPublisher observer, string relativeUri);


		public Task SubscribeDecisionOnce(AuthorizationSubscription subscription, IObserver<AuthorizationDecision> observer,
			string relativeUri);

		public Task SubscribeAllIdentifiableDecisionsOnce(MultiAuthorizationSubscription authzSubscription,
			MultiAuthorizationPublisher observer, string relativeUri);
	}

}
