using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    /// <summary>
    /// Stores relevant data in relation to corresponding tile / property / space
    /// </summary>
    public string spaceName;
    public string group;
    public string action;
    public bool purchasable;
    public int purchaseCost;
    public int[] rentPrices;
}
