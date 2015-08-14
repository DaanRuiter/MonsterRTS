using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public interface IGraphics
{
    string identity
    {
        get;
    }

    Color[] texture
    {
        get;
    }

    void Load(string identity, Texture2D texture);
}

public class EntityGraphics : IGraphics {
    
    private string m_entityIdentity;
    private Color[] m_texture;

    private Sprite m_defaultSprite;

    public void Load(string identity, Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;
        m_texture = texture.GetPixels();
        m_entityIdentity = identity;
        m_defaultSprite = Sprite.Create(texture, new Rect(0, 32, 32, 32), new Vector2(0.5f, 0.5f));
    }

    public string identity
    {
        get
        {
            return m_entityIdentity;
        }
    }

    public Sprite defaultSprite
    {
        get
        {
            return m_defaultSprite;
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
