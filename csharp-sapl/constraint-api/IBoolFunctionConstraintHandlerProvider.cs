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
using Newtonsoft.Json.Linq;

namespace csharp.sapl.constraint.api
{
	/// <summary>
	/// Constraint Handler Provider for C# Bool Functions
	/// </summary>
	public interface IBoolFunctionConstraintHandlerProvider : IResponsible
	{
		/// <summary>Gets the handler.</summary>
		/// <param name="constraint">The constraint.</param>
		Func<Boolean> GetHandler(JToken constraint);
	}
}
