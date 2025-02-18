using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestOnStartup
{
    [Test]
    public void CorrectGameDataPaths()
    {
        StartupManager startupManager = new StartupManager();
        Assert.IsTrue(startupManager.GetCustomGameDataDirectory() == "CustomGameData");
        Assert.IsTrue(startupManager.GetCustomBoardDataFileName() == "PropertyTycoonBoardData.json");
        Assert.IsTrue(startupManager.GetCustomBoardDataPath() == "CustomGameData/PropertyTycoonBoardData.json");

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestBoardDataHandlerWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
