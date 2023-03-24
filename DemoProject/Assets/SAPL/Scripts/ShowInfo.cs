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
using Sapl.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInfo : BreakingTheGlasBehavior
{
    private GameObject gridLayout;
    private TextPanelView textPanelView;
    private string nameTextBackup;
    private EventMethodEnforcement eventMethodEnforcement;

	private void Start()
	{
        gridLayout = gameObject.transform.Find("GridLayout").gameObject;

        var nameTextGameObject = gridLayout.transform.Find("Name_Text[TextPanel]").gameObject;
        textPanelView = (TextPanelView)nameTextGameObject.GetComponent("TextPanelView");
        nameTextBackup = textPanelView.Text;
    }

	public void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<TutorialStepMove>().TutorialStepMoveNext(2);

        eventMethodEnforcement = gridLayout.GetComponent<EventMethodEnforcement>();

        eventMethodEnforcement.ExecuteMethod();
    }

    public void OnTriggerExit(Collider other)
    {
        GameObject canvasUiLayer = GameObject.Find("CanvasUiLayer");
        var QuestionDialogScript = canvasUiLayer.transform.Find("QuestionDialogUiLayer").GetComponent<QuestionDialogUI>();
        if (QuestionDialogScript != null)
        {
            QuestionDialogScript.Hide();
        }
        Clear();
    }

    public void Show()
    {
        gridLayout.SetActive(true);
    }
    public void Clear()
    {
        textPanelView.Text = nameTextBackup;

        gridLayout.SetActive(false);
    }

    protected override void ExecuteBreakingTheGlas()
    {
        Show();
    }
}
