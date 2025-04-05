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
    GameObject gamePiece;

    [SerializeField] Image playerCardBackground;
    [SerializeField] Image playerCharacterBorder;
    [SerializeField] Image playerPieceImage;
    [SerializeField] TextMeshProUGUI pieceName;
    [SerializeField] Toggle humanToggle;

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
        gameObject.GetComponentInParent<PlayersList>().RemovePlayerCard(playerNumber, playerColour, gamePiece);
        Destroy(this.gameObject);
    }

    public void SetColours(Color colour)
    {
        playerColour = colour;
        playerCardBackground.color = colour;
        playerCharacterBorder.color = colour;
    }


    public void SetAndDisplayChosenGamePiece(GameObject selectedPiece)
    {
        gamePiece = selectedPiece;
        Piece piece = selectedPiece.GetComponent<Piece>();
        pieceName.text = piece.GetName();
        playerPieceImage.sprite = piece.GetSprite();

    }

    public void SetPlayerNumber(int playerNumber)
    {
        nameText.text = $"Player {playerNumber}";
        this.playerNumber = playerNumber;
    }

    public void ChangeCharacter()
    {
        SetAndDisplayChosenGamePiece(gameObject.GetComponentInParent<PlayersList>().ChangePlayerCharacter(gamePiece));
    }

    public void ChangeColour()
    {
        SetColours(gameObject.GetComponentInParent<PlayersList>().ChangePlayerColour(playerColour));
    }

    public GameObject GetPlayerPiece()
    {
        return gamePiece;
    }

    public Color GetPlayerColour()
    {
        return playerColour;
    }

    public int GetPlayerNumber()
    {
        return playerNumber;
    }

    public string GetPlayerName()
    {
        return $"Player {playerNumber}";
    }

    public bool GetIsHuman()
    {
        return !humanToggle.isOn;
    }
}

