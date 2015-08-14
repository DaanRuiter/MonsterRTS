using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System;

public class Database<T, G> 
    where T : IDataItem 
    where G : IGraphics 
{

    private string m_dataPath = Application.dataPath + "/../data/";
    private string m_fileExtenstion;

    private List<T> m_loadedData;
    private List<G> m_loadedGraphics;

    public Database(string folder, string fileExtenstion)
    {
        m_loadedData = new List<T>();
        m_loadedGraphics = new List<G>();

        m_dataPath += folder;
        m_fileExtenstion = fileExtenstion;
    }

    public List<T> loadedData
    {
        get
        {
            return m_loadedData;
        }
    }

    public List<G> loadedGraphics
    {
        get
        {
            return m_loadedGraphics;
        }
    }

    public void CreateItem()
    {
        Type type = typeof(T);
        Type graphicType = typeof(G);

        T item = (T)Activator.CreateInstance(type);
        G graphics = (G) Activator.CreateInstance(graphicType);

        m_loadedData.Add(item);
        m_loadedGraphics.Add(graphics);
    }

    public void LoadItems()
    {

        m_loadedData = new List<T>();
        string[] directories = Directory.GetDirectories(m_dataPath);

        BinaryFormatter bf = new BinaryFormatter();

        int itemID = 0;

        for (int i = 0; i < directories.Length; i++)
        {
            if (Directory.GetFiles(directories[i], "*." + m_fileExtenstion).Length <= 0)
                continue;

            string filePath = Directory.GetFiles(directories[i], "*." + m_fileExtenstion)[0];
            string[] splittedPath = filePath.Split('\\');

            if(!File.Exists(directories[i] + "/" + splittedPath[splittedPath.Length - 1]))
            {
                continue;
            }
            if (!File.Exists(directories[i] + "/graphics.png"))
            {
                continue;
            }

            FileStream dataFile = File.Open(directories[i] + "/" + splittedPath[splittedPath.Length - 1], FileMode.Open);
            Type graphicType = typeof(G);

            T item = (T)bf.Deserialize(dataFile);
            G graphics = (G)Activator.CreateInstance(graphicType);
            dataFile.Close();

            Texture2D texture = new Texture2D(WorldGraphicsManager.TILE_RESOLUTION, WorldGraphicsManager.TILE_RESOLUTION);

            texture.LoadImage(File.ReadAllBytes(directories[i] + "/graphics.png"));

            graphics.Load(item.identity, texture);

            m_loadedGraphics.Add(graphics);
            m_loadedData.Add(item);

            itemID++;

        }
    }

    public void SaveItems()
    {
        for (int i = 0; i < m_loadedData.Count; i++)
		{
            Directory.CreateDirectory(m_dataPath + "/" + m_loadedData[i].identity);
            FileStream dataFile = File.Create(m_dataPath + "/" + m_loadedData[i].identity + "/" + m_loadedData[i].identity + "." + m_fileExtenstion);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(dataFile, m_loadedData[i]);
            dataFile.Close();

            if (!File.Exists(m_dataPath + "/" + m_loadedData[i].identity + "/graphics.png"))
            {
                Texture2D texture = Resources.Load("Textures/default") as Texture2D;
                byte[] png = texture.EncodeToPNG();
                File.WriteAllBytes(m_dataPath + "/" + m_loadedData[i].identity + "/graphics.png", png);
            }
		}
    }

    public Color[] GetDefaultTexture(string tileIdentity)
    {
        for (int i = 0; i < m_loadedGraphics.Count; i++)
        {
            if (m_loadedGraphics[i].identity == tileIdentity)
                return m_loadedGraphics[i].texture;
        }
        return null;
    }

    public IGraphics GetGraphicsFor(string tileIdentity)
    {
        for (int i = 0; i < m_loadedGraphics.Count; i++)
        {
            if(m_loadedGraphics[i].identity == tileIdentity)
            {
                return m_loadedGraphics[i];
            }
        }
        return null;
    }

    public T GetDataFor(string identity)
    {
        for (int i = 0; i < m_loadedData.Count; i++)
        {
            if (m_loadedData[i].identity == identity)
                return m_loadedData[i];
        }
        return default(T);
    }
}
