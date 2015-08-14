using UnityEngine;
using System.Collections;

public class ChunkObject : MonoBehaviour {

    private Chunk m_chunk;
    private GameObject m_particleObject;

    private void Awake()
    {
        m_particleObject = transform.FindChild("Particles").gameObject;
    }

    public Chunk chunkData
    {
        get
        {
            return m_chunk;
        }
        set
        {
            m_chunk = value;
            m_chunk.chunkObject = this;
        }
    }

    public void SetGraphics(TextureMode mode)
    {
        GetComponent<Renderer>().sharedMaterial = new Material(Resources.Load("Materials/chunk") as Material);
        Texture2D oldTex = GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
        GetComponent<Renderer>().sharedMaterial.mainTexture = oldTex;
        GetComponent<Renderer>().sharedMaterial.mainTexture = ManagerInstance.Get<WorldGraphicsManager>().GetTextureForChunk(m_chunk.x, m_chunk.y, mode);
        m_chunk.AddChunkToDrawQueue();
    }

    public void EmiteParticles(float x, float y, Color particleColor)
    {
        //making vars ready;
        x = (1f / Chunk.WIDTH * x) - 0.5f;
        y = (1f / Chunk.HEIGHT * y) - 0.5f;
        particleColor *= 0.75f;

        //applying
        m_particleObject.transform.localPosition = new Vector3(x, y, 0.05f);
        m_particleObject.GetComponent<ParticleSystem>().startColor = particleColor;
        m_particleObject.GetComponent<ParticleSystem>().Emit(10);
    }

    public Texture2D texture
    {
        get
        {
            return GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
        }
    }
}
