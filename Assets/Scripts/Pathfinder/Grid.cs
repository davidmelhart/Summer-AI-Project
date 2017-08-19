﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public bool displayGrid;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    int influenceMin = int.MaxValue;
    int influenceMax = int.MinValue;

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    private Vector3 worldBottomLeft;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    void Update()
    {
        UpdateGrid();
    }

    void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        UpdateGrid();
    }

    void UpdateGrid() {
        Vector3 worldPoint;
        //Basic value map - keeps agents away from obstacles
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapBox(worldPoint, new Vector2(nodeRadius, nodeRadius), 0f, unwalkableMask));

                int influence = 0;

                if (walkable) {
                    for (float k = 4, b = 0.95f; k > 1.2f; k *= b, b -= 0.03f) {
                        influence += 5;
                    }
                } else {
                    influence = 1000;
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, influence);
                
            }
        }
        BlurInfluenceMap(2);

        //Creating influence impact for fires without modifying the basic grid and values
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                Collider2D node = Physics2D.OverlapBox(worldPoint, new Vector2(nodeRadius, nodeRadius), 0f);
                if (node) {
                    if (node.gameObject.CompareTag("fire")) {
                        List<Node> fireArea = GetNeighbourNodes(grid[x, y]);
                        for (int i = 0; i < fireArea.Count; i++) {
                            fireArea[i].influence = 10000;
                        }
                    }
                }

            }
        }
    }


    public Node NodeFromWorldPoint (Vector3 _worldPosition)
    {
        float percentX = (_worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (_worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
  
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    void BlurInfluenceMap (int blurSize) {
        int kernelSize = blurSize * 2 - 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] horizontalPass = new int[gridSizeX, gridSizeY];
        int[,] verticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++) {
            for (int x = -kernelExtents; x <= kernelExtents; x++) {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                horizontalPass[0, y] += grid[sampleX, y].influence;
            }
            for (int x = 1; x < gridSizeX; x++) {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                horizontalPass[x, y] = horizontalPass[x - 1, y] - grid[removeIndex, y].influence + grid[addIndex, y].influence;
            }
        }

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = -kernelExtents; y <= kernelExtents; y++) {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                verticalPass[x, 0] += horizontalPass[x, sampleY];
            }

            int blurredInfluence = Mathf.RoundToInt((float)verticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].influence = blurredInfluence;

            for (int y = 1; y < gridSizeY; y++) {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);
                verticalPass[x, y] = verticalPass[x, y - 1] - horizontalPass[x, removeIndex] + horizontalPass[x, addIndex];

                blurredInfluence = Mathf.RoundToInt((float)verticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].influence = blurredInfluence;

                if (blurredInfluence > influenceMax) {
                    influenceMax = blurredInfluence;
                }
                if (blurredInfluence < influenceMin) {
                    influenceMin = blurredInfluence;
                }
            }
        }
    }

    public List<Node> GetNeighbourNodes(Node node) {
        List<Node> neighbourNodes = new List<Node>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbourNodes.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbourNodes;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

        if ( grid != null && displayGrid) {
            foreach (Node n in grid) {
                Gizmos.color = Color.Lerp(new Color(1, 1, 1, 0.5f), new Color(1, 0, 0, 0.5f), Mathf.InverseLerp(influenceMin, influenceMax, n.influence));
                Gizmos.color = (n.walkable) ? Gizmos.color : new Color(1, 0, 0, 0.8f);
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-0.5f));
            }
        }
    }
}
