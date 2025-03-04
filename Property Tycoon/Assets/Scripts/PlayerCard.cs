using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI nameText;
    int playerNumber;
    Color playerColour;
    GameObject playerGameCharacter;

    // Start is called before the first frame update
    void Start()
    {
     
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Remove()
    {
        gameObject.GetComponentInParent<PlayersList>().RemovePlayerCard(playerNumber);
        Destroy(this.gameObject);
    }

    public void SetColours(Color colour)
    {

    }


    public void SetAndDisplayChosenGamePiece()
    {

    }

    public void SetPlayerNumber(int playerNumber)
    {
        nameText.text = $"Player {playerNumber}";
        this.playerNumber = playerNumber;
    }
}

