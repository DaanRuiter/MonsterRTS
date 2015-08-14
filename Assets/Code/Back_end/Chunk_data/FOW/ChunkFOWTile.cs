
public class ChunkFOWTile {

    private float m_fogStrength;
    private ChunkFOW m_component;

    public ChunkFOWTile(float strength, ChunkFOW component)
    {
        m_fogStrength = strength;
        m_component = component;
    }

    public float strength
    {
        get
        {
            return m_fogStrength;
        }
        set
        {
            m_fogStrength = value;
        }
    }

    public ChunkFOW component
    {
        get
        {
            return m_component;
        }
    }

    public void ChangeStrength(float difference)
    {
        m_fogStrength += difference;
    }
}
