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
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace pdp_unittest
{
    public class SubscriptionsTest
    {
        private MultiAuthorizationSubscription multiAuthorizationSubscription;
        private AuthorizationSubscription authorizationSubscription;

        [SetUp]
        public void Setup()
        {
            multiAuthorizationSubscription = new MultiAuthorizationSubscription();
            multiAuthorizationSubscription.AddAuthorizationSubscription("id1", "Robert", "Move", "GameObject", null);
            multiAuthorizationSubscription.AddAuthorizationSubscription("id2", "Manfred", "Play", "Component", null);
            multiAuthorizationSubscription.AddAuthorizationSubscription("id3", "Simon", "Try", "GameObject", null);
            authorizationSubscription = new AuthorizationSubscription(
                JValue.CreateString("ANNA"), JValue.CreateString("Move"), JValue.CreateString("GameObject"), JValue.CreateString("null")
            );
        }

        [Test]
        public void IsSubjectSetInSingleSubscription()
        {
            Assert.IsTrue(authorizationSubscription.Subject.Value<string>().Equals("ANNA"));
        }

        [Test]
        public void IsActionSetSingleSubscription()
        {
            Assert.IsTrue(authorizationSubscription.Action.Value<string>().Equals("Move"));
        }

        [Test]
        public void IsRecourceSetSingleSubscription()
        {
            Assert.IsTrue(authorizationSubscription.Resource.Value<string>().Equals("GameObject"));
        }

        [Test]
        public void IsSubjectSetInMultisubscription()
        {
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id1").Subject.Value<string>().Equals("Robert"));
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id2").Subject.Value<string>().Equals("Manfred"));
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id3").Subject.Value<string>().Equals("Simon"));
        }

        [Test]
        public void IsActionSetMultisubscription()
        {
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id1").Action.Value<string>().Equals("Move"));
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id2").Action.Value<string>().Equals("Play"));
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id3").Action.Value<string>().Equals("Try"));
        }

        [Test]
        public void IsRecourceSetMultisubscription()
        {
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id1").Resource.Value<string>().Equals("GameObject"));
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id2").Resource.Value<string>().Equals("Component"));
            Assert.IsTrue(multiAuthorizationSubscription.GetAuthorizationSubscriptionWithId("id3").Resource.Value<string>().Equals("GameObject"));
        }
    }
}