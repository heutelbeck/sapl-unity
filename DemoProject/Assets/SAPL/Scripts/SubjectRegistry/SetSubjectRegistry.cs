using Sapl.Internal.Registry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSubjectRegistry : MonoBehaviour
{
    public void SetSubject(string subject)
    {
        GameObject.Find("SaplRegistry").GetComponent<SaplRegistry>().RegisterSubject(subject);
    }
}
