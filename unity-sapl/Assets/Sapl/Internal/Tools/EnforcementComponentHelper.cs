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
using Sapl.Interfaces;
using System;
using UnityEngine;

namespace Sapl.Internal.Tools
{
    ///<summary>
    ///Tool-class for Sapl-Components
    ///</summary>
  
    static class EnforcementComponentHelper
    {
        ///<summary>
        /// Gets all <see cref="Sapl.Interfaces.ISaplExecuteable"/>
        ///</summary>
        ///<param name="gameObject">Name of the <see cref="UnityEngine.GameObject"/></param>
        ///<returns>An array of <see cref="Sapl.Interfaces.ISaplExecuteable"/></returns>
        public static ISaplExecuteable[] GetScripts(GameObject gameObject)
        {
            var scripts = gameObject.GetComponents<ISaplExecuteable>();
            if (scripts.Length == 0)
                Debug.LogError(gameObject.name + ": No executable sapl-scripts (ISaplExecuteable) found");

            return scripts;
        }



        //standard methods
        /// <summary>Standard method for e. g.  <see cref="Sapl.Components.GameObjectEnforcement.Execute"/></summary>
        /// <param name="gameObject">The game object.</param>
        /// <param name="type">The type of the component which uses this method.</param>
        public static void SetActive(GameObject gameObject, Type type)
        {
            gameObject.SetActive(true);
        }

        /// <summary>Standard method for e. g.  <see cref="Sapl.Components.GameObjectEnforcement.Execute"/></summary>
        /// <param name="gameObject">The game object.</param>
        /// <param name="type">The type of the component which uses this method.</param>
        public static void SetInactive(GameObject gameObject, Type type)
        {
            gameObject.SetActive(false);
        }
        /// <summary>Standard method for e. g.  <see cref="Sapl.Components.GameObjectEnforcement.Execute"/></summary>
        /// <param name="gameObject">The game object.</param>
        /// <param name="type">The type of the component which uses this method.</param>
        public static void SetEnabled(GameObject gameObject, Type type)
        {
            SetScripts(true, gameObject, type);
        }

        /// <summary>Standard method for e. g.  <see cref="Sapl.Components.GameObjectEnforcement.Execute"/></summary>
        /// <param name="gameObject">The game object.</param>
        /// <param name="type">The type of the component which uses this method.</param>
        public static void SetDisabled(GameObject gameObject, Type type)
        {
            SetScripts(false, gameObject, type);
        }
        


        private static void SetScripts(bool b, GameObject gameObject, Type type)
        {
            var scripts = gameObject.GetComponents<MonoBehaviour>();

            foreach (var script in scripts)
            {
                if (!(script is EnforcementBase))
                    script.enabled = b;
            }
        }

    }
}
