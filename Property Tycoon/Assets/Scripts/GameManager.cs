using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    Player[] players;
    int gameDuration = 300;
    int gameVersion = 0; //0 is standard, 1 is abridged

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
    public void SetGameDuration(int duration)
    {
        gameDuration = duration;
    }
    public int GetGameDuration()
    {
        return gameDuration;
    }
    public void SetGameVersion(int version)
    {
        gameVersion = version;
    }
    public int GetGameVersion()
    {
        return gameVersion;
    }
}

