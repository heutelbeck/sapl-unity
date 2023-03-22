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
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sapl.Internal.Registry;
using UnityEngine;

namespace Sapl.Tests
{
    public class TestsSaplRegistry
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestRegisterEnvironment()
        {
            var env = "TestEnvironment";
            var registry = SaplRegistry.GetRegistry();
            registry.SetEnvironment(env);
            Assert.AreEqual(env, registry.CurrentEnvironment);
        }

        [Test]
        public void TestRegisterSubjectString()
        {
            var sub = "TestSubject";
            var registry = SaplRegistry.GetRegistry();
            registry.RegisterSubject(sub);
            Assert.AreEqual(sub, registry.CurrentSubject.ToString());
        }

        [Test]
        public void TestRegisterSubjectObject()
        {
            var sub = new string[] { "test1", "test2" };
            var jToken = JToken.FromObject(sub);
            var registry = SaplRegistry.GetRegistry();
            registry.RegisterSubject(sub);
            Assert.AreEqual(jToken.ToString(Newtonsoft.Json.Formatting.Indented),registry.GetSubjectJsonString());
        }

        [Test]
        public void TestRegisterGameObject()
        {
            var registry = SaplRegistry.GetRegistry();
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var jToken = registry.RegisterGameObjectSubject(go);
            Assert.AreEqual(jToken.ToString(Newtonsoft.Json.Formatting.Indented), registry.GetSubjectJsonString());
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.



    }
}
