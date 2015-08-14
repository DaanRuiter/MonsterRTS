using UnityEngine;
using System.Collections.Generic;

public class ChunkTile
{
    private List<KeyValuePair<string, object>> m_properties;

    [System.NonSerialized]
    private Chunk m_chunkRef;
    [System.NonSerialized]
    private ChunkTileDrawmap m_drawMap;

    public ChunkTile(int x, int y, string tileType, bool transversable, Chunk chunkRef)
    {
        m_properties = new List<KeyValuePair<string, object>>();

        SetProperty("x", x);
        SetProperty("y", y);
        SetProperty("identity", tileType);
        SetProperty("transversable", transversable);

        m_chunkRef = chunkRef;
        m_drawMap = new ChunkTileDrawmap();
        AddToDrawQueue();
    }

    //properties
    public bool tranversable
    {
        get
        {
            return GetProperty<bool>("transversable");
        }
        set
        {
            SetProperty("transversable", value);
        }
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
        }
    }

    public int height
    {
        get
        {
            return GetProperty<int>("height");
        }
        set
        {
            SetProperty("height", value);
        }
    }

    public int x
    {
        get
        {
            return GetProperty<int>("x");
        }
    }

    public int y
    {
        get
        {
            return GetProperty<int>("y");
        }
    }

    public int worldX
    {
        get
        {
            return (chunk.x * Chunk.WIDTH + x);
        }
    }

    public int worldY
    {
        get
        {
            return (chunk.y * Chunk.HEIGHT + y);
        }
    }

    public Chunk chunk
    {
        get
        {
            return m_chunkRef;
        }
        set
        {
            m_chunkRef = value;
        }
    }

    public ChunkTileDrawmap drawMap
    {
        get
        {
            return m_drawMap;
        }
    }

    public void CopyProperties(List<KeyValuePair<string, object>> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            SetProperty(list[i].Key, list[i].Value);
        }
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

    public List<ChunkTile> GetAllNeighbors()
    {
        List<ChunkTile> neighbors = new List<ChunkTile>();
        ChunkTile[,] fullMap = ManagerInstance.Get<WorldManager>().completeMap;

        if(worldX > 0)
        {
            neighbors.Add( fullMap[worldX - 1, worldY]);
            if(worldY < WorldManager.worldHeight * Chunk.HEIGHT - 1)
                neighbors.Add(fullMap[worldX - 1, worldY + 1]);
            if(worldY > 0)
                neighbors.Add(fullMap[worldX - 1, worldY - 1]);
        }
        if(worldY > 0)
        {
            neighbors.Add(fullMap[worldX, worldY - 1]);
            if (worldX < WorldManager.worldWidth * Chunk.WIDTH - 1)
                neighbors.Add(fullMap[worldX + 1, worldY - 1]);
        }
        if(worldY < WorldManager.worldHeight * Chunk.HEIGHT - 1)
        {
            neighbors.Add(fullMap[worldX, worldY + 1]);
            if(worldX < WorldManager.worldWidth * Chunk.WIDTH - 1)
                neighbors.Add( fullMap[worldX + 1, worldY + 1]);
        }
        if (worldX < WorldManager.worldWidth * Chunk.WIDTH - 1)
             neighbors.Add(fullMap[worldX + 1, worldY]);

        return neighbors;
    }

    public ChunkTile GetNeighbor(string position)
    {
        List<ChunkTile> neighbors = GetAllNeighbors();

        switch (position)
        {
            case "left":
                return neighbors[0];
            case "top-left":
                return neighbors[1];
            case "top":
                return neighbors[2];
            case "top-right":
                return neighbors[3];
            case "right":
                return neighbors[4];
            case "bottom-right":
                return neighbors[5];
            case "bottom":
                return neighbors[6];
            case "bottom-left":
                return neighbors[7];
            default:
                return neighbors[0];
        }
    }

    public void AddToDrawQueue()
    {
        chunk.AddTileToDrawQueue(this);
    }

    public void ChangeTo(ChunkTileSerialized tile)
    {
        chunk.ChangeBlockAt(x, y, tile);
    }

    public Node ToNode()
    {
        return ManagerInstance.Get<PathingManager>().grid[worldX, worldY];
    }

    //debugging
    public void LogDrawMap(Border[,] _drawMap)
    {
        UnityEngine.Debug.Log("X: " + worldX + " - Y: " + worldY + "  --------------");
        string l1 = "";
        string l2 = "";
        string l3 = "";
        if (_drawMap[0, 2].border)
        {
            l1 += "[ <color=green>" + _drawMap[0, 2].type + "</color> ] ";
        }
        else
        {
            l1 += "[ <color=red>" + _drawMap[0, 2].type + "</color> ] ";
        }
        if (_drawMap[1, 2].border)
        {
            l1 += "[ <color=green>" + _drawMap[1, 2].type + "</color> ] ";
        }
        else
        {
            l1 += "[ <color=red>" + _drawMap[1, 2].type + "</color> ] ";
        }
        if (_drawMap[2, 2].border)
        {
            l1 += "[ <color=green>" + _drawMap[2, 2].type + "</color> ] ";
        }
        else
        {
            l1 += "[ <color=red>" + _drawMap[2, 2].type + "</color> ] ";
        }


        if (_drawMap[0, 1].border)
        {
            l2 += "[ <color=green>" + _drawMap[0, 1].type + "</color> ] ";
        }
        else
        {
            l2 += "[ <color=red>" + _drawMap[0, 1].type + "</color> ] ";
        }
        if (_drawMap[1, 1].border)
        {
            l2 += "[ <color=green>" + _drawMap[1, 1].type + "</color> ] ";
        }
        else
        {
            l2 += "[ <color=red>" + _drawMap[1, 1].type + "</color> ] ";
        }
        if (_drawMap[2, 1].border)
        {
            l2 += "[ <color=green>" + _drawMap[2, 1].type + "</color> ] ";
        }
        else
        {
            l2 += "[ <color=red>" + _drawMap[2, 1].type + "</color> ] ";
        }


        if (_drawMap[0, 0].border)
        {
            l3 += "[ <color=green>" + _drawMap[0, 0].type + "</color> ] ";
        }
        else
        {
            l3 += "[ <color=red>" + _drawMap[0, 0].type + "</color> ] ";
        }
        if (_drawMap[1, 0].border)
        {
            l3 += "[ <color=green>" + _drawMap[1, 0].type + "</color> ] ";
        }
        else
        {
            l3 += "[ <color=red>" + _drawMap[1, 0].type + "</color> ] ";
        }
        if (_drawMap[2, 0].border)
        {
            l3 += "[ <color=green>" + _drawMap[2, 0].type + "</color> ] ";
        }
        else
        {
            l3 += "[ <color=red>" + _drawMap[2, 0].type + "</color> ] ";
        }
        UnityEngine.Debug.Log(l1);
        UnityEngine.Debug.Log(l2);
        UnityEngine.Debug.Log(l3);
    }
}
