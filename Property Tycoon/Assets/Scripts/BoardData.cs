using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class BoardData : MonoBehaviour
{
    /// <summary>
    /// Stores an array of type <see cref="TileData"/>, containing information of all defined tiles
    /// </summary>
    public TileData[] tiles;
}
