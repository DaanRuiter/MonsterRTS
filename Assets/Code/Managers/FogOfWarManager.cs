using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWarManager : BManager {

    private ChunkFOWTile[,] m_completeMap;
    private ChunkFOW[,] m_components;

    private Dictionary<IEntity, int> m_visions = new Dictionary<IEntity, int>();
    private List<Vector2> m_visionPositions = new List<Vector2>();

    private void Awake()
    {
        if (m_completeMap == null)
            Init();
    }

    private void Init()
    {
        m_completeMap = new ChunkFOWTile[WorldManager.worldWidth * Chunk.WIDTH, WorldManager.worldHeight * Chunk.HEIGHT];
        m_components = new ChunkFOW[WorldManager.worldWidth, WorldManager.worldHeight];

        int i = 0;

        for (int x = 0; x < WorldManager.worldWidth; x++)
        {
            for (int y = 0; y < WorldManager.worldHeight; y++)
            {
                i++;
                GameObject FOWChunk = GameObject.Find("Chunk FOW " + x + "," + y);

                m_components[x, y] = FOWChunk.GetComponent<ChunkFOW>();
                m_components[x, y].Init();

                for (int tX = 0; tX < Chunk.WIDTH; tX++)
                {
                    for (int tY = 0; tY < Chunk.HEIGHT; tY++)
                    {
                        m_completeMap[x * Chunk.WIDTH + tX, y * Chunk.HEIGHT + tY] = FOWChunk.GetComponent<ChunkFOW>().fogMap[tX, tY];
                    }
                }
            }
        }
    }

    public override void OnUpdate()
    {
        int i = 0;
        foreach (KeyValuePair<IEntity, int> vision in m_visions)
        {
            if(m_visionPositions[i] != new Vector2(vision.Key.x, vision.Key.y))
            {
                DrawCircle(vision.Key.x, vision.Key.y, vision.Value, 0f);
                m_visionPositions[i] = new Vector2(vision.Key.x, vision.Key.y);
            }
            i++;
        }
    }

    public void SetMap(float[,] fogMap)
    {
        if (m_completeMap == null)
            Init();

        for (int x = 0; x < WorldManager.worldWidth; x++)
        {
            for (int y = 0; y < WorldManager.worldHeight; y++)
            {
                m_completeMap[x, y].strength = fogMap[x, y];
            }
        }
        DrawFog();
    }

    public void RegisterVision(IEntity entity, int range)
    {
        if (m_visions == null)
        {
            m_visions = new Dictionary<IEntity, int>();
            m_visionPositions = new List<Vector2>();
        }

        if(!m_visions.ContainsKey(entity))
        {
            m_visions.Add(entity, range);
            m_visionPositions.Add(new Vector2(entity.x, entity.y));

            DrawCircle(entity.x, entity.y, range, 0f);
        }
    }

    public void DrawCircle(int centerX, int centerY, int radius, float fogStrength)
    {
        m_completeMap[centerX, centerY].strength = fogStrength;

        int r = radius; // radius
        int ox = centerX, oy = centerY; // origin

        for (int x = -r; x < r; x++)
        {
            int height = (int)Mathf.Sqrt(r * r - x * x);

            for (int y = -height; y < height; y++)
            {
                if (x + ox >= 0 && x + ox < WorldManager.worldWidth * Chunk.WIDTH && y + oy >= 0 && y + oy < WorldManager.worldHeight * Chunk.HEIGHT)
                    m_completeMap[x + ox, y + oy].strength = fogStrength;
            }
        }
        DrawFog();
    }

    private void DrawFog()
    {

        for (int x = 0; x < WorldManager.worldWidth; x++)
        {
            for (int y = 0; y < WorldManager.worldHeight; y++)
            {
                m_components[x, y].DrawTexture();
            }
        }
    }

    public ChunkFOWTile[,] completeMap
    {
        get
        {
            return m_completeMap;
        }
    }
}
