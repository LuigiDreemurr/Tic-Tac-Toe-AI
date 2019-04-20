using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{

    public enum PLAYERS_ID : byte { PLAYER_NONE = 0, PLAYER_ONE, PLAYER_TWO };

    public const int MAX_COLUMNS = 3;   //max width of game board
    public const int MAX_ROWS = 3;      //max height of game board

    public const int CONNECT_NUM = 3;   //number of squares to connect in row in order to win
    public const int MAX_DEPTH = 9;     //max depth level for miniMax algorithm
    public const int INFINITY = 500000;

    public GameObject gamePiecePrefab;

    static bool gameFinished = false;

    static public GamePiece[,] gameBoard = new GamePiece[MAX_ROWS, MAX_COLUMNS];
    static PLAYERS_ID winningPlayer = PLAYERS_ID.PLAYER_NONE;

    //
    //This function is used to claim a move (by playerID) on the actual game board
    //
    static public void BoardClicked(int clickedRow, int clickedColumn, bool isOccupied, PLAYERS_ID playerID) //////////////////////////////////////////////////////////////////////////////////////////////////////////
    {
        if (!gameFinished && clickedColumn >= 0 && clickedColumn < MAX_COLUMNS && clickedRow >= 0 && clickedRow < MAX_ROWS)
        {
            if (!isOccupied)
            {
                Color colourPiece;
                if (playerID == PLAYERS_ID.PLAYER_TWO)
                {
                    colourPiece = new Color(0, 0, 255);
                }
                else
                {
                    colourPiece = new Color(255, 0, 0);
                }

                gameBoard[clickedRow, clickedColumn].AddPlayerToSquare(playerID, clickedRow, clickedColumn);
                gameBoard[clickedRow, clickedColumn].SetColor(colourPiece);

                if (CheckForWinner(gameBoard[clickedRow, clickedColumn]))
                {
                    print("Player " + playerID + " has won!!!!");
                    winningPlayer = playerID;
                    gameFinished = true;
                }
                else if (CheckForTie())
                {
                    winningPlayer = PLAYERS_ID.PLAYER_NONE;
                    print("GAME TIED!!!!");
                    gameFinished = true;
                }
            }
        }
    }
    //
    //This function implements the Minimax algorithm
    //
    //moveboard board   --represents the current game board as a scratch pad to make moves on (before actually making the move)
    //                  --board also holds the actual player's turn for the game (first player in the tree)
    //Move nextMove     --represents the next move to be made on the game board
    //                  --nexMove also holds a score value for the move
    //evaluatePlayer    --represents the players turn on the tree ( it will change as we traverse each level of the tree )
    //
    //maxDepth          --represents the number of levels to use in the minimax algorithm (how far down the tree do we want to go?)
    //
    //currentDepth      --represents the current leve we are evaluating
    //
    //return            --this function returns the best move to make on the board

    static public Move miniMax(moveBoard board, Move nextMove, GameBoard.PLAYERS_ID evaluatePlayer, int maxDepth, int currentDepth)
    {
        //
        //create a bestmove to return
        //
        Move bestMove = new Move();

        //
        //if game is over ( no more moves left/win/loss ) || we have reached our max depth to search || there are no children for this move and it isnt the very first move 
        //
        if (board.isGameOver() || currentDepth == maxDepth || ((currentDepth != 0) && nextMove.PossibleMoves.Count == 0))
        {//
         //
         //evaluate the board and return the best move for this board, the move data structure also holds a score value for the board
         //
            Move eMove = board.evaluate(board, nextMove);
            eMove.Depth = currentDepth;

            //
            //
            //copy the evaluated move into bestMove
            //
            eMove.Copy(bestMove);

        }
        else
        {   //
            //if we haven't reached a terminal node traverse the children and call minimax
            //
            Move currentBubbledUpScore = null;

            //
            //Initialize the bestmove object
            //
            nextMove.Copy(bestMove);
            bestMove.Score = INFINITY;

            //
            //Ask: whose turn is it on the tree? Keep track of the max or min score for all the possible moves (children)
            //
            if (board.CurrentPlayer == evaluatePlayer)
            {

                bestMove.Score = -INFINITY;

            }

            //
            //Loop through all the possible moves from nextMove
            //
            foreach (Move move in nextMove.PossibleMoves)
            {
                //
                //make the move ( take the turn ) and then call minimax for the next move
                //makeMove also adds the move to our move game board scratch pad
                //
                board.makeMove(move, evaluatePlayer);
                currentBubbledUpScore = miniMax(board, move, GetNextPlayerID(evaluatePlayer), maxDepth, currentDepth + 1);

                //
                //remove move from our game board (game board scratch pad)
                //
                board.RemoveMoveFromGameMoveBoard(move);

                //
                //if we are the current player, then take the highest score found of all the moves
                //
                if (board.CurrentPlayer == evaluatePlayer)
                {

                    if (currentBubbledUpScore.Score > bestMove.Score)
                    {

                        bestMove.Row = move.Row;
                        bestMove.Column = move.Column;
                        bestMove.Score = currentBubbledUpScore.Score;
                        bestMove.Depth = currentBubbledUpScore.Depth;

                    }
                    else if (currentBubbledUpScore.Score == bestMove.Score)
                    {

                        if (currentBubbledUpScore.Depth < bestMove.Depth)
                        {

                            bestMove.Row = move.Row;
                            bestMove.Column = move.Column;
                            bestMove.Score = currentBubbledUpScore.Score;
                            bestMove.Depth = currentBubbledUpScore.Depth;

                        }

                    }
                    

                }
                else
                {

                    if (currentBubbledUpScore.Score < bestMove.Score)
                    {

                        bestMove.Row = move.Row;
                        bestMove.Column = move.Column;
                        bestMove.Score = currentBubbledUpScore.Score;
                        bestMove.Depth = currentBubbledUpScore.Depth;

                    }

                }
                //
                // the opponent is the current player then take the lowest score of all the moves
                //

            }
        }
        return bestMove;
    }

    static public PLAYERS_ID GetNextPlayerID(PLAYERS_ID currPlayer)
    {
        PLAYERS_ID nextPlayer = GameBoard.PLAYERS_ID.PLAYER_NONE;
        //Update the current player to the next player
        if (currPlayer == GameBoard.PLAYERS_ID.PLAYER_ONE)
        {
            nextPlayer = GameBoard.PLAYERS_ID.PLAYER_TWO;
        }
        else if (currPlayer == GameBoard.PLAYERS_ID.PLAYER_TWO)
        {
            nextPlayer = GameBoard.PLAYERS_ID.PLAYER_ONE;
        }
        return nextPlayer;
    }


    static Move getBestMove(GameBoard.PLAYERS_ID player)
    {
        //make a moveBoard of the current state of the board
        moveBoard currentMoveBoard = new moveBoard();
        //this move board is used as a scratch pad to examine possible next moves
        currentMoveBoard.setCurrentGameBoardToMoveBoard(player);
        //using the moveboard the minimax function finds the next BEST move
        Move bestMove = miniMax(currentMoveBoard, currentMoveBoard.Root, player, MAX_DEPTH, 0);
        return bestMove;
    }

    static public void AI_BoardTurn()
    {
        if (!gameFinished)
        {
            //Get the best move from the AI!!!!
            Move BestMove = getBestMove(PLAYERS_ID.PLAYER_TWO);
            //make the best move on the board	
            BoardClicked(BestMove.Row, BestMove.Column, false, PLAYERS_ID.PLAYER_TWO);
        }
    }

    //if there are no more moves and no winner/loser then there is a tie
    static bool CheckForTie()
    {
        bool gameTied = true;
        for (int i = 0; i < MAX_ROWS; i++)
        {
            for (int j = 0; j < MAX_COLUMNS; j++)
            {
                if (!(gameBoard[i, j].Occupied))
                {
                    gameTied = false;
                }
            }
        }
        return gameTied;
    }
    //checks the actual game board for a winner
    static bool CheckForWinner(GamePiece playerGamePiece) //////////////////////////////////////////////////////////////////////////////////////////////////////////
    {
        bool bWon = false;
        PLAYERS_ID playerTurnID = playerGamePiece.PlayerID;

        for (int i = 0; i < MAX_ROWS; i++)
        {
            for (int j = 0; j < MAX_COLUMNS; j++)
            {
                GamePiece gamePiece = gameBoard[i, j];
                if (playerTurnID == gamePiece.PlayerID)
                {
                    int foundMatchCount = 0;
                    //Check above,
                    if (CheckPiecesAbove(ref gamePiece, ref foundMatchCount, playerTurnID))
                    {
                        bWon = true;
                        //found a winner 
                        break;
                    }
                    foundMatchCount = 0;
                    //Check below,
                    if (CheckPiecesBelow(ref gamePiece, ref foundMatchCount, playerTurnID))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }
                    foundMatchCount = 0;

                    //Check right,
                    if (CheckPiecesRight(ref gamePiece, ref foundMatchCount, playerTurnID))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }
                    foundMatchCount = 0;

                    //Check Left,
                    if (CheckPiecesLeft(ref gamePiece, ref foundMatchCount, playerTurnID))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }
                    foundMatchCount = 0;

                    //check upper right
                    if (CheckPiecesAboveRight(ref gamePiece, ref foundMatchCount, playerTurnID))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }
                    foundMatchCount = 0;
                    //check upper left
                    if (CheckPiecesAboveLeft(ref gamePiece, ref foundMatchCount, playerTurnID))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }
                    foundMatchCount = 0;
                    //check lower right
                    if (CheckPiecesBelowRight(ref gamePiece, ref foundMatchCount, playerTurnID))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }

                    foundMatchCount = 0;
                    //check lower left
                    if (CheckPiecesBelowLeft(ref gamePiece, ref foundMatchCount, playerTurnID))
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

    static bool CheckPiecesAbove(ref GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if (playerGamePiece.Row > 0 && foundMatchCount < CONNECT_NUM)
            {
                bFoundWin = CheckPiecesAbove(ref gameBoard[playerGamePiece.Row - 1, playerGamePiece.Column], ref foundMatchCount, playerTurnID);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }

    static bool CheckPiecesBelow(ref GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if (playerGamePiece.Row < (MAX_ROWS - 1) && foundMatchCount < CONNECT_NUM)
            {
                //if haven't found a match check the next squre above
                bFoundWin = CheckPiecesBelow(ref gameBoard[playerGamePiece.Row + 1, playerGamePiece.Column], ref foundMatchCount, playerTurnID);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }

    static bool CheckPiecesRight(ref GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if (playerGamePiece.Column < (MAX_COLUMNS - 1) && foundMatchCount < CONNECT_NUM)
            {
                //if haven't found a match check the next squre above
                bFoundWin = CheckPiecesRight(ref gameBoard[playerGamePiece.Row, playerGamePiece.Column + 1], ref foundMatchCount, playerTurnID);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }

    static bool CheckPiecesLeft(ref GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if (playerGamePiece.Column > 0 && foundMatchCount < CONNECT_NUM)
            {
                //if haven't found a match check the next squre above
                bFoundWin = CheckPiecesLeft(ref gameBoard[playerGamePiece.Row, playerGamePiece.Column - 1], ref foundMatchCount, playerTurnID);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }


    static bool CheckPiecesAboveRight(ref GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if (playerGamePiece.Row > 0 && playerGamePiece.Column < (MAX_COLUMNS - 1) && foundMatchCount < CONNECT_NUM)
            {
                //if haven't found a match check the next squre above
                bFoundWin = CheckPiecesAboveRight(ref gameBoard[playerGamePiece.Row - 1, playerGamePiece.Column + 1], ref foundMatchCount, playerTurnID);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }

    static bool CheckPiecesAboveLeft(ref GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if (playerGamePiece.Row > 0 && playerGamePiece.Column > 0 && foundMatchCount < CONNECT_NUM)
            {
                //if haven't found a match check the next squre above
                bFoundWin = CheckPiecesAboveLeft(ref gameBoard[playerGamePiece.Row - 1, playerGamePiece.Column - 1], ref foundMatchCount, playerTurnID);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }
    static bool CheckPiecesBelowRight(ref GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if (playerGamePiece.Row < (MAX_ROWS - 1) && playerGamePiece.Column < (MAX_COLUMNS - 1) && foundMatchCount < CONNECT_NUM)
            {
                bFoundWin = CheckPiecesBelowRight(ref gameBoard[playerGamePiece.Row + 1, playerGamePiece.Column + 1], ref foundMatchCount, playerTurnID);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }
    static bool CheckPiecesBelowLeft(ref GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID)
    {
        bool bFoundWin = false;

        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if (playerGamePiece.Row < (MAX_ROWS - 1) && playerGamePiece.Column > 0 && foundMatchCount < CONNECT_NUM)
            {
                bFoundWin = CheckPiecesBelowLeft(ref gameBoard[playerGamePiece.Row + 1, playerGamePiece.Column - 1], ref foundMatchCount, playerTurnID);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }
    //Update the UI
    void OnGUI() //////////////////////////////////////////////////////////////////////////////////////////////////////////
    {

        bool newGameButtonClicked = GUILayout.Button("NewGame");
        bool AITurnButtonClicked = GUILayout.Button("AI Turn");

        //Start a new game if necessary
        if (newGameButtonClicked)
        {
            InitGameBoard(false);
        }

        //Let the AI take a turn
        if (AITurnButtonClicked)
        {
            AI_BoardTurn();
        }

        //check for game end
        if (gameFinished)
        {
            if (winningPlayer == PLAYERS_ID.PLAYER_NONE)
            {
                GUI.Label(new Rect(0, 300, 300, 300), "Game Tied!");
            }
            else if (winningPlayer == PLAYERS_ID.PLAYER_ONE)
            {
                GUI.Label(new Rect(0, 300, 300, 300), "Player has won!");
            }
            else if (winningPlayer == PLAYERS_ID.PLAYER_TWO)
            {
                GUI.Label(new Rect(0, 300, 300, 300), "AI has won!");
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        InitGameBoard(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Initialize the actual game board
    void InitGameBoard(bool init) //////////////////////////////////////////////////////////////////////////////////////////////////////////
    {
        //playerTurn = 0;
        gameFinished = false;
        winningPlayer = PLAYERS_ID.PLAYER_NONE;

        for (int i = 0; i < MAX_ROWS; i++)
        {
            for (int j = 0; j < MAX_COLUMNS; j++)
            {
                if (init)
                {
                    GameObject gamePieceObj = (GameObject)Instantiate(gamePiecePrefab);  //calls the awake function, then returns 
                    gameBoard[i, j] = gamePieceObj.GetComponent<GamePiece>();
                }
                gameBoard[i, j].InitPlayerSquare(i, j);
            }
        }
    }

}

