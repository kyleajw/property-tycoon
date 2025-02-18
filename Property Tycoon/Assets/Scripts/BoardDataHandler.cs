using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BoardDataHandler : MonoBehaviour
{
    /// <summary>
    /// Manages and reads in data from converted board data (type JSON), which is parsed into the board during generation
    /// </summary>


    BoardData boardData;
    const string CUSTOM_GAME_DATA_DIRECTORY = "CustomGameData";
    const string BOARD_DATA_FILENAME = "PropertyTycoonBoardData.json";
    string customBoardDataPath;

    /// <summary>
    /// Initialises the boardData object and sets path for custom board data by the user, then verifies the existence of the path before the application starts.
    /// </summary>
    private void Awake()
    {
        boardData = gameObject.AddComponent<BoardData>();
        customBoardDataPath = $"{CUSTOM_GAME_DATA_DIRECTORY}/{BOARD_DATA_FILENAME}";
        VerifyCustomGameFiles();
        SetBoardData();
    }

    /// <summary>
    /// Scans application directory, checking if the CustomGameData folder exists, along with user-editable file PropertyTycoonBoardData.json
    /// In the event either of these do not exist (i.e. user deleted folder, fresh start etc), the folder & file is created with default values
    /// </summary>
    void VerifyCustomGameFiles()
    {
        try
        {
            if (Directory.Exists(CUSTOM_GAME_DATA_DIRECTORY))
            {
                if (!File.Exists(customBoardDataPath))
                {
                    Debug.Log("Custom Data Directory Exists, but corresponding BoardData file does not.\nCreating board data file from defaults..");
                    File.Copy($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}", customBoardDataPath);
                }
            }
            else
            {
                Debug.Log("Custom Data Directory does not exist.\nCreating custom data directory w/ board data file from defaults");
                Directory.CreateDirectory(CUSTOM_GAME_DATA_DIRECTORY);
                File.Copy($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}", customBoardDataPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to Read / Write: {e}");
            Application.Quit();
        }
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
        }
        catch (Exception e)
        {
            Debug.LogError($"Failure processing custom board data, reverting to default values..\nError: {e}");
            RevertToDefaults();
        }
        Debug.Log(JsonUtility.ToJson(boardData, true));

    }

    /// <summary>
    /// Generates <see cref="BoardData"/> off of default values, in event of an invalid PropertyTycoonBoardData.json file
    /// </summary>
    void RevertToDefaults()
    {
        try
        {
            string data = File.ReadAllText($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}");
            JsonUtility.FromJsonOverwrite(data, boardData);
            
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed reverting to factory settings: {e}");
        }

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
