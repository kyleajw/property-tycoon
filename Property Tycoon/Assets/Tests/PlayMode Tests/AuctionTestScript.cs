using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AuctionTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void AuctionTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    [OneTimeSetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator AuctionTestScriptWithEnumeratorPasses()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        LobbyHandler lobbyHandler = gameManager.GetComponent<LobbyHandler>();
        PlayersList playersList = GameObject.FindObjectOfType<PlayersList>();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();
        playersList.AddPlayerCard();


        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
