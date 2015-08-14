using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TileSelectionPanel : MonoBehaviour {

    public static TileSelectionPanel i;

    private List<GameObject> m_tileButtons;

    private int m_selectedButton;

    private void Awake()
    {
        i = this;

        m_tileButtons = new List<GameObject>();

        RefreshUI();
    }

	private void RefreshUI()
    {
        if (ManagerInstance.Get<DatabaseManager>().dataBase != null)
        {
            for (int i = 0; i < ManagerInstance.Get<DatabaseManager>().dataBase.loadedData.Count; i++)
            {
                ChunkTileSerialized tile = ManagerInstance.Get<DatabaseManager>().dataBase.loadedData[i];
                GameObject ui = GameObject.Instantiate(Resources.Load("UI/Tile Button") as GameObject);
                ui.transform.parent = transform;
                ui.name = "Tile: " + tile.identity;

                ui.GetComponent<RectTransform>().anchoredPosition = new Vector2(10 + i * 85, 0);
                ui.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
                ui.GetComponent<Image>().sprite = Sprite.Create(ManagerInstance.Get<DatabaseManager>().dataBase.loadedGraphics[i].ToTexture(ManagerInstance.Get<DatabaseManager>().dataBase.loadedGraphics[i].texture), new Rect(0, 0, WorldGraphicsManager.TILE_RESOLUTION, WorldGraphicsManager.TILE_RESOLUTION), new Vector2(0, 0));
                ui.GetComponent<TileSelectionButton>().tileIndex = i;

                m_tileButtons.Add(ui);
            }
        }
    }

    public void SelectTile(int tileIndex)
    {
        if (m_tileButtons[m_selectedButton] != null)
            Destroy(m_tileButtons[m_selectedButton].GetComponent<Outline>());

        m_selectedButton = tileIndex;
        m_tileButtons[m_selectedButton].AddComponent<Outline>();
    }

    public ChunkTileSerialized selectedTile
    {
        get
        {
            if(m_selectedButton < m_tileButtons.Count)
            {
                return ManagerInstance.Get<DatabaseManager>().dataBase.loadedData[m_selectedButton];
            }
            else
            {
                Debug.LogError("Cannot find selected tile");
                return null;
            }
        }
    }
}
