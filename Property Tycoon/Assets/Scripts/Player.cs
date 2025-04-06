using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject dicePrefab;
    [SerializeField] GameObject diceSpawnLocation;
    [SerializeField] Board board;
    [SerializeField] float diceSpawnOffset = 0.1f;
    [SerializeField] public int balance = 1500;
    [SerializeField] int jailTilePosition;
    [SerializeField] PlayerManager playerManager;

    public GameObject[] ownedProperties=new GameObject[28];
    Color playerColour;
    GameObject piece;

    public bool completedCycle = true; //change to false after development
    public bool isRetired = false;
    public int numberRolled = 0;
    bool isMyTurn;
    bool isHuman;
    bool menuReady = false;
    bool isMoving = false;
    bool hasThrown = false;
    bool rolled = false;
    bool isInJail = false;
    bool finishedTurn = false;
    bool doubleRolled = false;
    bool hasGetOutOfJailFreeCard = false;

    int inventoryValue = 1500;
    int doublesThisTurn = 0;
    int playerNumber;
    int dice1Value;
    int dice2Value;
    int position = 0;

    string playerName;
    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
    }

    public void AssignPiece(GameObject _piece)
    {
        piece = _piece;
    }

    private void Update()
    {
        if (rolled)
        {
            if (dice1Value==dice2Value)
            {
                SetDoubleRolled(true);
                doublesThisTurn++;

                if (doublesThisTurn == 3)
                {
                    GoToJail();
                }
            }
            else
            {
                doublesThisTurn = 0;
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
        menuReady = false;
        StartCoroutine(WaitForDiceToFinishRolling(multiplier));
    }

    public void MovePiece(int steps)
    {
        Debug.Log($"moving by {steps} steps");
        isMoving = true;
        StartCoroutine(MovePlayerAnimation(steps));
        //numberRolled = steps;
    }

    IEnumerator WaitForDiceToFinishRolling(float multiplier)
    {
        GameObject dice1 = Instantiate(dicePrefab, diceSpawnLocation.transform.position, diceSpawnLocation.transform.rotation);
        GameObject dice2 = Instantiate(dicePrefab, new Vector3(diceSpawnLocation.transform.position.x + diceSpawnOffset, diceSpawnLocation.transform.position.y, diceSpawnLocation.transform.position.z), diceSpawnLocation.transform.rotation);

        dice1.name = "Dice1";
        dice2.name = "Dice2";


        dice1.GetComponent<Dice>().Roll(multiplier);
        dice2.GetComponent<Dice>().Roll(multiplier);
        yield return new WaitUntil(() => !(dice1.GetComponent<Dice>().isRolling() && dice2.GetComponent<Dice>().isRolling()));
        dice1Value = dice1.GetComponent<Dice>().GetSideFacingUp();
        dice2Value = dice2.GetComponent<Dice>().GetSideFacingUp();
        rolled = true;

        yield return new WaitForSeconds(1.5f);
        numberRolled = dice1Value + dice2Value;
        Destroy(dice1);
        Destroy(dice2);
    }

    IEnumerator MovePlayerAnimation(int steps)
    {
        MeshRenderer meshRenderer = board.GetTileArray()[position].gameObject.GetComponentInChildren<MeshRenderer>();
        if(steps < 0)
        {
            for (int i = 0; i > steps; i--)
            {

                //meshRenderer.materials[1].DisableKeyword("_EMISSION");
                SetEmissionKeywordToAll(false, meshRenderer.materials);
                position--;
                if (position <= -1)
                {
                    position = board.GetTileArray().Length -1;
                }
                transform.position = board.GetTileArray()[position].transform.position;
                meshRenderer = board.GetTileArray()[position].gameObject.GetComponentInChildren<MeshRenderer>();
                //meshRenderer.materials[1].EnableKeyword("_EMISSION");
                SetEmissionKeywordToAll(true, meshRenderer.materials);
                yield return new WaitForSeconds(0.5f);

                //todo:
                // if tile not normal generic tile, assign emission position differently

            }
        }
        else
        {
            for (int i = 0; i < steps; i++)
            {

                //meshRenderer.materials[1].DisableKeyword("_EMISSION");
                SetEmissionKeywordToAll(false, meshRenderer.materials);
                position++;
                if (position >= board.GetTileArray().Length)
                {
                    position = 0;
                    completedCycle = true;
                    balance += 200; //money for passing go
                }
                transform.position = board.GetTileArray()[position].transform.position;
                meshRenderer = board.GetTileArray()[position].gameObject.GetComponentInChildren<MeshRenderer>();
                //meshRenderer.materials[1].EnableKeyword("_EMISSION");
                SetEmissionKeywordToAll(true, meshRenderer.materials);
                yield return new WaitForSeconds(0.5f);

                //todo:
                // if tile not normal generic tile, assign emission position differently

            }

        }
        yield return new WaitForSeconds(1);
        Debug.Log("player moved");
        isMoving = false;
        menuReady = true;
        dice1Value = 0;
        dice2Value = 0;

        // position on final tile
        // tell player manager (passing card through)
        playerManager.OnPlayerFinishedMoving(board.GetTileArray()[position]);
    }
    void SetEmissionKeywordToAll(bool set, Material[] materials)
    {

        foreach (Material mat in materials)
        {
            if (set)
            {
                mat.EnableKeyword("_EMISSION");
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
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
    // increments based on param given
    public void SetBalance(int money)
    {
        balance = balance + money;
    }
    public void SetInvValue(int money)
    {
        inventoryValue += money;
    }
    public void SetHasThrown(bool thrown)
    {
        hasThrown = thrown;
    }

    public void SetPlayerNumber(int n)
    {
        playerNumber=n;
    }

    public void SetPlayerName(string pName)
    {
        playerName = pName;
    }

    public void SetPlayerColour(Color colour)
    {
        playerColour = colour;
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
    public int GetInvValue()
    {
        return inventoryValue;
    }
    public int GetPlayerNumber()
    {
        return playerNumber;
    }

    public Color GetPlayerColour()
    {
        return playerColour;
    }

    public GameObject GetPlayerPiece()
    {
        return piece;
    }

    public string GetPlayerName()
    {
        return playerName;
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
    public bool IsMenuReady()
    {
        return menuReady;
    }
    public GameObject GetCurrentTile()
    {
        return board.GetTileArray()[position];
    }
    public string GetCurrentTileName()
    {
        string tileName = GetCurrentTile().GetComponent<Tile>().tileData.spaceName;
        return tileName;
    }

    public int GetPositionOnBoard()
    {
        return position;
    }

    //change to list / hash ??; functionality with these arrays are difficult outside of this one method..
    public void BuyProperty()
    {
        int index=0;
        for(int i=0;i<board.GetBank().properties.Length; i++)
        {
            string currentTileName=GetCurrentTileName();
            if (board.GetBank().properties[i] != null)
            {
                if (currentTileName == board.GetBank().properties[i].GetComponent<Tile>().tileData.spaceName)
                {
                    index = i;
                }
            }
        }
        ownedProperties[index] = board.GetBank().properties[index];
        board.GetBank().properties[index].GetComponent<Property>().SetOwnedBy(gameObject);
        board.GetBank().properties[index] = null;
        SetBalance(-GetCurrentTile().GetComponent<Tile>().tileData.purchaseCost);
    }
    public void AddProperty(GameObject[] from,int i)
    {
        ownedProperties[i] = from[i];
        from[i]=null;
    }
    public void PayRent(GameObject payee, GameObject receiver, int rent)
    {
        payee.GetComponent<Player>().SetBalance(-rent);
        receiver.GetComponent<Player>().SetBalance(rent);
    }
    public void SetDoubleRolled(bool option)
    {
        doubleRolled = option;    
    }
    public bool GetDoubleRolled()
    {
        return doubleRolled;
    }
    void UseGetOutOfJailFreeCard()
    {
        hasGetOutOfJailFreeCard = false;
    }

    public void ReceiveGetOutOfJailFreeCard()
    {
        hasGetOutOfJailFreeCard = true;
    }
}
