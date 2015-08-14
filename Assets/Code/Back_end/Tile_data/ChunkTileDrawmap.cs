using UnityEngine;
using System.Collections;

public enum BorderType
{
    None,
    Shadow,
    Merge
}

public class Border
{
    public BorderType type = BorderType.None;
    public bool border = false;

    public Border(BorderType type, bool isBorder)
    {
        this.type = type;
        this.border = isBorder;
    }
}

public class ChunkTileDrawmap {

    public BorderType type;

    private Border[,] m_borderMap;
    private bool[,] m_borderDrawMap;

    public ChunkTileDrawmap()
    {
        m_borderMap = new Border[3, 3];

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                m_borderMap[x, y] = new Border(BorderType.None, false);
            }
        }

        m_borderDrawMap = new bool[3, 3];
    }

    public bool needsRedraw
    {
        get
        {
            bool result = false;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if(m_borderMap[x, y].border != m_borderDrawMap[x, y])
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }

    public void Redraw()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                m_borderDrawMap[x, y] = m_borderMap[x, y].border;
            }
        }
    }

    public bool[,] redrawMap
    {
        get
        {
            bool[,] result = new bool[3, 3];
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (m_borderMap[x, y].border != m_borderDrawMap[x, y])
                    {
                        result[x, y] = true;
                    }
                }
            }
            return result;
        }
    }

    //border data
    public void SetBorderAt(int x, int y, BorderType type)
    {
        if (x >= 0 && x <= 2 && y >= 0 && y <= 2)
        {
            m_borderMap[x, y].border = true;
            m_borderMap[x, y].type = type;
        }
        else
        {
            Debug.LogError("invalid X & Y");
        }
    }

    public void RemoveBorderAt(int x, int y)
    {
        if (x >= 0 && x <= 2 && y >= 0 && y <= 2)
        {
            m_borderMap[x, y].border = false;
            m_borderMap[x, y].type = BorderType.None;
        }
        else
        {
            Debug.LogError("invalid X & Y");
        }
    }

    public bool HasBorderAt(int x, int y)
    {
        if (x >= 0 && x <= 2 && y >= 0 && y <= 2)
        {
            return m_borderMap[x, y].border;
        }
        Debug.LogError("invalid X & Y");
        return false;
    }

    public bool[,] drawMap
    {
        get
        {
            return m_borderDrawMap;
        }
    }

    public Border[,] borderMap
    {
        get
        {
            return m_borderMap;
        }
    }
}
