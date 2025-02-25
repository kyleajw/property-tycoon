using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] HumanPlayer player;

    public HumanPlayer GetCanvasOwner()
    {
        return this.player;
    }

}
