using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] bool singlePlayerDebug = false;
    [SerializeField] float diceRollForceMultiplier = 5.0f;
    [SerializeField] GameObject cam;
    [SerializeField] Board board;
    bool gameStarted = false;
    int playerCount;
    int turnNumber = 1;

    GameObject[] players;
    int currentPlayersTurn;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject startTile;

    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject rollButtonGroup;
    [SerializeField] TMP_Text turnAnnouncer;


    // Start is called before the first frame update
    void Start()
    {
        if (singlePlayerDebug)
        {
            playerCount = 1;
        }
        InitialisePlayers();
        AnnounceTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
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
        }
    }

    void InitialisePlayers()
    {
        players = new GameObject[playerCount];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = InstantiateHumanPlayer();
            players[i].name = $"Player {i + 1}";
            players[i].GetComponent<Player>().SetIsHuman(true);
            players[i].GetComponent<Player>().SetPlayerNumber(i);
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
        }
        else if (!currentPlayer.IsPlayersTurn() && rollButtonGroup.activeInHierarchy)
        {
            rollButtonGroup.SetActive(false);
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
}
