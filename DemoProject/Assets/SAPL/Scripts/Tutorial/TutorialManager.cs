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
