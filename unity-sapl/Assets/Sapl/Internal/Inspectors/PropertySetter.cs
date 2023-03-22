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
#if UNITY_EDITOR
using Sapl.Internal.Interfaces;

namespace Sapl.Internal.Inspectors
{
    ///<summary>
    /// Tool class for use in custom editors
    /// to set Sapl properties from inspector/>
    ///</summary>
    static class PropertySetter
    {
        /// <summary>Sets the property.</summary>
        /// <param name="target">The target.</param>
        public static void SetProperty(UnityEngine.Object target)
        {
            // Take out this if statement to set the value using setter when ever you change it in the inspector.
            // But then it gets called a couple of times when ever inspector updates
            // By having a button, you can control when the value goes through the setter and getter, your self.
            //if (GUILayout.Button("Use setters/getters"))
            {
                var baseType = target.GetType().BaseType;
                var type = target.GetType();
                if (baseType.GetInterface(nameof(IInspectorProperties)) != null)
                {                 
                    IInspectorProperties getterSetter = (IInspectorProperties)target;
                    
                    if (getterSetter.SubjectGameObject != null)
                        getterSetter.SubjectGameObject = getterSetter.SubjectGameObject;
                    
                    getterSetter.SubjectString = getterSetter.SubjectString;
                    getterSetter.Action = getterSetter.Action;
                    getterSetter.Resource = getterSetter.Resource;
                    getterSetter.Environment = getterSetter.Environment;
                    
                }
            }
        }
    }
}
#endif