using UnityEngine;
using System.Collections;


public class LightingManager : BManager {

    private GameObject m_lightPrefab;

	private void Start()
    {
        m_lightPrefab = Resources.Load("Lighting/Point") as GameObject;
    }

    private void OnUpdate()
    {

    }

    public void SetLightAt(ChunkTile tile, float range)
    {
        Light light = (GameObject.Instantiate(m_lightPrefab, WorldManager.GamePosAtTile(tile), Quaternion.identity) as GameObject).GetComponent<Light>();
        light.range = range;
    }
}
