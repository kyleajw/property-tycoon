using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject[] pieces;
    [SerializeField] GameObject dicePrefab;
    [SerializeField] GameObject diceSpawnLocation;
    [SerializeField] Board board;
    [SerializeField] float diceSpawnOffset = 0.1f;
    [SerializeField] int balance = 1500;
    [SerializeField] int jailTilePosition;

    GameObject piece;

    bool isMyTurn;
    bool isHuman;
    bool isMoving = false;
    bool hasThrown = false;
    bool rolled = false;
    bool isInJail = false;
    bool finishedTurn;

    int doublesThisTurn =0;
    int playerNumber;
    int dice1Value;
    int dice2Value;
    int position = 0;

    public void AssignPiece(int i)
    {
        piece=pieces[i];
        Debug.Log(piece);
    }

    private void Update()
    {
        if (rolled)
        {
            if(dice1Value == dice2Value)
            {
                doublesThisTurn++;

                if(doublesThisTurn == 3)
                {
                    GoToJail();
                }
            }
            MovePiece(dice1Value + dice2Value);
            rolled = false;
            
        }
    }

    /// <summary>
    /// Rolls both dice on the board, moves piece by however amount
    /// </summary>
    public void RollDice(float multiplier)
    {
        hasThrown = true;
        StartCoroutine(WaitForDiceToFinishRolling(multiplier));
    }

    void MovePiece(int steps)
    {
        Debug.Log($"moving by {steps} steps");
        isMoving = true;
        StartCoroutine(MovePlayerAnimation(steps));

    }

    IEnumerator WaitForDiceToFinishRolling(float multiplier)
    {
        GameObject dice1 = Instantiate(dicePrefab, diceSpawnLocation.transform.position, diceSpawnLocation.transform.rotation);
        GameObject dice2 = Instantiate(dicePrefab,  new Vector3(diceSpawnLocation.transform.position.x + diceSpawnOffset, diceSpawnLocation.transform.position.y, diceSpawnLocation.transform.position.z), diceSpawnLocation.transform.rotation);

        dice1.name = "Dice1";
        dice2.name = "Dice2";


        dice1.GetComponent<Dice>().Roll(multiplier);
        dice2.GetComponent<Dice>().Roll(multiplier);
        yield return new WaitUntil(() => !(dice1.GetComponent<Dice>().isRolling() && dice2.GetComponent<Dice>().isRolling()));
        dice1Value = dice1.GetComponent<Dice>().GetSideFacingUp();
        dice2Value = dice2.GetComponent<Dice>().GetSideFacingUp();
        rolled = true;

        yield return new WaitForSeconds(1.5f);

        Destroy(dice1);
        Destroy(dice2);

    }

    IEnumerator MovePlayerAnimation(int steps)
    {
        for(int i = 0; i < steps; i++)
        {
            position ++;
            if(position>= board.GetTileArray().Length)
            {
                position = 0;
            }
            transform.position = board.GetTileArray()[position].transform.position;
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1);
        Debug.Log("player moved");
        isMoving = false;
        hasThrown = false;
        if(dice1Value != dice2Value)
        {
            finishedTurn = true;
        }
        dice1Value = 0;
        dice2Value = 0;
    }

    public void GoToJail()
    {
        isInJail = true;
        position = jailTilePosition;
        transform.position = board.GetTileArray()[position].transform.position;
        doublesThisTurn = 0;
        finishedTurn = true;
    }

    public void SetTurn(bool isTurn)
    {
        isMyTurn=isTurn;
        finishedTurn = !isTurn;
    }

    public void SetPlayerNumber(int n)
    {
        playerNumber=n;
    }

    public void SetIsHuman(bool human)
    {
        isHuman=human;
    }

    public void SetDiceSpawnPosition(GameObject obj)
    {
        diceSpawnLocation = obj;
    }

    public void SetBoardObject(Board _board)
    {
        board = _board;
    }

    public bool IsInJail()
    {
        return isInJail;
    }

    public bool IsHuman()
    {
        return isHuman;
    }

    public int GetBalance()
    {
        return balance;
    }
    public int GetPlayerNumber()
    {
        return playerNumber;
    }
    public bool IsPlayersTurn()
    {
        return isMyTurn;
    }

    public bool IsPlayerMoving()
    {
        return isMoving;
    }

    public bool HasPlayerThrown()
    {
        return hasThrown;
    }

    public bool HasFinishedTurn()
    {
        return finishedTurn;
    }

}
