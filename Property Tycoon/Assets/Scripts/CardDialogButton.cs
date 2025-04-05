using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDialogButton : MonoBehaviour
{
    int choice;
    string text;

    public void SetCorrespondingChoice(int n)
    {
        choice = n;
    }

    public void SetButtonText(string s)
    {
        text = s;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void Close()
    {
        gameObject.GetComponentInParent<CardDialog>().Close(choice);
    }
}
