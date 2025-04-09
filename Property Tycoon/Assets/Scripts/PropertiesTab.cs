using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesTab : MonoBehaviour
{
    [SerializeField] Property largeCard;
    /// <summary>
    /// Displays the parsed card as a larger card in the property menu
    /// </summary>
    /// <param name="card"></param>
    public void SetCardOnDisplay(Property card)
    {
        largeCard.gameObject.SetActive(true);
        largeCard.SetAssociatedTile(card.GetAssociatedTile());
        largeCard.SetOwnedBy(card.GetOwnedBy());
    }
    public void SetBlank()
    {
        largeCard.gameObject.SetActive(false);
    }
    public Property GetCardOnDisplay()
    {
        return largeCard;
    }
}
