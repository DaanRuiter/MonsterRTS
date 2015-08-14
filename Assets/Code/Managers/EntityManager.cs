using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

public class EntityManager : BManager {

    private List<IEntity> m_activeEntities;
    private Database<Actor, EntityGraphics> m_actors;
    private Database<StaticEntity, EntityGraphics> m_staticEntities;
    private Database<DynamicEntity, EntityGraphics> m_dynamicEntities;

    private void Awake()
    {
        if (m_activeEntities == null)
            Init();
    }

    private void Init()
    {
        m_activeEntities = new List<IEntity>();
        m_actors = new Database<Actor, EntityGraphics>("entities/actors", "actor");
        m_staticEntities = new Database<StaticEntity, EntityGraphics>("entities/static", "staticEntity");
        m_dynamicEntities = new Database<DynamicEntity, EntityGraphics>("entities/dynamic", "dynamicEntity");

        m_actors.LoadItems();
        m_staticEntities.LoadItems();
        m_dynamicEntities.LoadItems();
    }

    public List<IEntity> loadedEntities
    {
        get
        {
            List<IEntity> entities = new List<IEntity>();

            for (int i = 0; i < m_actors.loadedData.Count; i++)
            {
                entities.Add(m_actors.loadedData[i]);
            }
            for (int i = 0; i < m_staticEntities.loadedData.Count; i++)
            {
                entities.Add(m_staticEntities.loadedData[i]);
            }
            for (int i = 0; i < m_dynamicEntities.loadedData.Count; i++)
            {
                entities.Add(m_dynamicEntities.loadedData[i]);
            }

            return entities;
        }
    }

    public List<IEntity> activeEntities
    {
        get
        {
            return m_activeEntities;
        }
    }

    public void SpawnEntityFromSave(IEntity entity)
    {
        if (m_activeEntities == null)
            Init();

        Vector3 position = WorldManager.GamePosAtTile(ManagerInstance.Get<WorldManager>().completeMap[entity.GetProperty<int>("x"), entity.GetProperty<int>("y")]);
        GameObject gameObject = GameObject.Instantiate(Resources.Load("Entities/" + entity.type), position, Quaternion.identity) as GameObject;

        switch (entity.type)
        {
            case EntityType.Actor:
                gameObject.GetComponent<EntityComponent>().SetData(entity, m_actors.GetGraphicsFor(entity.identity) as EntityGraphics);
                break;
            case EntityType.Static:
                gameObject.GetComponent<EntityComponent>().SetData(entity, m_staticEntities.GetGraphicsFor(entity.identity) as EntityGraphics);
                break;
            case EntityType.Dynamic:
                gameObject.GetComponent<EntityComponent>().SetData(entity, m_dynamicEntities.GetGraphicsFor(entity.identity) as EntityGraphics);
                break;
        }

        m_activeEntities.Add(entity);
    }

    public void SpawnEntity(string identity, EntityType type, int x, int y)
    {
        Vector3 position = WorldManager.GamePosAtTile(ManagerInstance.Get<WorldManager>().completeMap[x, y]);
        GameObject gameObject = GameObject.Instantiate(Resources.Load("Entities/" + type), position, Quaternion.identity) as GameObject;

        bool foundData = false;
        switch (type)
        {
            case EntityType.None:
                break;
            case EntityType.Actor:
                for (int i = 0; i < m_actors.loadedData.Count; i++)
                {
                    if(m_actors.loadedData[i].identity == identity)
                    {
                        gameObject.GetComponent<EntityComponent>().SetData(m_actors.loadedData[i].Clone(), m_actors.loadedGraphics[i]);
                        foundData = true;
                    }
                }
                break;
            case EntityType.Static:
                for (int i = 0; i < m_staticEntities.loadedData.Count; i++)
                {
                    if (m_staticEntities.loadedData[i].identity == identity)
                    {
                        gameObject.GetComponent<EntityComponent>().SetData(m_staticEntities.loadedData[i].Clone(), m_staticEntities.loadedGraphics[i]);
                        foundData = true;
                    }
                }
                break;
            case EntityType.Dynamic:
                for (int i = 0; i < m_dynamicEntities.loadedData.Count; i++)
                {
                    if (m_dynamicEntities.loadedData[i].identity == identity)
                    {
                        gameObject.GetComponent<EntityComponent>().SetData(m_dynamicEntities.loadedData[i].Clone(), m_dynamicEntities.loadedGraphics[i]);
                        foundData = true;
                    }
                }
                break;
            default:
                break;
        }

        m_activeEntities.Add(gameObject.GetComponent<EntityComponent>().entity);
        if (!foundData)
        {
            Destroy(gameObject);
            CommandInput.DebugLog(new string[] {"Entity Not Found"});
        }
    }

    public void UnRegisterEntity(IEntity entity)
    {
        if (m_activeEntities.Contains(entity))
            m_activeEntities.Remove(entity);
    }
}
