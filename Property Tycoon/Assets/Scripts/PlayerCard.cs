using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI nameText;
    int playerNumber;
    Color playerColour;
    GameObject playerGameCharacter;

    [SerializeField] Image playerCardBackground;
    [SerializeField] Image playerCharacterBorder;

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
        gameObject.GetComponentInParent<PlayersList>().RemovePlayerCard(playerNumber, playerColour);
        Destroy(this.gameObject);
    }

    public void SetColours(Color colour)
    {
        playerColour = colour;
        playerCardBackground.color = colour;
        playerCharacterBorder.color = colour;
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

