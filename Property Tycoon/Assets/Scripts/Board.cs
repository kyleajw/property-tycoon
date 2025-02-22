using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    BoardDataHandler boardDataHandler;
    BoardData boardData;
    GameObject[] tiles;
    int i;
    private void Start()
    {
        boardDataHandler = GameObject.Find("DataHandl").GetComponent<BoardDataHandler>();
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
            tiles[i].GetComponent<Tile>().tileData = boardData.tiles[i];
            //Update Text Mesh Pros and colours here
            if (tiles[i].GetComponent<Tile>().tileData.group != "Unique")
            {
                tiles[i].GetComponent<Tile>().UpdateText();
                if (tiles[i].GetComponent<Tile>().tileData.group != "Station" && tiles[i].GetComponent<Tile>().tileData.group != "Utilities")
                {
                    tiles[i].GetComponent<Tile>().UpdateColor();
                }
            }
        }
         
    }
}
