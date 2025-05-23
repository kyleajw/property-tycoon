using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.PlayerLoop;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject startTile;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject rollButtonGroup;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject finishTurnButton;
    [SerializeField] GameObject buyButton;
    [SerializeField] GameObject auctionButton;
    [SerializeField] GameObject retireButton;
    [SerializeField] GameObject timer;
    [SerializeField] public GameObject mortgageButton;
    [SerializeField] public GameObject sellButton;
    [SerializeField] public GameObject buildHouseButton;
    [SerializeField] public GameObject removeHouseButton;
    [SerializeField] public GameObject jailButtonGroup;
    [SerializeField] public GameObject houseCounter;
    [SerializeField] TextMeshProUGUI buildHouseText;
    [SerializeField] TextMeshProUGUI removeHouseText;
    [SerializeField] TextMeshProUGUI houseCounterText;
    [SerializeField] TextMeshProUGUI winnerText;
    [SerializeField] GameObject cam;
    [SerializeField] TMP_Text turnAnnouncer;
    [SerializeField] TMP_Text turnCounter;
    [SerializeField] TMP_Text invValueText;
    [SerializeField] Board board;
    [SerializeField] float diceRollForceMultiplier = 5.0f;
    [SerializeField] GameObject cardDialogPrefab;
    [SerializeField] GameObject hotelPrefab;
    [SerializeField] GameObject housePrefab;


    [SerializeField] GameObject playerPrefab;
    Camera mainCamera;

    [SerializeField] GameObject auctionGUI;
    int parkingTotal = 0;
    bool gameStarted = false;
    bool isGameOver = false;
    bool buyButtonPressed = false;
    bool rentPayable = false;
    bool auctionButtonPressed = false; 
    int playersRetired = 0;
    int turnNumber = 1;
    int cyclesCompleted = 0;
    int gameVersion = 1;

    Player[] playerData;
    GameObject[] players;
    int currentPlayersTurn;

    CardManager cardManager;

   
    // Start is called before the first frame update
    void Awake()
    {
        gameOverScreen.SetActive(false);
        mainCamera = Camera.main;
        InitialisePlayers();
        AnnounceTurn();
        SetGameVersion(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetGameVersion());
        if(gameVersion == 0)
        {
            timer.SetActive(false);
        }
        else if(gameVersion == 1)
        {
            timer.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            if (playersRetired == players.Length)
            {
                isGameOver = true;
                winnerText.gameObject.SetActive(true);
                winnerText.SetText("Players have all retired.");
                rollButtonGroup.SetActive(false);
                finishTurnButton.SetActive(false);
                buyButton.SetActive(false);
                auctionButton.SetActive(false);
                retireButton.SetActive(false);
                turnAnnouncer.gameObject.SetActive(false);
                turnCounter.gameObject.SetActive(false);
                invValueText.gameObject.SetActive(false);
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyHandler>().ResetValues();
                //chnage to game over screen
                gameOverScreen.SetActive(true);
            }
            mainCamera.gameObject.GetComponent<CameraHandler>().SetTarget(players[currentPlayersTurn]);
            //abridged version
            if (gameVersion == 1)
            {
                if (timer.GetComponent<Timer>().GetRemainingTime() <= 0)
                {
                    timer.SetActive(false);
                    if (players.Length > 1)
                    {
                        if (turnNumber % players.Length == 1) //ensures everyone ends on equal number of turns
                        {
                            isGameOver = true;
                            //end game and show winner
                            int maxInvValueIndex = 0;
                            int maxInvValue = 0;
                            for(int i=0; i < players.Length-1; i++)
                            {
                                if (players[i].GetComponent<Player>().GetInvValue() < players[i+1].GetComponent<Player>().GetInvValue())
                                {
                                    maxInvValueIndex = i + 1;
                                    maxInvValue = players[i + 1].GetComponent<Player>().GetInvValue();
                                }
                            }
                            //update winner text here and disable UI
                            winnerText.gameObject.SetActive(true);
                            winnerText.SetText($"Player {maxInvValueIndex + 1} has won with an Inventory value of £{maxInvValue}!");
                            rollButtonGroup.SetActive(false);
                            finishTurnButton.SetActive(false);
                            buyButton.SetActive(false);
                            auctionButton.SetActive(false);
                            retireButton.SetActive(false);
                            turnAnnouncer.gameObject.SetActive(false);
                            turnCounter.gameObject.SetActive(false);
                            invValueText.gameObject.SetActive(false);
                            GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyHandler>().ResetValues();
                            //chnage to game over screen
                            gameOverScreen.SetActive(true);
                        }
                    }
                    else
                    {
                        isGameOver = true;
                        //end game and show winner
                        //update winner text here and disable UI
                        winnerText.gameObject.SetActive(true);
                        winnerText.SetText($"Player 1 has won with an Inventory value of �{players[0].GetComponent<Player>().GetInvValue()}!");
                        rollButtonGroup.SetActive(false);
                        finishTurnButton.SetActive(false);
                        buyButton.SetActive(false);
                        auctionButton.SetActive(false);
                        retireButton.SetActive(false);
                        turnAnnouncer.gameObject.SetActive(false);
                        turnCounter.gameObject.SetActive(false);
                        invValueText.gameObject.SetActive(false);
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyHandler>().ResetValues();
                        //chnage to game over screen
                        gameOverScreen.SetActive(true);
                    }
                }
            }
            //standard
            else if (gameVersion == 0)
            {
                if (players.Length > 1)
                {
                    if (playersRetired == players.Length - 1)
                    {
                        int winnerIndex=0;
                        for(int i = 0; i < players.Length; i++)
                        {
                            if (!players[i].GetComponent<Player>().isRetired)
                            {
                                winnerIndex = i;
                            }
                        }
                        //display last player alive has won
                        winnerText.gameObject.SetActive(true);
                        winnerText.SetText($"Player {winnerIndex + 1} has won!");
                        rollButtonGroup.SetActive(false);
                        finishTurnButton.SetActive(false);
                        buyButton.SetActive(false);
                        auctionButton.SetActive(false);
                        retireButton.SetActive(false);
                        turnAnnouncer.gameObject.SetActive(false);
                        turnCounter.gameObject.SetActive(false);
                        invValueText.gameObject.SetActive(false);
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyHandler>().ResetValues();
                        //chnage to game over screen
                        gameOverScreen.SetActive(true);
                    }
                }
            }

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
                while(CheckIfRetired(players[currentPlayersTurn].GetComponent<Player>()))
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
                else
                {
                    finishTurnButton.SetActive(false);
                    retireButton.SetActive(false);
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
    /// <summary>
    /// Sets the corresponding player data from the lobby to instantiated player game objects.
    /// </summary>
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
    /// <summary>
    /// Spawns a new player on the starting tile
    /// </summary>
    /// <returns>New instantiated player game object</returns>
    GameObject InstantiatePlayer()
    {
        return Instantiate(playerPrefab, startTile.transform.position, Quaternion.identity);
    }

    //todo continue during refactor
    /// <summary>
    /// Handles most functionalities on landing on specific tiles. Mostly for AI agent.
    /// </summary>
    /// <param name="landedTile">The tile the player landed on</param>
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
                        if (players[currentPlayersTurn].GetComponent<Player>().GetInvValue() < 200)
                        {
                            RetirePlayer(players[currentPlayersTurn].GetComponent<Player>());
                        }
                        else if (players[currentPlayersTurn].GetComponent<Player>().GetBalance()<200)
                        {
                            finishTurnButton.SetActive(false);
                            Debug.Log("Must sell assets to afford fine");
                        }
                        else
                        {
                            finishTurnButton.SetActive(true);
                            players[currentPlayersTurn].GetComponent<Player>().SetBalance(-200);
                            players[currentPlayersTurn].GetComponent<Player>().SetInvValue(-200);
                        }
                        break;
                    case "Super Tax":
                        if (players[currentPlayersTurn].GetComponent<Player>().GetInvValue() < 100)
                        {
                            RetirePlayer(players[currentPlayersTurn].GetComponent<Player>());
                        }
                        else if (players[currentPlayersTurn].GetComponent<Player>().GetBalance() < 100)
                        {
                            finishTurnButton.SetActive(false);
                            Debug.Log("Must sell assets to afford fine");
                        }
                        else 
                        {
                            finishTurnButton.SetActive(true);
                            players[currentPlayersTurn].GetComponent<Player>().SetBalance(-100);
                            players[currentPlayersTurn].GetComponent<Player>().SetInvValue(-100);
                        }
                        break;
                }
                if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                {
                    players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                }
                break;
            case "Unique":
                Debug.Log("square tile");
                switch (tile.spaceName)
                {
                    case "Go":
                        Debug.Log("GO Tile, implemented elsewhere");
                        if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                        {
                            players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                        }
                        break;
                    case "Jail/Just visiting":
                        Debug.Log("JAIL TILE");
                        if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                        {
                            players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                        }
                        break;
                    case "Free Parking":
                        Debug.Log("player landed on free parking");
                        players[currentPlayersTurn].GetComponent<Player>().SetBalance(parkingTotal);
                        parkingTotal = 0;
                        if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                        {
                            players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                        }
                        break;
                    case "Go to Jail":
                        Debug.Log("player goes to jail");
                        players[currentPlayersTurn].GetComponent<Player>().GoToJail();
                        break;
                }
                break;
            default:
                Debug.Log("Property");

                if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                {
                    Player currentPlayer = players[currentPlayersTurn].GetComponent<Player>();
                    if (tile.purchasable && GetOwner(landedTile) == null && currentPlayer.completedCycle) {
                        players[currentPlayersTurn].GetComponent<EasyAgent>().OnLandsOnPurchasableProperty();
                    }
                    else if (tile.purchasable && GetOwner(landedTile) != null) {
                        HandleRent(currentPlayer);
                        players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                    }
                    else
                    {
                        players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                    }
                }
                break;
        }
        
    }
    /// <summary>
    /// Shows the card drawn in the middle of the screen
    /// </summary>
    /// <param name="cardType">Pot Luck / Opportunity Knocks</param>
    /// <param name="card">Data to be displayed on the card</param>
    void ShowCardDialog(string cardType, CardData card)
    {
        GameObject newCardDialog = Instantiate(cardDialogPrefab, gameCanvas.transform);
        CardDialog cardDialog = newCardDialog.GetComponent<CardDialog>();
        cardDialog.UpdateCardInfo(cardType, card);
        if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
        {
            players[currentPlayersTurn].GetComponent<EasyAgent>().OnCardDrawn(card.arg.Split(' ')[0] == "CHOICE:");
            newCardDialog.GetComponentInChildren<Button>().interactable = false;
        }
    }
    /// <summary>
    /// When the player closes the card dialog, compute the action associated with that card and the button they pressed.
    /// </summary>
    /// <param name="card">Card drawn</param>
    /// <param name="choice">Choice made (always 1 if not a card with multi choices)</param>
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
                    players[currentPlayersTurn].GetComponent<Player>().SetInvValue(amount);
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
                            player.GetComponent<Player>().SetInvValue(-amount);
                        }
                    }
                    players[currentPlayersTurn].GetComponent<Player>().SetBalance(total);
                    players[currentPlayersTurn].GetComponent<Player>().SetInvValue(total);
                }
                if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                {
                    players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                }
                break;
            case "PAY":
                Debug.Log("Player Pays");
                string payee = args[1];
                amount = Convert.ToInt32(args[2]);
                switch (payee)
                {
                    case "BANK":
                        if (players[currentPlayersTurn].GetComponent<Player>().GetInvValue() < amount)
                        {
                            RetirePlayer(players[currentPlayersTurn].GetComponent<Player>());
                        }
                        else if (players[currentPlayersTurn].GetComponent<Player>().GetBalance() < amount)
                        {
                            finishTurnButton.SetActive(false);
                            Debug.Log("Must sell assets to afford fine");
                        }
                        else
                        {
                            finishTurnButton.SetActive(true);
                            players[currentPlayersTurn].GetComponent<Player>().SetBalance(-amount);
                            players[currentPlayersTurn].GetComponent<Player>().SetInvValue(-amount);
                        }
                        break;
                    case "PARKING":
                        if (players[currentPlayersTurn].GetComponent<Player>().GetInvValue() < amount)
                        {
                            RetirePlayer(players[currentPlayersTurn].GetComponent<Player>());
                        }
                        else if (players[currentPlayersTurn].GetComponent<Player>().GetBalance() < amount)
                        {
                            finishTurnButton.SetActive(false);
                            Debug.Log("Must sell assets to afford fine");
                        }
                        else
                        {
                            finishTurnButton.SetActive(true);
                            players[currentPlayersTurn].GetComponent<Player>().SetBalance(-amount);
                            players[currentPlayersTurn].GetComponent<Player>().SetInvValue(-amount);
                        }
                        parkingTotal += amount;
                        break;
                }
                if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                {
                    players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
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
                if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                {
                    players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                }
                break;
            case "CHOICE:":
                Debug.Log("choice dialog");
                HandleMultiChoiceCard(card.arg.Substring(action.Length+1),choice);
                if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                {
                    players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                }
                break;
            case "VARIABLE:":
                Debug.Log("pay per this and this");
                HandleVariableCard(card.arg.Substring(action.Length));
                if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                {
                    players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                }
                break;
            default:
                Debug.Log("Unrecognized action:" + action);
                if (!players[currentPlayersTurn].GetComponent<Player>().IsHuman())
                {
                    players[currentPlayersTurn].GetComponent<EasyAgent>().EndTurn();
                }
                break;           
        }
        UpdateInvValueText();
    }
    /// <summary>
    /// Gets the name of a tile from parsed string
    /// </summary>
    /// <param name="locationString">Tile location</param>
    /// <returns>Stripped string containing purely the location name of the tile</returns>
    string ExtractLocation(string locationString)
    {
        return locationString.Substring(locationString.IndexOf("'")+1, locationString.LastIndexOf("'") - locationString.IndexOf("'") - 1);
    }
    /// <summary>
    /// Calculates the distance in steps from the players current position and the position of the given location
    /// </summary>
    /// <param name="location">Name of a tile on the board</param>
    /// <returns>Clockwise distance from player to given tile in steps</returns>
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
    /// <summary>
    /// Calculates the anti-clockwise distance in steps from the players current position and the position of the given location
    /// </summary>
    /// <param name="location">Name of a tile on the board</param>
    /// <returns>Anti-Clockwise distance from player to given tile in steps</returns>
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
    /// <summary>
    /// Finds the index / position of a given tile
    /// </summary>
    /// <param name="location">Name of tile</param>
    /// <returns>Index / Position of given tile</returns>
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
    /// <summary>
    /// Splits card choices into a switch/case statement. Applies the given action based on the choice made
    /// </summary>
    /// <param name="decisions">Choices that can be made</param>
    /// <param name="choice">Choice made</param>
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
    /// <summary>
    /// Splits the actions of a card where multiple factors are in play e.g. Pay the bank 100 per house and pay 300 per hotel
    /// </summary>
    /// <param name="actions"></param>
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
    /// <summary>
    /// Splits an action from a card into a switch/case statement and applies the appropriate calculation
    /// </summary>
    /// <param name="action">Action being made</param>
    /// <param name="person">Payee</param>
    /// <param name="arg">String array of actions split by spaces</param>
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
    /// <summary>
    /// Gets every hotel owned by the current player
    /// </summary>
    /// <returns>Amount of hotels owned by current player</returns>
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
    /// <summary>
    /// Gets every house owned by the current player
    /// </summary>
    /// <returns>Amount of houses owned by the player</returns>
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

    /// <summary>
    /// Displays the correct UI on the game canvas in relation to the current actions the player can do.
    /// </summary>
    /// <param name="currentPlayer">Player whos turn it is</param>
    void HandleCanvasVisibility(Player currentPlayer)
    {
        if (!isGameOver)
        {
            if (currentPlayer.IsPlayersTurn() && !rollButtonGroup.activeInHierarchy && !currentPlayer.IsPlayerMoving() && !currentPlayer.HasPlayerThrown() && !currentPlayer.IsInJail())
            {
                rollButtonGroup.SetActive(true);
                finishTurnButton.SetActive(false);
                buyButton.SetActive(false);
                auctionButton.SetActive(false);
                retireButton.SetActive(false);
                jailButtonGroup.SetActive(false);
            }
            else if (currentPlayer.IsInJail())
            {
                if (currentPlayer.GetTurnsInJail() == 2)
                {
                    currentPlayer.isInJail = false;
                    currentPlayer.ResetJailTurnCounter();
                    jailButtonGroup.SetActive(false);
                    currentPlayer.finishedTurn = true;
                }
                else
                {
                    jailButtonGroup.SetActive(true);
                }
                rollButtonGroup.SetActive(false);
                finishTurnButton.SetActive(false);
                buyButton.SetActive(false);
                auctionButton.SetActive(false);
                retireButton.SetActive(false);
            }
            else if (!currentPlayer.IsPlayersTurn() && rollButtonGroup.activeInHierarchy)
            {
                rollButtonGroup.SetActive(false);
                finishTurnButton.SetActive(false);
                buyButton.SetActive(false);              
                auctionButton.SetActive(false);
                retireButton.SetActive(false);
                jailButtonGroup.SetActive(false);
            }
            else if (currentPlayer.IsPlayersTurn() && currentPlayer.IsMenuReady() && currentPlayer.HasPlayerThrown())
            {
                //Debug.Log(currentPlayer.GetDoubleRolled());
                if (!currentPlayer.GetDoubleRolled())
                {
                    rollButtonGroup.SetActive(false);
                    jailButtonGroup.SetActive(false);
                    retireButton.SetActive(true);
                    finishTurnButton.SetActive(true);
                    if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.purchasable)
                    {
                        if (GetOwner(currentPlayer.GetCurrentTile()) != null)
                        {
                            buyButton.SetActive(false);
                            auctionButton.SetActive(false);
                            HandleRent(currentPlayer);
                        }
                        else if (GetOwner(currentPlayer.GetCurrentTile()) == null && currentPlayer.completedCycle) //checks if property has not been purchased yet
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
                        }
                    }
                }
                else
                {
                    Debug.Log("Entered Double roll state");
                    currentPlayer.SetTurn(true);
                    currentPlayer.SetHasThrown(false);
                    currentPlayer.SetDoubleRolled(false);
                    finishTurnButton.SetActive(false);
                    buyButton.SetActive(false);
                    auctionButton.SetActive(false);
                    turnNumber++;                  
                    rollButtonGroup.SetActive(true);
                    AnnounceTurn();
                }             
            }
            else if (currentPlayer.IsPlayersTurn() && currentPlayer.IsMenuReady() && currentPlayer.HasPlayerThrown())
            {
                if (GetOwner(currentPlayer.GetCurrentTile()) != null)
                {
                    buyButton.SetActive(false);
                    HandleRent(currentPlayer);
                }
            }
            
            if (auctionButtonPressed)
            {
                buyButton.SetActive(false);
                auctionButton.SetActive(false);
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
        UpdateInvValueText();
    }
    /// <summary>
    /// Rolls the dice with the amount of time the button was held down and a force multiplier being the factors
    /// </summary>
    /// <param name="timeHeld">Time the roll button was held down for</param>
    public void OnRollButtonReleased(float timeHeld)
    {
        rollButtonGroup.SetActive(false);
        players[currentPlayersTurn].GetComponent<Player>().RollDice(timeHeld * diceRollForceMultiplier);
    }
    /// <summary>
    /// Gets amount of players that have completed atleast one cycle of the board
    /// </summary>
    /// <returns>Amount of players who have completed a cycle</returns>
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
    /// <summary>
    /// Disable the finish turn and retire buttons an turn ended, set associated turn variables to false
    /// </summary>
    public void OnFinishedTurn()
    {
        players[currentPlayersTurn].GetComponent<Player>().SetTurn(false);
        players[currentPlayersTurn].GetComponent<Player>().SetHasThrown(false);
        finishTurnButton.SetActive(false);
        retireButton.SetActive(false);
        buyButtonPressed=false;
        auctionButtonPressed=false;
    }
    /// <summary>
    /// Calls the BuyProperty() method in the <see cref="Player"/> class.
    /// </summary>
    public void BuyPressed()
    {
        players[currentPlayersTurn].GetComponent<Player>().BuyProperty();
        UpdateInvValueText();
        Debug.Log(players[currentPlayersTurn].GetComponent<Player>().ownedProperties);
        buyButtonPressed = true;
    }

    public void BuyOutOfJailPressed()
    {
        players[currentPlayersTurn].GetComponent<Player>().isInJail = false;
        players[currentPlayersTurn].GetComponent<Player>().SetBalance(-50);
        players[currentPlayersTurn].GetComponent<Player>().SetInvValue(-50);
        parkingTotal += 50;
        jailButtonGroup.SetActive(false);
        players[currentPlayersTurn].GetComponent<Player>().finishedTurn = true;
    }
    public void StayInJailPressed()
    {
        players[currentPlayersTurn].GetComponent<Player>().SetJailTurnCounter();
        jailButtonGroup.SetActive(false);
        players[currentPlayersTurn].GetComponent<Player>().finishedTurn = true;

    }
    /// <summary>
    /// Get the <see cref="Auction"/> class and create and setup a new auction for the property the current player is landed on.
    /// </summary>
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
    /// <summary>
    /// Retires the current player
    /// </summary>
    public void RetirePressed()
    {
        RetirePlayer(players[currentPlayersTurn].GetComponent<Player>());
    }
    /// <summary>
    /// Returns to the lobby
    /// </summary>
    public void PlayAgainPressed()
    {
        SceneManager.LoadScene("Lobby");
    }
    /// <summary>
    /// Returns to the main menu
    /// </summary>
    public void ReturnToMenuPressed()
    {
        SceneManager.LoadScene("StartupScene");
    }
    /// <summary>
    /// Charge the correct amount and add a house onto the given property
    /// </summary>
    /// <param name="property">Property being upgraded</param>
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
                            //check if houses are within 1 between each property in the colour group
                            //first find the number of houses currently at each property
                            int groupLength = 0;
                            int selectedTileIndex = 0;
                            string group = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group;
                            for (int j = 0; j < board.GetTileArray().Length; j++)
                            {
                                if (board.GetTileArray()[j].GetComponent<Tile>().tileData.group == group)
                                {
                                    groupLength++;
                                }
                            }
                            int[] propHouseCount = new int[groupLength];
                            int index = 0;
                            for (int j = 0; j < players[currentPlayersTurn].GetComponent<Player>().ownedProperties.Length; j++)
                            {
                                if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[j] != null)
                                {
                                    if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[j].GetComponent<Tile>().tileData.group == group)
                                    {
                                        propHouseCount[index] = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount();
                                        //if its the tile we are building on, store for later checks
                                        if(property.tile.tileData.spaceName== players[currentPlayersTurn].GetComponent<Player>().ownedProperties[j].GetComponent<Tile>().tileData.spaceName)
                                        {
                                            selectedTileIndex = index;
                                        }
                                        index++;
                                    }
                                }
                            }
                            //now check if house counts are all within one
                            bool isEligibleToBuild = true;
                            int maxHouse = propHouseCount[0];
                            int minHouse = propHouseCount[0];
                            for(int j = 0; j < propHouseCount.Length-2; j++)
                            {
                                if(propHouseCount[j+1] > maxHouse)
                                {
                                    maxHouse = propHouseCount[j+1];
                                }
                                if(propHouseCount[j+1] < minHouse)
                                {
                                    minHouse = propHouseCount[j+1];
                                }
                            }
                            if((maxHouse+1) - minHouse > 1)
                            {
                                if(propHouseCount[selectedTileIndex] == maxHouse)
                                {
                                    isEligibleToBuild = false;
                                }
                            }
                            //is eligible if below is true
                            if (isEligibleToBuild)
                            {
                                players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().IncrementHouseCount();
                                if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount() == 5)
                                {
                                    buildHouseButton.SetActive(false);
                                    //add hotel prefab here and remove all house prefabs
                                    foreach(Transform house in players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform)
                                    {
                                        if(house.CompareTag("House"))
                                        {
                                            Destroy(house.gameObject);
                                        }
                                    }
                                    Instantiate(hotelPrefab, players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform, false);
                                }
                                else
                                {
                                    //add extra house prefab here
                                    GameObject newHouse = Instantiate(housePrefab, players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform, false);
                                    float tileWidth = 1f;
                                    int houseNumber = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount()-1;
                                    float offset = -(tileWidth / 2) + (houseNumber * (tileWidth / 4));
                                    if(offset >= 0)
                                    {
                                        offset += 0.25f;
                                    }
                                    newHouse.transform.localPosition = newHouse.transform.localPosition + new Vector3(offset, 0, 0);
                                    //for (int j = 0; j < players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform.childCount; j++)
                                    //{

                                    //    if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform.GetChild(j).CompareTag("House"))
                                    //    {
                                    //        offset = -(tileWidth / 2) + (houseNumber * (tileWidth / 4));

                                    //        if (offset >= 0)
                                    //        {
                                    //            offset += 0.25f;
                                    //        }
                                    //        Debug.Log(offset);
                                    //        GameObject house = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform.GetChild(j).gameObject;
                                    //        house.transform.localPosition = house.transform.localPosition + new Vector3(offset, 0, 0);
                                    //        houseNumber++;
                                    //        Debug.Log(houseNumber);
                                    //    }
                                    //}
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
                            else
                            {
                                Debug.Log("All properties must be at most one house apart.");
                            }
                        }
                    }
                }
            }
        }
        UpdateInvValueText();
    }
    /// <summary>
    /// Refund the correct amount and remove a house onto the given property
    /// </summary>
    /// <param name="property">Property being downgraded</param>
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
                            //check if houses are within 1 between each property in the colour group
                            //first find the number of houses currently at each property
                            int groupLength = 0;
                            int selectedTileIndex = 0;
                            string group = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.group;
                            for (int j = 0; j < board.GetTileArray().Length; j++)
                            {
                                if (board.GetTileArray()[j].GetComponent<Tile>().tileData.group == group)
                                {
                                    groupLength++;
                                }
                            }
                            int[] propHouseCount = new int[groupLength];
                            int index = 0;
                            for (int j = 0; j < players[currentPlayersTurn].GetComponent<Player>().ownedProperties.Length; j++)
                            {
                                if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[j] != null)
                                {
                                    if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[j].GetComponent<Tile>().tileData.group == group)
                                    {
                                        propHouseCount[index] = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount();
                                        //if its the tile we are building on, store for later checks
                                        if (property.tile.tileData.spaceName == players[currentPlayersTurn].GetComponent<Player>().ownedProperties[j].GetComponent<Tile>().tileData.spaceName)
                                        {
                                            selectedTileIndex = index;
                                        }
                                        index++;
                                    }
                                }
                            }
                            //now check if house counts are all within one
                            bool isEligibleToBuild = true;
                            int maxHouse = propHouseCount[0];
                            int minHouse = propHouseCount[0];
                            for (int j = 0; j < propHouseCount.Length - 2; j++)
                            {
                                if (propHouseCount[j + 1] > maxHouse)
                                {
                                    maxHouse = propHouseCount[j + 1];
                                }
                                if (propHouseCount[j + 1] < minHouse)
                                {
                                    minHouse = propHouseCount[j + 1];
                                }
                            }
                            if (maxHouse  - (minHouse-1) > 1)
                            {
                                if (propHouseCount[selectedTileIndex] == minHouse)
                                {
                                    isEligibleToBuild = false;
                                }
                            }
                            if (isEligibleToBuild)
                            {
                                players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().DecrementHouseCount();
                                if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount() == 0)
                                {
                                    removeHouseButton.SetActive(false);
                                    //remove last house prefab here
                                    foreach (Transform child in players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform)
                                    {
                                        if (child.CompareTag("House"))
                                        {
                                            Destroy(child.gameObject);
                                            break;
                                        }
                                    }
                                }
                                else if(players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Property>().GetHouseCount() == 4)
                                {
                                    //remove hotel and place 4 house prefabs
                                    foreach (Transform child in players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform)
                                    {
                                        if (child.CompareTag("Hotel"))
                                        {
                                            Destroy(child.gameObject);
                                            break;
                                        }
                                    }
                                    Instantiate(housePrefab, players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform, false);
                                    Instantiate(housePrefab, players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform, false);
                                    Instantiate(housePrefab, players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform, false);
                                    Instantiate(housePrefab, players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform, false);
                                    float tileWidth = 1f;
                                    int houseNumber = 0;
                                    float offset = -0.5f;
                                    for(int j = 0; j < players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform.childCount; j++)
                                    {

                                        if (players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform.GetChild(j).CompareTag("House"))
                                        {
                                            offset = -(tileWidth / 2) + (houseNumber * (tileWidth / 4));

                                            if (offset >= 0)
                                            {
                                                offset += 0.25f;
                                            }
                                            Debug.Log(offset);
                                            GameObject house = players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform.GetChild(j).gameObject;
                                            house.transform.localPosition = house.transform.localPosition + new Vector3(offset,0,0);
                                            houseNumber++;
                                            Debug.Log(houseNumber);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Transform child in players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].transform)
                                    {
                                        if (child.CompareTag("House"))
                                        {
                                            Destroy(child.gameObject);
                                            break;
                                        }
                                    }
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
                            else
                            {
                                Debug.Log("All properties must be at most one house apart.");
                            }
                        }
                    }
                }
            }
        }
        UpdateInvValueText();
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
                        UpdateInvValueText();
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
    /// <summary>
    /// Sells the parsed property back to the bank for the full purchase price, removing the property from the player's owned properties
    /// </summary>
    /// <param name="property">Property being sold</param>
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
                        UpdateInvValueText();
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
        UpdateInvValueText();
    }
    /// <summary>
    /// Gets the owner of a tile provided
    /// </summary>
    /// <param name="curTile">Current tile</param>
    /// <returns>Player game object</returns>
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
    /// <summary>
    /// Searches through all owned properties, checking if they own every property in the given colour group
    /// </summary>
    /// <param name="group">Colour group to check</param>
    /// <param name="owner">Player game object</param>
    /// <returns></returns>
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
    /// <summary>
    /// Gets the count of all stations the player provided owns.
    /// </summary>
    /// <param name="owner">Player game object</param>
    /// <returns>Number of stations owned by player provided</returns>
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
    /// <summary>
    /// Gets the player whos current turn it is
    /// </summary>
    /// <returns>Current player whos turn it is</returns>
    public GameObject GetCurrentPlayer()
    {
        return players[currentPlayersTurn];
    }
    /// <summary>
    /// Update the text corresponding to building / removing a house on a property on the property menu
    /// </summary>
    /// <param name="property">property selected</param>
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
    /// <summary>
    /// Charges the player a rent fee if they land on a tile owned by another user. If the value of their inventory is lower than the rent they are automatically retired
    /// </summary>
    /// <param name="currentPlayer"></param>
    public void HandleRent(Player currentPlayer)
    {
        //renting
        if (GetOwner(currentPlayer.GetCurrentTile()) != currentPlayer.gameObject) // checks if other player owns the property the player landed on
        {
            if (!currentPlayer.GetCurrentTile().GetComponent<Property>().isMortgaged) //checks if property is not mortgaged and owner can receive rent on it
            {
                if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.purchasable && currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group != "Utilities" && currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.group != "Station")
                {
                    if (currentPlayer.GetComponent<Player>().GetInvValue() < currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()])
                    {
                        //game over - rent cant be paid
                        RetirePlayer(currentPlayer);
                    }
                    //applies to normal properties
                    else if (currentPlayer.GetBalance() < currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()])
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
                            currentPlayer.SetInvValue(-currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()]);
                            GetOwner(currentPlayer.GetCurrentTile()).GetComponent<Player>().SetInvValue(currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.rentPrices[currentPlayer.GetCurrentTile().GetComponent<Property>().GetHouseCount()]);
                            UpdateInvValueText();
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
                            if (currentPlayer.GetComponent<Player>().GetInvValue() < 25)
                            {
                                //game over - rent cant be paid
                                RetirePlayer(currentPlayer);
                            }
                            else if (currentPlayer.GetBalance() < 25)
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
                                    currentPlayer.SetInvValue(-25);
                                    GetOwner(currentPlayer.GetCurrentTile()).GetComponent<Player>().SetInvValue(25);
                                    UpdateInvValueText();
                                    rentPayable = true;
                                }
                            }
                            break;
                        case 2:
                            if (currentPlayer.GetComponent<Player>().GetInvValue() < 50)
                            {
                                //game over - rent cant be paid
                                RetirePlayer(currentPlayer);
                            }
                            else if (currentPlayer.GetBalance() < 50)
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
                                    currentPlayer.SetInvValue(-50);
                                    GetOwner(currentPlayer.GetCurrentTile()).GetComponent<Player>().SetInvValue(50);
                                    UpdateInvValueText();
                                    rentPayable = true;
                                }
                            }
                            break;
                        case 3:
                            if (currentPlayer.GetComponent<Player>().GetInvValue() < 100)
                            {
                                //game over - rent cant be paid
                                RetirePlayer(currentPlayer);
                            }
                            else if (currentPlayer.GetBalance() < 100)
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
                                    currentPlayer.SetInvValue(-100);
                                    GetOwner(currentPlayer.GetCurrentTile()).GetComponent<Player>().SetInvValue(100);
                                    UpdateInvValueText();
                                    rentPayable = true;
                                }
                            }
                            break;
                        case 4:
                            if (currentPlayer.GetComponent<Player>().GetInvValue() < 200)
                            {
                                //game over - rent cant be paid
                                RetirePlayer(currentPlayer);
                            }
                            else if (currentPlayer.GetBalance() < 200)
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
                                    currentPlayer.SetInvValue(-200);
                                    GetOwner(currentPlayer.GetCurrentTile()).GetComponent<Player>().SetInvValue(200);
                                    UpdateInvValueText();
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
                        if(currentPlayer.GetComponent<Player>().GetInvValue()<currentPlayer.numberRolled * 10)
                        {
                            //game over - rent cant be paid
                            RetirePlayer(currentPlayer);
                        }
                        //rent is 10x the numbers rolled on the dice
                        else if (currentPlayer.GetBalance() < currentPlayer.numberRolled * 10)
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
                                currentPlayer.SetInvValue(-currentPlayer.numberRolled * 10);
                                GetOwner(currentPlayer.GetCurrentTile()).GetComponent<Player>().SetInvValue(currentPlayer.numberRolled*10);
                                UpdateInvValueText();
                                rentPayable = true;
                            }
                        }
                    }
                    else
                    {
                        if (currentPlayer.GetComponent<Player>().GetInvValue() < currentPlayer.numberRolled * 4)
                        {
                            //game over - rent cant be paid
                            RetirePlayer(currentPlayer);
                        }
                        //rent is 4x the numbers rolled on the dice
                        else if (currentPlayer.GetBalance() < currentPlayer.numberRolled * 4)
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
                                currentPlayer.SetInvValue(-currentPlayer.numberRolled * 4);
                                GetOwner(currentPlayer.GetCurrentTile()).GetComponent<Player>().SetInvValue(currentPlayer.numberRolled * 4);
                                UpdateInvValueText();
                                rentPayable = true;
                            }
                        }
                    }
                }
            }
        }
    }
    public void SetGameVersion(int version)
    {
        gameVersion = version;
    }
    public void UpdateInvValueText()
    {
        Debug.Log(players[currentPlayersTurn].GetComponent<Player>().GetInvValue());
        invValueText.SetText($"Inventory Value: {players[currentPlayersTurn].GetComponent<Player>().GetInvValue()}");
    }
    /// <summary>
    /// Returns all properties owned by the player to the bank, along with any money that they have.
    /// Sets the player to retired.
    /// </summary>
    /// <param name="currentPlayer">Player retiring</param>
    public void RetirePlayer(Player currentPlayer)
    {
        //return all player properties owned or mortgaged, if any, to the bank
        //return money to bank
        //set player to retired
        //hide current player object
        if(players.Length < 2)
        {
            gameOverScreen.SetActive(true);
        }
        else
        {
            for (int i = 0; i < currentPlayer.ownedProperties.Length; i++)
            {
                if (currentPlayer.ownedProperties[i] != null)
                {
                    currentPlayer.ownedProperties[i].GetComponent<Property>().isMortgaged = false;
                    currentPlayer.ownedProperties[i].GetComponent<Property>().SetOwnedBy(null);
                    board.GetBank().properties[i] = currentPlayer.ownedProperties[i];
                    currentPlayer.ownedProperties[i] = null;
                }
            }
            board.GetBank().SetBankBalance(currentPlayer.GetBalance());
            currentPlayer.SetBalance(-currentPlayer.GetBalance());
            currentPlayer.SetInvValue(-currentPlayer.GetInvValue());
            currentPlayer.isRetired = true;
            currentPlayer.gameObject.SetActive(false);
            currentPlayer.SetTurn(false);
            currentPlayer.SetHasThrown(false);
            playersRetired++;
        }
    }

    public bool CheckIfRetired(Player currentPlayer)
    {
        return currentPlayer.isRetired;
    }

    public GameObject[] GetPlayers()
    {
        return players;
    }
}
