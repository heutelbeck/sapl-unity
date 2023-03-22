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
using Sapl.Components;
using UnityEditor;

///<summary>
/// Custom editor for <see cref="EnvironmentPoint"/>
/// and derived classes
///</summary>
namespace Sapl.Internal.Inspectors
{
    [CustomEditor(typeof(EnvironmentPoint), true)]
    class EnvironmentPointInspector: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (target.GetType() == typeof(EnvironmentPoint))
            {
                EnvironmentPoint getterSetter = (EnvironmentPoint)target;
                getterSetter.Environment = getterSetter.Environment;                
            }
                // Take out this if statement to set the value using setter when ever you change it in the inspector.
                // But then it gets called a couple of times when ever inspector updates
                // By having a button, you can control when the value goes through the setter and getter, your self.
                //if (GUILayout.Button("Use setters/getters"))
        }
    }
}
#endif
