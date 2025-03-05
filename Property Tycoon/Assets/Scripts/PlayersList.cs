using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersList : MonoBehaviour
{
    [SerializeField] GameObject playerCardPrefab;
    [SerializeField] Transform addPlayerButton;
    [SerializeField] Button beginGameButton;
    [SerializeField] GameObject[] gamePieces;

    Queue<Color> defaultPlayerColours = new Queue<Color> ();
    Queue<GameObject> availablePieces = new Queue<GameObject>();
    List<GameObject> playerCards;
    const int MAX_PLAYERS = 5;
    const int MIN_PLAYERS = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerCards = new List<GameObject>();
        defaultPlayerColours.Enqueue(new Color32(255, 130, 130, 255));  // red
        defaultPlayerColours.Enqueue(new Color32(130, 155, 255, 255));  // blue
        defaultPlayerColours.Enqueue(new Color32(234, 250, 117, 255));  // yellow
        defaultPlayerColours.Enqueue(new Color32(130, 255, 138, 255));  // green
        defaultPlayerColours.Enqueue(new Color32(110, 82, 236, 255));   // purple
        defaultPlayerColours.Enqueue(new Color32(107, 76, 62, 255));    // brown
        foreach (GameObject piece in gamePieces)
        {
            availablePieces.Enqueue(piece);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(addPlayerButton.gameObject.activeInHierarchy && playerCards.Count >= MAX_PLAYERS)
        {
            addPlayerButton.gameObject.SetActive(false);
        }else if (!addPlayerButton.gameObject.activeInHierarchy && playerCards.Count < MAX_PLAYERS)
        {
            addPlayerButton.gameObject.SetActive(true);
        }

        if (playerCards.Count < MIN_PLAYERS && beginGameButton.IsInteractable())
        {
            beginGameButton.interactable = false;
        }
        else if (playerCards.Count >= MIN_PLAYERS && !beginGameButton.IsInteractable())
        {
            beginGameButton.interactable = true;
        }
    }

    public void AddPlayerCard()
    {
        Color playerColour = defaultPlayerColours.Dequeue();
        GameObject playerPiece = availablePieces.Dequeue();
        GameObject newPlayer = Instantiate(playerCardPrefab,transform.position, Quaternion.identity, this.gameObject.transform);
        PlayerCard playerCard = newPlayer.GetComponent<PlayerCard>();
        playerCards.Add(newPlayer);
        playerCard.SetAndDisplayChosenGamePiece(playerPiece);
        playerCard.SetPlayerNumber(playerCards.Count);
        playerCard.SetColours(playerColour);
        addPlayerButton.SetAsLastSibling();
    }

    public void RemovePlayerCard(int index, Color availableColour, GameObject gamePiece)
    {
        playerCards.RemoveAt(index-1);
        defaultPlayerColours.Enqueue(availableColour);
        availablePieces.Enqueue(gamePiece);
        UpdatePlayerNumbers();
    }

    void UpdatePlayerNumbers()
    {
        for (int i = 0; i < playerCards.Count; i++)
        {
            playerCards[i].GetComponent<PlayerCard>().SetPlayerNumber(i+1);
        }
    }

    public GameObject ChangePlayerCharacter(GameObject currentPiece)
    {
        availablePieces.Enqueue(currentPiece);
        return availablePieces.Dequeue();
    }

    public Color ChangePlayerColour(Color currentColor)
    {
        defaultPlayerColours.Enqueue(currentColor);
        return defaultPlayerColours.Dequeue();
    }

}
