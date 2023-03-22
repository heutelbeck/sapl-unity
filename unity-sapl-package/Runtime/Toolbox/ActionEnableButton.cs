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
using System.Collections;
using System.Collections.Generic;
using csharp.sapl.pdp.api;
using UnityEngine;
using UnityEngine.UI;

namespace Sapl.Components {
    public class ActionEnableButton : ActionSAPL
    {
        public override void Enforce(GameObject gameObject, Decision decision)
        {
        
            Button button = gameObject.GetComponent<Button>();
            if (button != null)
            {
                if (decision == Decision.PERMIT)
                {
                    button.enabled = true;
                }
                if (decision == Decision.DENY)
                {
                    button.enabled = false;
                }

            }
        }

      
    }
}
