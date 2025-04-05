using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Player[] players;


    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void SetPlayers(List<GameObject> playerCards)
    {
        players = new Player[playerCards.Count];
        foreach (GameObject playerCardObject in playerCards)
        {
            PlayerCard playerCard = playerCardObject.GetComponent<PlayerCard>();
            Player newPlayer = new Player();
            newPlayer.AssignPiece(playerCard.GetPlayerPiece());
            newPlayer.SetPlayerNumber(playerCard.GetPlayerNumber());
            newPlayer.SetPlayerName(playerCard.GetPlayerName());
            newPlayer.SetPlayerColour(playerCard.GetPlayerColour());
            newPlayer.SetIsHuman(playerCard.GetIsHuman());

            players[playerCard.GetPlayerNumber() - 1] = newPlayer;
        }
    }

    public Player[] GetPlayerData()
    {
        return players;
    }
}

