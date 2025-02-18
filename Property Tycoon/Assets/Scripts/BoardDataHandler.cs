using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BoardDataHandler : MonoBehaviour
{
    BoardData boardData;
    const string CUSTOM_GAME_DATA_DIRECTORY = "CustomGameData";
    const string BOARD_DATA_FILENAME = "PropertyTycoonBoardData.json";
    string customBoardDataPath;


    private void Awake()
    {
        boardData = gameObject.AddComponent<BoardData>();
        customBoardDataPath = $"{CUSTOM_GAME_DATA_DIRECTORY}/{BOARD_DATA_FILENAME}";
        VerifyCustomGameFiles();
        SetBoardData();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

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

    // reads board data json file,
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

    // creates board data from streamingAssets, default json file, in case of errors
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
}
