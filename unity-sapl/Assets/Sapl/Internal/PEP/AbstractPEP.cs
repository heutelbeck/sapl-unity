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
using csharp.sapl.pdp.api;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Enums;
using System;
using Sapl.Internal.Security.Constraints;

namespace Sapl.Internal.PEP
{
    /// <summary>Base class for PolicyEnforcementPoints</summary>
    abstract class AbstractPep
    {
        protected readonly RemotePolicyDecisionPoint pdp;
#nullable enable
        protected AuthorizationPublisher? authorizationPublisher;
        protected OnDecisionConstraintHandlerBundle? onDecisionBundle;
        protected OnExecutionConstraintHandlerBundle? onExecutionBundle;

        /// <summary>
        /// Is invoked when a property (usually the decision) of <see cref="csharp.sapl.pdp.api.AuthorizationPublisher"/>
        /// changes (<see cref="csharp.sapl.pdp.api.AuthorizationPublisher.PropertyChanged"/>).
        /// </summary>
        internal Action<AuthorizationDecision>? ExecuteOnDecisionChanged;

        protected AbstractPep()
        {
            pdp = new RemotePolicyDecisionPoint();
        }

        /// <summary>Used to check all on-execution-constraints.</summary>
        internal abstract void HandleOnExecutionBundle();

        /// <summary>Creates a <see cref="AuthorizationSubscription"></see> and gets the <see cref="AuthorizationDecision"></see>.</summary>
        /// <param name="endpoint"><see cref="DecisionTypeEnum.ONCE"/> or <see cref="DecisionTypeEnum.STREAM"/></param>
        /// <param name="subject">The subject.</param>
        /// <param name="action">The action.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="environment">The environment.</param>
        internal abstract void GetDecision
            (DecisionTypeEnum endpoint, JToken subject, string action,
            string resource, string environment = "");


        /// <summary>Unsubscribes from Sapl.</summary>
        internal virtual void Unsubscribe() 
        {
            if (authorizationPublisher != null)
                authorizationPublisher.Unsubscribe();
        }
    }
}
