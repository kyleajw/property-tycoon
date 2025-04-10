using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MovementTests
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
        yield return null;
    }
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MovePlayerForwardsStepsSmall()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Assert.AreEqual(0, player.GetPositionOnBoard());
        player.MovePiece(1);
        while (player.IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        Assert.AreEqual(1, player.GetPositionOnBoard());
        Assert.AreEqual(false, player.completedCycle);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MovePlayerForwardsStepsAverage()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Assert.AreEqual(0, player.GetPositionOnBoard());
        player.MovePiece(12);
        while (player.IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        Assert.AreEqual(12, player.GetPositionOnBoard());
        Assert.AreEqual(false, player.completedCycle);

        yield return null;
    }
    [UnityTest]
    public IEnumerator MovePlayerForwardsStepsLarge()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Assert.AreEqual(0, player.GetPositionOnBoard());
        player.MovePiece(31);
        while (player.IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        Assert.AreEqual(31, player.GetPositionOnBoard());
        Assert.AreEqual(false, player.completedCycle);

        yield return null;
    }
    [UnityTest]
    public IEnumerator MovePlayerBackwardsStepsSmall()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Assert.AreEqual(0, player.GetPositionOnBoard());
        player.MovePiece(-1);
        while (player.IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        Assert.AreEqual(39, player.GetPositionOnBoard());
        yield return null;
    }

    [UnityTest]
    public IEnumerator MovePlayerBackwardsStepsAverage()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Assert.AreEqual(0, player.GetPositionOnBoard());
        player.MovePiece(-12);
        while (player.IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        Assert.AreEqual(28, player.GetPositionOnBoard());
        yield return null;
    }
    [UnityTest]
    public IEnumerator MovePlayerBackwardsStepsLarge()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Assert.AreEqual(0, player.GetPositionOnBoard());
        player.MovePiece(-30);
        while (player.IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        Assert.AreEqual(10, player.GetPositionOnBoard());
        yield return null;
    }

    [UnityTest]
    public IEnumerator CompleteOneBoardCycle()
    {
        PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
        Player player = playerManager.GetCurrentPlayer().GetComponent<Player>();
        Assert.AreEqual(0, player.GetPositionOnBoard());
        player.MovePiece(41);
        while (player.IsPlayerMoving())
        {
            yield return new WaitForEndOfFrame();
        }
        Assert.AreEqual(1, player.GetPositionOnBoard());
        Assert.AreEqual(true, player.completedCycle);
        yield return null;
    }

}
