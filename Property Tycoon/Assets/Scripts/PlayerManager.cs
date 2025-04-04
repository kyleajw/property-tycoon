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
    [SerializeField] public GameObject mortgageButton;
    [SerializeField] public GameObject sellButton;
    [SerializeField] public GameObject buildHouseButton;
    [SerializeField] public GameObject removeHouseButton;
    [SerializeField] TextMeshProUGUI buildHouseText;
    [SerializeField] TextMeshProUGUI removeHouseText;
    [SerializeField] TextMeshProUGUI houseCounterText;
    [SerializeField] GameObject cam;
    [SerializeField] TMP_Text turnAnnouncer;
    [SerializeField] TMP_Text turnCounter;
    [SerializeField] Board board;
    [SerializeField] float diceRollForceMultiplier = 5.0f;

    [SerializeField] GameObject playerPrefab;
    Camera mainCamera;

    [SerializeField] GameObject auctionGUI;

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
                sellButton.SetActive(false);
                mortgageButton.SetActive(false);
                buildHouseButton.SetActive(false);
                removeHouseButton.SetActive(false);
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
                if (GetOwner(currentPlayer.GetCurrentTile()) != null)
                {
                    buyButton.SetActive(false);
                }
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
                else if (GetOwner(currentPlayer.GetCurrentTile()) != currentPlayer) // checks if other player owns the property the player landed on
                {
                    if (!currentPlayer.GetCurrentTile().GetComponent<Property>().isMortgaged) //checks if property is not mortgaged and owner can receive rent on it
                    {
                        if(currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.purchasable && currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group != "Utilities" && currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group != "Station")
                        {
                            //applies to normal properties
                            if (currentPlayer.GetBalance() < currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()])
                            {
                                //player needs to sell assets in order to pay for rent
                                //add text to show this
                                //rentPaid=true
                                //finishedTurnButton.SetActive(false)
                            }
                            else if (currentPlayer.GetBalance() > currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()])
                            {
                                currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()), currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()]);
                            }
                        }
                        //applies to stations
                        else if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group == "Station")
                        {

                        }
                        //applies to utilities
                        else if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group == "Utilities")
                        {

                        }
                    }
                }
            }
        }
    }

    void AnnounceTurn()
    {
        turnAnnouncer.SetText($"Player {currentPlayersTurn + 1}'s turn");
        turnAnnouncer.color = players[currentPlayersTurn].GetComponent<Player>().GetPlayerColour();
        turnCounter.SetText($"Total turns: {turnNumber}");
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
        DisplayAuctionGUI();
        BeginAuctionProcess();
    }

    void DisplayAuctionGUI()
    {

    }

    void BeginAuctionProcess()
    {

    }
    public void BuildHousePressed(Property property)
    {
        //add houses to selected tile
        for (int i = 0; i < players[currentPlayersTurn].GetComponent<Player>().ownedProperties.Length; i++)
        {
            if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i] != null)
            {
                if (property.tile.tileData.spaceName == players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.spaceName)
                {
                    //checks if  house count is less than 4 and that it isnt a property that houses cannot be built on
                    if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount()<5 && isColourGroupOwned(players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group))
                    {
                        if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group != "Utilities" && players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group != "Station")
                        {
                            players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().IncrementHouseCount();
                            if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount() == 5)
                            {
                                buildHouseButton.SetActive(false);
                                //add hotel prefab here and remove all house prefabs
                            }
                            else
                            {
                                //add extra house prefab here
                            }
                            switch (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group)
                            {
                                case ("Brown"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(-50);
                                    board.GetBank().SetBankBalance(50);
                                    break;
                                case ("Blue"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(-50);
                                    board.GetBank().SetBankBalance(50);
                                    break;
                                case ("Purple"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(-100);
                                    board.GetBank().SetBankBalance(100);
                                    break;
                                case ("Orange"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(-100);
                                    board.GetBank().SetBankBalance(100);
                                    break;
                                case ("Red"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(-150);
                                    board.GetBank().SetBankBalance(150);
                                    break;
                                case ("Yellow"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(-150);
                                    board.GetBank().SetBankBalance(150);
                                    break;
                                case ("Green"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(-200);
                                    board.GetBank().SetBankBalance(200);
                                    break;
                                case ("Deep Blue"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(-200);
                                    board.GetBank().SetBankBalance(200);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
    public void SellHousePressed(Property property)
    {
        //remove houses from selected tile
        for (int i = 0; i < players[currentPlayersTurn].GetComponent<Player>().ownedProperties.Length; i++)
        {
            if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i] != null)
            {
                if (property.tile.tileData.spaceName == players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.spaceName)
                {
                    //checks if  house count zero so no more can be removed and it is a property that can have houses on it
                    if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount() > 0)
                    {
                        if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group != "Utilities" && players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group != "Station")
                        {
                            players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().DecrementHouseCount();
                            if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount() == 0)
                            {
                                removeHouseButton.SetActive(false);
                                //remove last house prefab here
                            }
                            else if(players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount() == 4)
                            {
                                //remove hotel and place 4 house prefabs
                            }
                            else
                            {
                                //remove a house prefab here
                            }
                            switch (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group)
                            {
                                case ("Brown"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(50);
                                    board.GetBank().SetBankBalance(-50);
                                    break;
                                case ("Blue"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(50);
                                    board.GetBank().SetBankBalance(-50);
                                    break;
                                case ("Purple"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(100);
                                    board.GetBank().SetBankBalance(-100);
                                    break;
                                case ("Orange"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(100);
                                    board.GetBank().SetBankBalance(-100);
                                    break;
                                case ("Red"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(150);
                                    board.GetBank().SetBankBalance(-150);
                                    break;
                                case ("Yellow"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(150);
                                    board.GetBank().SetBankBalance(-150);
                                    break;
                                case ("Green"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(200);
                                    board.GetBank().SetBankBalance(-200);
                                    break;
                                case ("Deep Blue"):
                                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(200);
                                    board.GetBank().SetBankBalance(-200);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
    public void MortgagePressed(Property property)
    {
        //mortgage selected tile for half purchase value
        for(int i = 0; i < players[currentPlayersTurn].GetComponent<Player>().ownedProperties.Length; i++)
        {
            if(players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i] != null)
            {
                if (property.tile.tileData.spaceName == players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.spaceName)
                {
                    if (!players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().isMortgaged)
                    {
                        players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().isMortgaged = true;
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.purchaseCost / 2);
                        board.GetBank().SetBankBalance(-players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.purchaseCost / 2);
                        mortgageButton.SetActive(false);
                        sellButton.SetActive(false);
                        buildHouseButton.SetActive(false);
                        removeHouseButton.SetActive(false);
                        board.RefreshMenu(players[currentPlayersTurn]);
                    }
                }
            }
        }
    }
    public void SellPressed(Property property)
    {
        //sell back to bank for full purchase price
        //add back to bank property array and remove from player property array
        for (int i = 0; i < players[currentPlayersTurn].GetComponent<Player>().ownedProperties.Length; i++)
        {
            if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i] != null)
            {
                if (property.tile.tileData.spaceName == players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.spaceName)
                {
                    if (!players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().isMortgaged)
                    {
                        //give property back to bank and pay the player
                        players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().SetOwnedBy(null);
                        board.GetBank().properties[i] = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i];
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.purchaseCost);
                        board.GetBank().SetBankBalance(-players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.purchaseCost);
                        players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i] = null;
                        sellButton.SetActive(false);
                        mortgageButton.SetActive(false);
                        buildHouseButton.SetActive(false);
                        removeHouseButton.SetActive(false);
                        board.RefreshMenu(players[currentPlayersTurn]);
                    }
                }
            }
        }
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
    public bool isColourGroupOwned(string group)
    {
        //make it so group is found and checks if all properties of that colour are owned by the same person
        bool groupOwned = false;
        int totalCards=0;
        int ownedCards=0;
        for (int i=0; i< board.GetTileArray().Length;i++)
        {
            if(board.GetTileArray()[i].GetComponent<Tile>().tileData.group == group)
            {
                totalCards++;
            }
        }
        for(int i = 0; i<players[currentPlayersTurn].GetComponent<Player>().ownedProperties.Length; i++)
        {
            if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i] != null)
            {
                if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetOwnedBy() == players[currentPlayersTurn])
                {
                    ownedCards++;
                }
            }
        }
        if (totalCards == ownedCards)
        {
            groupOwned = true;
        }
        return groupOwned;
    }
    public void UpdateHouseText(Property property)
    {
        houseCounterText.text = property.GetHouseCount().ToString();
        switch (property.GetAssociatedTile().tileData.group)
        {
            case ("Brown"):
                buildHouseText.text = ("-£50");
                removeHouseText.text = ("+£50");
                break;
            case ("Blue"):
                buildHouseText.text = ("-£50");
                removeHouseText.text = ("+£50");
                break;
            case ("Purple"):
                buildHouseText.text = ("-£100");
                removeHouseText.text = ("+£100");
                break;
            case ("Orange"):
                buildHouseText.text = ("-£100");
                removeHouseText.text = ("+£100");
                break;
            case ("Red"):
                buildHouseText.text = ("-£150");
                removeHouseText.text = ("+£150");
                break;
            case ("Yellow"):
                buildHouseText.text = ("-£150");
                removeHouseText.text = ("+£150");
                break;
            case ("Green"):
                buildHouseText.text = ("-£200");
                removeHouseText.text = ("+£200");
                break;
            case ("Deep Blue"):
                buildHouseText.text = ("-£200");
                removeHouseText.text = ("+£200");
                break;
        }
    }
}
