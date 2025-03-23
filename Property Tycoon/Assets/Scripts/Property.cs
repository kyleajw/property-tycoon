using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Property : MonoBehaviour
{
    GameObject ownedBy;
    public bool isMortgaged=false;
    int houses = 0;
    public TextMeshProUGUI rentText,nameText;
    [SerializeField] Image cardColour;
    public Tile tile;

    public GameObject GetOwnedBy(){ 
        return ownedBy;
    }
    public int GetHouseCount()
    {
        return houses;
    }
    public void IncrementHouseCount()
    {
        houses+=1;
    }
    public void DecrementHouseCount()
    {
        houses -= 1;
    }
    public void SetOwnedBy(GameObject player)
    {
        ownedBy = player;
    }

    public void SetAssociatedTile(Tile data)
    {
        tile = data;
        nameText.text = tile.tileData.spaceName;
        cardColour.color = tile.GetColor();
        UpdateRentPrice();
    }
    public Tile GetAssociatedTile()
    {
        return tile;
    }

    void UpdateRentPrice()
    {
        if (tile.tileData.rentPrices == null || tile.tileData.rentPrices.Length == 0)
        {
            rentText.text = $"£{tile.tileData.purchaseCost}";
        }
        else
        {
            rentText.text = $"£{tile.tileData.rentPrices[houses]}";

        }
    }

    public Color GetCardColour()
    {
        return tile.GetColor();
    }
    public int GetDisplayedRent()
    {
        return tile.tileData.rentPrices[houses];
    }
}
