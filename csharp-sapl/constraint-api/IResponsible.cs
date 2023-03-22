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

namespace csharp.sapl.constraint.api
{
	/// <summary>Checks if the ConstraintHandlerProvider is responsible for a constraint</summary>
	public interface IResponsible : ISignal
	{
		/// <summary>Determines whether the specified constraint is responsible.</summary>
		/// <param name="constraint">The constraint.</param>
		/// <returns>
		///   <c>true</c> if the specified constraint is responsible; otherwise, <c>false</c>.</returns>
		bool IsResponsible(JToken constraint);
	}
}
