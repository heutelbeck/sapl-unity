using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDebug : MonoBehaviour
{
    public void printMessage(string message) {
        Debug.Log(message);
    }

    public void button1CLicked() {
        Debug.Log("Button 1 geklickt");
    }

    public void button2CLicked() {
        Debug.Log("Button 2 geklickt");
    }

}