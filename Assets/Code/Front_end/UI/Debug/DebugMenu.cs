using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour {

    private List<DebugMenuScreen> m_screens;
    private int m_selectedScreen = 0;

    private void Start()
    {
        m_screens = new List<DebugMenuScreen>();

        DebugMenuScreen FOW = new DebugMenuScreen("Fog of War");
        FOW.objects.Add("Frame: ", Time.frameCount);
        m_screens.Add(FOW);
    }

    private Rect m_windowRect = new Rect(10, 10, 250, 450);
    private void OnGUI()
    {
        GUI.Box(m_windowRect, "Debug Menu");

        if(m_selectedScreen == 0)
        {
            for (int i = 0; i < m_screens.Count; i++)
            {
                if (GUI.Button(new Rect(20, 20 + 20 * i, 100, 25), m_screens[i].screenName))
                    m_selectedScreen = i + 1;
            }
        }
        else
        {
            GUILayout.Label(m_screens[m_selectedScreen - 1].screenName);
        }
    }


}
