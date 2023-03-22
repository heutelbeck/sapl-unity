#if UNITY_EDITOR
using UnityEngine;

public class InitializeSAPLSecurity
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        Debug.Log("After Scene is loaded and game is running");
        Debug.Log("Scan started.");
        Debug.Log("Securitymanager initialisiert");
        Debug.Log(SecurityManager.Current.ActiveComponents());
        Debug.Log(SecurityManager.Current.ScenesBuilder());
        //Debug.Log(SecurityManager.Current.UIActicatorScenes());
        Debug.Log(SecurityManager.Current.Buttons());
        //Debug.Log(SecurityManager.Current.ButtonActivators());
    }
}
#endif
