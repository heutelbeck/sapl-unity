using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] tutorialObjekts; //Game Objects have to be added in Editor
    public int tutorialIndex;

    /// <summary>
    /// Set to true to go to the next tutorial step.
    /// </summary>
    public bool[] goNext; //Size has to be defined in Editor
    /// <summary>
    /// Set to true to go to the previous tutorial step.
    /// </summary>
    public bool[] goPrevious; //Size has to be defined in Editor

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < tutorialObjekts.Length; i++)
        {
            if(i == tutorialIndex)
            {
                tutorialObjekts[i].SetActive(true);
            }
            else
            {
                tutorialObjekts[i].SetActive(false);
            }
        }

        //go to the next tutorial step
        if (tutorialIndex < goNext.Length-1)
        {
            if (goNext[tutorialIndex])
            {
                tutorialIndex++;

                //Ensure the next TutorialStep is not skipped directly
                //by setting popUpNext and popUpPrevious for that step to false.
                if (tutorialIndex < goNext.Length)
                {
                    goNext[tutorialIndex] = false;
                }
                if (tutorialIndex < goPrevious.Length)
                {
                    goPrevious[tutorialIndex] = false;
                }
            }
        }

        //go to the previous tutorial step
        if (tutorialIndex > 0 && tutorialIndex < goPrevious.Length)
        {
            if (goPrevious[tutorialIndex])
            {
                tutorialIndex--;

                //Ensure the previous TutorialStep is not skipped directly
                //by setting popUpNext and popUpPrevious for that step to false.
                if (tutorialIndex < goNext.Length)
                {
                    goNext[tutorialIndex] = false;
                }
                if (tutorialIndex < goPrevious.Length)
                {
                    goPrevious[tutorialIndex] = false;
                }
            }
        }
    }

    /// <summary>
    /// Restarts the scene.
    /// </summary>
    public void RestartScene()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}
}
