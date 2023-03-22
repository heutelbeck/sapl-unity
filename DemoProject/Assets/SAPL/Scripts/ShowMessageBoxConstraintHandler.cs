using csharp.sapl.constraint.api;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Security.GlobalConstraintHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMessageBoxConstraintHandler : OnExecutionConstraintHandler, IRunnableConstraintHandlerProvider
{
    private static readonly string constraintToken = "ShowMessageBox";

    public Action GetHandler(JToken constraint)
    {
        return ShowMessageBox;
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

    private void ShowMessageBox()
    {
        //Debug.Log(gameObject.name + " ShowMessageBox");
        var QuestionDialog = gameObject.transform.Find("QuestionDialog").gameObject;
        QuestionDialog.SetActive(true);
    }
}

