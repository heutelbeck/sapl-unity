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
using Sapl.Components;
using Sapl.Internal;
using System.Reflection;
using UnityEditor.Events;
using UnityEngine;

namespace Sapl.Tests
{
    public class TestsComponentEnforcement
    {
        [Test]
        public void TestGetAction()
        {
            GameObject go = new GameObject();
            go.AddComponent<ComponentEnforcement>();
            var script = go.GetComponent<ComponentEnforcement>();
            var type = typeof(ComponentEnforcement);
            var getAction = type.GetMethod("GetAction", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("Enable Component", getAction.Invoke(script, null));
        }

        [Test]
        public void TestGetActionString()
        {
            GameObject go = new GameObject();
            go.AddComponent<ComponentEnforcement>();
            var script = go.GetComponent<ComponentEnforcement>();
            script.Action = "test";
            var type = typeof(ComponentEnforcement);
            var getAction = type.GetMethod("GetAction", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("test", getAction.Invoke(script, null));
        }
    }
}
