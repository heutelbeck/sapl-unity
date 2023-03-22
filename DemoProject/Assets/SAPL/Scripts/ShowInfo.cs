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
