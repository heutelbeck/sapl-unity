using csharp.sapl.constraint.api;
using FTK.UIToolkit.Primitives;
using Newtonsoft.Json.Linq;
using Sapl.Internal.Security.GlobalConstraintHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackenNameConstraintHandler : OnExecutionConstraintHandler, IRunnableConstraintHandlerProvider
{
    private static readonly string constraintToken = "BlackenName";

    public Action GetHandler(JToken constraint)
    {
        return BlackenName;
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

    private void BlackenName()
    {
        var nameText = gameObject.transform.Find("Name_Text[TextPanel]").gameObject;
        TextPanelView textPanelView = (TextPanelView)nameText.GetComponent("TextPanelView");
        textPanelView.Text = "***";
    }
}

