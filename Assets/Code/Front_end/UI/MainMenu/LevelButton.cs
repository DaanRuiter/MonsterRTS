using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : MonoBehaviour {

    private int m_index;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Click); 
    }

    public void SetIndex(int index)
    {
        m_index = index;
    }
    public void Click()
    {
        transform.parent.GetComponent<LevelLoader>().Load(m_index);
    }
}
