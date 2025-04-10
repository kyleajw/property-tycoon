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
    /// <summary>
    /// Split the choices (if there are two) into 2 buttons, else instantiate an OK button
    /// </summary>
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
    /// <summary>
    /// Creates a new button and adds it to the card dialog.
    /// </summary>
    /// <param name="buttonText">Message in the button</param>
    /// <param name="choice">Associated choice(1 if only one button)</param>
    void InstantiateButton(string buttonText, int choice)
    {
            GameObject newButton = Instantiate(cardDialogButtonPrefab, buttonLayoutGroup.transform);
            CardDialogButton cardDialogButton = newButton.GetComponent<CardDialogButton>();
            cardDialogButton.SetCorrespondingChoice(choice);
            cardDialogButton.SetButtonText(buttonText);
    }
    /// <summary>
    /// Sets the title and description of the card pop-up
    /// </summary>
    void SetText()
    {
        titleText.text = type;
        descriptionText.text = description;
    }
    /// <summary>
    /// Closes the card pop-up and processes the action in the <see cref="PlayerManager"/>, then destroys itself
    /// </summary>
    /// <param name="choice"></param>
    public void Close(int choice)
    {
        GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>().OnPlayerClosesCardDialog(card, choice);
        Destroy(gameObject);
    }
}
