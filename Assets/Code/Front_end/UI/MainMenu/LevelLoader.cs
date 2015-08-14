using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelLoader : MonoBehaviour {

    private GameObject m_levelTitlePrefab;
    private List<SaveGame> m_saves = new List<SaveGame>();

    private void Start()
    {
        m_levelTitlePrefab = Resources.Load("UI/MainMenu/Level Listing") as GameObject;
    }

    public void LoadLevels(string type)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            m_saves.Clear();
            Destroy(transform.GetChild(i).gameObject);
        }


        string dataPath = Application.dataPath + "/../data/saves/" + type;
        string[] directories = Directory.GetDirectories(dataPath);

        for (int i = 0; i < directories.Length; i++)
        {
            if (Directory.GetFiles(directories[i], "*.save").Length > 0)
            {
                string filePath = Directory.GetFiles(directories[i], "*.save")[0];
                string[] splittedPath = filePath.Split('\\');

                if (Directory.GetFiles(directories[i], "*.save").Length > 0)
                {
                    filePath = dataPath + "/" + splittedPath[splittedPath.Length - 2] + "/" + splittedPath[splittedPath.Length - 2] + ".save";
                    SaveGame save = SaveGame.LoadFromPath(filePath);
                    m_saves.Add(save);
                }
            }
        }

        for (int i = 0; i < m_saves.Count; i++)
        {
            GameObject listing = GameObject.Instantiate(m_levelTitlePrefab);
            listing.transform.SetParent(transform, false);
            listing.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -10 - (i * 33));
            listing.AddComponent<LevelButton>();
            listing.GetComponent<LevelButton>().SetIndex(i);
            listing.transform.FindChild("Text").GetComponent<Text>().text = m_saves[i].name;
        }
    }

    public void Load(int index)
    {
        Debug.Log(m_saves[index].name);
        GameManager.instance.saveGame = m_saves[index];
        GameManager.instance.LoadGame();
    }
}
