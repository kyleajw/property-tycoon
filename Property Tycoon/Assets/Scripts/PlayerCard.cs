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

    /// <summary>
    /// Removes this player from the lobby
    /// </summary>
    public void Remove()
    {
        gameObject.GetComponentInParent<PlayersList>().RemovePlayerCard(playerNumber, playerColour, gamePiece);
        Destroy(this.gameObject);
    }
    /// <summary>
    /// Sets the colour of the card to the colour parsed in
    /// </summary>
    /// <param name="colour">Given colour</param>
    public void SetColours(Color colour)
    {
        playerColour = colour;
        playerCardBackground.color = colour;
        playerCharacterBorder.color = colour;
    }

    /// <summary>
    /// Sets the game piece of the card to the piece parsed in
    /// </summary>
    /// <param name="selectedPiece">Given piece</param>
    public void SetAndDisplayChosenGamePiece(GameObject selectedPiece)
    {
        gamePiece = selectedPiece;
        Piece piece = selectedPiece.GetComponent<Piece>();
        pieceName.text = piece.GetName();
        playerPieceImage.sprite = piece.GetSprite();

    }
    /// <summary>
    /// Sets the players player number
    /// </summary>
    /// <param name="playerNumber">Given player number</param>
    public void SetPlayerNumber(int playerNumber)
    {
        nameText.text = $"Player {playerNumber}";
        this.playerNumber = playerNumber;
    }
    /// <summary>
    /// Changes the player piece to the next piece in the lobby queue
    /// </summary>
    public void ChangeCharacter()
    {
        SetAndDisplayChosenGamePiece(gameObject.GetComponentInParent<PlayersList>().ChangePlayerCharacter(gamePiece));
    }
    /// <summary>
    /// Changes the player colour to the next colour in the lobby queue
    /// </summary>
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

