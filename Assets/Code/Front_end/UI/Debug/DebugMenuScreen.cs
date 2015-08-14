using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugMenuScreen  
{
    public string screenName;
    public Dictionary<string, object> objects;

    public DebugMenuScreen(string name)
    {
        screenName = name;
        objects = new Dictionary<string, object>();
    }
}
