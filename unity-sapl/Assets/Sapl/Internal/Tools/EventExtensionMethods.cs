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
using System.Reflection;
using UnityEngine.Events;

namespace Sapl.Internal.Tools
{
    static class EventExtensionMethods
    {
        /// <summary>Gets the non-persistent listener count.</summary>
        /// <param name="unityEvent">The unity event.</param>
        /// <returns>
        /// The count of non-persistent listeners.
        /// </returns>
        internal static int GetListenerCount(this UnityEventBase unityEvent)
        {
            var field = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            var invokeCallList = field.GetValue(unityEvent);
            var property = invokeCallList.GetType().GetProperty("Count");
            return (int)property.GetValue(invokeCallList);
        }
    }
}
