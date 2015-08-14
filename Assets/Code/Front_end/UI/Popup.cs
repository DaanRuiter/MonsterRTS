using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class Popup : MonoBehaviour {

    private static CanvasGroup m_popupGroup;

    private static Text m_text;
    private static Text m_title;

    private void Awake()
    {
        m_popupGroup = GetComponent<CanvasGroup>();
        m_text = transform.FindChild("Message").GetComponent<Text>();
        m_title = transform.FindChild("Title").GetComponent<Text>();
        SetPanel(false);
    }

    //toggles
    private static void SetBlur(bool state)
    {
        Camera.main.GetComponent<Blur>().enabled = state;
    }

    public void Close()
    {
        SetPanel(false);
    }

    public static void SetPanel(bool on)
    {
        if(on)
        {
            m_popupGroup.alpha = 1;
            m_popupGroup.blocksRaycasts = true;
            SetBlur(true);
        }
        else
        {
            m_popupGroup.alpha = 0;
            m_popupGroup.blocksRaycasts = false;
            SetBlur(false);
        }
    }

    public static void Message(string title, string message)
    {
        SetPanel(true);
        m_title.text = title;
        m_text.text = message;
    }
}
