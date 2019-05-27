using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Move
{

    private int score;
    private int rowPos;
    private int colPos;
    private bool isOccupied;
    private int currentDepth; //used for best move (store the current depth of the best found move)
    private GameBoard.PLAYERS_ID occupiedByPlayer;
    private List<Move> children;

    public int Depth
    {

        get
        {

            return currentDepth;

        }

        set
        {

            currentDepth = value;

        }

    }

    public int Score
    {

        get
        {

            return score;

        }

        set
        {

            score = value;

        }

    }

    public int Row
    {

        get
        {

            return rowPos;

        }

        set
        {

            rowPos = value;

        }

    }

    public int Column
    {

        get
        {

            return colPos;

        }

        set
        {

            colPos = value;

        }

    }

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

    public List<Move> PossibleMoves
    {

        get
        {

            return children;

        }

    }

    public GameBoard.PLAYERS_ID PlayerID
    {

        get
        {

            return occupiedByPlayer;

        }

        set
        {

            occupiedByPlayer = value;

        }

    }

    public void Copy(Move copyMove)
    {

        copyMove.Score = Score;
        copyMove.Row = Row;
        copyMove.Column = Column;
        copyMove.PlayerID = PlayerID;
        copyMove.Occupied = Occupied;
        copyMove.currentDepth = currentDepth;

    }

    public void ClearChildrenList()
    {

        children.Clear();

    }

    public Move(int row, int column)
    {

        children = new List<Move>();
        initMove(row, column);

    }

    public Move()
    {

        children = new List<Move>();
        initMove(-1, -1);

    }

    public void initMove(int row, int column)
    {

        score = 0;
        rowPos = row;
        colPos = column;
        isOccupied = false;
        children.Clear();
        occupiedByPlayer = GameBoard.PLAYERS_ID.PLAYER_NONE;
        currentDepth = 0;

    }

    public void setOccupied(GameBoard.PLAYERS_ID occupiedBy)
    {

        isOccupied = true;
        occupiedByPlayer = occupiedBy;

    }

    public void clearOccupied()
    {

        isOccupied = false;
        occupiedByPlayer = GameBoard.PLAYERS_ID.PLAYER_NONE;

    }

}
