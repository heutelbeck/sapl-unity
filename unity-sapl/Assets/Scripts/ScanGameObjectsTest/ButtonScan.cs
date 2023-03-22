using csharp.sapl.pdp.api;
using csharp.sapl.pdp.remote;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScan : MonoBehaviour
{
    private object subject;
    public object Subject { get => subject; set { if (subject != value) subject = value; } }
    private string action;
    public string Action { get => action; set { if (action != value) action = value; } }
    private string resource;
    public string Resource { get => resource; set { if (resource != value) resource = value; } }
    private string environment;
    public string Environment { get => environment; set { if (environment != value) environment = value; } }
    private AuthorizationDecision authDecision;
    public AuthorizationDecision AuthDecision { get => authDecision; set { if (authDecision != value) authDecision = value; } }
   // private Authorization auth;
    public void OnClickNewObject()
    {        
      /*  var authSubscription = new AuthorizationSubscription(
                      JValue.CreateString("test"), JValue.CreateString(Action), JValue.CreateString(Resource), JValue.CreateString(Environment)
                 );
        var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(authSubscription);
       var auth = new Authorization();
        auth.DecisionChanged += AuthorizationOnDecisionChanged;
        //auth.subscription = authSubscription;
        RemotePolicyDecisionPoint decisionPoint = new RemotePolicyDecisionPoint();
        */
        //AuthDecision = ConnectionManager.Current.Decide(authSubscription, auth, serialized);
       
        //go.GetHashCode();
        //var hash2 = go2.GetHashCode();
    }

    private void AuthorizationOnDecisionChanged(object sender, EventArgs e)
    {
        /*
        if(sender is Authorization)
        {
            authDecision = ((Authorization)sender).AuthorizationDecision;
            if (authDecision.Decision == Decision.PERMIT) CreateGameObject();
            Debug.Log(authDecision.DecisionString);
        }
        */
    }

    public void TestDecisionChanged(Decision decision)
    {
        Debug.Log("test: " + decision.ToString());
    }

    public void CreateGameObject()
    {
        var scene = SceneManager.GetActiveScene();
        var gameObjects = scene.GetRootGameObjects();

        Debug.Log("NewObject");

        Subject = this;
        Action = "create";


        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "New GameObject";
        //GameObject go2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        //go.name = "New GameObject2";
        //var t = go.GetType().ToString();
        var rend = go.GetComponent<Renderer>();
        rend.material.color = Color.red;

        //go.AddComponent<SaplMonoBehavior>();
        //var g = go.GetComponent<SaplMonoBehavior>();


        //var transform = go.transform;
        //var o = JsonUtility.ToJson(transform);//
        //var o1 = JObject.FromObject(transform);
        //var str = transform.ToString();
        //var o2 = JObject.Parse(str);
        //var o3 = JsonConvert.SerializeObject(go, Formatting.Indented);
        //var o4 = JsonConvert.SerializeObject(transform, Formatting.Indented);

    }


    public void ActionTest()
    {
        
    }
    public void OnClickSetActive()
    {
        var obj = Resources
            .FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(g => g.CompareTag("InactiveSphere"));

       var b = Resources.FindObjectsOfTypeAll(typeof(GameObject));

        if (obj != null) obj.SetActive(true);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
