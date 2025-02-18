using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BoardDataHandler : MonoBehaviour
{
    Board board = new();
    const string CUSTOM_GAME_DATA_DIRECTORY = "CustomGameData";
    const string BOARD_DATA_FILENAME = "PropertyTycoonBoardData.json";

    //if error occurs (e.g. user data inputted is wrong, default to json file from StreamingAssets)

    // Start is called before the first frame update
    void Start()
    {
        VerifyCustomGameFiles();
    }

    void VerifyCustomGameFiles()
    {
        try
        {
            if (Directory.Exists(CUSTOM_GAME_DATA_DIRECTORY))
            {
                Debug.Log("custom folder exists");
                if (File.Exists($"{CUSTOM_GAME_DATA_DIRECTORY}/{BOARD_DATA_FILENAME}"))
                {
                    ReadBoardData();
                }
                else
                {
                    File.Copy($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}", $"{CUSTOM_GAME_DATA_DIRECTORY}/{BOARD_DATA_FILENAME}");
                    ReadBoardData();

                }
            }
            else
            {
                Directory.CreateDirectory(CUSTOM_GAME_DATA_DIRECTORY);
                File.Copy($"{Application.streamingAssetsPath}/{BOARD_DATA_FILENAME}", $"{CUSTOM_GAME_DATA_DIRECTORY}/{BOARD_DATA_FILENAME}");
                ReadBoardData();

            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to Read / Write: {e.ToString()}");
            Application.Quit();
        }
    }

    void ReadBoardData()
    {
        try
        {
            string data = File.ReadAllText($"{CUSTOM_GAME_DATA_DIRECTORY}/{BOARD_DATA_FILENAME}");
            JsonUtility.FromJsonOverwrite(data, board);
            Debug.Log(JsonUtility.ToJson(board, true));
        }
        catch (Exception e)
        {

            throw;
        }
    }
}
