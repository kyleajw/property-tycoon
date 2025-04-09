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
            //MovePiece(dice1Value + dice2Value);
            rolled = false;
        }
    }

    /// <summary>
    /// Rolls both dice on the board.
    /// </summary>
    public void RollDice(float multiplier)
    {
        hasThrown = true;
        menuReady = false;
        StartCoroutine(WaitForDiceToFinishRolling(multiplier));
    }
    /// <summary>
    /// Moves player clockwise across the board by x amount of steps
    /// </summary>
    /// <param name="steps">Amount to move by</param>
    public void MovePiece(int steps)
    {
        Debug.Log($"moving by {steps} steps");
        isMoving = true;
        StartCoroutine(MovePlayerAnimation(steps));
        //numberRolled = steps;
    }
    /// <summary>
    /// Rolls the die, then moves the player with the total of the dice as the number of steps, if the player is not in jail.
    /// </summary>
    /// <param name="multiplier">Force multiplier</param>
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
        if (!isInJail)
        {
            MovePiece(numberRolled);
        }
    }
    /// <summary>
    /// Gradually moves the player across the board for x amount of steps, uses lerp to transform the player's position between each tile, lighting the tiles up as they cross them.
    /// </summary>
    /// <param name="steps">Number of steps to move</param>
    IEnumerator MovePlayerAnimation(int steps)
    {
        MeshRenderer meshRenderer = board.GetTileArray()[position].gameObject.GetComponentInChildren<MeshRenderer>();
        float duration = 0.3f;
        if (steps < 0)
        {
            for (int i = 0; i > steps; i--)
            {
                float time = 0;

                //meshRenderer.materials[1].DisableKeyword("_EMISSION");
                SetEmissionKeywordToAll(false, meshRenderer.materials);
                position--;
                if (position <= -1)
                {
                    position = board.GetTileArray().Length -1;
                }
                Vector3 startPosition = transform.position;
                while (time < duration)
                { 
                    float smoothedValue = time / duration;
                    smoothedValue = smoothedValue * smoothedValue * (3f - 2f * smoothedValue);
                    transform.position = Vector3.Lerp(startPosition, board.GetTileArray()[position].transform.position, smoothedValue);
                    time += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                transform.position = board.GetTileArray()[position].transform.position;
                meshRenderer = board.GetTileArray()[position].gameObject.GetComponentInChildren<MeshRenderer>();
                SetEmissionKeywordToAll(true, meshRenderer.materials);
                //meshRenderer.materials[1].EnableKeyword("_EMISSION");

                //todo:
                // if tile not normal generic tile, assign emission position differently

            }
        }
        else
        {
            for (int i = 0; i < steps; i++)
            {
                
                float time = 0;
                //meshRenderer.materials[1].DisableKeyword("_EMISSION");
                SetEmissionKeywordToAll(false, meshRenderer.materials);
                position++;
                if (position >= board.GetTileArray().Length)
                {
                    position = 0;
                    completedCycle = true;
                    balance += 200; //money for passing go
                }
                Vector3 startPosition = transform.position;
                while (time < duration)
                {
                    float smoothedValue = time / duration;
                    smoothedValue = smoothedValue * smoothedValue * (3f - 2f * smoothedValue);
                    transform.position = Vector3.Lerp(startPosition, board.GetTileArray()[position].transform.position, smoothedValue);
                    time += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                transform.position = board.GetTileArray()[position].transform.position;
                transform.position = board.GetTileArray()[position].transform.position;
                meshRenderer = board.GetTileArray()[position].gameObject.GetComponentInChildren<MeshRenderer>();
                //meshRenderer.materials[1].EnableKeyword("_EMISSION");
                SetEmissionKeywordToAll(true, meshRenderer.materials);

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
    /// <summary>
    /// Sends the player to jail, and sets the isInJail var to true. If the player is an AI agent, instantly ends the turn.
    /// </summary>
    public void GoToJail()
    {
        isInJail = true;
        position = jailTilePosition;
        transform.position = board.GetTileArray()[position].transform.position;
        doublesThisTurn = 0;

        finishedTurn = true;
        if (!isHuman)
        {
            gameObject.GetComponent<EasyAgent>().EndTurn();
        }
    }

    public void SetTurn(bool isTurn)
    {
        isMyTurn=isTurn;
        finishedTurn = !isTurn;
        if(isMyTurn && !isHuman)
        {
            gameObject.GetComponent<EasyAgent>().OnMyTurn();
        }
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

    /// <summary>
    /// Buys the property that the player is currently on, if possible.
    /// </summary>
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
    /// <summary>
    /// Pays the provided rent to the correct player. Takes money out of current players balance and puts the same amount into the player receiving the rent money.
    /// </summary>
    /// <param name="payee">Player paying</param>
    /// <param name="receiver">Player receiving payment</param>
    /// <param name="rent">Amount to pay</param>
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
