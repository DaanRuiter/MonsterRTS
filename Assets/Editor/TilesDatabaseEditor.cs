using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TilesDatabaseEditor : EditorWindow {

    private static EditorWindow m_window;

    private Database<ChunkTileSerialized, TileGraphics> m_dataBase;

    [MenuItem("Database/Tiles")]
    public static void ShowWindow()
    {
        m_window = EditorWindow.GetWindow(typeof(TilesDatabaseEditor));
        m_window.titleContent = new GUIContent("□ Tiles");
    }

    private string newPropertyKey = "New Property";
    private bool showNewPropertyField = false;
    private ChunkTileSerialized tileToAddTo;
    private void OnGUI()
    {
        if (m_dataBase == null)
            m_dataBase = new Database<ChunkTileSerialized, TileGraphics>("tiles", "tile");

        if (GUILayout.Button("Load Tiles"))
        {
            m_dataBase.LoadItems();
        }

        if (m_dataBase.loadedData != null)
        {
            List<ChunkTileSerialized> tiles = m_dataBase.loadedData;

            for (int i = 0; i < tiles.Count; i++)
            {
                for (int p = 0; p < tiles[i].GetAllProperties().Count; p++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(tiles[i].GetAllProperties()[p].Key);
                    if (tiles[i].GetAllProperties()[p].Value.GetType() == typeof(string))
                    {
                        tiles[i].SetProperty(tiles[i].GetAllProperties()[p].Key, EditorGUILayout.TextField(tiles[i].GetProperty<string>(tiles[i].GetAllProperties()[p].Key)));
                    }
                    if (tiles[i].GetAllProperties()[p].Value.GetType() == typeof(int))
                    {
                        tiles[i].SetProperty(tiles[i].GetAllProperties()[p].Key, EditorGUILayout.IntField(tiles[i].GetProperty<int>(tiles[i].GetAllProperties()[p].Key)));
                    }
                    if (tiles[i].GetAllProperties()[p].Value.GetType() == typeof(float))
                    {
                        tiles[i].SetProperty(tiles[i].GetAllProperties()[p].Key, EditorGUILayout.FloatField(tiles[i].GetProperty<float>(tiles[i].GetAllProperties()[p].Key)));
                    }
                    if (tiles[i].GetAllProperties()[p].Value.GetType() == typeof(bool))
                    {
                        tiles[i].SetProperty(tiles[i].GetAllProperties()[p].Key, EditorGUILayout.Toggle(tiles[i].GetProperty<bool>(tiles[i].GetAllProperties()[p].Key)));
                    }
                    if(GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        tiles[i].GetAllProperties().Remove(tiles[i].GetAllProperties()[p]);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                if (m_dataBase.loadedGraphics.Count > i)
                {
                    if (m_dataBase.loadedGraphics[i].texture == null)
                    {
                        GUILayout.Box("No graphics.png detected.");
                    }
                    else
                    {
                        GUILayout.Box(m_dataBase.loadedGraphics[i].ToTexture(m_dataBase.loadedGraphics[i].texture));
                    }
                }
                    GUILayout.BeginVertical();
                        if(GUILayout.Button("Open Folder"))
                        {
                            System.Diagnostics.Process.Start(tiles[i].dataPath);
                        }
                    GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                if (GUILayout.Button("New Property"))
                {
                    showNewPropertyField = true;
                    tileToAddTo = tiles[i];
                }
                Line();
            }
            if (GUILayout.Button("New Tile"))
            {
                m_dataBase.CreateItem();
            }
            if (GUILayout.Button("Save Tiles"))
            {
                m_dataBase.SaveItems();
            }
            Line();

            if (showNewPropertyField)
                ShowNewProperyWindow();
        }
    }

    private void ShowNewProperyWindow()
    {
        newPropertyKey = GUILayout.TextField(newPropertyKey);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("String"))
        {
            tileToAddTo.SetProperty(newPropertyKey, "New String");
            showNewPropertyField = false;
        }
        if (GUILayout.Button("Int"))
        {
            tileToAddTo.SetProperty(newPropertyKey, 0);
            showNewPropertyField = false;
        }
        if (GUILayout.Button("Float"))
        {
            tileToAddTo.SetProperty(newPropertyKey, 0f);
            showNewPropertyField = false;
        }
        if (GUILayout.Button("Bool"))
        {
            tileToAddTo.SetProperty(newPropertyKey, false);
            showNewPropertyField = false;
        }
        GUILayout.EndHorizontal();
    }

    public void Line()
    {
        GUILayout.Label("_________________________________________________________________________________________________________________________________________________________________________");
        GUILayout.Space(10);
    }
}
