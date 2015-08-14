using UnityEngine;
using System.Collections;

public class GrassMod : ITileMod
{
    //contructor
    public GrassMod()
    {

    }

    //mod properies
    public string modName
    {
        get
        {
            return "Grass Mod";
        }
    }

    public string modAuthor
    {
        get
        {
            return "Chequered";
        }
    }

    //mod methods
    public void OnStart()
    {
        Debug.Log("Grass Mod Loaded!");
    }

    public void OnSave()
    {

    }

    public void OnLoad()
    {

    }

    public void OnPause()
    {

    }

    public void OnResume()
    {

    }

    //tile properties
    public string tileIdentity
    {
        get
        {
            return "grass";
        }
    }


    //tile methods
    public void OnTileEnter()
    {

    }

    public void OnTileExit()
    {

    }

    //removes all fog of war around the newly created tile
    public void OnTileBuild(int x, int y)
    {
        ManagerInstance.Get<FogOfWarManager>().DrawCircle(x, y, 2, 0.55f);
    }

    //undos the OnTileBuild Effect
    public void OnTileDestroy(int x, int y)
    {
        ManagerInstance.Get<FogOfWarManager>().DrawCircle(x, y, 2, 0.95f);
    }
}

