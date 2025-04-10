using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class LobbyTests
{
    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator CreateGameWithMinPlayers()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        LobbyHandler lobbyHandler = gameManager.GetComponent<LobbyHandler>();
        PlayersList playersList = GameObject.FindObjectOfType<PlayersList>();
        playersList.AddPlayerCard();
        Assert.AreEqual(1, playersList.GetPlayerCards().Count);

        lobbyHandler.LoadGame();
        yield return new WaitForSeconds(1);
        Assert.AreEqual("GameScene", SceneManager.GetActiveScene().name);
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Assert.AreEqual(1, playerManager.GetPlayers().Length);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CreateGameWithMaxPlayers()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        LobbyHandler lobbyHandler = gameManager.GetComponent<LobbyHandler>();
        PlayersList playersList = GameObject.FindObjectOfType<PlayersList>();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        Assert.AreEqual(5, playersList.GetPlayerCards().Count);

        lobbyHandler.LoadGame();
        yield return new WaitForSeconds(1);

        Assert.AreEqual("GameScene", SceneManager.GetActiveScene().name);
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Assert.AreEqual(5, playerManager.GetPlayers().Length);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CreateGameWithAveragePlayers()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        LobbyHandler lobbyHandler = gameManager.GetComponent<LobbyHandler>();
        PlayersList playersList = GameObject.FindObjectOfType<PlayersList>();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        Assert.AreEqual(3, playersList.GetPlayerCards().Count);

        lobbyHandler.LoadGame();
        yield return new WaitForSeconds(1);

        Assert.AreEqual("GameScene", SceneManager.GetActiveScene().name);
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Assert.AreEqual(3, playerManager.GetPlayers().Length);
        yield return null;
    }

    [UnityTest]
    public IEnumerator RemovePlayerFromLobby()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        LobbyHandler lobbyHandler = gameManager.GetComponent<LobbyHandler>();
        PlayersList playersList = GameObject.FindObjectOfType<PlayersList>();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        Assert.AreEqual(5, playersList.GetPlayerCards().Count);
        PlayerCard card = playersList.GetPlayerCards()[0].GetComponent<PlayerCard>();
        playersList.RemovePlayerCard(1, card.GetPlayerColour(), card.GetPlayerPiece());
        Assert.AreEqual(4, playersList.GetPlayerCards().Count);

        lobbyHandler.LoadGame();
        yield return new WaitForSeconds(1);

        Assert.AreEqual("GameScene", SceneManager.GetActiveScene().name);
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Assert.AreEqual(4, playerManager.GetPlayers().Length);
        yield return null;
    }
}
