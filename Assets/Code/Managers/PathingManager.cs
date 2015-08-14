using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathingManager : BManager {
        
        public Node[,] grid;
        private List<Node> m_path;
        private List<Path> m_paths;

        private void Start()
        {
            if (grid == null)
                Init();
        }

        private void Init() {
            grid = new Node[WorldManager.worldWidth * Chunk.WIDTH, WorldManager.worldHeight * Chunk.HEIGHT];
            ChunkTile[,] map = ManagerInstance.Get<WorldManager>().completeMap;

            for (int x = 0; x < WorldManager.worldWidth * Chunk.WIDTH; x++)
            {
                for (int y = 0; y < WorldManager.worldHeight * Chunk.HEIGHT; y++)
                {
                    ChunkTile tile = map[x, y];
                    grid[x, y] = new Node(tile.tranversable, tile.worldX, tile.worldY);
                }
            }

            m_path = new List<Node>();
            m_paths = new List<Path>();
        }
 
        public void SetTraversableAt(int x, int y, bool traversable)
        {
            if (grid == null)
                Init();

            grid[x, y].walkable = traversable;
        }

        void Update() {
                //FindPath(seeker.position,target.position);
        }

        public List<Node> GetPath(ChunkTile startTile, ChunkTile targetTile)
        {
                Node startNode = grid[startTile.worldX + 1, startTile.worldY + 1];
                Node targetNode = grid[targetTile.worldX, targetTile.worldY];

            for (int i = 0; i < m_paths.Count; i++)
            {
                if (m_paths[i].start.gridX - 1 == startNode.gridX)
                    if (m_paths[i].start.gridY - 1 == startNode.gridY)
                        if (m_paths[i].finish.gridX == targetNode.gridX)
                            if (m_paths[i].finish.gridY == targetNode.gridY)
                                    return m_paths[i].path;
            }

 
                List<Node> openSet = new List<Node>();
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                        Node currentNode = openSet[0];
                        for (int i = 1; i < openSet.Count; i ++) {
                                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                                        currentNode = openSet[i];
                                }
                        }
 
                        openSet.Remove(currentNode);
                        closedSet.Add(currentNode);
 
                        if (currentNode == targetNode) {
                            RetracePath(startNode, targetNode);
                            Path path = new Path(m_path);
                            if(path.path.Count > 0)
                                m_paths.Add(path);
                            return m_path;
                        }
 
                        foreach (Node neighbour in GetNeighbours(currentNode)) {
                                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                                        continue;
                                }
 
                                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                                        neighbour.gCost = newMovementCostToNeighbour;
                                        neighbour.hCost = GetDistance(neighbour, targetNode);
                                        neighbour.parent = currentNode;
 
                                        if (!openSet.Contains(neighbour))
                                                openSet.Add(neighbour);
                                }
                        }
                }
                return m_path;
        }
 
        void RetracePath(Node startNode, Node endNode) {
                List<Node> path = new List<Node>();
                Node currentNode = endNode;
 
                while (currentNode != startNode) {
                        path.Add(currentNode);
                        currentNode = currentNode.parent;
                }
                path.Reverse();
 
                m_path = path;
 
        }
 
        int GetDistance(Node nodeA, Node nodeB) {
                int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
                int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
 
                if (dstX > dstY)
                        return 14*dstY + 10* (dstX-dstY);
                return 14*dstX + 10 * (dstY-dstX);
        }

        private List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < WorldManager.worldWidth * Chunk.WIDTH && checkY >= 0 && checkY < WorldManager.worldHeight * Chunk.HEIGHT)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

}
 
public class Node {
       
        public bool walkable;
        public int gridX;
        public int gridY;
 
        public int gCost;
        public int hCost;
        public Node parent;
       
        public Node(bool _walkable, int _gridX, int _gridY) {
                walkable = _walkable;
                gridX = _gridX;
                gridY = _gridY;
        }
 
        public int fCost {
                get {
                        return gCost + hCost;
                }
        }
}

public static class ExtensionMethods
{
    public static Vector3 ToVector3(this Node node)
    {
        return new Vector3((node.gridX - Chunk.WIDTH / 2) + 0.5f, (node.gridY - Chunk.HEIGHT / 2) + 0.5f, -0.5f);
    }

    public static void LookAt(this IEntity entity, Vector3 target)
    {
        Vector3 diff = target - entity.gameObject.transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        entity.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }
}