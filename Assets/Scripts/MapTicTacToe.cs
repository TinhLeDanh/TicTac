using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTicTacToe : MonoBehaviour
{
    public enum TicTacType
    {
        X,
        O
    }

    public int width;
    public int height;
    public GameObject parentGO;
    public GameObject xPrefab;
    public GameObject oPrefab;
    public TicTacType turn;

    private Vector2[,] cells;
    private int[,] matrix;
    private Camera cam;
    private Vector2 cellSize;
    private Vector2 mousePosition;
    private Vector2Int nearestCoordinates;

    private float halfCameraWidth;
    private float halfCameraHeight;

    private void Awake()
    {
        cam = Camera.main;
        cells = new Vector2[width, height];
        matrix = new int[width, height];
    }

    // Start is called before the first frame update
    void Start()
    {
        var mainCamera = Camera.main;
        var cameraHeight = 2f * mainCamera.orthographicSize;
        var cameraWidth = cameraHeight * mainCamera.aspect;
        halfCameraWidth = cameraWidth / 2;
        halfCameraHeight = cameraHeight / 2;
        turn = TicTacType.O;

        cellSize.x = cameraWidth / width;
        cellSize.y = cameraHeight / height;

        Draw();
        SetupCells();
    }

    private void Draw()
    {
        for (float x = -halfCameraWidth; x <= halfCameraWidth; x += cellSize.x)
        {
            Coordinates.Draw(gameObject.transform, new Coordinates(x, -halfCameraHeight),
                new Coordinates(x, halfCameraHeight));
        }

        for (float y = -halfCameraHeight; y <= halfCameraHeight; y += cellSize.y)
        {
            Coordinates.Draw(gameObject.transform, new Coordinates(-halfCameraWidth, y), new Coordinates(halfCameraWidth, y));
        }
    }

    private void SetupCells()
    {
        for (int i = 1; i < width; i++)
        {
            for (int j = 1; j < height; j++)
            {
                matrix[i, j] = 0;
            }
        }

        cells[0, 0] = new Vector2(-halfCameraWidth, halfCameraHeight);
        cells[0, 0].x += cellSize.x / 2;
        cells[0, 0].y -= cellSize.y / 2;

        for (int i = 1; i < height; i++)
        {
            cells[0, i] = new Vector2(cells[0, i - 1].x, cells[0, i - 1].y - cellSize.y);
        }
        for (int i = 1; i < width; i++)
        {
            cells[i, 0] = new Vector2(cells[i - 1, 0].x - cellSize.x, cells[i - 1, 0].y);
        }

        for (int i = 1; i < width; i++)
        {
            for (int j = 1; j < height; j++)
            {
                cells[i, j] = new Vector2(cells[i - 1, j].x + cellSize.x, cells[i, j - 1].y - cellSize.y);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            if (turn == TicTacType.X)
            {
                Vector2 newPosition = GetNearestCell(mousePosition);

                Debug.Log(nearestCoordinates.x + " -" + nearestCoordinates.y);
                if(matrix[nearestCoordinates.x, nearestCoordinates.y] != 0)
                {
                    return;
                }

                GameObject go = Instantiate(xPrefab, newPosition, Quaternion.identity);
                go.transform.parent = parentGO.transform;
                turn = TicTacType.O;
            }
            else if (turn == TicTacType.O)
            {
                Debug.Log(nearestCoordinates.x + " -" + nearestCoordinates.y);
                Debug.Log(matrix[nearestCoordinates.x, nearestCoordinates.y]);
                if (matrix[nearestCoordinates.x, nearestCoordinates.y] != 0)
                {
                    return;
                }

                GameObject go = Instantiate(oPrefab, GetNearestCell(mousePosition), Quaternion.identity);
                go.transform.parent = parentGO.transform;
                turn = TicTacType.X;
            }

            if (CheckWinCondition(nearestCoordinates))
            {
                Debug.Log("Win");
            }
        }
    }

    private bool CheckWinCondition(Vector2 center)
    {
        int player = (turn == TicTacType.O) ? 1 : 2;
        
        // Check for a horizontal win
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (matrix[i, j] == player && 
                    matrix[i, j + 1] == player && 
                    matrix[i, j + 2] == player && 
                    matrix[i, j + 3] == player && 
                    matrix[i, j + 4] == player)
                {
                    return true;
                }
            }
        }

        // Check for a vertical win
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (matrix[i, j] == player && 
                    matrix[i + 1, j] == player &&
                    matrix[i + 2, j] == player &&
                    matrix[i + 3, j] == player &&
                    matrix[i + 4, j] == player)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private Vector2 GetNearestCell(Vector2 position)
    {
        Vector2 minCell = new Vector2(999, 999);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Vector2.Distance(position, cells[i, j]) < Vector2.Distance(position, minCell))
                {
                    minCell = cells[i, j];
                    if (turn == TicTacType.O)
                        matrix[i, j] = 1;
                    else if (turn == TicTacType.X)
                        matrix[i, j] = 2;
                    nearestCoordinates.x = i;
                    nearestCoordinates.y = j;
                }
            }
        }

        return minCell;
    }
}
