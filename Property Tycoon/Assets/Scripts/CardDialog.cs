using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDialog : MonoBehaviour
{
    string type;
    string description;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject buttonLayoutGroup;
    [SerializeField] GameObject cardDialogButtonPrefab;
    CardData card;

    public void UpdateCardInfo(string type, CardData card)
    {
        this.type = type;
        description = card.description;
        this.card = card;
        SetButtons();
        SetText();
    }

    void SetButtons()
    {
        string[] args = card.arg.Split(" ");
        string action = args[0];
        if(action == "CHOICE:")
        {
            string s = card.arg.Substring(action.Length);
            string[] decisions = s.Split(" OR ");
            InstantiateButton(decisions[0], 1);
            InstantiateButton(decisions[1], 2);
        }
        else
        {
            InstantiateButton("OK", 1);
        }
    }

    void InstantiateButton(string buttonText, int choice)
    {
            GameObject newButton = Instantiate(cardDialogButtonPrefab, buttonLayoutGroup.transform);
            CardDialogButton cardDialogButton = newButton.GetComponent<CardDialogButton>();
            cardDialogButton.SetCorrespondingChoice(choice);
            cardDialogButton.SetButtonText(buttonText);
    }

    void SetText()
    {
        titleText.text = type;
        descriptionText.text = description;
    }

    public void Close(int choice)
    {
        GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>().OnPlayerClosesCardDialog(card, choice);
        Destroy(gameObject);
    }
}
