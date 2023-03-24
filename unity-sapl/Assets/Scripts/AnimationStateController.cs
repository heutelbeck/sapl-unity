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
using csharp.sapl.pdp.api;
using Sapl.Components;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    Subject subjectO;
    private TextMeshProUGUI obligations;
    private TextMeshProUGUI securityMessage;


#pragma warning disable IDE0044 // Are set by Animator.SetBool();
    private bool canDance = false;
    private bool canWalk = false;
    private bool canPunch = false;
    private bool canJump = false;
#pragma warning restore IDE0044 // Modifizierer "readonly" hinzufügen


    private const string subject = "ANNA";
    private readonly CancellationTokenSource cancellationTokenSource;

    private readonly AuthorizationPublisher authorizationPublisherDance;
    private readonly MultiAuthorizationPublisher multiAuthorizationPublisher;

    void Awake()
    {
        SetFields();
        SetTextFields();

        ////For Once
        //Warnings disabled because of fire and forget pattern at this case
#pragma warning disable CS4014
        this.AuthorizeOnceAsync(nameof(canWalk), subject, null, nameof(Walk));
#pragma warning restore CS4014
#pragma warning disable CS4014
        this.AuthorizeOnceAsync(nameof(canPunch), subject, null, nameof(Punch));
#pragma warning restore CS4014
#pragma warning disable CS4014
        this.AuthorizeOnceAsync(nameof(canJump), subject, null, nameof(Jump));
#pragma warning restore CS4014



        //For observing decision from Stream
        //this.AuthorizeAsStream(nameof(canWalk), null, null, nameof(Walk));
        //this.AuthorizeAsStream(nameof(canPunch), null, null, nameof(Punch));
        //this.AuthorizeAsStream(nameof(canJump), null, null, nameof(Jump));





        //MultiAuthorizationSubscription multiAuthorizationSubscription = this.MultiAuthorizationSubscription();
        //multiAuthorizationSubscription.AddAuthorizationSubscription(nameof(canJump), subject, nameof(Jump), nameof(canJump), null);
        //multiAuthorizationSubscription.AddAuthorizationSubscription(nameof(canWalk), subject, nameof(Walk), nameof(canWalk), null);
        //multiAuthorizationSubscription.AddAuthorizationSubscription(nameof(canPunch), subject, nameof(Punch), nameof(canPunch), null);
        //this.AuthorizeAsStream(multiAuthorizationSubscription);

    }

    private void ResetTextFields()
    {
        securityMessage.text = string.Empty;
        obligations.text = string.Empty;
    }


    // Start is called before the first frame update
    void SetFields()
    {
        subjectO = GetComponent<Subject>();
        animator = GetComponent<Animator>();
    }



    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            Walk();
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            Punch();
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            Dance();
        }

        if (Input.GetKeyUp(KeyCode.J))
        {
            Jump();
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            DontPunch();
            DontWalk();
            DontDance();
            DontJump();
            ResetTextFields();
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            ResetTextFields();
        }
    }


    private void Walk()
    {
        //if (this.TryRaiseObligations(this.multiAuthorizationPublisher))
        //{
        //    animator.SetBool("isWalking", canWalk);
        //}
        ShowIfNotAllowed(nameof(Walk), canWalk);
        animator.SetBool("isWalking", canWalk);
    }

    private void DontWalk()
    {
        animator.SetBool("isWalking", false);
    }

    private void Dance()
    {
        animator.SetBool("isDancing", canDance);
    }

    private void DontDance()
    {
        animator.SetBool("isDancing", false);
    }

    private void Punch()
    {
        ShowIfNotAllowed(nameof(Punch), canPunch);
        animator.SetBool("isPunching", canPunch);
    }

    private void DontPunch()
    {
        animator.SetBool("isPunching", false);
    }

    private void Jump()
    {
        ShowIfNotAllowed(nameof(Jump), canJump);
        animator.SetBool("isJumping", canJump);

    }

    private void DontJump()
    {
        animator.SetBool("isJumping", false);
    }

    public void Log(string message)
    {
        var textFields = SceneManager.GetActiveScene().GetRootGameObjects()
            .SelectMany(g => g.GetComponentsInChildren<TextMeshProUGUI>());

        if (textFields.Any(tf => tf.name.Equals("txtObligation")))
        {
            StringBuilder sb = new ();
            
            sb.AppendLine(message);
            sb.AppendLine(textFields.FirstOrDefault(tf => tf.name.Equals("txtObligation"))!.text);
            textFields.FirstOrDefault(tf => tf.name.Equals("txtObligation"))!.text = sb. ToString();
        }

        Debug.Log(message);
    }

    private void ShowIfNotAllowed(string action, bool value)
    {
        if (!value)
        {
            securityMessage.text = $"{action} is not permitted";
        }
    }

    private void SetTextFields()
    {
        var textFields = SceneManager.GetActiveScene().GetRootGameObjects()
            .SelectMany(g => g.GetComponentsInChildren<TextMeshProUGUI>());

        if (textFields.Any())
        {
            securityMessage = textFields.FirstOrDefault(tf => tf.name.Equals("txtSecurity"));
            obligations = textFields.FirstOrDefault(tf => tf.name.Equals("txtObligation"));
        }
    }

   

}
