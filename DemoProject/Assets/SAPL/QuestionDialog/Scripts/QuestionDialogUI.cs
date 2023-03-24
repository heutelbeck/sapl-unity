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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionDialogUI : MonoBehaviour {

    public static QuestionDialogUI Instance { get; private set; }



    private TextMeshProUGUI textMeshPro;
    private Button yesBtn;
    private Button noBtn;

    private void Awake() {
        Instance = this;

        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        yesBtn = transform.Find("YesBtn").GetComponent<Button>();
        noBtn = transform.Find("NoBtn").GetComponent<Button>();

        Hide();
    }

    public void ShowQuestion(string questionText, Action yesAction, Action noAction) {
        gameObject.SetActive(true);

        textMeshPro.text = questionText;
        yesBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.AddListener(() => {
            Hide();
            yesAction();
        });
        noBtn.onClick.RemoveAllListeners();
        noBtn.onClick.AddListener(() => {
            Hide();
            noAction();
        });
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
    
}