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
