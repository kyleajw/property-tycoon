using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    GameObject physicalCharacter;
    private void Start()
    {
        physicalCharacter = Instantiate(gameObject.GetComponent<Player>().GetPlayerPiece(), this.transform);
    }
}
