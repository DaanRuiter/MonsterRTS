using UnityEngine;
using System.Collections;

public class DatabaseManager : BManager {

    private Database<ChunkTileSerialized, TileGraphics> m_tileDB;

    private void Init()
    {
        m_tileDB = new Database<ChunkTileSerialized, TileGraphics>("tiles", "tile");
        m_tileDB.LoadItems();
    }

    public Database<ChunkTileSerialized, TileGraphics> dataBase
    {
        get
        {
            if (m_tileDB == null)
                Init();

            return m_tileDB;
        }
    }
}
