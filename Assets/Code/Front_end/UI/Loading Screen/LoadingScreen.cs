using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour {

    private static Text m_loadingDesc;

    private AsyncOperation load;

    private void Start()
    {
        m_loadingDesc = transform.FindChild("Panel").FindChild("Text").GetComponent<Text>();
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {

        Destroy(GameObject.Find("MainMenu"));
        yield return new WaitForEndOfFrame();

        load = Application.LoadLevelAdditiveAsync(1);

        yield return load;

        Destroy(this.gameObject);
    }

    public static void SetLoadingDescText(string desc)
    {
        m_loadingDesc.text = desc;
    }
}
