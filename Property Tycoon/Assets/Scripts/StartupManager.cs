using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StartupManager : MonoBehaviour
{
    const string CUSTOM_GAME_DATA_DIRECTORY = "CustomGameData";
    const string BOARD_DATA_FILENAME = "PropertyTycoonBoardData.json";
    const string POT_LUCK_CARDS_FILENAME = "PropertyTycoonPotLuckData.json";
    const string OPPORTUNITY_KNOCKS_CARDS_FILENAME = "PropertyTycoonOpportunityKnocksData.json";


    const string CUSTOM_GAME_BOARD_DATA_PATH = CUSTOM_GAME_DATA_DIRECTORY + "/" + BOARD_DATA_FILENAME;
    const string CUSTOM_POT_LUCK_CARDS_PATH = CUSTOM_GAME_DATA_DIRECTORY + "/" + POT_LUCK_CARDS_FILENAME;
    const string CUSTOM_OPPORTUNITY_KNOCKS_CARDS_PATH = CUSTOM_GAME_DATA_DIRECTORY + "/" + OPPORTUNITY_KNOCKS_CARDS_FILENAME;


    private static StartupManager _instance;
    public static StartupManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

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
                    Debug.Log("Custom Data Directory Exists, but corresponding BoardData file does not.\nCreating boardData file from defaults..");
                    File.Copy($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}", CUSTOM_GAME_BOARD_DATA_PATH);
                }
                if (!File.Exists(CUSTOM_OPPORTUNITY_KNOCKS_CARDS_PATH))
                {
                    Debug.Log("Custom Data Directory Exists, but corresponding Opportunity Knocks file does not.\nCreating Opportunity Knocks file from defaults..");
                    File.Copy($"{Application.streamingAssetsPath}/{OPPORTUNITY_KNOCKS_CARDS_FILENAME}", CUSTOM_OPPORTUNITY_KNOCKS_CARDS_PATH);
                }
                if (!File.Exists(CUSTOM_POT_LUCK_CARDS_PATH))
                {
                    Debug.Log("Custom Data Directory Exists, but corresponding Pot Luck file does not.\nCreating Pot Luck file from defaults..");
                    File.Copy($"{Application.streamingAssetsPath}/{POT_LUCK_CARDS_FILENAME}", CUSTOM_POT_LUCK_CARDS_PATH);
                }
            }
            else
            {
                Debug.Log("Custom Data Directory does not exist.\nCreating custom data directory w/ all files from defaults");
                Directory.CreateDirectory(CUSTOM_GAME_DATA_DIRECTORY);
                CopyFromDefaults();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to Read / Write: {e}");
            Application.Quit();
        }
    }

    void CopyFromDefaults()
    {
        File.Copy($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}", CUSTOM_GAME_BOARD_DATA_PATH);
        File.Copy($"{Application.streamingAssetsPath}/{OPPORTUNITY_KNOCKS_CARDS_FILENAME}", CUSTOM_OPPORTUNITY_KNOCKS_CARDS_PATH);
        File.Copy($"{Application.streamingAssetsPath}/{POT_LUCK_CARDS_FILENAME}", CUSTOM_POT_LUCK_CARDS_PATH);

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

    public string GetPotLuckCardsFileName()
    {
        return POT_LUCK_CARDS_FILENAME;
    }

    public string GetOpportunityKnocksCardsFileName()
    {
        return OPPORTUNITY_KNOCKS_CARDS_FILENAME;
    }

    public string GetCustomPotLuckCardsPath()
    {
        return CUSTOM_POT_LUCK_CARDS_PATH;
    }

    public string GetCustomOpportunityKnocksCardsPath()
    {
        return CUSTOM_OPPORTUNITY_KNOCKS_CARDS_PATH;
    }
}
