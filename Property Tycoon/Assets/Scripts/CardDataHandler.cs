using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardDataHandler : MonoBehaviour
{
    StartupManager startupManager;

    OpportunityKnocksCardCollection opportunityKnocks;
    PotLuckCardCollection potLuck;

    string potLuckCollectionFileName;
    string opportunityKnocksCollectionFileName;

    string customPotLuckPath;
    string customOpportunityKnocksPath;

    private void Awake()
    {
        opportunityKnocks = gameObject.AddComponent<OpportunityKnocksCardCollection>();
        potLuck = gameObject.AddComponent<PotLuckCardCollection>();
        startupManager = GetComponent<StartupManager>();
        opportunityKnocksCollectionFileName = startupManager.GetOpportunityKnocksCardsFileName();
        potLuckCollectionFileName = startupManager.GetPotLuckCardsFileName();
        customPotLuckPath = startupManager.GetCustomPotLuckCardsPath();
        customOpportunityKnocksPath = startupManager.GetCustomOpportunityKnocksCardsPath();

        SetPotLuckData();
        SetOpportunityKnocksData();
        
    }
    /// <summary>
    /// Reads in the pot luck data from the corresponding JSON file
    /// </summary>
    void SetPotLuckData()
    {
        try
        {
            string data = File.ReadAllText(customPotLuckPath);
            JsonUtility.FromJsonOverwrite(data, potLuck);
            if (ScanForNullCardData(potLuck.cards))
            {
                RevertToPotLuckDefaults();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failure processing custom pot luck data, reverting to default values..\nError: {e}");
            RevertToPotLuckDefaults();
        }
        Debug.Log($"potLuck values (JSON format):\n{JsonUtility.ToJson(potLuck, true)}");
    }
    /// <summary>
    /// Reads in the opportunity knocks data from the corresponding JSON file
    /// </summary>
    void SetOpportunityKnocksData()
    {
        try
        {
            string data = File.ReadAllText(customOpportunityKnocksPath);
            JsonUtility.FromJsonOverwrite(data, opportunityKnocks);
            if (ScanForNullCardData(opportunityKnocks.cards))
            {
                RevertToOpportunityKnocksDefaults();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failure processing custom oportunity knocks data, reverting to default values..\nError: {e}");
            RevertToOpportunityKnocksDefaults();
        }
        Debug.Log($"Opportunity Knocks values (JSON format):\n{JsonUtility.ToJson(opportunityKnocks, true)}");
    }
    /// <summary>
    /// In case of error when using custom user data, use the default values in the streamingAssets path
    /// </summary>
    void RevertToPotLuckDefaults()
    {
        Debug.LogError("Invalid Custom Data, reverting to default values..");
        try
        {
            string data = File.ReadAllText($"{Application.streamingAssetsPath}/{potLuckCollectionFileName}");
            JsonUtility.FromJsonOverwrite(data, potLuck);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed reverting to factory settings: {e}");
        }

    }
    /// <summary>
    /// In case of error when using custom user data, use the default values in the streamingAssets path
    /// </summary>
    void RevertToOpportunityKnocksDefaults()
    {
        Debug.LogError("Invalid Custom Data, reverting to default values..");
        try
        {
            string data = File.ReadAllText($"{Application.streamingAssetsPath}/{opportunityKnocksCollectionFileName}");
            JsonUtility.FromJsonOverwrite(data, opportunityKnocks);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed reverting to factory settings: {e}");
        }
    }
    /// <summary>
    /// checks for any null values in the card array
    /// </summary>
    /// <param name="cards">Array of card data read in from the json file</param>
    /// <returns></returns>
    bool ScanForNullCardData(CardData[] cards)
    {
        if(cards == null || cards.Length == 0)
        {
            return true;
        }
        foreach (CardData card in cards)
        {
            if (card.action == null || card.description == null || card.arg == null) return true;
        }
        return false;
    }

}
