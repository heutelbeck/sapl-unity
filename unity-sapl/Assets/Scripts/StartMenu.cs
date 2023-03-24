using csharp.sapl.pdp.api;
using Sapl.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : ActionSAPL
{

    public string selectedButton;
    private AuthorizationPublisher publisher;


    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

   public void StartAvatarGame()
    {
        SceneManager.LoadScene(2);
    }


   public override void EnforceOnAction(AuthorizationPublisher publisher)
   {
       this.publisher = publisher;
       base.EnforceOnAction(publisher);
   }
}
