using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    BoardDataHandler boardDataHandler;
    BoardData boardData;
    Player player;
    GameObject[] tiles;
    int i;

    private void Start()
    {
        boardDataHandler = GameObject.Find("GameDataManager").GetComponent<BoardDataHandler>();
        GenerateBoard();
    }
    public void GenerateBoard()
    {
        //Retrieves Board data from BoardDataHandler
        boardData=boardDataHandler.GetBoardData();
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        System.Array.Sort(tiles, (a, b) => { return a.GetComponent<Tile>().position.CompareTo(b.GetComponent<Tile>().position); });
        
        for (i = 0; i < boardData.tiles.Length; i++)
        {
            //Finds the tile in position i and assigns the appropriate data to it
            //Update Text Mesh Pros and colours here
            tiles[i].GetComponent<Tile>().tileData = boardData.tiles[i];
            string group = tiles[i].GetComponent<Tile>().tileData.group;
            if (group != "Unique")
            {
                    tiles[i].GetComponent<Tile>().UpdateNameText();
                if (tiles[i].GetComponent<Tile>().tileData.purchasable)
                {

                    tiles[i].GetComponent<Tile>().UpdatePriceText();
                }
                if (group != "Station" && group != "Utilities" && group != "Tax" && group != "Opportunity Knocks" && group != "Pot Luck")
                {
                    tiles[i].GetComponent<Tile>().UpdateColor();
                }
            }

        }
         
    }

    public GameObject[] GetTileArray()
    {
        return tiles;
    }
}
