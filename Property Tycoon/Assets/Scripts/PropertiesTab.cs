using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesTab : MonoBehaviour
{
    [SerializeField] Property largeCard;
    public void SetCardOnDisplay(Property card)
    {
        largeCard.SetAssociatedTile(card.GetAssociatedTile());
        largeCard.SetOwnedBy(card.GetOwnedBy());
    }
}
