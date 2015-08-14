using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System;

public enum SaveType
{
    Vanilla,
    Tutorial,
    Mod
}

[System.Serializable]
public class SaveGame {

    private string m_saveName;

    private string[,] m_tiles;
    private float[,] m_FOW;
    private List<IEntity> m_entities;
    private SaveType m_saveType;

    private List<IEntity> m_entityDependencies;
    private List<ChunkTileSerialized> m_tileDependencies;

    public bool loaded = false;

    public SaveGame(string saveName)
    {
        m_saveName = saveName;
    }

    public string name
    {
        get
        {
            return m_saveName;
        }
    }

    public SaveType type
    {
        get
        {
            return m_saveType;
        }
        set
        {
            m_saveType = value;
        }
    }

    public IEnumerator Save()
    {
        //TODO Update Save UI - Saving terrain;
        Popup.Message("Saving..", "Saving Terrain");
        yield return new WaitForEndOfFrame();

        m_tiles = new string[WorldManager.worldWidth * Chunk.WIDTH, WorldManager.worldHeight * Chunk.HEIGHT];
        ChunkTile[,] map = ManagerInstance.Get<WorldManager>().completeMap;
        for (int x = 0; x < WorldManager.worldWidth * Chunk.WIDTH; x++)
        {
            for (int y = 0; y < WorldManager.worldHeight * Chunk.HEIGHT; y++)
            {
                m_tiles[x, y] = map[x, y].identity;
            }
        }

        //TODO Update Save UI - Saving entities;
        Popup.Message("Saving..", "Saving Entities");
        yield return new WaitForEndOfFrame();

        m_entities = ManagerInstance.Get<EntityManager>().activeEntities;

        for (int i = 0; i < m_entities.Count; i++)
        {
            m_entities[i].SetProperty("x", m_entities[i].x);
            m_entities[i].SetProperty("y", m_entities[i].y);

            Debug.Log(m_entities[i].x);
            Debug.Log(m_entities[i].y);
        }

        //TODO Update Save UI - Saving Fog of War;
        Popup.Message("Saving..", "Saving Fog of War");
        yield return new WaitForEndOfFrame();

        m_FOW = new float[WorldManager.worldWidth * Chunk.WIDTH, WorldManager.worldHeight * Chunk.HEIGHT];
        ChunkFOWTile[,] FOWTiles = ManagerInstance.Get<FogOfWarManager>().completeMap;
        for (int x = 0; x < WorldManager.worldWidth * Chunk.WIDTH; x++)
        {
            for (int y = 0; y < WorldManager.worldHeight * Chunk.HEIGHT; y++)
            {
                m_FOW[x, y] = FOWTiles[x, y].strength;
            }
        }

        //TODO Update Save UI - Saving dependencies - entities;
        Popup.Message("Saving..", "Saving Dependencies - Entities");
        yield return new WaitForEndOfFrame();

        m_entityDependencies = ManagerInstance.Get<EntityManager>().loadedEntities;

        //TODO Update Save UI - Saving dependencies - tiles;
        Popup.Message("Saving..", "Saving Dependencies - Tiles");
        yield return new WaitForEndOfFrame();

        m_tileDependencies = ManagerInstance.Get<DatabaseManager>().dataBase.loadedData;

        //TODO Update Save UI - Writing to disk
        Popup.Message("Saving..", "Writing To Disk");
        yield return new WaitForEndOfFrame();

        if(m_saveName == null)
            m_saveName = System.DateTime.Now.ToString();

        char[] chars = {'/', ':', ' '};
        string trimmedName = m_saveName.Trim(chars);

        //TODO Update Save UI - Writing to disk
        yield return new WaitForEndOfFrame();

        bool succes = true;
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            string dataPath = Application.dataPath + "/../data/saves/" + m_saveType.ToString().ToLower() + "/" + trimmedName;
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            FileStream file = File.Create(dataPath + "/" + trimmedName + ".save");
            bf.Serialize(file, this);
            file.Close();
        }catch(FileNotFoundException error)
        {
            Debug.LogError(error);
            succes = false;
        }

        Popup.Message("Saved", "Game Saved.");
        loaded = true;
        yield break;
    }

    public static SaveGame LoadSaveFile(string saveName)
    {
        SaveGame result = null;

        //TODO Check dependencies

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            string dataPath = Application.dataPath + "/../data/saves/" + saveName;
            if (!Directory.Exists(dataPath))
                return result;

            FileStream file = File.Open(dataPath + "/" + saveName + ".save", FileMode.Open);
            result = bf.Deserialize(file) as SaveGame;

            file.Close();
        }
        catch (FileNotFoundException error)
        {
            Debug.LogError(error);
        }
        return result;
    }

    public static SaveGame LoadFromPath(string path)
    {
        SaveGame result = null;

        //TODO Check dependencies

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (!File.Exists(path))
                return result;

            FileStream file = File.Open(path, FileMode.Open);
            result = bf.Deserialize(file) as SaveGame;
            file.Close();
        }
        catch (FileNotFoundException error)
        {
            Debug.LogError(error);
        }
        return result;
    }

    /*
     * 
    private string[,] m_tiles;
    private float[,] m_FOW;
    private List<IEntity> m_entities;

    private List<IEntity> m_entityDependencies;
    private List<ChunkTileSerialized> m_tileDependencies;
     * */

    public void LoadSaveIntoGame()
    {
        ChunkTile[,] map = ManagerInstance.Get<WorldManager>().completeMap;
        for (int x = 0; x < WorldManager.worldWidth * Chunk.WIDTH; x++)
        {
            for (int y = 0; y < WorldManager.worldHeight * Chunk.HEIGHT; y++)
            {
                map[x, y].ChangeTo(ManagerInstance.Get<DatabaseManager>().dataBase.GetDataFor(m_tiles[x, y]));
            }
        }

        if (ManagerInstance.Get<FogOfWarManager>() != null)
            ManagerInstance.Get<FogOfWarManager>().SetMap(m_FOW);

        for (int i = 0; i < m_entities.Count; i++)
        {
            ManagerInstance.Get<EntityManager>().SpawnEntityFromSave(m_entities[i]);
        }
    }
}
