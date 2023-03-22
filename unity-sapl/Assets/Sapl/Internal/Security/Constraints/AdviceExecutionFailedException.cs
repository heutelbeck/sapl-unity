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

namespace Sapl.Internal.Security.Constraints
{
	/// <summary>This Exception is thrown when the execution of an advice failed</summary>
	class AdviceExecutionFailedException : Exception
	{
        /// <summary>Initializes a new instance of the <see cref="AdviceExecutionFailedException" /> class.</summary>
        public AdviceExecutionFailedException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AdviceExecutionFailedException" /> class.</summary>
        /// <param name="message">The message which will be shown.</param>
        public AdviceExecutionFailedException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AdviceExecutionFailedException" /> class.</summary>
        /// <param name="message">The message which will be shown.</param>
        /// <param name="inner">The exception thrown by <see cref="Sapl.Internal.Security.Constraints.ConstraintEnforcementService"/>.</param>
        public AdviceExecutionFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
