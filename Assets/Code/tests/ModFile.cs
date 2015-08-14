using UnityEngine;
using System.Collections;

public interface IScript
{
    string modName
    {
        get;
    }

    string modDescription
    {
        get;
    }

    void OnStart();
    void OnUpdate();
    void OnSave();
    void OnLoad();
    void OnPause();
    void OnResume();
}

public interface ITileScript : IScript
{
    /// <summary>
    /// the tiles affected by this script.
    /// some special attributes:
    /// all: all loaded tiles
    /// traversable: all traversable tiles
    /// non-traversable: all non traversable tiles
    /// </summary>
    string[] tiles
    {
        get;
    }

    void OnTileEnter(IEntity entity);
    void OnTileExit(IEntity entity);

    void OnTileBuild(int x, int y);
    void OnTileDestroy(int x, int y);
}

public interface IEntityScript : IScript
{
    /// <summary>
    /// all the entities affected by this script.
    /// some special attributes:
    /// actor: all actors
    /// static: all static entities
    /// dynamic: all dynamic entities
    /// human: all human actors
    /// mob: all mobs
    /// </summary>
    string[] entities
    {
        get;
    }

    void OnEntityTileEnter(ChunkTile tile);
    void OnEntityTileExit(ChunkTile tile);

    void OnEntitySpawn(ChunkTile tileSpawnedOn);
    void OnEntityDeath(ChunkTile tileKilledOn);
}

