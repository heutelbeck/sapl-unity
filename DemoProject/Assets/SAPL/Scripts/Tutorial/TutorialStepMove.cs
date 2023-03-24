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

public class TutorialStepMove : MonoBehaviour
{
    public void TutorialStepMoveNext(int index)
    {
        TutorialManager tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        tutorialManager.goNext[index] = true;
    }

    public void TutorialStepMoveNext()
    {
        TutorialManager tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        tutorialManager.goNext[tutorialManager.tutorialIndex] = true;
    }

    public void TutorialStepMovePrevious(int index)
    {
        TutorialManager tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        tutorialManager.goPrevious[index] = true;
    }

    public void TutorialStepMovePrevious()
    {
        TutorialManager tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        tutorialManager.goPrevious[tutorialManager.tutorialIndex] = true;
    }
}
