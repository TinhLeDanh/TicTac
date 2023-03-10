using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum MovementState
    {
        Up,
        Down,
        Left,
        Right,
        Stop
    }

    public static GameController instance;

    [Header("Map Setting")]
    public int height = 10;
    public int width = 10;

    [Header("Game Setting")]
    public float foodSpawnTime;
    public int maxFood = 5;

    [Header("Player Setting")]
    public const int PlayerMaxSize = 20;
    public int playerSize = 3;
    public float cellPerSecond;
    public MovementState movementState;

    protected int[,] mapMatrix;

    private Vector2Int head;
    [SerializeField]
    private Vector2Int[] playerBody;
    private GameObject[,] cells;
    public MovementState preMovementState;

    private float timeCounter;
    private float foodTimeCounter;
    private int foodCount;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        movementState = new MovementState();

        cells = new GameObject[height, width];
        mapMatrix = new int[height, width];
        playerBody = new Vector2Int[PlayerMaxSize];
        head.x = (int)(height / 2);
        head.y = (int)(width / 2);
        playerBody[0].x = head.x;
        playerBody[0].y = head.y;
        movementState = MovementState.Down;
        preMovementState = MovementState.Down;
        timeCounter = 0;
        foodTimeCounter = 0;

        SetupMatrix();
        SetupPlayer();
    }

    public void SetupMatrix()
    {
        Transform cellPrefab = transform.GetChild(0);
        float cellWidth = cellPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float cellHeight = cellPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        int k = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Transform cell = Instantiate(cellPrefab, transform);
                cell.name = i.ToString() + "-" + j.ToString();
                cell.localPosition = new Vector3(j * cellWidth, -i * cellHeight, 0);
                cell.localScale = new Vector3(1, 1, 1);
                cells[i, j] = cell.gameObject;
                k++;
            }
        }
        Destroy(cellPrefab.gameObject);
    }

    //FOrmular: Width * (i-1) + j
    public void SetupPlayer()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == head.x && j == head.y)
                {
                    cells[i, j].GetComponent<SpriteRenderer>().color = Color.red;
                    for (int k = 1; k < playerSize; k++)
                    {
                        playerBody[k].x = i - k;
                        playerBody[k].y = j;

                        mapMatrix[playerBody[k].x, playerBody[k].y] = 1;
                        SetCellColor(playerBody[k], Color.blue);
                    }
                    break;
                }
            }
        }
    }

    private void Update()
    {
        HandleInput();
        HandleMovement(Time.deltaTime);
        HandleFood(Time.deltaTime);
        Line();
    }

    private void Line()
    {

    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(movementState == MovementState.Left)
            {
                return;
            }
            preMovementState = movementState;
            movementState = MovementState.Right;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (movementState == MovementState.Right)
            {
                return;
            }
            preMovementState = movementState;
            movementState = MovementState.Left;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (movementState == MovementState.Up)
            {
                return;
            }
            preMovementState = movementState;
            movementState = MovementState.Down;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (movementState == MovementState.Down)
            {
                return;
            }
            preMovementState = movementState;
            movementState = MovementState.Up;
        }
    }

    private void HandleMovement(float deltaTime)
    {
        if (timeCounter > 0)
        {
            timeCounter -= deltaTime;
        }
        else
        {
            if (movementState == MovementState.Up)
                MovePlayerUp();
            else if (movementState == MovementState.Down)
                MovePlayerDown();
            else if (movementState == MovementState.Left)
                MovePlayerLeft();
            else if (movementState == MovementState.Right)
                MovePlayerRight();

            timeCounter = cellPerSecond;
        }
    }

    public void HandleFood(float deltaTime)
    {
        if (foodCount >= maxFood)
            return;

        if (foodTimeCounter > 0)
        {
            foodTimeCounter -= deltaTime;
        }
        else
        {
            Vector2Int randPosition;

            while (true)
            {
                randPosition = new Vector2Int((int)Random.Range(0, width - 1), (int)Random.Range(0, height - 1));
                if (!CompareCellColor(randPosition, Color.blue)
                    && !CompareCellColor(randPosition, Color.red)
                    && !CompareCellColor(randPosition, Color.green))
                {
                    break;
                }
            }

            mapMatrix[randPosition.x, randPosition.y] = 2;
            SetCellColor(randPosition, Color.green);

            foodCount++;
            foodTimeCounter = foodSpawnTime;
        }
    }

    public void MovePlayerUp()
    {
        if (IsValidMove())
        {
            UpdatePlayerBody();

            head.x--;

            if (head.x < 0)
                head.x = height - 1;
            else if (head.x >= height)
                head.x = 0;

            playerBody[0] = head;

            if (IsFood(head))
                AddPlayerSize();

            if (IsPlayerBody(head))
            {
                return;
            }

            UpdateMatrix();
        }
    }

    public void MovePlayerDown()
    {
        if (IsValidMove())
        {
            UpdatePlayerBody();

            head.x++;

            if (head.x < 0)
                head.x = height - 1;
            else if (head.x >= height)
                head.x = 0;

            playerBody[0] = head;

            if (IsFood(head))
                AddPlayerSize();

            if (IsPlayerBody(head))
            {
                return;
            }

            UpdateMatrix();
        }

    }

    public void MovePlayerLeft()
    {
        if (IsValidMove())
        {
            UpdatePlayerBody();

            head.y--;

            if (head.y < 0)
                head.y = width - 1;
            else if (head.y >= width)
                head.y = 0;

            playerBody[0] = head;

            if (IsFood(head))
                AddPlayerSize();

            if (IsPlayerBody(head))
            {
                return;
            }

            UpdateMatrix();
        }

    }

    public void MovePlayerRight()
    {
        if (IsValidMove())
        {
            UpdatePlayerBody();

            head.y++;

            if (head.y < 0)
                head.y = width - 1;
            else if (head.y >= width)
                head.y = 0;

            playerBody[0] = head;

            if (IsFood(head))
                AddPlayerSize();

            if (IsPlayerBody(head))
            {
                return;
            }

            UpdateMatrix();
        }
    }

    public void AddPlayerSize()
    {
        if (playerSize == PlayerMaxSize)
        {
            return;
        }

        foodCount--;
        playerBody[playerSize + 1] = playerBody[playerSize];

        SetCellColor(playerBody[playerSize], Color.blue);

        playerSize++;
    }

    private bool IsValidMove()
    {
        if (movementState == MovementState.Down && preMovementState == MovementState.Up)
            return false;
        else if (movementState == MovementState.Up && preMovementState == MovementState.Down)
            return false;
        else if (movementState == MovementState.Left && movementState == MovementState.Right)
            return false;
        else if (movementState == MovementState.Right && movementState == MovementState.Left)
            return false;

        return true;
    }

    private bool IsFood(Vector2Int nextMove)
    {
        if (mapMatrix[nextMove.x, nextMove.y] == 2)
        {
            return true;
        }

        return false;
    }

    private bool IsPlayerBody(Vector2Int nextMove)
    {
        //if(CompareCellColor(nextMove, Color.blue))
        if (mapMatrix[nextMove.x, nextMove.y] == 1)
        {
            movementState = MovementState.Stop;
            return true;
        }
        return false;
    }

    public void UpdatePlayerBody()
    {
        for (int i = playerSize; i >= 1; i--)
        {
            playerBody[i].x = playerBody[i - 1].x;
            playerBody[i].y = playerBody[i - 1].y;
        }
    }

    public void UpdateMatrix()
    {
        mapMatrix[playerBody[0].x, playerBody[0].y] = 1;

        for (int i = 1; i < playerSize; i++)
        {
            mapMatrix[playerBody[i].x, playerBody[i].y] = 1;
            SetCellColor(playerBody[i], Color.blue);
        }

        mapMatrix[playerBody[playerSize].x, playerBody[playerSize].y] = 0;
        SetCellColor(playerBody[playerSize], Color.white);
        SetCellColor(head, Color.red);
    }

    public void SetCellColor(Vector2Int position, Color color)
    {
        cells[position.x, position.y].GetComponent<SpriteRenderer>().color = color;
    }

    public bool CompareCellColor(Vector2Int position, Color color)
    {
        if (cells[position.x, position.y].GetComponent<SpriteRenderer>().color == color)
            return true;

        return false;
    }
}
