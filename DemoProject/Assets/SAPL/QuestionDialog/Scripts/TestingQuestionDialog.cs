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
using UnityEngine;
using UnityEditor;

public class TestingQuestionDialog : MonoBehaviour {


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            QuestionDialogUI.Instance.ShowQuestion("Are you sure you want to quit the game?", () => {
                Debug.Log("Test");
                QuestionDialogUI.Instance.ShowQuestion("Are you really sure?", () => {
                    Application.Quit();
                    EditorApplication.ExitPlaymode();
                }, () => {
                     // Do nothing
                });
            }, () => {
                // Do nothing on No
            });
        }
    }

}