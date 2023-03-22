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
using NUnit.Framework;
using Sapl.Components;
using Sapl.Internal;
using System.Reflection;
using UnityEngine;

namespace Sapl.Tests
{
    public class TestsEnforcementBase
    {
        [Test]
        public void TestGetSubjectGameObject()
        {
            GameObject go = new();
            go.AddComponent<GameObjectEnforcement>();
            var script = go.GetComponent<GameObjectEnforcement>();
            go.name = "Test";
            script.SubjectGameObject = go;
            var type = typeof(EnforcementBase);
            var getSubject = type.GetMethod("GetSubject", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("{\"name\":\"Test\",\"tag\":\"Untagged\"}", JsonConvert.SerializeObject(getSubject.Invoke(script, null)));

        }

        [Test]
        public void TestGetSubjectString()
        {
            GameObject go = new();
            go.AddComponent<GameObjectEnforcement>();
            var script = go.GetComponent<GameObjectEnforcement>();
            script.SubjectString = "TestSubject";
            var type = typeof(EnforcementBase);
            var getSubject = type.GetMethod("GetSubject", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("\"TestSubject\"", JsonConvert.SerializeObject(getSubject.Invoke(script, null)));
        }

        [Test]
        public void TestGetResource()
        {
            GameObject go = new();
            go.AddComponent<GameObjectEnforcement>();
            var script = go.GetComponent<GameObjectEnforcement>();
            script.Resource = "TestResource";
            var type = typeof(EnforcementBase);
            var getSubject = type.GetMethod("GetResource", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("TestResource", getSubject.Invoke(script, null));
        }

        [Test]
        public void TestGetEnvironment()
        {
            GameObject go = new();
            go.AddComponent<GameObjectEnforcement>();
            go.AddComponent<EnvironmentPoint>();
            var script = go.GetComponent<EnvironmentPoint>();
            script.Environment = "EnvironmentPoint";
            var script1 = go.GetComponent<GameObjectEnforcement>();
            script1.Environment = "TestEnvironment";
            var type = typeof(EnforcementBase);
            var getEnvironment = type.GetMethod("GetEnvironment", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("TestEnvironment", getEnvironment.Invoke(script1, null));
 
        }

        [Test]
        public void TestGetEnvironmentPoint()
        {
            GameObject go = new();
            go.AddComponent<GameObjectEnforcement>();
            go.AddComponent<EnvironmentPoint>();
            var script = go.GetComponent<EnvironmentPoint>();
            script.Environment = "EnvironmentPoint";
            var script1 = go.GetComponent<GameObjectEnforcement>();
            var type = typeof(EnforcementBase);
            var getEnvironment = type.GetMethod("GetEnvironment", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("EnvironmentPoint", getEnvironment.Invoke(script1, null));

        }

    }
}
