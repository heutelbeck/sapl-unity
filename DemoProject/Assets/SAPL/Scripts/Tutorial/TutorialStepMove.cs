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
