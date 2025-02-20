using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Tools for making debugging the BoardDataHandler easier
/// </summary>
public class MenuItems : MonoBehaviour
{
    [MenuItem("Tools/CustomGameData/Delete CustomGameData Folder", true)]
    private static bool CheckCustomGameDataFolderExists()
    {
        return Directory.Exists("CustomGameData");
    }

    /// <summary>
    /// Deletes the CustomGameData folder, and contents within it
    /// </summary>
    [MenuItem("Tools/CustomGameData/Delete CustomGameData Folder")]
    private static void ClearCustomGameData() 
    { 
        if (File.Exists("CustomGameData/PropertyTycoonBoardData.json")) { File.Delete("CustomGameData/PropertyTycoonBoardData.json"); }
        Directory.Delete("CustomGameData");
    }

    [MenuItem("Tools/CustomGameData/Clear Custom Board Data", true)]
    private static bool CheckCustomBoardDataExists()
    {
        return Directory.Exists("CustomGameData") && File.Exists("CustomGameData/PropertyTycoonBoardData.json");
    }

    /// <summary>
    /// Deletes PropertyTycoonBoardData.json found within directory CustomGameData
    /// </summary>
    [MenuItem("Tools/CustomGameData/Clear Custom Board Data")]
    private static void ClearCustomBoardData()
    {
        File.Delete("CustomGameData/PropertyTycoonBoardData.json");
    }

    /// <summary>
    /// Deletes and replaces PropertyTycoonBoardData.json with values found from the defaults file, located in the StreamingAssets folder.
    /// If the CustomGameData folder does not exist when this tool is used, it is created first, then creates the PropertyTycoonBoardData.json file within it.
    /// </summary>
    [MenuItem("Tools/CustomGameData/Reset to defaults")]
    private static void ResetCustomData()
    {
        if (Directory.Exists("CustomGameData"))
        {
            if (File.Exists("CustomGameData/PropertyTycoonBoardData.json"))
            {
                File.Delete("CustomGameData/PropertyTycoonBoardData.json");
            }
        }
        else
        {
            Directory.CreateDirectory("CustomGameData");
        }
        File.Copy($"{Application.streamingAssetsPath}/PropertyTycoonBoardData.json", "CustomGameData/PropertyTycoonBoardData.json");

    }

}
