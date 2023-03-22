using Sapl.Interfaces;
using UnityEngine;

public class TestScript2 : MonoBehaviour, ISaplExecuteable
{
    public void OnPermit()
    {
        Debug.Log(gameObject.name + " TestScript2 OnPermit");
    }

    public void OnNotPermit()
    {
        Debug.Log(gameObject.name + " TestScript2 OnNotPermit");
    }

    public void OnClick()
    {
        Debug.Log("Klick");
        //var script = gameObject.GetComponent<EventMethodEnforcement>();
        //var script = gameObject.AddComponent<EventMethodEnforcement>();
        

        //script.ExecuteMethodParam(TestParam,TestParamNotPermit);
    }

    public void TestParam(object args)
    {

    }

    public void TestParamNotPermit(object args)
    {
        
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
