using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGraphics : IGraphics {

    private string m_identity;
    private Color[] m_texture;
    private Color m_particleColor;

    public void Load(string identity, Texture2D texture)
    {
        m_texture = texture.GetPixels(); ;
        m_identity = identity;
        LoadParticleColor(texture.GetPixels());
    }

    public Color[] texture
    {
        get
        {
            return m_texture;
        }
    }

    public Color particleColor
    {
        get
        {
            return m_particleColor;
        }
    }

    public string identity
    {
        get
        {
            return m_identity;
        }
    }

    public Texture2D ToTexture(Color[] pixels)
    {
        Texture2D tex = new Texture2D(WorldGraphicsManager.TILE_RESOLUTION, WorldGraphicsManager.TILE_RESOLUTION);
        tex.SetPixels(pixels);
        return tex;
    }

    public void LoadParticleColor(Color[] texture)
    {
        m_particleColor = new Color(1, 1, 1);
        for (int i = 0; i < texture.Length; i++)
        {
            m_particleColor += texture[i];
        }
        m_particleColor *= 1f / texture.Length;
    }
}
