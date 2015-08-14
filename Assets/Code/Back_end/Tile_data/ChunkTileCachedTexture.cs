using UnityEngine;
using System.Collections.Generic;

public class ChunkTileCachedTexture {

    private string m_tileIdentity;
    private Border[,] m_borders;
    private Color[] m_texture;

    public ChunkTileCachedTexture(string tileIdentity, Border[,] borderMap, Color[] texturePixels)
    {
        m_tileIdentity = tileIdentity;
        m_borders = new Border[3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                m_borders[x, y] = new Border(borderMap[x, y].type, borderMap[x, y].border);
            }
        }
        m_texture = texturePixels;
    }

    public string tileIdentity
    {
        get
        {
            return m_tileIdentity;
        }
    }

    public Border[,] borderMap
    {
        get
        {
            return m_borders;
        }
    }

    public Color[] texture
    {
        get
        {
            return m_texture;
        }
    }
}
