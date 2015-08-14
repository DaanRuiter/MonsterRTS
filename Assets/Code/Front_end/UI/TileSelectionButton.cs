using UnityEngine;
using System.Collections;

public class TileSelectionButton : MonoBehaviour {

    public int tileIndex;

    public void SelectTile()
    {
        transform.parent.GetComponent<TileSelectionPanel>().SelectTile(tileIndex);
        //TODO SET MODE TO BUILD
    }
}
