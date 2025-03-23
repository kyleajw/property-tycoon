using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCard : MonoBehaviour
{
    PropertiesTab propertiesTab;
    PlayerManager playerManager;
    // Start is called before the first frame update
    void Start()
    {
        propertiesTab = GetComponentInParent<PropertiesTab>();
    }

    public void OnSelected(Property property)
    {
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
        if (property.isMortgaged)
        {
            playerManager.mortgageButton.SetActive(false);
            playerManager.sellButton.SetActive(false);
            playerManager.buildHouseButton.SetActive(false);
            playerManager.removeHouseButton.SetActive(false);
        }
        else
        {
            playerManager.mortgageButton.SetActive(true);
            playerManager.sellButton.SetActive(true);
            playerManager.buildHouseButton.SetActive(true);
            playerManager.removeHouseButton.SetActive(true);
        }
        propertiesTab.SetCardOnDisplay(property);
        playerManager.UpdateHouseText(property);
    }
}
