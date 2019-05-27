using UnityEngine;
using System.Collections;

/// <summary>
/// Script was aquired from Niagara College and altured and studied by MATT
/// Game piece is what it sounds like, the script that goes on the game piece.
/// </summary>
public class GamePiece : MonoBehaviour
{

    //private members
    public const float BOUND_MIN_X = -12;
    public const float BOUND_MAX_X = 15;
    public const float BOUND_MIN_Z = -15;
    public const float BOUND_MAX_Z = 10;
    public float spaceBetween = 1;

    private const float YPos = 2.0f;

    private GameBoard.PLAYERS_ID playerID;
    private float _speed = 5f;
    private float _maxSpeed = 10f;
    private bool isOccupied = false;
    private bool _inBounds = true;

    private Vector3 _startPosition = Vector3.zero;
    private Vector3 _endPosition = Vector3.zero;

    private int row;
    private int column;

    /// <summary>
    /// Same game piece but for diffrent players
    /// </summary>
    public GameBoard.PLAYERS_ID PlayerID
    {

        get
        {

            return playerID;

        }

        set
        {

            playerID = value;

        }

    }

    public int Row
    {

        get
        {

            return row;

        }

        set
        {

            row = value;

        }

    }

    public int Column
    {

        get
        {

            return column;

        }

        set
        {

            column = value;

        }

    }

    /// <summary>
    /// Game funtions with empty/player zero game pieces that can get
    /// a new value from current player, if its not already taken
    /// </summary>
    public bool Occupied
    {

        get
        {

            return isOccupied;

        }

        set
        {

            isOccupied = value;

        }

    }

    void Awake()
    {

        spaceBetween = 4;
        row = -1;
        column = -1;
        isOccupied = false;

    }

    public void SetStartPosition(Vector3 startPos)
    {

        _startPosition = startPos;

    }

    public void SetEndPosition(Vector3 endPos)
    {

        _endPosition = endPos;

    }

    public void SetParameters(float speed, float maxSpeed, int boardRow, int boardColumn)
    {

        _speed = speed;
        _maxSpeed = maxSpeed;
        row = boardRow;
        column = boardColumn;

    }

    public void SetBoundValid(bool inbound)
    {

        _inBounds = inbound;

    }

    // Update is called once per frame
    void Update()
    {

        transform.position = _endPosition;

    }

    private void OnMouseDown()
    {

        print("game piece " + " row: " + row + ", column: " + column);
        GameBoard.BoardClicked(row, column, isOccupied, GameBoard.PLAYERS_ID.PLAYER_ONE);

    }

    public void SetColor(Color newColor)
    {

        GetComponent<Renderer>().material.color = newColor;

    }

    public void AddPlayerToSquare(GameBoard.PLAYERS_ID playerNum, int boardRow, int boardColumn)
    {

        isOccupied = true;
        playerID = playerNum;
        row = boardRow;
        column = boardColumn;

    }

    public void InitPlayerSquare(int boardRow, int boardColumn)
    {

        isOccupied = false;
        playerID = GameBoard.PLAYERS_ID.PLAYER_NONE;
        row = boardRow;
        column = boardColumn;
        SetGamePiecePositions();
        SetParameters(5.0f, 10.0f, boardRow, boardColumn);
        GetComponent<Renderer>().material.color = new Color(255, 255, 0);

    }

    void SetGamePiecePositions()
    {

        float startX = GamePiece.BOUND_MIN_X + (column * spaceBetween);
        float endX = GamePiece.BOUND_MIN_X + (column * spaceBetween);

        float startZ = GamePiece.BOUND_MAX_Z;
        float endZ = GamePiece.BOUND_MAX_Z - row * spaceBetween;

        SetStartPosition(new Vector3(startX, YPos, startZ));
        SetEndPosition(new Vector3(endX, YPos, endZ));

    }

}
