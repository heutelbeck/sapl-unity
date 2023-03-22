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
using csharp.sapl.pdp.remote;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace csharp.sapl.pdp.api
{

    /// <summary>
    /// Encapsulates all available SubscriptionTypes to get a Decision
    /// </summary>
    public class RemotePolicyDecisionPoint : IPolicyDecisionPoint
    {
        private static string DECIDEONCE = "/api/pdp/decide-once";
        private static string DECIDE = "/api/pdp/decide";
        //private static string MULTI_DECIDE_ONCE = "/api/pdp/multi-decide-once";
        private static string MULTI_DECIDE_ONCE = "/api/pdp/multi-decide";
        private static string MULTI_DECIDE = "/api/pdp/multi-decide";
        private static string MULTI_DECIDE_ALL = "/api/pdp/multi-decide-all";


        /// <summary>
        /// Awaitable Task for getting a Decision for one time.
        /// </summary>
        /// <param name="authzSubscription"></param>
        /// <returns></returns>
        public Task<AuthorizationDecision> DecideOnceAsync(AuthorizationSubscription authzSubscription)
        {
            return SaplClient.Current.DecideOnceAsync(authzSubscription, DECIDEONCE);
        }

        //Decide
        /// <summary>
        /// Observer, implements the INotifyDecisionChanged interface
        /// raises an event when the decision was changed by the policy
        /// </summary>
        /// <param name="authzSubscription"></param>
        /// <returns></returns>
        public IObserver<AuthorizationDecision> DecideOnce(AuthorizationSubscription authzSubscription)
        {
            ((IAuthorizationProvider)AuthorizationProvider.Current).InitializeConnection(SaplClient.Current);
            var publisher = new AuthorizationPublisher(authzSubscription);
            publisher.Subscribe(AuthorizationProvider.Current);
            ((IAuthorizationProvider)AuthorizationProvider.Current).TrackDecisionOnce(authzSubscription, publisher, DECIDEONCE);
            return publisher;
        }

        //Decide
        /// <summary>
        /// Observe a Decision for a single subscription
        /// </summary>
        /// <param name="authzSubscription"></param>
        /// <returns></returns>
        public IObserver<AuthorizationDecision> Decide(AuthorizationSubscription authzSubscription)
        {
            ((IAuthorizationProvider)AuthorizationProvider.Current).InitializeConnection(SaplClient.Current);
            var publisher = new AuthorizationPublisher(authzSubscription);
            publisher.Subscribe(AuthorizationProvider.Current);
            ((IAuthorizationProvider)AuthorizationProvider.Current).TrackDecision(authzSubscription, publisher, DECIDE);
            return publisher;
        }

        //Multi Decide
        /// <summary>
        /// Get an Enumarator of all Decisions for one Multisubscription
        /// </summary>
        /// <param name="multiAuthzSubscription"></param>
        /// <returns></returns>
        public IEnumerator<IdentifiableAuthorizationDecision> Decide(MultiAuthorizationSubscription multiAuthzSubscription)
        {
            MultiAuthorizationPublisher multiPublisher = new MultiAuthorizationPublisher(multiAuthzSubscription);
            MultiAuthorizationProvider multiProvider =
                new MultiAuthorizationProvider(SaplClient.Current);
            multiProvider.Subscribe(multiPublisher);
            multiProvider.TrackIdentifiableDecisions(multiAuthzSubscription, MULTI_DECIDE);
            return multiPublisher.MultiAuthorizationDecision.GetEnumerator();
        }

        public IEnumerator<IdentifiableAuthorizationDecision> DecideOnce(MultiAuthorizationSubscription multiAuthzSubscription)
        {
            MultiAuthorizationPublisher multiPublisher = new MultiAuthorizationPublisher(multiAuthzSubscription);
            MultiAuthorizationProvider multiProvider =
                new MultiAuthorizationProvider(SaplClient.Current);
            multiProvider.Subscribe(multiPublisher);
            multiProvider.TrackIdentifiableDecisionsOnce(multiAuthzSubscription, MULTI_DECIDE);
            return multiPublisher.MultiAuthorizationDecision.GetEnumerator();
        }

        public IObserver<MultiAuthorizationDecision> MultiDecideOnce(MultiAuthorizationSubscription multiAuthzSubscription)
        {
            MultiAuthorizationPublisher multiPublisher = new MultiAuthorizationPublisher(multiAuthzSubscription);
            MultiAuthorizationProvider multiProvider =
                new MultiAuthorizationProvider(SaplClient.Current);
            multiProvider.Subscribe(multiPublisher);
            multiProvider.TrackIdentifiableDecisionsOnce(multiAuthzSubscription, MULTI_DECIDE_ONCE);
            return multiPublisher;
        }

        //Testmethod for WPF-GUI
        public IObserver<MultiAuthorizationDecision> MultiDecide(MultiAuthorizationSubscription multiAuthzSubscription)
        {
            MultiAuthorizationPublisher multiPublisher = new MultiAuthorizationPublisher(multiAuthzSubscription);
            MultiAuthorizationProvider multiProvider =
                new MultiAuthorizationProvider(SaplClient.Current);
            multiProvider.Subscribe(multiPublisher);
            multiProvider.TrackIdentifiableDecisions(multiAuthzSubscription, MULTI_DECIDE);
            return multiPublisher;
        }



        //Multi Decide All
        public IObserver<MultiAuthorizationDecision> DecideAll(MultiAuthorizationSubscription multiAuthzSubscription)
        {
            IObserver<MultiAuthorizationDecision> multiPublisher = new MultiAuthorizationPublisher(multiAuthzSubscription);
            MultiAuthorizationProvider multiProvider =
                new MultiAuthorizationProvider(SaplClient.Current);
            multiProvider.Subscribe(multiPublisher);
            multiProvider.TrackDecisions(multiAuthzSubscription, MULTI_DECIDE_ALL);
            return multiPublisher;
        }

    }
}
