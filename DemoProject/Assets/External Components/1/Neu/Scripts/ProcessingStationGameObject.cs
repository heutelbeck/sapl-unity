using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingStationGameObject : MonoBehaviour {

    [SerializeField] private GameObject mainInfo;

    private bool visible;

    private void Start() {
        mainInfo.SetActive(false);
        visible = false;
    }

    void OnMouseDown() {
        Debug.Log("Gestartet");
        mainInfo.SetActive(visible != true);
    }

}