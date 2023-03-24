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
using FTK.UIToolkit.Primitives;
using System;
using UnityEngine;

namespace FTK.UIToolkit.Util
{
    [Serializable]
    public class SubUIWrapper : MonoBehaviour
    {
        [SerializeField]
        private InspectableDictionary<string, GameObject> gameObjects;
        public GameObject GetGameObject(string key) => gameObjects[key];
        public bool HasGameObject(string key) => gameObjects.ContainsKey(key);
        public bool TryGetGameObject(string key, out GameObject obj) => gameObjects.TryGetValue(key, out obj);


        [SerializeField]
        private InspectableDictionary<string, TextPanelView> textPanels;
        public TextPanelView GetTextPanel(string key) => textPanels[key];
        public bool HasTextPanel(string key) => textPanels.ContainsKey(key);
        public bool TryGetTextPanel(string key, out TextPanelView obj) => textPanels.TryGetValue(key, out obj);


        [SerializeField]
        private InspectableDictionary<string, SimpleButtonView> buttons;
        public SimpleButtonView GetButton(string key) => buttons[key];
        public bool HasButton(string key) => buttons.ContainsKey(key);
        public bool TryGetButton(string key, out SimpleButtonView obj) => buttons.TryGetValue(key, out obj);


        [SerializeField]
        private InspectableDictionary<string, SubUIWrapper> linkedChildren;
        public SubUIWrapper GetLinkedChild(string key) => linkedChildren[key];
        public bool HasLinkedChild(string key) => linkedChildren.ContainsKey(key);
        public bool TryGetLinkedChild(string key, out SubUIWrapper obj) => linkedChildren.TryGetValue(key, out obj);

        public void Apply()
        {
            gameObjects.ForceInspectorToDictionary();
            textPanels.ForceInspectorToDictionary();
            buttons.ForceInspectorToDictionary();
            linkedChildren.ForceInspectorToDictionary();
        }

    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SubUIWrapper))]
    public class SubUIWrapperEditor : UnityEditor.Editor
    {
        private GUIStyle buttonStyle;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SubUIWrapper suw = (SubUIWrapper)target;
            buttonStyle = new GUIStyle(GUI.skin.button);
            if (GUILayout.Button("Apply", buttonStyle))
            {
                suw.Apply();
            }
        }

    }
#endif
}