using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class ChunkTileSerialized : IDataItem {

    //properties
    private List<KeyValuePair<string, object>> m_properties = new List<KeyValuePair<string, object>>();

    //states
    [System.NonSerialized]
    private bool m_unsaved;
    [System.NonSerialized]
    private int m_loadIndex;

    public ChunkTileSerialized()
    {

    }

    public void SetDefaultProperties(string tileIdentity, bool traversable)
    {
        if (GetProperty<string>("identity") == null)
            SetProperty("identity", tileIdentity);
        if (GetProperty<bool>("traversable") == null)
            SetProperty("traversable", traversable);
        if (GetProperty<int>("height") == null)
            SetProperty("height", 0);
    }

    public string identity
    {
        get
        {
            return GetProperty<string>("identity");
        }
        set
        {
            SetProperty("identity", value);
            m_unsaved = true;
        }
    }

    public bool traversable
    {
        get
        {
            return GetProperty<bool>("traversable");
        }
        set
        {
            SetProperty("traversable", value);
            m_unsaved = true;
        }
    }

    public string dataPath
    {
        get
        {
            return Application.dataPath + "/../data/" + identity;
        }
    }

    public TileGraphics graphics
    {
        get
        {
            return ManagerInstance.Get<DatabaseManager>().dataBase.loadedGraphics[m_loadIndex];
        }
    }

    //properties

    public List<KeyValuePair<string, object>> GetAllProperties()
    {
        return m_properties;
    }

    public T GetProperty<T>(string key)
    {

        for (int i = 0; i < m_properties.Count; i++)
        {
            if (m_properties[i].Key == key)
            {
                return (T)m_properties[i].Value;
            }
        }
        return default(T);
    }

    public void SetProperty(string key, object value)
    {
        bool exists = false;
        for (int i = 0; i < m_properties.Count; i++)
        {
            if (m_properties[i].Key == key)
            {
                m_properties[i] = new KeyValuePair<string, object>(key, value);
                exists = true;
            }
        }
        if (!exists)
        {
            m_properties.Add(new KeyValuePair<string, object>(key, value));
        }
    }
}
