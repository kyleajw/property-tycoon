using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    int turnNumber = 1;
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
                Debug.Log("E Pressed");
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
            if (currentPlayer.GetCurrentTile().GetComponent<Tile>().tileData.purchasable){
                buyButton.SetActive(true);
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
    }
    public void BuyPressed()
    {
        players[currentPlayersTurn].GetComponent<Player>().BuyProperty();
        Debug.Log(players[currentPlayersTurn].GetComponent<Player>().ownedProperties);
    }
}
