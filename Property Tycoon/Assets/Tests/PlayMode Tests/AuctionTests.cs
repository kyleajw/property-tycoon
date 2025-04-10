using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AuctionTests
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
        playersList.AddPlayerCard();
        lobbyHandler.LoadGame();
        yield return new WaitForSeconds(1);
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        foreach (GameObject playerObject in playerManager.GetPlayers())
        {
            Player player = playerObject.GetComponent<Player>();
            player.completedCycle = true;
        }
        playerManager.GetCurrentPlayer().GetComponent<Player>().MovePiece(1);
        while(playerManager.GetCurrentPlayer().GetComponent<Player>().IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        playerManager.AuctionPressed();
        yield return null;
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerBids1()
    {
        Auction auction = GameObject.FindObjectOfType<Auction>();
        auction.OnPlayerBids(1);
        Assert.AreEqual(1, auction.GetHighestBid());
        yield return null;
    }
    [UnityTest]
    public IEnumerator PlayerBids10()
    {
        Auction auction = GameObject.FindObjectOfType<Auction>();
        auction.OnPlayerBids(10);
        Assert.AreEqual(10, auction.GetHighestBid());
        yield return null;
    }
    [UnityTest]
    public IEnumerator PlayerBids100()
    {
        Auction auction = GameObject.FindObjectOfType<Auction>();
        auction.OnPlayerBids(100);
        Assert.AreEqual(100, auction.GetHighestBid());
        yield return null;
    }

    [UnityTest]
    public IEnumerator IsCorrectPropertyBeingAuctioned()
    {
        Auction auction = GameObject.FindObjectOfType<Auction>();
        Assert.AreEqual("The Old Creek", auction.GetPropertyBeingAuctioned().GetComponent<Tile>().tileData.spaceName);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CorrectTurnOrder()
    {
        Auction auction = GameObject.FindObjectOfType<Auction>();
        int playerNumber = auction.GetCurrentBidder().GetComponent<Player>().GetPlayerNumber();
        Assert.AreEqual(1, playerNumber);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerWinsAuction()
    {
        Auction auction = GameObject.FindObjectOfType<Auction>();
        Bank bank = GameObject.FindObjectOfType<Bank>();
        Player player = auction.GetCurrentBidder().GetComponent<Player>();
        auction.OnPlayerBids(1);
        auction.OnPlayerLeavesAuction();
        Assert.AreEqual(1499, player.GetBalance());
        Assert.IsNotNull(player.ownedProperties[0]);
        Assert.IsNull(bank.GetProperty(0));

        yield return null;
    }

}
