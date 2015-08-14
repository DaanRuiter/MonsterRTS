using UnityEngine;
using System.Collections;

public class MouseInput : MonoBehaviour {

    private static ChunkTile m_hoveredTile;

    public GameObject selector;

    private float m_selectorScale = 1;
    private string m_mode = "build";

    private static IEntity m_hoveringEntity;

    private void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonUp(1))
            m_mode = "none";

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Chunk") >> 1))
        {
            Vector3 mouseHit = hit.transform.InverseTransformPoint(hit.point);
            int x = (int) Mathf.Floor(Chunk.WIDTH * (mouseHit.x + 0.5f));
            int y = (int) Mathf.Floor(Chunk.HEIGHT * (mouseHit.y + 0.5f));

            selector.SetActive(true);
            selector.transform.position = new Vector3((hit.transform.position.x + x) - (8f - m_selectorScale / 2), (hit.transform.position.y + y) - (8f - m_selectorScale / 2), -1);

            Chunk chunk = hit.transform.GetComponent<ChunkObject>().chunkData;

            m_hoveredTile = ManagerInstance.Get<WorldManager>().completeMap[chunk.x * Chunk.WIDTH + x, chunk.y * Chunk.HEIGHT + y];

            if(Input.GetMouseButton(0))
            {
                if(m_mode == "build")
                {
                    if (m_selectorScale == 1)
                    {
                        chunk.ChangeBlockAt(x, y, TileSelectionPanel.i.selectedTile);
                    }
                    else
                    {
                        ChunkTile startTile = ManagerInstance.Get<WorldManager>().completeMap[chunk.x * Chunk.WIDTH + x, chunk.y * Chunk.HEIGHT + y];
                        ChunkTile endTile = ManagerInstance.Get<WorldManager>().completeMap[(chunk.x * Chunk.WIDTH + x) + (int)m_selectorScale, (chunk.y * Chunk.HEIGHT + y) + (int)m_selectorScale];
                        ManagerInstance.Get<WorldManager>().ChangeArea(startTile, endTile, TileSelectionPanel.i.selectedTile);
                    }
                }else if(m_mode == "light")
                {
                    ChunkTile tile = chunk.tiles[x, y];
                    ManagerInstance.Get<FogOfWarManager>().DrawCircle(tile.worldX, tile.worldY, 35, 0f);
                }
                if(m_mode == "neighbor")
                {
                    chunk.tiles[x, y].LogDrawMap(chunk.tiles[x, y].drawMap.borderMap);
                }
            }
        }
        else
        {
            selector.SetActive(false);
        }
        if (m_mode == "spawn")
        {
            if(Input.GetMouseButtonUp(0))
                CommandInput.Spawn(new string[] { "Actor", "try2", "" + m_hoveredTile.worldX, "" + m_hoveredTile.worldY });
        }

        if(Input.GetKeyUp(KeyCode.UpArrow))
        {
            m_selectorScale += 1;
            selector.transform.localScale = new Vector2(m_selectorScale, m_selectorScale);
        }
        if(Input.GetKeyUp(KeyCode.DownArrow))
        {
            if(m_selectorScale > 1)
                m_selectorScale -= 1;

            selector.transform.localScale = new Vector2(m_selectorScale, m_selectorScale);
        }
    }

    public static void SetHoveringEntity(IEntity entity)
    {
        if (m_hoveringEntity != null)
            m_hoveringEntity.OnDeselect();

        m_hoveringEntity = entity;
    }

    public static void RemoveHoveringEntity()
    {
        m_hoveringEntity = null;
    }

    public static IEntity hoveringEntity
    {
        get
        {
            return m_hoveringEntity;
        }
    }

    public static ChunkTile hoveredTile
    {
        get
        {
            return m_hoveredTile;
        }
    }

    public void SetMode(string mode)
    {
        m_mode = mode;
    }
}
