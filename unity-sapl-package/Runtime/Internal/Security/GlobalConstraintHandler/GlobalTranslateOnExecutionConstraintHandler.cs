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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Security.Constraints;
using UnityEngine;

namespace Sapl.Internal.Security.GlobalConstraintHandler
{
    /// <summary>Translate game object constraint Handler for "OnExecution" constraints</summary>

    public class GlobalTranslateOnExecutionConstraintHandler : OnExecutionConstraintHandler, IJsonConsumerConstraintHandlerProvider
    {
        private const string constraintToken = "Translate:OnExecution";

        public Action<JToken> GetHandler(JToken constraint)
        {
            return Translate;
        }

        public bool IsResponsible(JToken constraint)
        {
            if (TryGetConstraintTypeFromObligation((JObject) constraint, out string constraintType))
			{
                return constraintType.Equals(constraintToken);
            }
            return false;
        }

        void Translate(JToken token)
        {
            var constraints = token.ToArray();
            foreach (JObject constraint in constraints)
            {
                if (IsResponsible(constraint))
                {
                    TryGetTargetFromObligation(constraint, out GameObject target);
                    TryGetDataTypeFromObligation(constraint, out string dataType);
                    if (dataType.Equals("Vector3"))
					{
                        if (TryGetDataFromObligation<JsonTranslateVector3>(constraint, out JsonTranslateVector3 jsonVector))
                        {
                            if (target.TryGetComponent<Transform>(out Transform transform))
                            {

                                //Transform transform = target.GetComponent<Transform>();
                                Vector3 vector = new(jsonVector.X, jsonVector.Y, jsonVector.Z);
                                transform.Translate(vector);
                            }
                            else
                            {
                                throw (new Exception("Transform not found on GameObject: " + target.name));
                            }
                        }
                    }
                    else
					{
                        throw (new Exception("Unsupported dataType for Translate constraint: " + dataType));
                    }
                }
            }
        }

        internal class JsonTranslateVector3
        {
            [JsonProperty]
            public float[] Value { get; set; }

            public float X
            {
                get
                {
                    return Value[0];
                }
            }

            public float Y
            {
                get
                {
                    return Value[1];
                }
            }

            public float Z
            {
                get
                {
                    return Value[2];
                }
            }
        }
    }
}
