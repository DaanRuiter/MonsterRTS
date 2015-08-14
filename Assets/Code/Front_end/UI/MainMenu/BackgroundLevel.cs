using UnityEngine;
using System.Collections;

public class BackgroundLevel : MonoBehaviour {

	private void Start()
    {
        CommandInput.ToggleFOW();
        SaveGame bgLvl = SaveGame.LoadSaveFile("mainmenu");
        bgLvl.LoadSaveIntoGame();

        Camera.main.transform.localPosition = randomPoint;
    }

    private Vector3 destination = Vector3.zero;
    private void Update()
    {
        if (destination == Vector3.zero)
        {
            destination = randomPoint;
        }
        Camera.main.transform.localPosition = Vector3.MoveTowards(Camera.main.transform.localPosition, destination, 0.005f);
        if (Vector3.Distance(Camera.main.transform.localPosition, destination) < 0.5f)
        {
            destination = Vector3.zero;
        }
    }

    private Vector3 randomPoint
    {
        get
        {
            return new Vector3(Random.Range(-4f, WorldManager.worldWidth * Chunk.WIDTH - 4f), Random.Range(-4f, WorldManager.worldHeight * Chunk.HEIGHT - 4f), Camera.main.transform.localPosition.z);
        }
    }
}
