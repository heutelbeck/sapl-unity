using Sapl.Interfaces;
using UnityEngine;

public class TestScript : MonoBehaviour, ISaplExecuteable, IComponentEnforcement
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPermit()
    {
        Debug.Log(gameObject.name + " TestScript OnPermit");
    }

    public void OnNotPermit()
    {
        Debug.Log(gameObject.name + " TestScript OnNotPermit");
    }
}
