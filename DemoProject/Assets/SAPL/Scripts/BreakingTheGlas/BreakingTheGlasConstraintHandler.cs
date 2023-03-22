using csharp.sapl.constraint.api;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Security.GlobalConstraintHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingTheGlasConstraintHandler : OnExecutionConstraintHandler, IRunnableConstraintHandlerProvider
{
    public string constraintToken;

    [TextAreaAttribute]
    public string dialogMessage;

    public BreakingTheGlasBehavior breakingTheGlasBehavior;
    public Action GetHandler(JToken constraint)
    {
        return BreakingTheGlas;
    }

    public bool IsResponsible(JToken constraint)
    {
        if (TryGetConstraintTypeFromObligation((JObject)constraint, out string constraintType))
        {
            if (constraintType.Equals(constraintToken))
            {
                return true;
            }
        }
        return false;
    }

    private void BreakingTheGlas()
    {
        if (breakingTheGlasBehavior == null)
        {
            breakingTheGlasBehavior = gameObject.GetComponent<BreakingTheGlasBehavior>();
        }
        breakingTheGlasBehavior.PermitExecution = false;
        GameObject canvasUiLayer = GameObject.Find("CanvasUiLayer");
        var QuestionDialogScript = canvasUiLayer.transform.Find("QuestionDialogUiLayer").GetComponent<QuestionDialogUI>();
        QuestionDialogScript.ShowQuestion(dialogMessage,
            () => { breakingTheGlasBehavior.ExecuteWithPermission(); },
            () => { breakingTheGlasBehavior.ResetState(); });
    }
}
