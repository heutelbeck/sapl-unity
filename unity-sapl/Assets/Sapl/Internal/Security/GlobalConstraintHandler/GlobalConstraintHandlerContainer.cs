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
using csharp.sapl.constraint.api;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Registry;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Sapl.Internal.Security.GlobalConstraintHandler
{
	/// <summary>
	///   <para>A container for alle global constraint handlers.</para>
	///   <para>Is created on game start and is not destroyed on load (scene).</para>
	/// </summary>
	public class GlobalConstraintHandlerContainer : MonoBehaviour
    {
        private static GameObject container;

        [RuntimeInitializeOnLoadMethod]
	    static void OnGameStart()
	    {
            container = new GameObject("GlobalConstraintHandlerContainer");
            container.AddComponent<GlobalLoggingOnDecisionConstraintHandler>();
            container.AddComponent<GlobalLoggingOnExecutionConstraintHandler>();
            container.AddComponent<GlobalTranslateOnDecisionConstraintHandler>();
            container.AddComponent<GlobalTranslateOnExecutionConstraintHandler>();
            container.AddComponent<GlobalRotateOnDecisionConstraintHandler>();
            container.AddComponent<GlobalRotateOnExecutionConstraintHandler>();
            GameObject.DontDestroyOnLoad(container);
	    }
    }
}

