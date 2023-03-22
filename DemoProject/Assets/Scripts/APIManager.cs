using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    void Awake()
    {
        instance = this;
    }

    public bool IsViewPrinterStatusEnabled(string user)
    {
        return (user == "Admin" || user == "Operator");
    }

    public bool IsViewPrintJobsEnabled(string user)
    {
        return (user == "Admin" || user == "Operator");
    }

    public bool IsStartPrintJobEnabled(string user)
    {
        return (user == "Admin");
    }

    public bool IsViewMaterialEnabled(string user)
    {
        return (user == "Admin" || user == "Operator");
    }

    public bool IsAddMaterialEnabled(string user)
    {
        return (user == "Admin");
    }

    public bool IsCheckPrintHeadsEnabled(string user)
    {
        return (user == "Admin");
    }

}