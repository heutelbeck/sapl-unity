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
using Sapl.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecureOnTriggerEnterCat : MonoBehaviour
{
    private EventMethodEnforcement eventMethodEnforcement;

    public void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<TutorialStepMove>().TutorialStepMoveNext(3);

        eventMethodEnforcement = gameObject.GetComponent<EventMethodEnforcement>();
        eventMethodEnforcement.ExecuteMethod();
    }

    public void OnTriggerStay(Collider other)
    {
        eventMethodEnforcement = gameObject.GetComponent<EventMethodEnforcement>();
        eventMethodEnforcement.ExecuteMethod();
    }

    public void MoveAway()
    {
        //Set walkAway parameter of Cat's Animator to true
        gameObject.GetComponent<Animator>().SetBool("walkAway", true);
    }
}
