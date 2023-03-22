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
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Sapl.Internal.Security.GlobalConstraintHandler
{
    /// <summary>
    /// Provides methods for editing a Json constraint object
    /// </summary>
    public class ConstraintHandlerMonoBehavior: MonoBehaviour
    {
		/// <summary>Tries to get constraint type from obligation.</summary>
		/// <param name="obligation">The obligation.</param>
		/// <param name="constraintType">Type of the constraint.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TryGetConstraintTypeFromObligation(JObject obligation, out string constraintType)
        {
            return TryGetStringParameterFromObligation(obligation, ConstraintParameters.constraintType, out constraintType);
        }

		/// <summary>Tries to get message from obligation.</summary>
		/// <param name="obligation">The obligation.</param>
		/// <param name="message">The message.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TryGetMessageFromObligation(JObject obligation, out string message)
        {
            return TryGetStringParameterFromObligation(obligation, ConstraintParameters.message, out message);
        }

		/// <summary>Tries to get target from obligation.</summary>
		/// <param name="obligation">The obligation.</param>
		/// <param name="target">The target.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TryGetTargetFromObligation(JObject obligation, out GameObject target)
        {
            string targetString;
            if (obligation.TryGetValue(ConstraintParameters.target, out JToken targetToken))
            {
                targetString = targetToken.ToString();
                target = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name.Equals(targetString));
                return target != null;
            }
            target = null;
            return false;
        }

		/// <summary>Tries to get component from obligation.</summary>
		/// <param name="obligation">The obligation.</param>
		/// <param name="component">The component.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TryGetComponentFromObligation(JObject obligation, out string component)
        {
            return TryGetStringParameterFromObligation(obligation, ConstraintParameters.component, out component);
        }

		/// <summary>Tries to get data type from obligation.</summary>
		/// <param name="obligation">The obligation.</param>
		/// <param name="dataType">Type of the data.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TryGetDataTypeFromObligation(JObject obligation, out string dataType)
        {
            return TryGetStringParameterFromObligation(obligation, ConstraintParameters.dataType, out dataType);
        }

		/// <summary>Tries to get data from obligation.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obligation">The obligation.</param>
		/// <param name="data">The data.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TryGetDataFromObligation<T> (JObject obligation, out T data)
        {
            if (obligation.TryGetValue(ConstraintParameters.data, out JToken dataToken))
            {
                data = dataToken.ToObject<T>();
                return true;
            }
            data = default;
            return false;
        }

		/// <summary>Tries to get time span from obligation.</summary>
		/// <param name="obligation">The obligation.</param>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TryGetTimeSpanFromObligation(JObject obligation, out int timeSpan)
        {
            if (obligation.TryGetValue(ConstraintParameters.timeSpan, out JToken timeSpanJToken))
            {
                timeSpan = timeSpanJToken.ToObject<int>();
                return true;
            }
            timeSpan = 0;
            return false;
        }

        /// <summary>Tries to get string parameter from obligation.</summary>
        /// <param name="obligation">The obligation.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>true if successful; otherwise, false.</returns>
        protected bool TryGetStringParameterFromObligation(JObject obligation, string parameterName, out string parameterValue)
        {
            if (obligation.TryGetValue(parameterName, out JToken typeToken))
            {
                parameterValue = typeToken.ToString();
                return true;
            }
            parameterValue = "";
            return false;
        }

        protected IEnumerator RestoreColor(Color colorLast, Renderer renderer, int timeSpan)
        {
            yield return new WaitForSeconds(timeSpan);
            renderer.material.color = colorLast;
        }
        protected IEnumerator RestoreColorBlock(ColorBlock colorBlockLast, Button button, int timeSpan)
        {
            yield return new WaitForSeconds(timeSpan);
            button.colors = colorBlockLast;
        }

		/// <summary>Tries to set private field.</summary>
		/// <param name="obligation">The obligation.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TrySetPrivateField(JObject obligation)
        {
			if (!TryGetTargetFromObligation(obligation, out GameObject go))
				return false;

			if (!TryGetTargetField(obligation, out string fieldName))
				return false;


			var type = go.GetType();

            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(go, Color.black);

            return true;
        }

		/// <summary>Tries to get target field.</summary>
		/// <param name="obligation">The obligation.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>true if successful; otherwise, false.</returns>
		protected bool TryGetTargetField(JObject obligation, out string fieldName)
        {
            if (obligation.TryGetValue("targetField", out JToken fieldToken))
            {
                fieldName = fieldToken.ToString();
                return true;
            }
            fieldName = "";
            return false;
        }
    }
}
