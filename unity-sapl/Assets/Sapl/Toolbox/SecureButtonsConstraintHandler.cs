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
using System.Linq;
using csharp.sapl.constraint.api;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Security.GlobalConstraintHandler;
using UnityEngine;
using UnityEngine.UI;

namespace Sapl.Components
{
    public class SecureButtonsConstraintHandler : OnDecisionConstraintHandler, IConsumerConstraintHandlerProvider<JToken>
    {

        private static GameObject emptyObj;
        private const string constraintToken = "enableButton";

        [SerializeReference]
        public Resource recource;

        public bool IsResponsible(JToken constraint)
        {
            var obligation = constraint.ToObject<JObject>();
            JToken token;
            if (obligation.TryGetValue("type", out token))
                return token.ToString().Equals(constraintToken);
            else
                return false;
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnGameStart()
        {
            emptyObj = new GameObject("enableButton");
            emptyObj.AddComponent<SecureButtonsConstraintHandler>();
            GameObject.DontDestroyOnLoad(emptyObj);
        }
        public Action<JToken> GetHandler(JToken constraint)
        {
            return Enable;
        }

        public void Enable(JToken token)
        {
            var obligations = token.ToArray();  //JArray.Parse(token.ToString());
            foreach (JObject obligation in obligations)
            {
                if (IsResponsible(obligation))
                {
                    foreach (GameObject gameObject in recource.resources)
                    {
                        Button button = gameObject.GetComponent<Button>();
                        if (button != null)
                        {
                            button.enabled = true;
                        }
                    }

                }
            }
        }

    } 
}