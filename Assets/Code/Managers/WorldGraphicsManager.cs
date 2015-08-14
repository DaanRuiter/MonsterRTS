using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TextureMode
{
    None,
    Default,
    Active,
    Serialized
}

public class WorldGraphicsManager : BManager {

    public const int TILE_RESOLUTION = 32;

    public bool playMode = true;

    private List<Chunk> m_chunkDrawQueue = new List<Chunk>();

    private List<ChunkTileCachedTexture> m_textureCache = new List<ChunkTileCachedTexture>();

    public override void OnUpdate()
    {
        if(m_chunkDrawQueue.Count > 0)
        {
            for (int i = 0; i < m_chunkDrawQueue.Count; i++)
            {
                GetTextureForChunk(m_chunkDrawQueue[i].x, m_chunkDrawQueue[i].y, TextureMode.Active);
            }
        }
        m_chunkDrawQueue.Clear();
    }

	public Texture2D GetTextureForChunk(int chunkX, int chunkY, TextureMode mode)
    {

        switch (mode)
        {
            case TextureMode.None:
                break;
            case TextureMode.Default:
                Texture2D tex = new Texture2D(Chunk.WIDTH * TILE_RESOLUTION, Chunk.HEIGHT * TILE_RESOLUTION);
                tex.filterMode = FilterMode.Point;

                for (int x = 0; x < tex.width; x++)
                {
                    for (int y = 0; y < tex.height; y++)
                    {
                        tex.SetPixel(x, y, Color.green);
                    }
                }

        tex.Apply();
        return tex;
            case TextureMode.Active:
                List<ChunkTile> drawQueue = ManagerInstance.Get<WorldManager>().GetChunkAt(chunkX, chunkY).tileDrawQueue;
                Texture2D activeTex = ManagerInstance.Get<WorldManager>().GetChunkObjectAt(chunkX, chunkY).texture;

                if (activeTex == null)
                    activeTex = new Texture2D(Chunk.WIDTH * TILE_RESOLUTION, Chunk.HEIGHT * TILE_RESOLUTION);
                    activeTex.filterMode = FilterMode.Point;

                for (int i = 0; i < drawQueue.Count; i++)
                {
                    activeTex.SetPixels(drawQueue[i].x * TILE_RESOLUTION, drawQueue[i].y * TILE_RESOLUTION, TILE_RESOLUTION, TILE_RESOLUTION, TileTexture(drawQueue[i].worldX, drawQueue[i].worldY));
                    if(drawQueue[i].identity != "default")
                        if (playMode)
                            ManagerInstance.Get<WorldManager>().GetChunkObjectAt(chunkX, chunkY).EmiteParticles(drawQueue[i].x, drawQueue[i].y, TileSelectionPanel.i.selectedTile.graphics.particleColor);
                }

                drawQueue.Clear();
                activeTex.Apply();
                return activeTex;
            case TextureMode.Serialized:
                break;
            default:
                break;
        }

        return null;
    }

    private bool NeedsRedraw()
    {
        bool result = false;

        return result;
    }

    private Color[] TileTexture(int x, int y)
    {
        ChunkTile tile = ManagerInstance.Get<WorldManager>().completeMap[x, y];

            if (x >= 0 && x < WorldManager.worldWidth * Chunk.WIDTH && y >= 0 && y < WorldManager.worldHeight * Chunk.HEIGHT)
            {
                //check for cached version
                for (int i = 0; i < m_textureCache.Count; i++)
                {
                    if(m_textureCache[i].tileIdentity == tile.identity)
                    {
                        bool foundCached = true;
                        for (int bX = 0; bX < 3; bX++)
                        {
                            for (int bY = 0; bY < 3; bY++)
                            {
                                if(m_textureCache[i].borderMap[bX, bY].border != tile.drawMap.borderMap[bX, bY].border)
                                    foundCached = false;
                                if (m_textureCache[i].borderMap[bX, bY].type != tile.drawMap.borderMap[bX, bY].type)
                                    foundCached = false;
                            }
                        }
                        if (foundCached)
                        {
                            return m_textureCache[i].texture;
                        }
                    }
                }


                Color[] pix = new Color[TILE_RESOLUTION * TILE_RESOLUTION];

                for (int i = 0; i < pix.Length; i++)
                {
                    pix[i] = ManagerInstance.Get<DatabaseManager>().dataBase.GetDefaultTexture(tile.identity)[i] * Random.Range(1.05f, 1.2f);
                }

                if(x -1 >= 0)
                        if (tile.drawMap.borderMap[0, 1].border)
                        {
                            //draw shadowMap
                            if (tile.drawMap.borderMap[0, 1].type == BorderType.Shadow)
                            {
                                for (int i = 0; i < pix.Length; i++)
                                {
                                    if (i % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.35f, 0.45f);
                                    }
                                    if ((i - 1) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.45f, 0.55f);
                                    }
                                    if ((i - 2) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.55f, 0.65f);
                                    }
                                    if ((i - 3) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.65f, 0.75f);
                                    }
                                    if ((i - 4) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.75f, 0.85f);
                                    }
                                }
                            }
                            else if (tile.drawMap.borderMap[0, 1].type == BorderType.Merge)
                            {
                                Color[] neightborTex = ManagerInstance.Get<DatabaseManager>().dataBase.GetGraphicsFor(ManagerInstance.Get<WorldManager>().completeMap[x - 1, y].identity).texture;

                                for (int i = 0; i < pix.Length; i++)
                                {
                                    Gradient gradient = new Gradient();

                                    GradientColorKey[] gck = new GradientColorKey[2];
                                    gck[0].color = pix[i];
                                    gck[0].time = 0.0f;
                                    gck[1].color = neightborTex[i];
                                    gck[1].time = 1.0f;

                                    // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
                                    GradientAlphaKey[] gak = new GradientAlphaKey[2];
                                    gak[0].alpha = 1.0f;
                                    gak[0].time = 0.0f;
                                    gak[1].alpha = 1f;
                                    gak[1].time = 1.0f;

                                    gradient.SetKeys(gck, gak);

                                    if (i % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.8f);
                                    }
                                    if ((i - 1) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.775f);
                                    }
                                    if ((i - 2) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.65f);
                                    }
                                    if ((i - 3) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.525f);
                                    }
                                    if ((i - 4) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.4f);
                                    }
                                    if ((i - 5) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.375f);
                                    }
                                    if ((i - 6) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.25f);
                                    }
                                    if ((i - 7) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.125f);
                                    }
                                    if ((i - 8) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0f);
                                    }
                                }
                            }
                        }
                if (y + 1 < WorldManager.worldHeight * Chunk.HEIGHT)
                        if (tile.drawMap.borderMap[1, 2].border)
                        {
                            if (tile.drawMap.borderMap[1, 2].type == BorderType.Shadow)
                            {
                                for (int i = 0; i < pix.Length; i++)
                                {
                                    if (i > pix.Length - (TILE_RESOLUTION + 1))
                                    {
                                        pix[i] *= Random.Range(0.35f, 0.45f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 2 && i < pix.Length - (TILE_RESOLUTION + 1))
                                    {
                                        pix[i] *= Random.Range(0.45f, 0.55f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 3 && i < pix.Length - (TILE_RESOLUTION + 1) * 2)
                                    {
                                        pix[i] *= Random.Range(0.55f, 0.65f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 4 && i < pix.Length - (TILE_RESOLUTION + 1) * 3)
                                    {
                                        pix[i] *= Random.Range(0.65f, 0.75f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 5 && i < pix.Length - (TILE_RESOLUTION + 1) * 4)
                                    {
                                        pix[i] *= Random.Range(0.75f, 0.85f);
                                    }
                                }
                            }
                            else if (tile.drawMap.borderMap[1, 2].type == BorderType.Merge)
                            {
                                Color[] neightborTex = ManagerInstance.Get<DatabaseManager>().dataBase.GetGraphicsFor(ManagerInstance.Get<WorldManager>().completeMap[x, y + 1].identity).texture;

                                for (int i = 0; i < pix.Length; i++)
                                {
                                    Gradient gradient = new Gradient();

                                    GradientColorKey[] gck = new GradientColorKey[2];
                                    gck[0].color = pix[i];
                                    gck[0].time = 0.0f;
                                    gck[1].color = neightborTex[i];
                                    gck[1].time = 1.0f;

                                    // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
                                    GradientAlphaKey[] gak = new GradientAlphaKey[2];
                                    gak[0].alpha = 1.0f;
                                    gak[0].time = 0.0f;
                                    gak[1].alpha = 1f;
                                    gak[1].time = 1.0f;

                                    gradient.SetKeys(gck, gak);

                                    if (i > pix.Length - (TILE_RESOLUTION + 1))
                                    {
                                        pix[i] = gradient.Evaluate(0.8f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 2 && i < pix.Length - (TILE_RESOLUTION + 1))
                                    {
                                        pix[i] = gradient.Evaluate(0.775f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 3 && i < pix.Length - (TILE_RESOLUTION + 1) * 2)
                                    {
                                        pix[i] = gradient.Evaluate(0.65f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 4 && i < pix.Length - (TILE_RESOLUTION + 1) * 3)
                                    {
                                        pix[i] = gradient.Evaluate(0.525f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 5 && i < pix.Length - (TILE_RESOLUTION + 1) * 4)
                                    {
                                        pix[i] = gradient.Evaluate(0.4f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 6 && i < pix.Length - (TILE_RESOLUTION + 1) * 5)
                                    {
                                        pix[i] = gradient.Evaluate(0.375f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 7 && i < pix.Length - (TILE_RESOLUTION + 1) * 6)
                                    {
                                        pix[i] = gradient.Evaluate(0.25f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 8 && i < pix.Length - (TILE_RESOLUTION + 1) * 7)
                                    {
                                        pix[i] = gradient.Evaluate(0.125f);
                                    }
                                    if (i > pix.Length - (TILE_RESOLUTION + 1) * 9 && i < pix.Length - (TILE_RESOLUTION + 1) * 8)
                                    {
                                        pix[i] = gradient.Evaluate(0f);
                                    }
                                }
                            }
                        }
                if (x + 1 < WorldManager.worldWidth * Chunk.WIDTH)
                        if (tile.drawMap.borderMap[2, 1].border)
                        {
                            if (tile.drawMap.borderMap[2, 1].type == BorderType.Shadow)
                            {
                                for (int i = 0; i < pix.Length; i++)
                                {
                                    if ((i + 1) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.35f, 0.45f);
                                    }
                                    if ((i + 2) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.45f, 0.55f);
                                    }
                                    if ((i + 3) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.55f, 0.65f);
                                    }
                                    if ((i + 4) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.65f, 0.75f);
                                    }
                                    if ((i + 5) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] *= Random.Range(0.75f, 0.85f);
                                    }
                                }
                            }
                            else if (tile.drawMap.borderMap[2, 1].type == BorderType.Merge)
                            {
                                Color[] neightborTex = ManagerInstance.Get<DatabaseManager>().dataBase.GetGraphicsFor(ManagerInstance.Get<WorldManager>().completeMap[x + 1, y].identity).texture;

                                for (int i = 0; i < pix.Length; i++)
                                {
                                    Gradient gradient = new Gradient();

                                    GradientColorKey[] gck = new GradientColorKey[2];
                                    gck[0].color = pix[i];
                                    gck[0].time = 0.0f;
                                    gck[1].color = neightborTex[i];
                                    gck[1].time = 1.0f;

                                    // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
                                    GradientAlphaKey[] gak = new GradientAlphaKey[2];
                                    gak[0].alpha = 1.0f;
                                    gak[0].time = 0.0f;
                                    gak[1].alpha = 1f;
                                    gak[1].time = 1.0f;

                                    gradient.SetKeys(gck, gak);

                                    if ((i + 1) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.8f);
                                    }
                                    if ((i + 2) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.775f);
                                    }
                                    if ((i + 3) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.65f);
                                    }
                                    if ((i + 4) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.525f);
                                    }
                                    if ((i + 5) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.4f);
                                    }
                                    if ((i + 6) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.375f);
                                    }
                                    if ((i + 7) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.25f);
                                    }
                                    if ((i + 8) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0.125f);
                                    }
                                    if ((i + 9) % TILE_RESOLUTION == 0)
                                    {
                                        pix[i] = gradient.Evaluate(0f);
                                    }
                                }
                            }
                        }
                if (y - 1 >= 0)
                        if (tile.drawMap.borderMap[1, 0].border)
                        {
                            if (tile.drawMap.borderMap[1, 0].type == BorderType.Shadow)
                            {
                                for (int i = 0; i < pix.Length; i++)
                                {
                                    //TODO Fix 1 pixelline shade
                                    if (i < TILE_RESOLUTION)
                                    {
                                        pix[i] *= Random.Range(0.35f, 0.45f);
                                    }
                                    if (i < TILE_RESOLUTION * 2 && i > TILE_RESOLUTION)
                                    {
                                        pix[i] *= Random.Range(0.45f, 0.55f);
                                    }
                                    if (i < TILE_RESOLUTION * 3 && i > TILE_RESOLUTION * 2)
                                    {
                                        pix[i] *= Random.Range(0.55f, 0.65f);
                                    }
                                    if (i < TILE_RESOLUTION * 4 && i > TILE_RESOLUTION * 3)
                                    {
                                        pix[i] *= Random.Range(0.65f, 0.75f);
                                    }
                                    if (i < TILE_RESOLUTION * 5 && i > TILE_RESOLUTION * 4)
                                    {
                                        pix[i] *= Random.Range(0.75f, 0.85f);
                                    }
                                }
                            }
                            else if (tile.drawMap.borderMap[1, 0].type == BorderType.Merge)
                            {
                                Color[] neightborTex = ManagerInstance.Get<DatabaseManager>().dataBase.GetGraphicsFor(ManagerInstance.Get<WorldManager>().completeMap[x, y - 1].identity).texture;

                                for (int i = 0; i < pix.Length; i++)
                                {
                                    Gradient gradient = new Gradient();

                                    GradientColorKey[] gck = new GradientColorKey[2];
                                    gck[0].color = pix[i];
                                    gck[0].time = 0.0f;
                                    gck[1].color = neightborTex[i];
                                    gck[1].time = 1.0f;

                                    // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
                                    GradientAlphaKey[] gak = new GradientAlphaKey[2];
                                    gak[0].alpha = 1.0f;
                                    gak[0].time = 0.0f;
                                    gak[1].alpha = 1f;
                                    gak[1].time = 1.0f;

                                    gradient.SetKeys(gck, gak);

                                    if (i < TILE_RESOLUTION)
                                    {
                                        pix[i] = gradient.Evaluate(0.8f);
                                    }
                                    if (i < TILE_RESOLUTION * 2 && i > TILE_RESOLUTION)
                                    {
                                        pix[i] = gradient.Evaluate(0.775f);
                                    }
                                    if (i < TILE_RESOLUTION * 3 && i > TILE_RESOLUTION * 2)
                                    {
                                        pix[i] = gradient.Evaluate(0.65f);
                                    }
                                    if (i < TILE_RESOLUTION * 4 && i > TILE_RESOLUTION * 3)
                                    {
                                        pix[i] = gradient.Evaluate(0.525f);
                                    }
                                    if (i < TILE_RESOLUTION * 5 && i > TILE_RESOLUTION * 4)
                                    {
                                        pix[i] = gradient.Evaluate(0.4f);
                                    }
                                    if (i < TILE_RESOLUTION * 6 && i > TILE_RESOLUTION * 5)
                                    {
                                        pix[i] = gradient.Evaluate(0.375f);
                                    }
                                    if (i < TILE_RESOLUTION * 7 && i > TILE_RESOLUTION * 6)
                                    {
                                        pix[i] = gradient.Evaluate(0.25f);
                                    }
                                    if (i < TILE_RESOLUTION * 8 && i > TILE_RESOLUTION * 7)
                                    {
                                        pix[i] = gradient.Evaluate(0.125f);
                                    }
                                    if (i < TILE_RESOLUTION * 9 && i > TILE_RESOLUTION * 8)
                                    {
                                        pix[i] = gradient.Evaluate(0f);
                                    }
                                }
                            }
                        }

                tile.drawMap.Redraw();

                //cache the new texture
                ChunkTileCachedTexture textureCache = new ChunkTileCachedTexture(tile.identity, tile.drawMap.borderMap, pix);
                m_textureCache.Add(textureCache);

                return pix;
            }
            return DebugTexture(tile.identity);
    }

    private Color[] DebugTexture(string type)
    {
        Color[] col = new Color[TILE_RESOLUTION * TILE_RESOLUTION];
        for (int i = 0; i < col.Length; i++)
        {
            switch (type)
            {
                case "default":
                    col[i] = Color.green;
                    break;
                case "default red":
                    col[i] = Color.red;
                    break;
            }
        }

        return col;
    }

    private Color DebugColor(string type)
    {
        Color col = new Color();
            switch (type)
            {
                case "default":
                    col = Color.green;
                    break;
                case "default red":
                    col = Color.red;
                    break;
            }
        return col;
    }

    private Color Mix(Color from, Color to, float percent)
    {
        float amountFrom = 1.0f - percent;

        return new Color(
        (int)(from.a * amountFrom + to.a * percent),
        (int)(from.r * amountFrom + to.r * percent),
        (int)(from.g * amountFrom + to.g * percent),
        (int)(from.b * amountFrom + to.b * percent));
    }

    public void AddToDrawQueue(Chunk chunkToAdd)
    {
        if (m_chunkDrawQueue == null)
            m_chunkDrawQueue = new List<Chunk>();

        if(!m_chunkDrawQueue.Contains(chunkToAdd))
            m_chunkDrawQueue.Add(chunkToAdd);
    }
}
