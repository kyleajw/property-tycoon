using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Codice.CM.Common.CmCallContext;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject startTile;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject rollButtonGroup;
    [SerializeField] GameObject finishTurnButton;
    [SerializeField] GameObject buyButton;
    [SerializeField] GameObject auctionButton;
    [SerializeField] GameObject cam;
    [SerializeField] TMP_Text turnAnnouncer;
    [SerializeField] Board board;
    [SerializeField] float diceRollForceMultiplier = 5.0f;

    [SerializeField] GameObject playerPrefab;
    Camera mainCamera;

    bool gameStarted = false;
    bool buyButtonPressed = false;
    int turnNumber = 1;
    int houseInp;
    int cyclesCompleted = 0;

    Player[] playerData;
    GameObject[] players;
    int currentPlayersTurn;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        InitialisePlayers();
        AnnounceTurn();
    }

    // Update is called once per frame
    void Update()
    {

        if (gameStarted)
        {
            mainCamera.gameObject.GetComponent<CameraHandler>().SetTarget(players[currentPlayersTurn]);

            if (players[currentPlayersTurn].GetComponent<Player>().HasFinishedTurn())
            {
                players[currentPlayersTurn].GetComponent<Player>().SetTurn(false);
                if(currentPlayersTurn+1 >= players.Length)
                {
                    currentPlayersTurn = 0;
                }
                else
                {
                    currentPlayersTurn++;
                }
                players[currentPlayersTurn].GetComponent<Player>().SetTurn(true);
                turnNumber++;
                AnnounceTurn();
            }
            else
            {
                
                Player currentPlayer = players[currentPlayersTurn].GetComponent<Player>();
                if (currentPlayer.IsHuman())
                {
                    HandleCanvasVisibility(currentPlayer);
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                board.TogglePropertyMenu(players[currentPlayersTurn]);
            }
        }
    }

    void InitialisePlayers()
    {
        //players = new GameObject[playerCount];
        playerData = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().GetPlayerData();
        players = new GameObject[playerData.Length];
        for (int i = 0; i < players.Length; i++)
        {
            if (playerData[i].IsHuman())
            {
                players[i] = InstantiateHumanPlayer();
                //players[i] = InstantiateHumanPlayer(players[i]);
                players[i].GetComponent<Player>().SetIsHuman(true);
            }
            players[i].GetComponent<Player>().AssignPiece(playerData[i].GetPlayerPiece());
            players[i].GetComponent<Player>().SetPlayerNumber(playerData[i].GetPlayerNumber());
            players[i].GetComponent<Player>().SetPlayerName(playerData[i].GetPlayerName());
            players[i].GetComponent<Player>().SetPlayerColour(playerData[i].GetPlayerColour());

            players[i].GetComponent<Player>().SetBoardObject(board);
            players[i].GetComponent<Player>().SetDiceSpawnPosition(cam);
        }
        players[0].GetComponent<Player>().SetTurn(true);
        gameStarted = true;
        gameCanvas.SetActive(true);
    }

    GameObject InstantiateHumanPlayer()
    {
        return Instantiate(playerPrefab, startTile.transform.position, Quaternion.identity);
    }

    void HandleCanvasVisibility(Player currentPlayer)
    {
        if (currentPlayer.IsPlayersTurn() && !rollButtonGroup.activeInHierarchy && !currentPlayer.IsPlayerMoving() && !currentPlayer.HasPlayerThrown() && !currentPlayer.IsInJail())
        {
            rollButtonGroup.SetActive(true);
            finishTurnButton.SetActive(false);
            buyButton.SetActive(false);
            auctionButton.SetActive(false);
        }
        else if (!currentPlayer.IsPlayersTurn() && rollButtonGroup.activeInHierarchy)
        {
            rollButtonGroup.SetActive(false);
            finishTurnButton.SetActive(false);
            buyButton.SetActive(false);
            auctionButton.SetActive(false);
        }
        else if (currentPlayer.IsPlayersTurn() && currentPlayer.IsMenuReady() && currentPlayer.HasPlayerThrown() && currentPlayer.completedCycle)
        {
            rollButtonGroup.SetActive(false);
            finishTurnButton.SetActive(true);
            if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.purchasable)
            {
                if (GetOwner(currentPlayer.GetCurrentTile()) == null) //checks if property has not been purchased yet
                {
                    buyButton.SetActive(true);
                    if (buyButtonPressed)
                    {
                        buyButton.SetActive(false);
                    }
                    if (CheckCycles() >= 2)
                    {
                        auctionButton.SetActive(true);
                    }
                    else
                    {
                        auctionButton.SetActive(false);
                    }
                }
                else if (GetOwner(currentPlayer.GetCurrentTile())==currentPlayer) // checks if the propery is owned by the player who landed on it
                {
                    //display build property building buttons here
                }
                else if (GetOwner(currentPlayer.GetCurrentTile()) != currentPlayer) // checks if other player owns the property the player landed on
                {
                    if(currentPlayer.GetBalance()<currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouses()])
                    {
                        //player needs to sell assets in order to pay for rent
                    }
                    else
                    {
                        currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()),currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouses()]);
                    }
                }
            }
        }
    }

    void AnnounceTurn()
    {
        turnAnnouncer.SetText($"Player {currentPlayersTurn + 1}'s turn | Turn Number: {turnNumber}");
    }

    public void OnRollButtonReleased(float timeHeld)
    {
        rollButtonGroup.SetActive(false);
        players[currentPlayersTurn].GetComponent<Player>().RollDice(timeHeld * diceRollForceMultiplier);
    }
    public int CheckCycles()
    {
        cyclesCompleted = 0;
        for(int i=0; i<players.Length; i++)
        {
            if (players[i].GetComponent<Player>().completedCycle)
            {
                cyclesCompleted++;
            }
        }
        return cyclesCompleted;
    }
    public void OnFinishedTurn()
    {
        players[currentPlayersTurn].GetComponent<Player>().SetTurn(false);
        players[currentPlayersTurn].GetComponent<Player>().SetHasThrown(false);
        buyButtonPressed=false;
    }
    public void BuyPressed()
    {
        players[currentPlayersTurn].GetComponent<Player>().BuyProperty();
        Debug.Log(players[currentPlayersTurn].GetComponent<Player>().ownedProperties);
        buyButtonPressed = true;
    }
    public void AuctionPressed()
    {

    }
    public void TradePressed()
    {

    }
    public void BuildHousePressed()
    {
        //add houses to selected tile
        //incremetn property house counter
    }
    public void MortgagePressed()
    {
        //mortgage selected tile back to the back for half purchase value
        //property.isMortgaged=true
        //add check for if property is mortgaged to disable rent payment
    }
    public void SellPressed()
    {
        //sell back to bank for full purchase price
        //add back to bank property array and remove from player property array
        //chnage 11 in here to the selected tiles position in players property arrray
        //board.GetBank().properties[11] = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[11];
        //players[currentPlayersTurn].GetComponent<Player>().ownedProperties[11] = null;
        //players[currentPlayersTurn].GetComponent<Player>().SetBalance(players[currentPlayersTurn].GetComponent<Player>().GetSelectedTile().GetComponent<Tile>().tileData.purchaseCost);
        //board.GetBank().SetBankBalance(-players[currentPlayersTurn].GetComponent<Player>().GetSelectedTile().GetComponent<Tile>().tileData.purchaseCost);
    }
    public GameObject GetOwner(GameObject curTile)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (curTile.GetComponent<Property>().GetOwnedBy() == players[i])
            {
                return players[i];
            }
        }
        return null;
    }
}
