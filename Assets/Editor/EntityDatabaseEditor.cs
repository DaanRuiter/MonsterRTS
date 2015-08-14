using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class EntityDatabaseEditor : EditorWindow
{

    private static EditorWindow m_window;

    private EntityType m_currentType;

    private Database<Actor, EntityGraphics> m_actors;
    private Database<StaticEntity, EntityGraphics> m_staticEntities;
    private Database<DynamicEntity, EntityGraphics> m_dynamicEntities;

    [MenuItem("Database/Entities")]
    public static void ShowWindow()
    {
        m_window = EditorWindow.GetWindow(typeof(EntityDatabaseEditor));
        m_window.titleContent = new GUIContent("⌘ Entities");
    }

    private void OnGUI()
    {
        if (m_actors == null)
        {
            m_actors = new Database<Actor, EntityGraphics>("entities/actors", "actor");
            m_staticEntities = new Database<StaticEntity, EntityGraphics>("entities/static", "staticEntity");
            m_dynamicEntities = new Database<DynamicEntity, EntityGraphics>("entities/dynamic", "dynamicEntity");
            m_currentType = EntityType.None;
        }

        if (GUILayout.Button("Load Entities"))
        {
            m_actors.LoadItems();
            m_staticEntities.LoadItems();
            m_dynamicEntities.LoadItems();
        }

        GUILayout.BeginHorizontal();
        if (m_currentType == EntityType.Actor)
            GUI.contentColor = Color.yellow;

        if(GUILayout.Button("Actors"))
        {
            m_currentType = EntityType.Actor;
        }

        GUI.contentColor = Color.white;
        if (m_currentType == EntityType.Static)
            GUI.contentColor = Color.yellow;

        if (GUILayout.Button("Static"))
        {
            m_currentType = EntityType.Static;
        }

        GUI.contentColor = Color.white;
        if (m_currentType == EntityType.Dynamic)
            GUI.contentColor = Color.yellow;

        if (GUILayout.Button("Dynamic"))
        {
            m_currentType = EntityType.Dynamic;
        }

        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();

        switch (m_currentType)
        {
            case EntityType.None:
                break;
            case EntityType.Actor:
                foreach (Actor actor in m_actors.loadedData)
                {
                    for (int i = 0; i < actor.GetAllProperties().Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(actor.GetAllProperties()[i].Key);
                        string value = actor.GetProperty<string>(actor.GetAllProperties()[i].Key);
                        value = EditorGUILayout.TextField(value, GUILayout.Width(Screen.width * 0.65f));
                        actor.SetProperty(actor.GetAllProperties()[i].Key, value);
                        GUILayout.EndHorizontal();
                    }
                    Line();
                }
                break;
            case EntityType.Static:
                foreach (StaticEntity entity in m_staticEntities.loadedData)
                {
                    for (int i = 0; i < entity.GetAllProperties().Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(entity.GetAllProperties()[i].Key);
                        string value = entity.GetProperty<string>(entity.GetAllProperties()[i].Key);
                        value = EditorGUILayout.TextField(value, GUILayout.Width(Screen.width * 0.65f));
                        entity.SetProperty(entity.GetAllProperties()[i].Key, value);
                        GUILayout.EndHorizontal();
                    }
                    Line();
                }
                break;
            case EntityType.Dynamic:
                foreach (DynamicEntity entity in m_dynamicEntities.loadedData)
                {
                    for (int i = 0; i < entity.GetAllProperties().Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(entity.GetAllProperties()[i].Key);
                        string value = entity.GetProperty<string>(entity.GetAllProperties()[i].Key);
                        value = EditorGUILayout.TextField(value, GUILayout.Width(Screen.width * 0.65f));
                        entity.SetProperty(entity.GetAllProperties()[i].Key, value);
                        GUILayout.EndHorizontal();
                    }
                    Line();
                }
                break;
            default:
                break;
        }

        if(m_currentType != EntityType.None)
        {
            if(GUILayout.Button("New " + m_currentType))
            {
                switch (m_currentType)
                {
                    case EntityType.None:
                        break;
                    case EntityType.Actor:
                        m_actors.CreateItem();
                        break;
                    case EntityType.Static:
                        m_staticEntities.CreateItem();
                        break;
                    case EntityType.Dynamic:
                        m_dynamicEntities.CreateItem();
                        break;
                    default:
                        break;
                }
            }

            if (GUILayout.Button("Save " + m_currentType))
            {
                switch (m_currentType)
                {
                    case EntityType.None:
                        break;
                    case EntityType.Actor:
                        m_actors.SaveItems();
                        break;
                    case EntityType.Static:
                        m_staticEntities.SaveItems();
                        break;
                    case EntityType.Dynamic:
                        m_dynamicEntities.SaveItems();
                        break;
                    default:
                        break;
                }
            }

        }
    }

    public void Line()
    {
        GUILayout.Label("_________________________________________________________________________________________________________________________________________________________________________");
        GUILayout.Space(10);
    }
}
