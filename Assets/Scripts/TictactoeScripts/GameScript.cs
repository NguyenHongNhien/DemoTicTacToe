using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;


public class GameScript : MonoBehaviour
{

    public static GameScript instance;
    public string gameMode;
    public GameObject player1, player2, bar;
    public Text Instructions, player2Name;

    public enum Seed { EMPTY, PLAYER1, PLAYER2 };


    public Seed Turn;

    // to keep track of the empty, cross and nought cells
    public GameObject[] allSpawns = new GameObject[9];

    // to maintain the state of the cell
    public Seed[] player = new Seed[9];

    // to track the first and last cells of the winning row or column or diagonal
    Vector2 pos1, pos2;
    void MakeInstance(){
        if(instance==null)
        {
            instance = this;
        }
    }
    private void Awake()
    {
        // to get the Game mode information from the previous scene
        GameObject persistantObj = GameObject.FindGameObjectWithTag("PersistantObj") as GameObject;
        gameMode = persistantObj.GetComponent<PersistanceScript>().gameMode;
        Destroy(persistantObj);
        MakeInstance();

        // gameMode = "vscpu";

        if (gameMode == "vscpu")
            player2Name.text = "CPU Player";
        else
            player2Name.text = "2nd Player";

        // set turn as 1st player which is CROSS
        Turn = Seed.PLAYER1;

        // Set initial instruction
        Instructions.text = "Turn: 1st Player";

        // to maintain the state of the cell
        for (int i = 0; i < 9; i++)
            player[i] = Seed.EMPTY;

    }
 
        public void Spawn(GameObject emptycell, int id)
    {
        // conditions to spawn player1 or player2
        if (Turn == Seed.PLAYER1)
        {
            allSpawns[id] = Instantiate(player1, emptycell.transform.position, Quaternion.identity);
            player[id] = Turn;

            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                // change the instructions
                Instructions.text = "Player-1 win!!!";
                               // Spawn bar
                float slope = calculateSlope();
                Instantiate(bar, calculateCenter(), Quaternion.Euler(0, 0, slope));
                WinLoseScript.instance.Player1Win();
                
            }
            else
            {
                // change the turn
                Turn = Seed.PLAYER2;

                // change the instructions
                Instructions.text = "Turn: 2nd Player";
            }
        }

        else if (Turn == Seed.PLAYER2 && gameMode == "2player")
        {
            allSpawns[id] = Instantiate(player2, emptycell.transform.position, Quaternion.identity);
            player[id] = Turn;

            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                // change the instructions
                Instructions.text = "Player-2 win!!!";

                // Spawn bar
                float slope = calculateSlope();
                Instantiate(bar, calculateCenter(), Quaternion.Euler(0, 0, slope));
                WinLoseScript.instance.Player2Win();
                
            }
            else
            {
                // change the turn
                Turn = Seed.PLAYER1;

                // change the instructions
                Instructions.text = "Turn: 1st Player";
            }
        }

        if (Turn == Seed.PLAYER2 && gameMode == "vscpu")
        {
                int bestScore = -1, bestPos = -1, score;

                for (int i = 0; i < 9; i++)
                {
                    if (player[i] == Seed.EMPTY)
                    {
                        player[i] = Seed.PLAYER2;
                        score = minimax(Seed.PLAYER1, player, -1000, +1000);
                        player[i] = Seed.EMPTY;

                        if (bestScore < score)
                        {
                            bestScore = score;
                            bestPos = i;
                        }
                    }
                }

                if (bestPos > -1)
                {
                    allSpawns[bestPos] = Instantiate(player2, allSpawns[bestPos].transform.position, Quaternion.identity);
                    player[bestPos] = Turn;
                }
            

            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                // change the instructions
                Instructions.text = "CPU win!!!";

                // Spawn bar
                float slope = calculateSlope();
                Instantiate(bar, calculateCenter(), Quaternion.Euler(0, 0, slope));
                WinLoseScript.instance.CPUWin();
            }
            else
            {
                // change the turn
                Turn = Seed.PLAYER1;

                // change the instructions
                Instructions.text = "Turn: 1st Player";
            }
        }

        if (IsDraw())
        {
            // change the turn
            Turn = Seed.EMPTY;

            // change the instructions
            Instructions.text = "It is a draw!!";
        }

        Destroy(emptycell);
    }
       bool IsAnyEmpty()
    {
        bool empty = false;
        for (int i = 0; i < 9; i++)
        {
            if (player[i] == Seed.EMPTY)
            {
                empty = true;
                break;
            }
        }
        return empty;
    }

    bool Won(Seed currPlayer)
    {
        bool hasWon = false;

        int[,] allConditions = new int[8, 3] { {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
                                                {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
                                                {0, 4, 8}, {2, 4, 6} };

        // check conditions
        for (int i = 0; i < 8; i++)
        {
            if (player[allConditions[i, 0]] == currPlayer &
                player[allConditions[i, 1]] == currPlayer &
                player[allConditions[i, 2]] == currPlayer)
            {
                hasWon = true;

                // keep track of the winning positions to spawn a Bar
                pos1 = allSpawns[allConditions[i, 0]].transform.position;
                pos2 = allSpawns[allConditions[i, 2]].transform.position;
                break;
            }
        }
        return hasWon;
    }

    bool IsDraw()
    {
        bool player1Won, player2Won, anyEmpty;

        // check if player-1 has won or not
        player1Won = Won(Seed.PLAYER1);

        // check if player-2 has won or not
        player2Won = Won(Seed.PLAYER2);

        // check if there is any empty cell or not
        anyEmpty = IsAnyEmpty();

        bool isDraw = false;

        if (player1Won == false & player2Won == false & anyEmpty == false)
            isDraw = true;

        return isDraw;
    }

    Vector2 calculateCenter()
    {
        float x = (pos1.x + pos2.x) / 2,
            y = (pos1.y + pos2.y) / 2;

        return new Vector2(x, y);
    }

    float calculateSlope()
    {
        float slope;

        if (pos1.x == pos2.x)
            slope = 0.0f;
        else if (pos1.y == pos2.y)
            slope = 90.0f;
        else if (pos1.x > 0.0f)
            slope = -45.0f;
        else
            slope = 45.0f;

        return slope;
    }

    int minimax(Seed currPlayer, Seed[] board, int alpha, int beta)
    {

        if (IsDraw())
            return 0;

        if (Won(Seed.PLAYER2))
            return +1;

        if (Won(Seed.PLAYER1))
            return -1;


        int score;

        if(currPlayer == Seed.PLAYER2)
        {
            for(int i=0; i < 9; i++)
            {
                if(board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.PLAYER2;
                    score = minimax(Seed.PLAYER1, board, alpha, beta);
                    board[i] = Seed.EMPTY;

                    if (score > alpha)
                        alpha = score;

                    if (alpha > beta)
                        break;
                }
            }

            return alpha;
        }

        else
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.PLAYER1;
                    score = minimax(Seed.PLAYER2, board, alpha, beta);
                    board[i] = Seed.EMPTY;

                    if (score < beta)
                        beta = score;

                    if (alpha > beta)
                        break;
                }
            }

            return beta;
        }
    }
    

}
