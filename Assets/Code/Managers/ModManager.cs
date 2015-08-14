using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ModManager : BManager {

    private List<IScript> m_mods;

    private GUIStyle m_modListStyle;

    private void Start()
    {
        m_mods = new List<IScript>();

        //LoadMods();

        m_modListStyle = new GUIStyle();
        m_modListStyle.fontSize = 12;
        m_modListStyle.normal.textColor = Color.white;
    }
    
    public override void OnUpdate()
    {
        if (!loaded)
            return;

        for (int i = 0; i < m_mods.Count; i++)
        {
            m_mods[i].OnUpdate();
        }
    }

    public void LoadMods()
    {
        m_mods = new List<IScript>();
        string dataPath = Application.dataPath + "/../data/scripts/";

        string[] directories = Directory.GetDirectories(dataPath);
        int foundTileFiles = directories.Length;

            for (int i = 0; i < foundTileFiles; i++)
            {
                if (Directory.GetFiles(directories[i], "*.dll").Length > 0)
                {
                    string filePath = Directory.GetFiles(directories[i], "*.dll")[0];
                    string[] splittedPath = filePath.Split('\\');
                    Assembly.LoadFrom(directories[i] + "/" + splittedPath[splittedPath.Length - 1]);
                }
            }

        Assembly[] assembly = AppDomain.CurrentDomain.GetAssemblies();

        for (int i = 0; i < assembly.Length; i++)
        {
            for (int j = 0; j < assembly[i].GetTypes().Length; j++)
            {
                if (assembly[i].GetTypes()[j].GetInterface("IMod") == typeof(IScript))
                {
                    if (!assembly[i].GetTypes()[j].IsInterface)
                    {
                        try
                        {
                            IScript mod = Activator.CreateInstance(assembly[i].GetTypes()[j]) as IScript;

                            mod.OnStart();

                            m_mods.Add(mod);
                        }
                        catch
                        {
                            Debug.LogError("Invalid script");
                        }
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        if(loaded)
        {
            for (int i = 0; i < m_mods.Count; i++)
            {
                GUI.Label(new Rect(20, (13 * i) * 2, 400, 13), "" + m_mods[i].modName, m_modListStyle);
                GUI.Label(new Rect(35, (13 * i) * 2 + 25, 400, 13), "" + m_mods[i].modDescription, m_modListStyle);
            }
        }
    }

    public List<IScript> loadedMods
    {
        get
        {
            if (m_mods == null)
                m_mods = new List<IScript>();
            return m_mods;
        }
    }

    public bool loaded
    {
        get
        {
            if (m_mods != null)
                return true;
            return false;
        }
    }
}
