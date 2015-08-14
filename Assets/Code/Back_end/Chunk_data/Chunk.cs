using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk 
{
    public const int WIDTH = 16;
    public const int HEIGHT = 16;

    private int m_x;
    private int m_y;
    private ChunkTile[,] m_tiles;
    private List<ChunkTile> m_tileDrawQueue;
    private ChunkObject m_chunkobject;

    public Chunk(int chunkX, int chunkY)
    {
        m_tileDrawQueue = new List<ChunkTile>();

        m_x = chunkX;
        m_y = chunkY;
        m_tiles = new ChunkTile[WIDTH, HEIGHT];

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                m_tiles[x, y] = new ChunkTile(x, y, "grass", true, this);
                m_tiles[x, y].ChangeTo(ManagerInstance.Get<DatabaseManager>().dataBase.GetDataFor(m_tiles[x, y].identity));
            }
        }
    }

    //--------------Set Methods--------------
    public void ChangeBlockAt(int x, int y, ChunkTileSerialized newTile, bool checkNeighbors = true)
    {
        if (m_tiles[x, y].identity != newTile.identity)
        {
            m_tiles[x, y].AddToDrawQueue();

            if(checkNeighbors)
            {
                List<ChunkTile> neighbors = m_tiles[x, y].GetAllNeighbors();
                for (int i = 0; i < m_tiles[x, y].GetAllNeighbors().Count; i++)
                {
                    if (neighbors[i] == null)
                        break;

                    int nY = 1;
                    int nX = 1;
                    if (m_tiles[x, y].worldX > neighbors[i].worldX)
                    {
                        nX++;
                    }
                    else if (m_tiles[x, y].worldX < neighbors[i].worldX)
                    {
                        nX--;
                    }
                    if (m_tiles[x, y].worldY > neighbors[i].worldY)
                    {
                        nY++;
                    }
                    else if (m_tiles[x, y].worldY < neighbors[i].worldY)
                    {
                        nY--;
                    }

                    if (neighbors[i].identity != newTile.identity)
                    {
                        if (neighbors[i].GetProperty<int>("height") < newTile.GetProperty<int>("height"))
                        {
                            neighbors[i].drawMap.SetBorderAt(nX, nY, BorderType.Shadow);
                        }
                        else if (neighbors[i].GetProperty<int>("height") == newTile.GetProperty<int>("height"))
                        {
                            neighbors[i].drawMap.SetBorderAt(nX, nY, BorderType.Merge);
                        }
                        if (nX == 0)
                            nX = 2;
                        else if (nX == 2)
                            nX = 0;
                        if (nY == 0)
                            nY = 2;
                        else if (nY == 2)
                            nY = 0;
                        if (neighbors[i].GetProperty<int>("height") > newTile.GetProperty<int>("height"))
                        {
                            m_tiles[x, y].drawMap.SetBorderAt(nX, nY, BorderType.Shadow);
                        }
                        else
                        {
                            m_tiles[x, y].drawMap.SetBorderAt(nX, nY, BorderType.None);
                        }
                        neighbors[i].AddToDrawQueue();
                    }
                    else
                    {
                        neighbors[i].drawMap.RemoveBorderAt(nX, nY);
                        if (nX == 0)
                            nX = 2;
                        else if (nX == 2)
                            nX = 0;
                        if (nY == 0)
                            nY = 2;
                        else if (nY == 2)
                            nY = 0;
                        m_tiles[x, y].drawMap.RemoveBorderAt(nX, nY);
                        neighbors[i].AddToDrawQueue();
                    }
                }
            }

            //mod
            if(ManagerInstance.Get<ModManager>() != null)
            {
                for (int i = 0; i < ManagerInstance.Get<ModManager>().loadedMods.Count; i++)
                {
                    if (ManagerInstance.Get<ModManager>().loadedMods[i].GetType().GetInterface("ITileScript") == typeof(ITileScript))
                    {
                        ITileScript tileScript = ((ITileScript)ManagerInstance.Get<ModManager>().loadedMods[i]);
                        for (int t = 0; t < tileScript.tiles.Length; t++)
                        {
                            if (tileScript.tiles[t] == newTile.identity)
                            {
                                tileScript.OnTileBuild(m_tiles[x, y].worldX, m_tiles[x, y].worldY);
                            }
                            else if (tileScript.tiles[t] == m_tiles[x, y].identity)
                            {
                                tileScript.OnTileDestroy(m_tiles[x, y].worldX, m_tiles[x, y].worldY);
                            }
                        }
                    }
                }
            }
            m_tiles[x, y].CopyProperties(newTile.GetAllProperties());

            if (ManagerInstance.Get<PathingManager>() != null)
                ManagerInstance.Get<PathingManager>().SetTraversableAt(m_tiles[x, y].worldX, m_tiles[x, y].worldY, newTile.traversable);
        }
    }

    //--------------Get Methods--------------
    //parameters
    public ChunkTile GetBlockAt(int x, int y)
    {
        return m_tiles[x, y];
    }

    //generic
    public ChunkTile[,] tiles
    {
        get
        {
            return m_tiles;
        }
    }

    public int x
    {
        get
        {
            return m_x;
        }
    }

    public int y
    {
        get
        {
            return m_y;
        }
    }

    public ChunkObject chunkObject
    {
        get
        {
            return m_chunkobject;
        }
        set
        {
            m_chunkobject = value;
        }
    }

    public void AddTileToDrawQueue(ChunkTile tileToAdd)
    {
        if(!m_tileDrawQueue.Contains(tileToAdd))
        {
            m_tileDrawQueue.Add(tileToAdd);
            AddChunkToDrawQueue();
        }
    }

    public void AddChunkToDrawQueue()
    {
        ManagerInstance.Get<WorldGraphicsManager>().AddToDrawQueue(this);
    }

    public List<ChunkTile> tileDrawQueue
    {
        get
        {
            return m_tileDrawQueue;
        }
    }

    public void ClearDrawQueue()
    {
        m_tileDrawQueue.Clear();
    }
}
