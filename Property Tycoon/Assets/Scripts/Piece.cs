using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] string pieceName;

    public Sprite GetSprite()
    {
        return sprite;
    }

    public string GetName()
    {
        return pieceName;
    }
}
