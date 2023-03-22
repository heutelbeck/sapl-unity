using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectJobBtns : MonoBehaviour
{

    [HideInInspector] public int numberOfJobs;
    [HideInInspector] public JobSelectScrollController jobSelectScrollController;

    [SerializeField] Text jobBtnText; 
    // Start is called before the first frame update
    void Start()
    {
        jobBtnText.text = "Job - " +(numberOfJobs + 1).ToString(); 
    }

    public void OnJobSelectButtonClick()
    {
        jobSelectScrollController.OnJobSelectButtonClick(numberOfJobs);
    }
}
