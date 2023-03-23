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

namespace csharp.sapl.constraint.api
{
	/// <summary>Compares the priority</summary>
	public interface IHasPriority : IComparable<IHasPriority>
	{

		/// <summary>Gets the priority.</summary>
		int GetPriority();

		/// <summary>
		/// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		new int CompareTo(IHasPriority other)
		{
			return other.GetPriority().CompareTo(GetPriority());
		}

	}
}
