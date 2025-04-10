using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Manages and reads in data from converted board data (type JSON), which is parsed into the board during generation
/// </summary>
public class BoardDataHandler : MonoBehaviour
{
    const int INTENDED_BOARD_SIZE = 40;

    BoardData boardData;
    StartupManager startupManager;

    string customBoardDataPath;
    string boardDataFileName;

    /// <summary>
    /// Initialises the boardData object and sets path for custom board data by the user, then verifies the existence of the path before the application starts.
    /// </summary>
    private void Start()
    {
        boardData = gameObject.AddComponent<BoardData>();
        startupManager = gameObject.GetComponent<StartupManager>();
        customBoardDataPath = startupManager.GetCustomBoardDataPath();
        boardDataFileName = startupManager.GetCustomBoardDataFileName();
        SetBoardData();
    }

    /// <summary>
    /// Converts & Assigns json data from PropertyTycoonBoardData.json to <see cref="BoardData"/> object.
    /// </summary>
    void SetBoardData()
    {
        try
        {
            string data = File.ReadAllText(customBoardDataPath);
            JsonUtility.FromJsonOverwrite(data, boardData);
            if (ScanForNullBoardData())
            {
                RevertToDefaults();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failure processing custom board data, reverting to default values..\nError: {e}");
            RevertToDefaults();
        }
        Debug.Log($"boardData values (JSON format):\n{JsonUtility.ToJson(boardData, true)}");

    }

    /// <summary>
    /// Generates <see cref="BoardData"/> off of default values, in event of an invalid PropertyTycoonBoardData.json file
    /// </summary>
    void RevertToDefaults()
    {
        Debug.LogError("Invalid Custom Data, reverting to default values..");
        try
        {
            string data = File.ReadAllText($"{Application.streamingAssetsPath}/{boardDataFileName}");
            JsonUtility.FromJsonOverwrite(data, boardData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed reverting to factory settings: {e}");
        }

    }

    /// <summary>
    /// Searches through <see cref="BoardData"/> boardData, checking for any null / invalid data in each tile / property
    /// </summary>
    /// <returns>True if null / invalid data is found, false otherwise</returns>
    bool ScanForNullBoardData()
    {
        if (boardData == null) return true;
        if (boardData.tiles.Length != INTENDED_BOARD_SIZE)
        {
            Debug.LogError($"Length of boardData does not match INTENDED_BOARD_DATA\nboardData.tiles.length: {boardData.tiles.Length} | INTENDED_BOARD_SIZE: {INTENDED_BOARD_SIZE}");
            return true;
        }
        else
        {
            foreach(TileData tile in boardData.tiles)
            {
                if (tile == null || tile.spaceName == null || tile.group == null || tile.action == null) return true;
                if (tile.purchasable)
                {
                    if (!(tile.purchaseCost > 0)) { 
                        return true;
                    }
                    
                    if(!(tile.group == "Station" || tile.group == "Utilities" || tile.group == "Unique" || tile.group == "Opportunity Knocks" || tile.group == "Pot Luck" || tile.group == "Tax"))
                    {
                        return tile.rentPrices.Length != 6;
                    }
                }
                else
                {
                    if (tile.group == "Unique" || tile.group == "Pot Luck" || tile.group == "Opportunity Knocks" || tile.group == "Tax")
                    {
                        String[] actions = { "Collect", "Take", "Pay" };

                        foreach (String action in actions)
                        {
                            if (tile.action.Contains(action)){
                                return false;
                            };
                        }
                        return true;
                    }
                }

            }
        }
        return false;
    }


    /// <summary>
    /// Generic getter method for initialised board data
    /// </summary>
    /// <returns>An instance of <see cref="BoardData"/> class, specifically <see cref="BoardData"/> boardData</returns>
    public BoardData GetBoardData()
    {
        return boardData;
    }
}
