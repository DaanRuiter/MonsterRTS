using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path  {
    public List<Node> path;

    public Path(List<Node> path)
    {
        this.path = path;
    }

    public Node start
    {
        get
        {
            if(path.Count > 0)
                return path[0];
            return null;
        }
    }

    public Node finish
    {
        get
        {
            if (path.Count > 0)
                return path[path.Count - 1];
            return null;
        }
    }
}
