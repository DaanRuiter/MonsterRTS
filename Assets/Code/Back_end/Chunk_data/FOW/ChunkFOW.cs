using UnityEngine;
using System.Collections;

public class ChunkFOW : MonoBehaviour {

    private ChunkFOWTile[,] m_fogMap;

    public void Init()
    {
        m_fogMap = new ChunkFOWTile[Chunk.WIDTH, Chunk.HEIGHT];

        for (int x = 0; x < Chunk.WIDTH; x++)
        {
            for (int y = 0; y < Chunk.HEIGHT; y++)
            {
                m_fogMap[x, y] = new ChunkFOWTile(0.55f, this);
            }
        }

        GetComponent<Renderer>().material.mainTexture = fogTexture;

    }

    private Texture2D fogTexture
    {
        get
        {
            Texture2D tex = new Texture2D(Chunk.WIDTH, Chunk.HEIGHT);
            tex.filterMode = FilterMode.Point;

            for (int x = 0; x < Chunk.WIDTH; x++)
            {
                for (int y = 0; y < Chunk.HEIGHT; y++)
                {
                    tex.SetPixel(x, y, new Color(0, 0, 0, m_fogMap[x, y].strength));
                }
            }
            tex.Apply();
            return tex;
        }
    }

    public void DrawTexture()
    {
        GetComponent<Renderer>().material.mainTexture = fogTexture;
    }

    public ChunkFOWTile[,] fogMap
    {
        get
        {
            return m_fogMap;
        }
    }

    public void SetFogAt(int x, int y, float fogStrength)
    {
        if (x >= 0 && x < WorldManager.worldWidth && y >= 0 && y < WorldManager.worldHeight)
        {
            m_fogMap[x, y].strength = fogStrength;
        }
    }
}
