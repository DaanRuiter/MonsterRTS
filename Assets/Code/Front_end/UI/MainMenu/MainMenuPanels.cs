using UnityEngine;
using System.Collections;

public class MainMenuPanels : MonoBehaviour {

    private CanvasGroup m_currentPanel;

	public void OpenPanel(CanvasGroup panel)
    {
        //close current panel

        if(m_currentPanel != null)
        {
            m_currentPanel.alpha = 0;
            m_currentPanel.blocksRaycasts = false;
        }

        panel.alpha = 1;
        panel.blocksRaycasts = true;
    }
}
