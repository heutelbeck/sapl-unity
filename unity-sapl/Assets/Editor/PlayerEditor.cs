using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PlayerMovement))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
       PlayerMovement player = (PlayerMovement)target;
        player.textToBeShown = "Hallo der Text wird angezeigt";
        GUILayout.Label(player.textToBeShown);
    }
}
