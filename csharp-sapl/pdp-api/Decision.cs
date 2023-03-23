﻿/*
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
namespace csharp.sapl.pdp.api
{
	/// <summary>
	/// The different possible outcomes of a SAPL decision.
	///
	/// PERMIT grants access to the resource, while respecting potential obligations,
	/// advises, or resource transformation.
	///
	/// DENY denies access to the resource.
	///
	/// INDETERMINATE means that an error occurred during the decision process.
	/// Access must be denied in this case.
	///
	/// NOT_APPLICABLE means no policies were found matching the authorization
	/// subscription. Access must be denied in this case.
	/// </summary>
	public enum Decision
	{

		/// <summary>
		/// PERMIT grants access to the resource, while respecting potential obligations,
		/// advises, or resource transformation.
		/// </summary>
		PERMIT,

		/// <summary>
		/// DENY denies access to the resource.
		/// </summary>
		DENY,

		/// <summary>
		/// INDETERMINATE means that an error occurred during the decision process.
		/// Access must be denied in this case.
		/// </summary>
		INDETERMINATE,

		/// <summary>
		/// NOT_APPLICABLE means no policies were found matching the authorization
		/// subscription. Access must be denied in this case.
		/// </summary>
		NOT_APPLICABLE

	}
}
