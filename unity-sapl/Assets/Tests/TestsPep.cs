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
using NUnit.Framework;
using csharp.sapl.pdp.api;
using Sapl.Internal;
using Sapl.Internal.Enums;
using Sapl.Internal.Registry;
using System.Reflection;

using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Sapl.Tests
{
    public class TestsPep
    {
        
        [Test]
        public void TestCurrentDecisionChange()
        {
            var assembly = typeof(EnforcementBase).Assembly;
            var type = assembly.GetType("Sapl.Internal.PEP.PolicyEnforcementPoint");
            var ar = new[] { typeof(GameObject) };
            var ctr = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, ar, null);

            GameObject go = new ();
            go.name = "TestObject";
            var connection = ctr.Invoke(new object[] { go});
            
            var getDecision = type.GetMethod("GetDecision", BindingFlags.NonPublic | BindingFlags.Instance);
            var param = new object[] { DecisionTypeEnum.ONCE, JToken.FromObject("Subject"), "action", "resource", "environment" };
            getDecision.Invoke(connection, param);
            
            var publisher = (AuthorizationPublisher)type.GetField("authorizationPublisher", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(connection);
            publisher.AuthorizationDecision.Decision = Decision.DENY;
            var decision = publisher.AuthorizationDecision.Decision;

            Assert.AreEqual(Decision.DENY, decision);


        }
    }
}