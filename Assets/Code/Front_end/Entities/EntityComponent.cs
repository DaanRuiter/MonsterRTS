using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityComponent : MonoBehaviour {

    private IEntity m_entity;
    private EntityGraphics m_graphics;

    public void SetData(IEntity actor, EntityGraphics graphics)
    {
        m_entity = actor;
        m_entity.gameObject = this.gameObject;
        m_entity.OnSpawn();

        m_graphics = graphics;

        GetComponent<SpriteRenderer>().sprite = m_graphics.defaultSprite;
    }

    private void Update()
    {
        if (m_entity == null)
            return;

        m_entity.OnUpdate(Time.deltaTime);

        if(Input.GetMouseButtonUp(0))
        {
            if(m_entity == MouseInput.hoveringEntity)
            {
                m_entity.OnSelect();
            }
            else
            {
                m_entity.OnDeselect();
            }
        }
    }

    private void OnMouseEnter()
    {
        MouseInput.SetHoveringEntity(m_entity);  
    }

    private void OnMouseExit()
    {
        MouseInput.RemoveHoveringEntity();
    }

    public IEntity entity
    {
        get
        {
            return m_entity;
        }
    }
}
