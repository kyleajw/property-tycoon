using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PropertyTests
{
    [UnitySetUp]
    public IEnumerator InitialSetup()
    {
        SceneManager.LoadScene("Lobby");
        yield return new WaitForSeconds(1);
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        LobbyHandler lobbyHandler = gameManager.GetComponent<LobbyHandler>();
        PlayersList playersList = GameObject.FindObjectOfType<PlayersList>();
        playersList.AddPlayerCard();
        lobbyHandler.LoadGame();
        yield return new WaitForSeconds(1);
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        foreach (GameObject playerObject in playerManager.GetPlayers())
        {
            Player player = playerObject.GetComponent<Player>();
            player.completedCycle = true;
        }
       

        yield return null;
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerBuysProperty()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        playerManager.GetCurrentPlayer().GetComponent<Player>().MovePiece(1);
        while (playerManager.GetCurrentPlayer().GetComponent<Player>().IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        playerManager.BuyPressed();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Bank bank = GameObject.FindObjectOfType<Bank>();
        Assert.IsNotNull(player.ownedProperties[0]);
        Assert.IsNull(bank.properties[0]);
        Assert.AreEqual(1440, player.GetBalance());
        yield return null;
    }
    [UnityTest]
    public IEnumerator PlayerBuysStation()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        playerManager.GetCurrentPlayer().GetComponent<Player>().MovePiece(5);
        while (playerManager.GetCurrentPlayer().GetComponent<Player>().IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        playerManager.BuyPressed();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Bank bank = GameObject.FindObjectOfType<Bank>();
        Assert.IsNotNull(player.ownedProperties[2]);
        Assert.IsNull(bank.properties[2]);
        Assert.AreEqual(1300, player.GetBalance());
        yield return null;
    }
}
