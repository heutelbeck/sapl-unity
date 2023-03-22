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
using Sapl.Internal.Structs;
using System;
using UnityEngine;

namespace Sapl.Internal.Registry
{
    ///<summary>
    /// Provides global subject and environment vor Sapl Components
    ///</summary>
    public sealed class SaplRegistry: MonoBehaviour
    {
#nullable enable
        private static GameObject? emptyObj;

        public delegate void PropertyChangedEventHandler(string value);
        public event PropertyChangedEventHandler? SubjectChanged;      
        public event PropertyChangedEventHandler? EnvironmentChanged;

        private JToken? currentSubject;
        /// <summary>Gets the current subject.</summary>
        /// <value>The current subject.</value>
        /// <returns>The registered subject as string or null</returns>
        public JToken? CurrentSubject 
        { 
            get => currentSubject;
            private set
            {
                if (currentSubject != value)
                {
                    currentSubject = value;
                    if (SubjectChanged != null && currentSubject != null)
                        SubjectChanged.Invoke(currentSubject.ToString());
                }
             }
        }

        private string? currentEnvironment;
        /// <summary>Gets the current environment.</summary>
        /// <value>The current environment.</value>
        /// <returns>The registered environment as string or null</returns>
        public string? CurrentEnvironment
        {
            get => currentEnvironment;
            private set
            {
                if (currentEnvironment != value)
                {
                    currentEnvironment = value;
                    if (EnvironmentChanged != null && currentEnvironment != null) EnvironmentChanged.Invoke(currentEnvironment.ToString());
                }
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnGameStart()
        {
            emptyObj = new GameObject("SaplRegistry");
            emptyObj.AddComponent<SaplRegistry>();
            GameObject.DontDestroyOnLoad(emptyObj);
            Debug.Log("SaplRegistry instantiated: " + emptyObj.name);
        }

        public static SaplRegistry GetRegistry()
        {
            return emptyObj!.GetComponent<SaplRegistry>();
        }

        private void Awake()
        {
            currentSubject = JToken.FromObject(System.Environment.UserName);//default: Windows-Username
        }

        ///<summary>
        /// Used to register a environment
        ///</summary>
        ///<param name="environment">Name of the Environment</param>
        public void SetEnvironment(string environment)
        {
            CurrentEnvironment = environment;
        }

        ///<summary>
        /// Used to register a string-subject
        ///</summary>
        ///<param name="subject">Name of the subject</param>
        public void RegisterSubject(string subject)
        {
            CurrentSubject = JToken.FromObject(subject);
        }

        ///<summary>
        /// Used to register an subject of arbitrary type 
        ///</summary>
        ///<param name="subject">Instance of the subject</param>
        ///<returns>A boolean value which indicates if the object could successfully be serialized</returns>
        public bool RegisterSubject(object subject)
        {           
            try
            {
                CurrentSubject = JToken.FromObject(subject); 
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        ///<summary>
        /// Used to register an subject of type <see cref="UnityEngine.GameObject"/> 
        ///</summary>
        ///<param name="go">Instance of the subject</param>
        ///<returns>A <see cref="Newtonsoft.Json.Linq.JToken"/> which represents the subject</returns>
        public JToken RegisterGameObjectSubject(GameObject go)
        {
            GameObjectSubjectStruct sub = new (go.name, go.tag);
            CurrentSubject = JToken.FromObject(sub); 
            return CurrentSubject;
        }

        ///<returns>The registered subject as json-string or null</returns>
        public string? GetSubjectJsonString()
        {
            return currentSubject?.ToString(Newtonsoft.Json.Formatting.Indented);
        }

    }
}

