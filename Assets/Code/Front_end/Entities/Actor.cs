using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Actor : IEntity, System.ICloneable
{
    private List<KeyValuePair<string, object>> m_properties = new List<KeyValuePair<string, object>>();

    [System.NonSerialized]
    private List<IEntityScript> m_entityScripts;
    [System.NonSerialized]
    private GameObject m_gameObject;
    [System.NonSerialized]
    private List<Node> m_currentPath;
    [System.NonSerialized]
    private List<Node> m_displayPath;

    public Actor()
    {
        SetProperty("identity", "New Actor");
        SetProperty("hitPoints", "10");
        SetProperty("faction", "player");
    }

    #region Getters/Setters

    /// <inheritdoc/>
    /// <summary>
    /// the name of the actor
    /// </summary>
    public string identity
    {
        get { return GetProperty<string>("identity"); }
        set { SetProperty("identity", value); }
    }
    public int hitPoints
    {
        get { return GetProperty<int>("hitPoints"); }
        set { SetProperty("hitPoints", value); }
    }
    public string faction
    {
        get { return GetProperty<string>("faction"); }
        set { SetProperty("faction", value); }
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
        set
        {
            m_gameObject.transform.position = WorldManager.GamePosAtTile(ManagerInstance.Get<WorldManager>().completeMap[value, y]);
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
        set
        {
            m_gameObject.transform.position = WorldManager.GamePosAtTile(ManagerInstance.Get<WorldManager>().completeMap[x, value]);
        }
    }

    public EntityType type
    {
        get
        {
            return EntityType.Actor;
        }
    }

    public Sprite GetSprite(string type)
    {
        return null;
    }

    public ChunkTile currentTile
    {
        get
        {
            return WorldManager.TileAtGamePos(m_gameObject.transform.position);
        }
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

    public List<Node> currentPath
    {
        get
        {
            return m_currentPath;
        }
        set
        {
            m_currentPath = value;
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
            if(m_properties[i].Key == key)
            {
                return (T) m_properties[i].Value;
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

    #endregion

    //CUSTOM ACTOR BEHAVIOUR
    #region Custom Actor Properties

    [System.NonSerialized]
    private bool m_selected;
    [System.NonSerialized]
    private SpriteRenderer m_renderer;
    [System.NonSerialized]
    private LineRenderer m_line;

    private List<ActorJob> m_jobQueue;

    #endregion

    #region Entity Events
    /// <summary>
    /// The event fired when the entity is spawned into the game world
    /// </summary>
    public void OnSpawn()
    {
        ManagerInstance.Get<FogOfWarManager>().RegisterVision(this, 10);
        m_renderer = m_gameObject.GetComponent<SpriteRenderer>();
        m_line = m_gameObject.GetComponent<LineRenderer>();
        m_jobQueue = new List<ActorJob>();
    }

    /// <summary>
    /// the event fired when the entity is destroyed in the game world
    /// </summary>
    public void OnDestroy()
    {
        ManagerInstance.Get<EntityManager>().UnRegisterEntity(this);
    }

    //Logic loop
    //Plan:
    //job state(Idle, moving, doing)

    //Job Queue
        //Job: position, type(kill, build, collect), job target, importance

    //if idle
        //look for job
            //if no job wait (jobSeekingCooldown)

    //if moving
        //go to job position
            //if postition reached
                //state = doing

    //if doing
        //tell target job progress
            //if progress = complete

    //goto next job item


    public void OnUpdate(float deltaTime)
    {
        if(m_jobQueue.Count > 0)
        {
            switch (currentJob.state)
            {
                case ActorJobState.Moving:
                    FollowPath();
                    break;
                case ActorJobState.Doing:
                    currentJob.OnAction();
                    break;
                case ActorJobState.Done:
                    m_jobQueue.Remove(currentJob);
                    break;
            }
        }
        else
        {
            //seek new job
            //if found new job
           // ActorJob job = new ActorJob(this, ActorJobType.Kill, ManagerInstance.Get<WorldManager>().completeMap[25, 25], this);
           // m_jobQueue.Add(job);

            //currentPath.AddRange(ManagerInstance.Get<PathingManager>().GetPath(currentTile, job.targetTile));
            //else waitfor(jobSeekingCooldown)
        }

        //old
        if (m_selected)
        {
            if (Input.GetMouseButton(1))
            {
                if (MouseInput.hoveredTile.tranversable)
                {
                    m_displayPath = new List<Node>();
                    m_displayPath.AddRange(ManagerInstance.Get<PathingManager>().GetPath(currentTile, MouseInput.hoveredTile));
                    m_line.enabled = true;
                    UpdatePathLine();
                }
            }
            if(Input.GetMouseButtonUp(1))
            {
                m_currentPath = new List<Node>();
                m_currentPath.AddRange(ManagerInstance.Get<PathingManager>().GetPath(currentTile, MouseInput.hoveredTile));
                ActorJob moveJob = new ActorJob(this, ActorJobType.Move, MouseInput.hoveredTile, null);
                m_jobQueue.Add(moveJob);
            }
        }
    }

    public void OnSelect()
    {
        m_selected = true;
        m_renderer.color = Color.yellow;
        m_line.enabled = true;
    }

    public void OnDeselect()
    {
        m_selected = false;
        m_renderer.color = Color.white;
        m_line.enabled = false;
    }

    public void OnAttack(IEntity target)
    {
        throw new System.NotImplementedException();
    }

    public void OnAttacked(IEntity attacker)
    {
        throw new System.NotImplementedException();
    }
#endregion

    //Actor methods
    #region Actor Methods
    public void UpdatePathLine()
    {
        m_line.SetVertexCount(m_displayPath.Count + 1);
        m_line.SetPosition(0, gameObject.transform.position);

        for (int i = 1; i < m_displayPath.Count + 1; i++)
        {
            Vector3 pos = WorldManager.GamePosAtTile(m_displayPath[i - 1]);
            pos.z = -0.5f;
            m_line.SetPosition(i, pos);
        }
    }

    public void FollowPath()
    {
        if (m_currentPath == null)
            return;

        if (m_currentPath.Count > 0)
        {
            m_gameObject.transform.position = Vector3.MoveTowards(m_gameObject.transform.position, currentPath[0].ToVector3(), 0.015f);
            this.LookAt(currentPath[0].ToVector3());
            UpdatePathLine();
            if (Vector3.Distance(m_gameObject.transform.position, currentPath[0].ToVector3()) < 0.05f)
            {
                currentPath.Remove(currentPath[0]);
                m_displayPath.Remove(m_displayPath[0]);
            }
        }
        else
        {
            currentJob.position.reached = true;
            m_line.enabled = false;
            if(currentJob.type == ActorJobType.Move)
                currentJob.state = ActorJobState.Done;
            else
                currentJob.state = ActorJobState.Doing;
        }
    }

    public ActorJob currentJob
    {
        get
        {
            if (m_jobQueue.Count > 0)
                return m_jobQueue[0];
            return null;
        }
    }
    #endregion
}

#region Job Classes/Enums
public enum ActorJobType
{
    None,
    Move,
    Kill,
    Build,
    Collect
}

public enum ActorJobState
{
    Moving,
    Doing,
    Done
}

[System.Serializable]
public class Position
{
    public int x, y = 0;
    public bool reached = false;
    public Position(int x, int y) { this.x = x; this.y = y; }
}

[System.Serializable]
public class ActorJob
{
    public ActorJobType type;
    public Position position;
    public IEntity target;
    public ActorJobState state;
    private IEntity m_jobOrigin;

    public ActorJob(IEntity jobForEntity, ActorJobType type, ChunkTile tile, IEntity target)
    {
        this.type = type;
        this.position = new Position(tile.worldX, tile.worldY);
        this.target = target;
        m_jobOrigin = jobForEntity;
    }

    public ChunkTile targetTile
    {
        get
        {
            return ManagerInstance.Get<WorldManager>().completeMap[position.x, position.y];
        }
    }
    
    public bool isDone
    {
        get
        {
            if (state == ActorJobState.Done)
                return true;
            return false;
        }
    }

    public void OnAction()
    {
        if(type == ActorJobType.Kill)
        {
            if(targetTile != null)
            {
                m_jobOrigin.OnAttack(target);
                target.OnAttacked(m_jobOrigin);
            }
        }
    }
}
#endregion