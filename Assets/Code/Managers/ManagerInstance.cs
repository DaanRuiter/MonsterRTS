using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManagerInstance : MonoBehaviour {

    private static GameObject g;
    private static List<BManager> managers;

    private void Awake()
    {
        g = gameObject;
        managers = new List<BManager>();

        for (int i = 0; i < g.GetComponents(typeof(BManager)).Length; i++)
        {
            managers.Add(g.GetComponents(typeof(BManager))[i] as BManager);
        }
    }

	public static BManager Get<BManager> ()
    {
        if (g == null)
            g = GameObject.Find("World").gameObject;

        return g.GetComponent<BManager>();
    }

    private void Update()
    {
        for (int i = 0; i < managers.Count; i++)
        {
            managers[i].OnUpdate();
        }
    }
}
