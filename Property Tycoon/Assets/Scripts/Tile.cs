using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    enum TileType
    {
        Square, Normal, Unique
    };

    enum Group
    {
        Blue, Brown, Purple,  Station, Utilities, Special
    };

    enum Action { Pay, Collect, Take}

    bool isPurchasable;
    string propertyName;
    TileType tileType;
    Group tileGroup;
    Action tileAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTileData(string name, bool purchasable, int type, int group, int action)
    {
        propertyName = name;
        isPurchasable = purchasable;
        tileType = (TileType)type;
        tileGroup = (Group)group;
        tileAction = (Action)action;

    }

    void ApplyDataVisually()
    {

    }
}
