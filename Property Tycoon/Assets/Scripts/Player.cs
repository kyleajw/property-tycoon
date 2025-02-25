using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] bool debuggingAsSinglePlayer = false;
    [SerializeField] GameObject[] pieces;
    [SerializeField] GameObject dicePrefab;
    GameObject piece;
    [SerializeField] int balance = 1500;

    [SerializeField] float diceSpawnOffset = 0.1f;
    [SerializeField] GameObject diceRollPosition;

    [SerializeField] Board board;

    int playerNumber;
    bool isMyTurn;
    bool isMoving = false;
    bool hasThrown = false;
    bool rolled = false;
    int dice1Value;
    int dice2Value;

    int position = 0;

    private void Start()
    {
        if (debuggingAsSinglePlayer)
        {
            isMyTurn = true;
            playerNumber = 0;
        }
    }
    public void AssignPiece(int i)
    {
        piece=pieces[i];
        Debug.Log(piece);
    }

    private void Update()
    {
        if (rolled)
        {
            MovePiece(dice1Value + dice2Value);
            rolled = false;
            dice1Value = 0;
            dice2Value = 0;
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

//-----------------to implement in later sprint cycle------------------
    public void SetTurn(bool isTurn)
    {
        isMyTurn=isTurn;
    }

    public void SetPlayerNumber(int n)
    {
        playerNumber=n;
    }

    void OnNewTurn()
    {
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

    IEnumerator WaitForDiceToFinishRolling(float multiplier)
    {
        GameObject dice1 = Instantiate(dicePrefab, diceRollPosition.transform.position, Quaternion.identity);
        GameObject dice2 = Instantiate(dicePrefab,  new Vector3(diceRollPosition.transform.position.x + diceSpawnOffset, diceRollPosition.transform.position.y, diceRollPosition.transform.position.z), Quaternion.identity);

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
    }
}
