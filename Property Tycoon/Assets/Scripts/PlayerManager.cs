using System;
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
    [SerializeField] public GameObject houseCounter;
    [SerializeField] TextMeshProUGUI buildHouseText;
    [SerializeField] TextMeshProUGUI removeHouseText;
    [SerializeField] TextMeshProUGUI houseCounterText;
    [SerializeField] GameObject cam;
    [SerializeField] TMP_Text turnAnnouncer;
    [SerializeField] TMP_Text turnCounter;
    [SerializeField] Board board;
    [SerializeField] float diceRollForceMultiplier = 5.0f;
    [SerializeField] GameObject cardDialogPrefab;

    [SerializeField] GameObject playerPrefab;
    Camera mainCamera;

    [SerializeField] GameObject auctionGUI;
    int parkingTotal = 0;
    bool gameStarted = false;
    bool buyButtonPressed = false;
    bool auctionButtonPressed = false;
    bool rentPayable = false; 
    int turnNumber = 1;
    int houseInp;
    int cyclesCompleted = 0;

    Player[] playerData;
    GameObject[] players;
    int currentPlayersTurn;

    CardManager cardManager;

   
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
                rentPayable = false;
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
                houseCounter.SetActive(false);
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
            players[i] = InstantiatePlayer();
            if (playerData[i].IsHuman())
            {
                //players[i] = InstantiateHumanPlayer(players[i]);
                players[i].GetComponent<Player>().SetIsHuman(true);
            }
            else
            {
                players[i].GetComponent<Player>().SetIsHuman(false);
                players[i].AddComponent<EasyAgent>(); // default AI behaviour
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

    GameObject InstantiatePlayer()
    {
        return Instantiate(playerPrefab, startTile.transform.position, Quaternion.identity);
    }

    //todo continue during refactor
    public void OnPlayerFinishedMoving(GameObject landedTile)
    {
        //!!!! NO CHECKS FOR IF PLAYER CAN AFFORD CHARGES
        TileData tile = landedTile.GetComponent<Tile>().tileData;
        switch (tile.group)
        {
            case "Pot Luck":
                cardManager = gameObject.GetComponent<CardManager>();
                Debug.Log("Player Draws Pot Luck Card");
                ShowCardDialog("Pot Luck",cardManager.DrawPotLuckCard());
                break;
            case "Opportunity Knocks":
                cardManager = gameObject.GetComponent<CardManager>();
                Debug.Log("Player Draws Opportunity Knocks Card");
                ShowCardDialog("Opportunity Knocks",cardManager.DrawOpportunityKnocksCard());
                break;
            case "Tax": // NOT CUSTOMISABLE - HARDCODED
                Debug.Log("tax player");
                switch (tile.spaceName)
                {
                    case "Income Tax":
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(-200);
                        break;
                    case "Super Tax":
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(-100);
                        break;
                }
                break;
            case "Unique":
                Debug.Log("square tile");
                switch (tile.spaceName)
                {
                    case "Go":
                        Debug.Log("GO Tile, implemented elsewhere");
                        break;
                    case "Jail/Just visiting":
                        Debug.Log("JAIL TILE");
                        break;
                    case "Free Parking":
                        Debug.Log("player landed on free parking");
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(parkingTotal);
                        parkingTotal = 0;
                        break;
                    case "Go to Jail":
                        Debug.Log("player goes to jail");
                        players[currentPlayersTurn].GetComponent<Player>().GoToJail();
                        break;
                }
                break;
            default:
                Debug.Log("Implemented elsewhere");
                break;
        }
    }

    void ShowCardDialog(string cardType, CardData card)
    {
        GameObject newCardDialog = Instantiate(cardDialogPrefab, gameCanvas.transform);
        CardDialog cardDialog = newCardDialog.GetComponent<CardDialog>();
        cardDialog.UpdateCardInfo(cardType, card);
    }

    public void OnPlayerClosesCardDialog(CardData card, int choice)
    {
        string[] args = card.arg.Split(' ');
        string action = args[0];
        int amount;
        switch (action)
        {
            case "RECEIVE":
                Debug.Log("Player Receives");
                string payer = args[args.Length - 1];
                amount = Convert.ToInt32(args[1]);
                if (payer == "BANK")
                {
                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(amount);
                }
                else if (payer == "ALL")
                {
                    int total = 0;
                    foreach(GameObject player in players)
                    {
                        if(!(player.GetComponent<Player>().GetPlayerNumber() == players[currentPlayersTurn].GetComponent<Player>().GetPlayerNumber()))
                        {
                            total += amount;
                            player.GetComponent<Player>().SetBalance(-amount);
                        }
                    }
                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(total);
                }
                break;
            case "PAY":
                Debug.Log("Player Pays");
                string payee = args[1];
                amount = Convert.ToInt32(args[2]);
                switch (payee)
                {
                    case "BANK":
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(-amount);
                        break;
                    case "PARKING":
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(-amount);
                        parkingTotal += amount; // todo
                        break;
                }

                break;
            case "MOVE":
                Debug.Log("Player Moves");
                string direction = args[1];
                string location = args[2];

                if(!int.TryParse(location, out _))
                {
                    location = ExtractLocation(card.arg);
                }
                int steps =0;
                
                if (direction == "FORWARDS")
                {
                    if (!int.TryParse(location, out _))
                    {
                        steps = FindForwardDistanceFromPlayer(location);
                    }
                    else
                    {
                        steps = Convert.ToInt32(location);
                    }
                    players[currentPlayersTurn].GetComponent<Player>().MovePiece(steps);

                }else if (direction == "BACKWARDS")
                {
                    if(!int.TryParse(location, out _))
                    {
                        steps = FindBackwardsDistanceFromPlayer(location);
                    }
                    else
                    {
                        steps = Convert.ToInt32(location)*-1;
                    }
                    players[currentPlayersTurn].GetComponent<Player>().MovePiece(steps);
                }
                Debug.Log($"Steps: {steps} | Direction: {direction} | Location{location}");
                break;
            case "JAIL":
                Debug.Log("Player goes to jail");
                players[currentPlayersTurn].GetComponent<Player>().GoToJail();
                break;
            case "JAILFREE":
                Debug.Log("Player receives GOOJF Card");
                players[currentPlayersTurn].GetComponent<Player>().ReceiveGetOutOfJailFreeCard();
                break;
            case "CHOICE:":
                Debug.Log("choice dialog");
                HandleMultiChoiceCard(card.arg.Substring(action.Length+1),choice); // should work????
                break;
            case "VARIABLE:":
                Debug.Log("pay per this and this");
                HandleVariableCard(card.arg.Substring(action.Length)); // unknown if works
                break;
            default:
                Debug.Log("Unrecognized action:" + action);
                break;
        }
    }

    string ExtractLocation(string locationString)
    {
        return locationString.Substring(locationString.IndexOf("'")+1, locationString.LastIndexOf("'") - locationString.IndexOf("'") - 1);
    }

    int FindForwardDistanceFromPlayer(string location)
    {
        int playerPosition = players[currentPlayersTurn].GetComponent<Player>().GetPositionOnBoard();
        int tilePosition = FindTileIndex(location);
        int distance = tilePosition - playerPosition;
        Debug.Log(playerPosition);
        Debug.Log(tilePosition);
        Debug.Log(distance);
        if (distance < 0)
        {
            distance = board.GetTileArray().Length - Math.Abs(distance);
        }

        return distance;
    }

    int FindBackwardsDistanceFromPlayer(string location)
    {
        int playerPosition = players[currentPlayersTurn].GetComponent<Player>().GetPositionOnBoard();
        int tilePosition = FindTileIndex(location);
        int distance = tilePosition - playerPosition;


        if(distance > 0)
        {
            distance = board.GetTileArray().Length + Math.Abs(distance);
        }
        Debug.Log($"Player position: {playerPosition} | Tile position: {tilePosition} | Distance: {distance}");
        return distance;
    }

    int FindTileIndex(string location)
    {
        int tileIndex = -1;
        Debug.Log(location);
        int i = 0;
        GameObject[] tiles = board.GetTileArray();
        while(tileIndex == -1 && i < tiles.Length)
        {
            if (tiles[i].GetComponent<Tile>().tileData.spaceName == location)
            {
                tileIndex = tiles[i].GetComponent<Tile>().position;
            }
            i++;
        }

        return tileIndex;
    }

    void HandleMultiChoiceCard(string decisions, int choice)
    {
        string decision = decisions.Split(" OR ")[choice - 1];
        string action = decision.Split(" ")[0];
        Debug.Log(decision);
        Debug.Log(action);
        switch (action)
        {
            case "PAY":
                Debug.Log("pay");
                string payee = decision.Split(" ")[1];
                int amount = Convert.ToInt32(decision.Split(" ")[2]);
                switch (payee)
                {
                    case "BANK":
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(-amount);
                        break;
                    case "PARKING":
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(-amount);
                        parkingTotal += amount; // todo
                        break;
                }
                break;
            case "OPPORTUNITY":
                Debug.Log("take opportunity card");
                ShowCardDialog("Opportunity Knocks", cardManager.DrawOpportunityKnocksCard());
                break;
            default:
                Debug.Log("decision not implemented");
                break;
        }

    }

    void HandleVariableCard(string actions)
    {
        string action = actions.Split(" ")[0];
        string person = actions.Split(" ")[1];
        string[] factors = actions.Substring(action.Length + person.Length+ 1).Split(" AND ");
        string[] factorA = factors[0].Split(" ");
        string[] factorB = factors[1].Split(" ");

        HandleFactor(action, person, factorA);
        HandleFactor(action, person, factorB);

    }

    void HandleFactor(string action, string person, string[] arg)
        //action person x per y AND w per z
    {
        switch (action)
        {
            case "PAY":
                switch (person)
                {
                    case "BANK":
                        int amount = Convert.ToInt32(arg[0]);
                        string factor = arg[2];
                        int cost = 0;
                        if(factor == "HOUSE")
                        {
                            cost = amount * GetTotalHousesOwnedByPlayer();
                        }
                        else if(factor == "HOTEL")
                        {
                            cost = amount * GetTotalHotelsOwnedByPlayer();
                        }
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(-cost);
                        break;
                }
                break;
        }
    }

    int GetTotalHotelsOwnedByPlayer()
    {
        int total = 0;
        GameObject[] ownedProperties = players[currentPlayersTurn].GetComponent<Player>().ownedProperties;
        for (int i = 0; i < ownedProperties.Length; i++)
        {
            if (ownedProperties[i] != null)
            {
                Property property = ownedProperties[i].GetComponent<Property>();
                int houseCount = property.GetHouseCount();
                if (houseCount == 5)
                {
                    total++;
                }
            }
        }
        return total;
    }

    int GetTotalHousesOwnedByPlayer()
    {
        int total = 0;
        GameObject[] ownedProperties = players[currentPlayersTurn].GetComponent<Player>().ownedProperties;
        for(int i = 0; i < ownedProperties.Length; i++)
        {
            if (ownedProperties[i] != null)
            {
                Property property = ownedProperties[i].GetComponent<Property>();
                int houseCount = property.GetHouseCount();
                if(houseCount < 5)
                {
                    total += houseCount;
                }
            }
        }
        return total;
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
                    auctionButton.SetActive(false);
                    HandleRent(currentPlayer);
                }
                if (GetOwner(currentPlayer.GetCurrentTile()) == null) //checks if property has not been purchased yet
                {
                    buyButton.SetActive(true);
                    if (buyButtonPressed)
                    {
                        buyButton.SetActive(false);
                        auctionButton.SetActive(false);
                    }
                    if (CheckCycles() >= 2)
                    {
                        auctionButton.SetActive(true);
                    }
                    else
                    {
                        auctionButton.SetActive(false);
                    }
                    if (auctionButtonPressed)
                    {
                        buyButton.SetActive(false);
                        auctionButton.SetActive(false);
                    }
                }
            }
        }
        else if (currentPlayer.IsPlayersTurn() && currentPlayer.IsMenuReady() && currentPlayer.HasPlayerThrown())
        {
            if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.purchasable && GetOwner(currentPlayer.GetCurrentTile()) != null)
            {
                buyButton.SetActive(false);
                auctionButton.SetActive(false);
                HandleRent(currentPlayer);

            }
            finishTurnButton.SetActive(true);
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
        auctionButtonPressed=false;
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
        Auction auction = gameObject.GetComponent<Auction>();
        auction.Setup(players[currentPlayersTurn], players, players[currentPlayersTurn].GetComponent<Player>().GetCurrentTile());
        auctionButton.SetActive(false);
        buyButton.SetActive(false);
        auctionButtonPressed = true;
    }

    void DisplayAuctionGUI()
    {
        auctionGUI.SetActive(true);
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
                    if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount()<5 && isColourGroupOwned(players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group, players[currentPlayersTurn]))
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
                        houseCounter.SetActive(false);
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
                        houseCounter.SetActive(false);
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
    public bool isColourGroupOwned(string group, GameObject owner)
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
        for(int i = 0; i<owner.GetComponent<Player>().ownedProperties.Length; i++)
        {
            if (owner.GetComponent<Player>().ownedProperties[i] != null)
            {
                if (owner.GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group == group)
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
    public int GetStationsOwned(GameObject owner)
    {
        int ownedCards = 0;
        for (int i = 0; i < owner.GetComponent<Player>().ownedProperties.Length; i++)
        {
            if (owner.GetComponent<Player>().ownedProperties[i] != null)
            {
                if (owner.GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group == "Station")
                {
                    ownedCards++;
                }
            }
        }
        return ownedCards;
    }
    public void UpdateHouseText(Property property)
    {
        houseCounterText.text = property.GetHouseCount().ToString();
        switch (property.GetAssociatedTile().tileData.group)
        {
            case ("Brown"):
                buildHouseText.text = ("-�50");
                removeHouseText.text = ("+�50");
                break;
            case ("Blue"):
                buildHouseText.text = ("-�50");
                removeHouseText.text = ("+�50");
                break;
            case ("Purple"):
                buildHouseText.text = ("-�100");
                removeHouseText.text = ("+�100");
                break;
            case ("Orange"):
                buildHouseText.text = ("-�100");
                removeHouseText.text = ("+�100");
                break;
            case ("Red"):
                buildHouseText.text = ("-�150");
                removeHouseText.text = ("+�150");
                break;
            case ("Yellow"):
                buildHouseText.text = ("-�150");
                removeHouseText.text = ("+�150");
                break;
            case ("Green"):
                buildHouseText.text = ("-�200");
                removeHouseText.text = ("+�200");
                break;
            case ("Deep Blue"):
                buildHouseText.text = ("-�200");
                removeHouseText.text = ("+�200");
                break;
        }
    }
    public void HandleRent(Player currentPlayer)
    {
        //renting
        if (GetOwner(currentPlayer.GetCurrentTile()) != currentPlayer.gameObject) // checks if other player owns the property the player landed on
        {
            if (!currentPlayer.GetCurrentTile().GetComponent<Property>().isMortgaged) //checks if property is not mortgaged and owner can receive rent on it
            {
                if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.purchasable && currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group != "Utilities" && currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group != "Station")
                {
                    //applies to normal properties
                    if (currentPlayer.GetBalance() < currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()])
                    {
                        finishTurnButton.SetActive(false);
                        //player needs to sell assets in order to pay for rent
                        //add text to show this
                        Debug.Log("Player must sell assets to afford rent");
                        Debug.Log("Cost=" + currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()]);
                        Debug.Log("Bal=" + currentPlayer.GetBalance());
                        rentPayable = false;
                    }
                    else
                    {
                        if (!rentPayable)
                        {
                            currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()), currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()]);
                            rentPayable = true;
                        }
                    }
                }
                //applies to stations
                else if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group == "Station")
                {
                    switch (GetStationsOwned(GetOwner(currentPlayer.GetCurrentTile())))
                    {
                        //checks if the owner of the station the player has landed on, has other stations
                        case 1:
                            if (currentPlayer.GetBalance() < 25)
                            {
                                finishTurnButton.SetActive(false);
                                //player needs to sell assets in order to pay for rent
                                //add text to show this
                                Debug.Log("Player must sell assets to afford rent");
                                Debug.Log("Cost=25");
                                Debug.Log("Bal=" + currentPlayer.GetBalance());
                                rentPayable = false;
                            }
                            else
                            {
                                if (!rentPayable)
                                {
                                    currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()), 25);
                                    rentPayable = true;
                                }
                            }
                            break;
                        case 2:
                            if (currentPlayer.GetBalance() < 50)
                            {
                                finishTurnButton.SetActive(false);
                                //player needs to sell assets in order to pay for rent
                                //add text to show this
                                Debug.Log("Player must sell assets to afford rent");
                                Debug.Log("Cost=50");
                                Debug.Log("Bal=" + currentPlayer.GetBalance());
                                rentPayable = false;
                            }
                            else
                            {
                                if (!rentPayable)
                                {
                                    currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()), 50);
                                    rentPayable = true;
                                }
                            }
                            break;
                        case 3:
                            if (currentPlayer.GetBalance() < 100)
                            {
                                finishTurnButton.SetActive(false);
                                //player needs to sell assets in order to pay for rent
                                //add text to show this
                                Debug.Log("Player must sell assets to afford rent");
                                rentPayable = false;
                            }
                            else
                            {
                                if (!rentPayable)
                                {
                                    currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()), 100);
                                    rentPayable = true;
                                }
                            }
                            break;
                        case 4:
                            if (currentPlayer.GetBalance() < 200)
                            {
                                finishTurnButton.SetActive(false);
                                //player needs to sell assets in order to pay for rent
                                //add text to show this
                                Debug.Log("Player must sell assets to afford rent");
                                rentPayable = false;
                            }
                            else
                            {
                                if (!rentPayable)
                                {
                                    currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()), 200);
                                    rentPayable = true;
                                }
                            }
                            break;
                    }

                }
                //applies to utilities
                else if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group == "Utilities")
                {
                    if (isColourGroupOwned(currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group, GetOwner(currentPlayer.GetCurrentTile())))
                    {
                        //rent is 10x the numbers rolled on the dice
                        if (currentPlayer.GetBalance() < currentPlayer.numberRolled * 10)
                        {
                            finishTurnButton.SetActive(false);
                            //player needs to sell assets in order to pay for rent
                            //add text to show this
                            Debug.Log("Player must sell assets to afford rent");
                            rentPayable = false;
                        }
                        else if (currentPlayer.GetBalance() > currentPlayer.numberRolled * 10)
                        {
                            if (!rentPayable)
                            {
                                currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()), currentPlayer.numberRolled * 10);
                                rentPayable = true;
                            }
                        }
                    }
                    else
                    {
                        //rent is 4x the umbers rolled on the dice
                        if (currentPlayer.GetBalance() < currentPlayer.numberRolled * 4)
                        {
                            finishTurnButton.SetActive(false);
                            //player needs to sell assets in order to pay for rent
                            //add text to show this
                            Debug.Log("Player must sell assets to afford rent");
                            rentPayable = false;
                        }
                        else if (currentPlayer.GetBalance() > currentPlayer.numberRolled * 4)
                        {
                            if (!rentPayable)
                            {
                                currentPlayer.PayRent(currentPlayer.gameObject, GetOwner(currentPlayer.GetCurrentTile()), currentPlayer.numberRolled * 4);
                                rentPayable = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
