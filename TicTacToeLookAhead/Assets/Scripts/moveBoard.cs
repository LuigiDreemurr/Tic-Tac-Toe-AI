using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class moveBoard
{

    private const int NO_SCORE = -100000; //exesive but great
    private const int WIN_SCORE = 1;
    private const int LOSE_SCORE = -1;
    private const int TIE_SCORE = 0;

    private Move rootMove;
    private GameBoard.PLAYERS_ID currentPlayer;
    private bool isMoveGameFinished;
    public Move[,] gameMoveBoard;

    public void makeMove(Move nextMove, GameBoard.PLAYERS_ID nextMovePlayer)
    {
        
        //add move to our scratch pad game board
        AddMovetoGameMoveBoard(nextMove, nextMovePlayer);

        nextMove.setOccupied(nextMovePlayer);

        //if the nextMove is made then create a list of children with the remaining possible moves
        AddPossibleMovesForNextMove(nextMove);

    }

    public void AddMovetoGameMoveBoard(Move nextMove, GameBoard.PLAYERS_ID nextMovePlayer)
    {

        int nextMoveRow = nextMove.Row;
        int nextMoveColumn = nextMove.Column;

        //update the board with the move by the currentplayer
        gameMoveBoard[nextMoveRow, nextMoveColumn].setOccupied(nextMovePlayer);

    }

    public void RemoveMoveFromGameMoveBoard(Move nextMove)
    {

        int nextMoveRow = nextMove.Row;
        int nextMoveColumn = nextMove.Column;

        //clear the board with the move by the currentplayer
        gameMoveBoard[nextMoveRow, nextMoveColumn].clearOccupied();

    }

    // returns the score for the current position from the point of view of the given player.
    public Move evaluate(moveBoard board, Move evalMove)
    {

        int boardScore = evalBoardForAI();
        evalMove.Score = boardScore;
        return evalMove;

    }

    public int evalBoardForAI()
    {

        int boardScore = TIE_SCORE;

        if (CheckForWinner(GameBoard.PLAYERS_ID.PLAYER_ONE, GameBoard.CONNECT_NUM))
        {

            boardScore = LOSE_SCORE;

        }
        else if (CheckForWinner(GameBoard.PLAYERS_ID.PLAYER_TWO, GameBoard.CONNECT_NUM))
        {

            boardScore = WIN_SCORE;

        }

        return boardScore;

    }

    public bool CheckForWinner(GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bWon = false;

        for (int i = 0; i < GameBoard.MAX_ROWS; i++)
        {

            for (int j = 0; j < GameBoard.MAX_COLUMNS; j++)
            {

                Move movePiece = gameMoveBoard[i, j];

                if (playerTurnID == movePiece.PlayerID)
                {

                    int foundMatchCount = 0;

                    //Check above,
                    if (CheckPatternAbove(movePiece, ref foundMatchCount, playerTurnID, maxConnect))
                    {

                        bWon = true;
                        //found a winner 
                        break;

                    }

                    foundMatchCount = 0;
                    //Check below,
                    if (CheckPatternBelow(movePiece, ref foundMatchCount, playerTurnID, maxConnect))
                    {

                        //found a winner 
                        bWon = true;
                        break;

                    }
                    foundMatchCount = 0;

                    //Check right,
                    if (CheckPatternRight(movePiece, ref foundMatchCount, playerTurnID, maxConnect))
                    {

                        //found a winner 
                        bWon = true;
                        break;

                    }

                    foundMatchCount = 0;

                    //Check Left,
                    if (CheckPatternLeft(movePiece, ref foundMatchCount, playerTurnID, maxConnect))
                    {

                        //found a winner 
                        bWon = true;
                        break;

                    }

                    foundMatchCount = 0;

                    //check upper right
                    if (CheckPatternAboveRight(movePiece, ref foundMatchCount, playerTurnID, maxConnect))
                    {

                        //found a winner 
                        bWon = true;
                        break;

                    }

                    foundMatchCount = 0;

                    //check upper left
                    if (CheckPatternAboveLeft(movePiece, ref foundMatchCount, playerTurnID, maxConnect))
                    {

                        //found a winner 
                        bWon = true;
                        break;

                    }

                    foundMatchCount = 0;

                    //check lower right
                    if (CheckPatternBelowRight(movePiece, ref foundMatchCount, playerTurnID, maxConnect))
                    {

                        //found a winner 
                        bWon = true;
                        break;

                    }

                    foundMatchCount = 0;

                    //check lower left
                    if (CheckPatternBelowLeft(movePiece, ref foundMatchCount, playerTurnID, maxConnect))
                    {

                        //found a winner 
                        bWon = true;
                        break;

                    }

                }

            }

            if (bWon)
            {

                break;

            }

        }

        return bWon;

    }

    public bool isGameOver()
    {

        isMoveGameFinished = false;

        if (CheckForWinner(GameBoard.PLAYERS_ID.PLAYER_ONE, GameBoard.CONNECT_NUM))
        {

            isMoveGameFinished = true;

        }
        else if (CheckForWinner(GameBoard.PLAYERS_ID.PLAYER_TWO, GameBoard.CONNECT_NUM))
        {

            isMoveGameFinished = true;

        }

        return isMoveGameFinished;

    }

    public GameBoard.PLAYERS_ID CurrentPlayer
    {

        get
        {

            return currentPlayer;

        }

        set
        {

            currentPlayer = value;

        }

    }

    public Move Root
    {

        get
        {

            return rootMove;

        }

        set
        {

            rootMove = value;

        }

    }

    public moveBoard()
    {

        rootMove = new Move(-1, -1);
        gameMoveBoard = new Move[GameBoard.MAX_ROWS, GameBoard.MAX_COLUMNS];
        initMoveBoard();

    }

    public void initMoveBoard()
    {

        rootMove.ClearChildrenList();
        isMoveGameFinished = false;
        currentPlayer = GameBoard.PLAYERS_ID.PLAYER_NONE;

        for (int i = 0; i < GameBoard.MAX_ROWS; i++)
        {

            for (int j = 0; j < GameBoard.MAX_COLUMNS; j++)
            {

                gameMoveBoard[i, j] = new Move(i, j);
                gameMoveBoard[i, j].Score = 0;

            }

        }

    }

    public void setCurrentGameBoardToMoveBoard(GameBoard.PLAYERS_ID initPlayer)
    {

        for (int i = 0; i < GameBoard.MAX_ROWS; i++)
        {

            for (int j = 0; j < GameBoard.MAX_COLUMNS; j++)
            {

                gameMoveBoard[i, j].Occupied = GameBoard.gameBoard[i, j].Occupied;
                gameMoveBoard[i, j].PlayerID = GameBoard.gameBoard[i, j].PlayerID;

            }

        }

        //set up the root node to match the current game board 
        CurrentPlayer = initPlayer;
        rootMove.PlayerID = initPlayer;
        rootMove.Score = NO_SCORE;
        AddPossibleMovesForNextMove(rootMove);

    }
    private void AddPossibleMovesForNextMove(Move nextMove)
    {

        for (int i = 0; i < GameBoard.MAX_ROWS; i++)
        {

            for (int j = 0; j < GameBoard.MAX_COLUMNS; j++)
            {

                if (!gameMoveBoard[i, j].Occupied)
                {

                    Move posMove = new Move();
                    gameMoveBoard[i, j].Copy(posMove);
                    nextMove.PossibleMoves.Add(posMove);

                }

            }

        }

    }

    private bool CheckPatternAbove(Move movePiece, ref int foundMatchCount, GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bFoundPattern = false;

        //Piece must be current players value
        if (movePiece.PlayerID != GameBoard.PLAYERS_ID.PLAYER_NONE && movePiece.PlayerID == playerTurnID)
        {

            foundMatchCount++;

            if (foundMatchCount == maxConnect)
            {

                bFoundPattern = true;

            }
            else if (movePiece.Row > 0 && foundMatchCount < maxConnect)
            {

                //if haven't found a match check the next squre above
                bFoundPattern = CheckPatternAbove(gameMoveBoard[movePiece.Row - 1, movePiece.Column], ref foundMatchCount, playerTurnID, maxConnect);

            }

        }

        return bFoundPattern;

    }

    private bool CheckPatternBelow(Move movePiece, ref int foundMatchCount, GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bFoundPattern = false;
        if (movePiece.PlayerID != GameBoard.PLAYERS_ID.PLAYER_NONE && movePiece.PlayerID == playerTurnID)
        {

            foundMatchCount++;

            if (foundMatchCount == maxConnect)
            {

                bFoundPattern = true;

            }
            else if (movePiece.Row < (GameBoard.MAX_ROWS - 1) && foundMatchCount < maxConnect)
            {

                //if haven't found a match check the next squre below
                bFoundPattern = CheckPatternBelow(gameMoveBoard[movePiece.Row + 1, movePiece.Column], ref foundMatchCount, playerTurnID, maxConnect);

            }

        }

        return bFoundPattern;

    }

    private bool CheckPatternRight(Move movePiece, ref int foundMatchCount, GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bFoundPattern = false;
        if (movePiece.PlayerID != GameBoard.PLAYERS_ID.PLAYER_NONE && movePiece.PlayerID == playerTurnID)
        {

            foundMatchCount++;
            if (foundMatchCount == maxConnect)
            {

                bFoundPattern = true;

            }
            else if (movePiece.Column < (GameBoard.MAX_COLUMNS - 1) && foundMatchCount < maxConnect)
            {

                //if haven't found a match check the next squre to the right
                bFoundPattern = CheckPatternRight(gameMoveBoard[movePiece.Row, movePiece.Column + 1], ref foundMatchCount, playerTurnID, maxConnect);

            }

        }

        return bFoundPattern;

    }

    private bool CheckPatternLeft(Move movePiece, ref int foundMatchCount, GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bFoundPattern = false;

        if (movePiece.PlayerID != GameBoard.PLAYERS_ID.PLAYER_NONE && movePiece.PlayerID == playerTurnID)
        {

            foundMatchCount++;

            if (foundMatchCount == maxConnect)
            {

                bFoundPattern = true;

            }
            else if (movePiece.Column > 0 && foundMatchCount < maxConnect)
            {

                //if haven't found a match check the next squre above
                bFoundPattern = CheckPatternLeft(gameMoveBoard[movePiece.Row, movePiece.Column - 1], ref foundMatchCount, playerTurnID, maxConnect);

            }

        }

        return bFoundPattern;

    }

    private bool CheckPatternAboveRight(Move movePiece, ref int foundMatchCount, GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bFoundPattern = false;

        if (movePiece.PlayerID == playerTurnID)
        {

            foundMatchCount++;

            if (foundMatchCount == maxConnect)
            {

                bFoundPattern = true;

            }
            else if (movePiece.Row > 0 && movePiece.Column < (GameBoard.MAX_COLUMNS - 1) && foundMatchCount < maxConnect)
            {

                bFoundPattern = CheckPatternAboveRight(gameMoveBoard[movePiece.Row - 1, movePiece.Column + 1], ref foundMatchCount, playerTurnID, maxConnect);

            }

        }

        return bFoundPattern;
    }

    private bool CheckPatternAboveLeft(Move movePiece, ref int foundMatchCount, GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bFoundPattern = false;

        if (movePiece.PlayerID == playerTurnID)
        {

            foundMatchCount++;
            if (foundMatchCount == maxConnect)
            {

                bFoundPattern = true;

            }
            else if (movePiece.Row > 0 && movePiece.Column > 0 && foundMatchCount < maxConnect)
            {

                bFoundPattern = CheckPatternAboveLeft(gameMoveBoard[movePiece.Row - 1, movePiece.Column - 1], ref foundMatchCount, playerTurnID, maxConnect);

            }

        }

        return bFoundPattern;

    }

    private bool CheckPatternBelowRight(Move movePiece, ref int foundMatchCount, GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bFoundPattern = false;
        if (movePiece.PlayerID == playerTurnID)
        {

            foundMatchCount++;
            if (foundMatchCount == maxConnect)
            {

                bFoundPattern = true;

            }
            else if (movePiece.Row < (GameBoard.MAX_ROWS - 1) && movePiece.Column < (GameBoard.MAX_COLUMNS - 1) && foundMatchCount < maxConnect)
            {

                bFoundPattern = CheckPatternBelowRight(gameMoveBoard[movePiece.Row + 1, movePiece.Column + 1], ref foundMatchCount, playerTurnID, maxConnect);

            }

        }

        return bFoundPattern;

    }

    private bool CheckPatternBelowLeft(Move movePiece, ref int foundMatchCount, GameBoard.PLAYERS_ID playerTurnID, int maxConnect)
    {

        bool bFoundPattern = false;

        if (movePiece.PlayerID == playerTurnID)
        {

            foundMatchCount++;

            if (foundMatchCount == maxConnect)
            {

                bFoundPattern = true;

            }
            else if (movePiece.Row < (GameBoard.MAX_ROWS - 1) && movePiece.Column > 0 && foundMatchCount < maxConnect)
            {

                bFoundPattern = CheckPatternBelowLeft(gameMoveBoard[movePiece.Row + 1, movePiece.Column - 1], ref foundMatchCount, playerTurnID, maxConnect);

            }

        }

        return bFoundPattern;

    }

}


