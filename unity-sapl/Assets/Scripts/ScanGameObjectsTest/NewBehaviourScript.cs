using csharp.sapl.pdp.api;
using Newtonsoft.Json.Linq;
using Sapl.Components;
using Sapl.Interfaces;
using Sapl.Internal.Registry;
using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour, IComponentEnforcement
{
    private SaplRegistry registry;
    // Start is called before the first frame update
    void Start()
    {
        registry = SaplRegistry.GetRegistry();

        
        

        var script = gameObject.GetComponent<EventMethodEnforcement>();
        //script.ExecuteOnNotPermitted.AddListener(OnNotPermit);

        //registry.SetEnvironment("oldEnvorinment");

        //gameObject.AddComponent<EventMethodEnforcement>();
        //var script = gameObject.GetComponent<EventMethodEnforcement>();



        //Debug.Log("Test");


    }

    // Update is called once per frame
 

   public void OnPermit()
    {
        Debug.Log("Test OnPermit");
    }

    public void OnNotPermit()
    {
        Debug.Log("Test OnNotPermitted");
    }

    public void OnDecisionChanged(Decision d)
    {
        Debug.Log("Test OnDecisionChanged " + d.ToString());
    }

    public void ChangeEnvironment()
    {
        //var a = gameObject.GetComponent<EnforcementBase>();
        //a.action = "act";
        //registry = GameObject.FindObjectOfType<SaplRegistry>();
        //registry.SetEnvironment("newEnvorinment");

        var e = EnvironmentPoint.GetEnvironmentPoint(gameObject);
        e.Environment = "testenvironment";
    }

    public void Test()
    {
        
        registry.RegisterSubject("RegistrySubject");
        registry.SetEnvironment("RegistryEnvironment");
        var a = registry.CurrentEnvironment;
        var b = registry.GetSubjectJsonString();
        var script = gameObject.GetComponent<GameObjectEnforcement>();
        script.SubjectString = "RegistrySubject1";
    }

    public void MyMethod()
    {
        var script = gameObject.GetComponent<EventMethodEnforcement>();
        var param= new string[] {"one", "two", "three"};
        script.ExecuteMethodParam(MyHandler, null, param);
    }


    

    void MyHandler(object param)
    {
        var p = (string[])param;
        Debug.Log(p[1]);
    }



    public void Test3()
    {
        //SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        //SerializedProperty tagsProp = tagManager.FindProperty("tags");
        //SerializedProperty layersProp = tagManager.FindProperty("layers");

        

        var go = new GameObject();
        go.name = ("testName");
        go.tag = ("InactiveSphere");
        //go.AddComponent<GameObjectEnforcement>();
        //var script = go.GetComponent<GameObjectEnforcement>();
        //        var a = script.isActiveAndEnabled;
        var b = this.enabled;
       //script.SubjectString = "SubjectString";
        //script.SubjectGameObject = go;
        registry.RegisterGameObjectSubject(go);
        //registry.RegisterSubject("testSubject");
    }
    private void OnDisable()
    {
        //Debug.Log("Test2");
    }

    private void OnEnable()
    {
        //Debug.Log("Test1");

    }
    public void TestEP()
    {
        var ep = EnvironmentPoint.GetEnvironmentPoint(gameObject);
        //Destroy(ep);
        ep.Environment = "XXXXX";
    }

    public void TestDisposable()
    {
         var script = gameObject.GetComponent<EventMethodEnforcement>();
        Destroy(script);
    }
}
