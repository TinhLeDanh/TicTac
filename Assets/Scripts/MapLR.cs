using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLR : MonoBehaviour
{
    // Define the grid size and spacing
    int numRows = 10;
    int numCols = 10;
    float spacing = 1.0f;

    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        // Define the position array for the lines
        Vector3[] positions = new Vector3[2 * (numRows + numCols)];

        // Add the horizontal lines to the position array
        for (int i = 0; i <= numRows; i++)
        {
            float yPos = -i * spacing;
            positions[2 * i] = new Vector3(0, yPos, 0);
            positions[2 * i + 1] = new Vector3(numCols * spacing, yPos, 0);
        }

        // Add the vertical lines to the position array
        for (int j = 0; j <= numCols; j++)
        {
            float xPos = j * spacing;
            positions[2 * (numRows + j)] = new Vector3(xPos, 0, 0);
            positions[2 * (numRows + j) + 1] = new Vector3(xPos, -numRows * spacing, 0);
        }

        // Set the positions array in the Line Renderer component
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
