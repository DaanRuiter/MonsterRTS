using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IDataItem
{
    string identity
    {
        get;
    }
}

public interface IEntity : IDataItem
{
    //global properties
    List<IEntityScript> scripts
    {
        get;
    }

    int x
    {
        get;
    }

    int y
    {
        get;
    }


    //events
    void OnSpawn();
    void OnDestroy();

    void OnUpdate(float deltaTime);
    void OnAttack(IEntity target);
    void OnAttacked(IEntity attacker);

    void OnSelect();
    void OnDeselect();

    //property methods
    List<KeyValuePair<string, object>> GetAllProperties();
    void SetProperty(string key, object value);
    T GetProperty<T>(string key);

    //runtime properties
    Sprite GetSprite(string type);
    GameObject gameObject
    {
        get;
        set;
    }

    //types
    EntityType type
    {
        get;
    }
}

public enum EntityType
{
    None,
    Actor,
    Static,
    Dynamic
}
