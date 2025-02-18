using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StartupManager : MonoBehaviour
{
    const string CUSTOM_GAME_DATA_DIRECTORY = "CustomGameData";
    const string BOARD_DATA_FILENAME = "PropertyTycoonBoardData.json";
    string CUSTOM_GAME_BOARD_DATA_PATH = CUSTOM_GAME_DATA_DIRECTORY + BOARD_DATA_FILENAME;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        VerifyCustomGameFiles();
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
                if (!File.Exists(CUSTOM_GAME_BOARD_DATA_PATH))
                {
                    Debug.Log("Custom Data Directory Exists, but corresponding BoardData file does not.\nCreating board data file from defaults..");
                    File.Copy($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}", CUSTOM_GAME_BOARD_DATA_PATH);
                }
            }
            else
            {
                Debug.Log("Custom Data Directory does not exist.\nCreating custom data directory w/ board data file from defaults");
                Directory.CreateDirectory(CUSTOM_GAME_DATA_DIRECTORY);
                File.Copy($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}", CUSTOM_GAME_BOARD_DATA_PATH);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to Read / Write: {e}");
            Application.Quit();
        }
    }

    public string GetCustomGameDataDirectory()
    {
        return CUSTOM_GAME_DATA_DIRECTORY;
    }

    public string GetCustomBoardDataFileName()
    {
        return BOARD_DATA_FILENAME;
    }

    public string GetCustomBoardDataPath()
    {
        return CUSTOM_GAME_BOARD_DATA_PATH;
    }

}
