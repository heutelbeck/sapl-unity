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
using UnityEngine;
using static csharp.sapl.constraint.ISignal;

namespace Sapl.Internal.Security.GlobalConstraintHandler
{
	/// <summary>A <see cref="ConstraintHandlerMonoBehavior" /> that is executed "OnDecision"</summary>
	public abstract class OnDecisionConstraintHandler: ConstraintHandlerMonoBehavior
    {
        private readonly Signal signal = Signal.ON_DECISION;

        public Signal GetSignal()
        {
            return this.signal;
        }
    }
}