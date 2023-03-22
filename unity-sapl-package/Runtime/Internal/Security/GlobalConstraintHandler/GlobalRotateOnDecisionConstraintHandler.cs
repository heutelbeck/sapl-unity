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
	/// <summary>Rotate game object constraint Handler for "OnDecision" constraints</summary>
	public class GlobalRotateOnDecisionConstraintHandler : OnDecisionConstraintHandler, IJsonConsumerConstraintHandlerProvider
    {
        private const string constraintToken = "Rotate:OnDecision";

        public Action<JToken> GetHandler(JToken constraint)
        {
            return Rotation;
        }

        public bool IsResponsible(JToken constraint)
        {
            if (TryGetConstraintTypeFromObligation((JObject)constraint, out string constraintType))
            {
                return constraintType.Equals(constraintToken);
            }
            return false;
        }

        void Rotation(JToken token)
        {
            var constraints = token.ToArray();
            foreach (JObject constraint in constraints)
            {
                if (IsResponsible(constraint))
                {
                    TryGetTargetFromObligation(constraint, out GameObject target);
                    TryGetDataTypeFromObligation(constraint, out string dataType);
                    if (dataType.Contains("Vector3"))
                    {
                        if (TryGetDataFromObligation<JsonRotateData>(constraint, out JsonRotateData jsonVector))
                        {
                            if (target.TryGetComponent<Transform>(out Transform transform))
                            {
                                Vector3 vector = new(jsonVector.XAngle, jsonVector.YAngle, jsonVector.ZAngle);
                                if (dataType.Contains("World"))
								{
                                    transform.Rotate(vector, Space.World);
								}
                                else if (dataType.Contains("Self"))
								{
                                    transform.Rotate(vector, Space.Self);
                                }
                                else
								{
                                    throw (new Exception("Unsupported dataType for Rotate constraint: " + dataType));
                                }
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

        internal class JsonRotateData
        {
            [JsonProperty]
            public float[] Value { get; set; }

            public float XAngle
            {
                get
                {
                    return Value[0];
                }
            }

            public float YAngle
            {
                get
                {
                    return Value[1];
                }
            }

            public float ZAngle
            {
                get
                {
                    return Value[2];
                }
            }
        }
    }
}
