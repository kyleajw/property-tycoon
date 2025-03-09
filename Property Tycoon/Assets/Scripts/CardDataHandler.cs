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

    bool ScanForNullCardData(CardData[] cards)
    {
        if(cards == null || cards.Length == 0)
        {
            return true;
        }
        foreach (CardData card in cards)
        {
            if (card.action == null || card.description == null) return true;
        }
        return false;
    }

}
