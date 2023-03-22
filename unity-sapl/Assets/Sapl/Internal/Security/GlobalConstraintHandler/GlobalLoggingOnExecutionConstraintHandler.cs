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
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Registry;
using Sapl.Internal.Security.Constraints;
using UnityEngine;

namespace Sapl.Internal.Security.GlobalConstraintHandler
{
	/// <summary>Logging constraint Handler for "OnExecution" constraints</summary>
	public class GlobalLoggingOnExecutionConstraintHandler : OnExecutionConstraintHandler, IJsonConsumerConstraintHandlerProvider
    {
        private static readonly string constraintToken = "Logging:OnExecution";

        public Action<JToken> GetHandler(JToken constraint)
        {
            return Logging;
        }

        public bool IsResponsible(JToken constraint)
        {
            if (TryGetConstraintTypeFromObligation((JObject)constraint, out string constraintType))
            {
                return constraintType.Equals(constraintToken);
            }
            return false;
        }

        private void Logging(JToken token)
        {
            var constraints = token.ToArray();
            foreach (JObject constraint in constraints)
            {
                if (IsResponsible(constraint))
                {
                    if (!TryGetMessageFromObligation(constraint, out string message))
					{
                        throw (new Exception("Message not found in Constraint"));
                    }
                    string subject;
                    try
					{
                        subject = SaplRegistry.GetRegistry().GetSubjectJsonString();
					}                    
                    catch
					{
                        throw (new Exception("SaplRegistry not found"));  
                    }
                    var timestamp = DateTime.Now.ToString(new CultureInfo("de-DE"));
                    Debug.Log("Timestamp: " + timestamp + "\n" + 
                                "Subject: " + subject + "\n" + 
                                "Message: " + message);
                }
            }
        }
    }
}

