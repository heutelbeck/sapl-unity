using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JobSelectScrollController : MonoBehaviour
{
    [SerializeField] TMP_Text jobNumberText;
    [SerializeField] int numberOfJobs;
    [SerializeField] GameObject jobBtnPref;
    [SerializeField] Transform jobBtnParent;

    // Start is called before the first frame update
    void Start()
    {
        LoadJobBtns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadJobBtns()
    {
        for (int i=0; i< numberOfJobs; i++)
        {
            GameObject jobBtnObj = Instantiate(jobBtnPref, jobBtnParent) as GameObject;
            jobBtnObj.GetComponent<SelectJobBtns>().numberOfJobs = i;
            jobBtnObj.GetComponent<SelectJobBtns>().jobSelectScrollController = this;
        }
    }

    public void OnJobSelectButtonClick(int jobNumber)
    {
        jobNumberText.text = "Selected Job: Job" + (jobNumber + 1);
    }
}
