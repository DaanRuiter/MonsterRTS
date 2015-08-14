using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : BManager {

    public static int worldWidth = 4;
    public static int worldHeight = 4;

    public bool useFogOfWar;

    private Chunk[,] m_chunks;
    private GameObject[,] m_chunkObjects;
    private ChunkTile[,] m_completeMap;

    private void Awake()
    {
        Transform parent = null;
        if(transform.parent != null)
            parent = transform.parent;

        m_chunks = new Chunk[worldWidth, worldHeight];
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                m_chunks[x, y] = new Chunk(x, y);
            }
        }

        m_completeMap = new ChunkTile[worldWidth * Chunk.WIDTH, worldHeight * Chunk.HEIGHT];
        for (int cX = 0; cX < worldWidth; cX++)
        {
            for (int cY = 0; cY < worldHeight; cY++)
            {
                for (int tX = 0; tX < Chunk.WIDTH; tX++)
                {
                    for (int tY = 0; tY < Chunk.HEIGHT; tY++)
                    {
                        m_completeMap[cX * Chunk.WIDTH + tX, cY * Chunk.HEIGHT + tY] = m_chunks[cX, cY].tiles[tX, tY];
                    }
                }
            }
        }

        m_chunkObjects = new GameObject[worldWidth, worldHeight];
        for (int wX = 0; wX < worldWidth; wX++)
        {
            for (int wY = 0; wY < worldHeight; wY++)
            {
                Vector2 pos = new Vector2(wX * 16, wY * 16);
                m_chunkObjects[wX, wY] = GameObject.Instantiate(Resources.Load("World/Chunk"), pos, Quaternion.identity) as GameObject;
                m_chunkObjects[wX, wY].GetComponent<ChunkObject>().chunkData = m_chunks[wX, wY];
                m_chunkObjects[wX, wY].GetComponent<ChunkObject>().SetGraphics(TextureMode.Active);

                if (parent != null)
                    m_chunkObjects[wX, wY].transform.parent = parent;

                if (useFogOfWar)
                {
                    GameObject FOWChunk = GameObject.Instantiate(Resources.Load("World/Chunk FOW"), new Vector3(wX * 16, wY * 16, -1), Quaternion.identity) as GameObject;
                    FOWChunk.name = "Chunk FOW " + wX + "," + wY;
                    if (parent != null)
                        FOWChunk.transform.parent = parent;
                }
            }
        }

        if (GameManager.instance != null)
        {
            if (GameManager.instance.saveGame.name != null)
            {
                GameManager.instance.saveGame.LoadSaveIntoGame();
            }
        }

        staticMap = m_completeMap;
    }

    public void ChangeArea(ChunkTile startTile, ChunkTile endTile, ChunkTileSerialized newTile)
    {
        int width = endTile.worldX - startTile.worldX;
        int height = endTile.worldY - startTile.worldY;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ChunkTile tile = m_completeMap[startTile.worldX + x, startTile.worldY + y];

                tile.chunk.ChangeBlockAt(tile.x, tile.y, newTile);
            }
        }
    }

    public Chunk GetChunkAt(int x, int y)
    {
        if (x * y >= m_chunks.Length)
            return null;

        return m_chunks[x, y];
    }

    public ChunkObject GetChunkObjectAt(int x, int y)
    {
        return m_chunkObjects[x, y].GetComponent<ChunkObject>();
    }

    public ChunkTile[,] completeMap
    {
        get
        {
            return m_completeMap;
        }
    }

    public Chunk[,] chunks
    {
        get
        {
            return m_chunks;
        }
    }

    private static ChunkTile[,] staticMap;
    public static ChunkTile TileAtGamePos(Vector3 positon)
    {

        int x = (int)positon.x + Chunk.WIDTH / 2;
        int y = (int)positon.y + Chunk.HEIGHT / 2;

        if(x >= 0 && x < WorldManager.worldWidth * Chunk.WIDTH && y >= 0 && y < WorldManager.worldHeight * Chunk.HEIGHT)
        {
            return staticMap[x, y];
        }
        return null;
    }

    public static Vector3 GamePosAtTile(ChunkTile tile)
    {
        Vector3 pos = new Vector3(0, 0, -0.1f);

        pos.x = (tile.worldX - Chunk.WIDTH / 2) + 0.5f;
        pos.y = (tile.worldY - Chunk.HEIGHT / 2) + 0.5f;

        return pos;
    }

    public static Vector3 GamePosAtTile(Node node)
    {
        Vector3 pos = new Vector3(0, 0, -0.1f);

        pos.x = (node.gridX - Chunk.WIDTH / 2) + 0.5f;
        pos.y = (node.gridY - Chunk.HEIGHT / 2) + 0.5f;

        return pos;
    }
}
