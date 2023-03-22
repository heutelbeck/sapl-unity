#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public sealed class SecurityManager
{
    private static readonly object padlock = new object();
    private static SecurityManager current;
    SecurityManager()
    {

    }
    public static SecurityManager Current
    {
        get
        {
            lock (padlock)
            {
                if (current == null)
                {
                    current = new SecurityManager();
                }
                return current;
            }
        }
    }

    public void Init()
    {
        if (current == null)
        {
            current = this;
        }
    }

    private List<string> Scenes()
    {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            //if (scene.enabled)
            scenes.Add(scene.path);
        }
        return scenes;
    }   

    private List<Component> GetComponents()
    {
        var gameobjects = GameObject.FindObjectsOfType<Component>().ToList();
        List<Component> components = new List<Component>();
        foreach (var item in gameobjects)
        {
            if (!components.Any(c=>c.name.Equals(item.name)))
            {
                components.Add(item);
            }
        }
        return components;
    }

    private List<Button> GetButtons()
    {
        return GameObject.FindObjectsOfType<Button>().ToList();
    }

    public StringBuilder ScenesBuilder()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Alle Scenen des Projektes:");
        foreach (var item in Scenes())
        {
            builder.AppendLine(item);
        }
        return builder;
    }

    public StringBuilder Buttons()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Alle geladenen Buttons:");
        foreach (var item in GetButtons())
        {
            builder.AppendLine(item.name);
        }
        return builder;
    }


    public StringBuilder ActiveComponents()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Alle geladenen Komponenten:");
        foreach (var item in GetComponents())
        {
            builder.AppendLine(item.name);
        }
        return builder;
    }

}
#endif