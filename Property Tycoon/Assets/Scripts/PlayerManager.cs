using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Codice.CM.Common.CmCallContext;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    [SerializeField] GameObject playerPrefab;
    Camera mainCamera;

    bool gameStarted = false;
    bool isGameOver = false;
    bool buyButtonPressed = false;
    bool rentPayable = false;
    int playersRetired = 0;
    int turnNumber = 1;
    int cyclesCompleted = 0;
    int gameVersion = 0;

    Player[] playerData;
    GameObject[] players;
    int currentPlayersTurn;


    // Start is called before the first frame update
    void Start()
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
                                    maxInvValue = i + 1;
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
                            //chnage to game over screen
                            gameOverScreen.SetActive(true);
                        }
                    }
                    else
                    {
                        isGameOver = true;
                        //end game and show winner
                        int maxInvValueIndex = 0;
                        int maxInvValue = players[0].GetComponent<Player>().GetInvValue();
                        for (int i = 0; i < players.Length - 1; i++)
                        {
                            if (players[i].GetComponent<Player>().GetInvValue() < players[i + 1].GetComponent<Player>().GetInvValue())
                            {
                                maxInvValueIndex = i + 1;
                                maxInvValue = i + 1;
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
                        //chnage to game over screen
                        gameOverScreen.SetActive(true);
                    }
                }
            }
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
                        //chnage to game over screen
                        gameOverScreen.SetActive(true);
                    }
                }
            }
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
            if (playerData[i].IsHuman())
            {
                players[i] = InstantiateHumanPlayer();
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
        if (!isGameOver)
        {
            if (currentPlayer.IsPlayersTurn() && !rollButtonGroup.activeInHierarchy && !currentPlayer.IsPlayerMoving() && !currentPlayer.HasPlayerThrown() && !currentPlayer.IsInJail())
            {
                rollButtonGroup.SetActive(true);
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
            }
            else if (currentPlayer.IsPlayersTurn() && currentPlayer.IsMenuReady() && currentPlayer.HasPlayerThrown() && currentPlayer.completedCycle)
            {
                //Debug.Log(currentPlayer.GetDoubleRolled());
                if (!currentPlayer.GetDoubleRolled())
                {
                    rollButtonGroup.SetActive(false);
                    retireButton.SetActive(true);
                    finishTurnButton.SetActive(true);
                }
                else
                {
                    //bug where buying property in this state allows double buy of current tile and first property on board
                    //buy button also doesnt dissapear after first purchase
                    Debug.Log("Entered Double roll state");
                    currentPlayer.SetTurn(true);
                    currentPlayer.SetHasThrown(false);
                    currentPlayer.SetDoubleRolled(false);
                    finishTurnButton.SetActive(false);;
                    turnNumber++;                  
                    rollButtonGroup.SetActive(true);
                    AnnounceTurn();
                }
                if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.purchasable)
                {
                    if (GetOwner(currentPlayer.GetCurrentTile()) != null)
                    {
                        buyButton.SetActive(false);
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
                    }
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
        }    
    }

    void AnnounceTurn()
    {
        turnAnnouncer.SetText($"Player {currentPlayersTurn + 1}'s turn");
        turnAnnouncer.color = players[currentPlayersTurn].GetComponent<Player>().GetPlayerColour();
        turnCounter.SetText($"Total turns: {turnNumber}");
        UpdateInvValueText();
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
        UpdateInvValueText();
        Debug.Log(players[currentPlayersTurn].GetComponent<Player>().ownedProperties);
        buyButtonPressed = true;
    }
    public void AuctionPressed()
    {

    }
    public void RetirePressed()
    {
        RetirePlayer(players[currentPlayersTurn].GetComponent<Player>());
    }
    public void PlayAgainPressed()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void ReturnToMenuPressed()
    {
        SceneManager.LoadScene("StartupScene");
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
        UpdateInvValueText();
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
                        players[currentPlayersTurn].GetComponent<Player>().SetInvValue(-players[currentPlayersTurn].GetComponent<Player>().ownedProperties[i].GetComponent<Tile>().tileData.purchaseCost / 2);
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
                                currentPlayer.SetInvValue(currentPlayer.numberRolled * 10);
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
                                currentPlayer.SetInvValue(currentPlayer.numberRolled * 4);
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
    public void RetirePlayer(Player currentPlayer)
    {
        //return all player properties owned or mortgaged, if any, to the bank
        //return money to bank
        //set player to retired
        //hide current player object
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
    public bool CheckIfRetired(Player currentPlayer)
    {
        return currentPlayer.isRetired;
    }
}
