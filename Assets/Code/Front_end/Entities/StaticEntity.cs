using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StaticEntity : IEntity, System.ICloneable
{
    private List<KeyValuePair<string, object>> m_properties = new List<KeyValuePair<string, object>>();

    [System.NonSerialized]
    private List<IEntityScript> m_entityScripts;
    [System.NonSerialized]
    private GameObject m_gameObject;

    public StaticEntity()
    {
        SetProperty("identity", "New Static Entity");
        SetProperty("faction", "player");
    }

    /// <inheritdoc/>
    /// <summary>
    /// the name of the actor
    /// </summary>
    public string identity
    {
        get { return GetProperty<string>("identity"); }
        set { SetProperty("identity", value); }
    }

    /// <summary>
    /// all scripts for the entity
    /// </summary>
    public List<IEntityScript> scripts
    {
        get
        {
            return m_entityScripts;
        }
    }

    /// <summary>
    /// The x position of the tile the entity is currently standing on.
    /// Use transform.position.x for the entity scene position
    /// </summary>
    public int x
    {
        get
        {
            return WorldManager.TileAtGamePos(m_gameObject.transform.position).worldX;
        }
    }

    /// <summary>
    /// The y position of the tile the entity is currently standing on.
    /// Use transform.position.y for the entity scene position
    /// </summary>
    public int y
    {
        get
        {
            return WorldManager.TileAtGamePos(m_gameObject.transform.position).worldY;
        }
    }

    public EntityType type
    {
        get
        {
            return EntityType.Static;
        }
    }

    public Sprite GetSprite(string type)
    {
        return null;
    }

    public GameObject gameObject
    {
        get
        {
            return m_gameObject;
        }
        set
        {
            m_gameObject = value;
        }
    }

    //Reflection
    public Actor Clone()
    {
        return (Actor)this.MemberwiseClone();
    }

    object System.ICloneable.Clone()
    {
        return Clone();
    }

    //properties

    public List<KeyValuePair<string, object>> GetAllProperties()
    {
        return m_properties;
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


    /// <summary>
    /// The event fired when the entity is spawned into the game world
    /// </summary>
    public void OnSpawn()
    {
        ManagerInstance.Get<FogOfWarManager>().RegisterVision(this, 15);
    }

    /// <summary>
    /// the event fired when the entity is destroyed in the game world
    /// </summary>
    public void OnDestroy()
    {

    }

    public void OnUpdate(float deltaTime)
    {

    }

    public void OnSelect()
    {

    }


    public void OnAttack(IEntity target)
    {
        throw new System.NotImplementedException();
    }

    public void OnAttacked(IEntity attacker)
    {
        throw new System.NotImplementedException();
    }

    public void OnDeselect()
    {
        throw new System.NotImplementedException();
    }
}
