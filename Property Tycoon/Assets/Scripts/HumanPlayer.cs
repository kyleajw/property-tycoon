using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Script for human-specific methods.
/// Mainly handles UI components away from the main functionality of the <see cref="Player"/> class
/// </summary>
public class HumanPlayer : MonoBehaviour
{
    Player player;
    [SerializeField]GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponent<Player>();
    }

    private void Update()
    {
        if (player.IsPlayersTurn() && !canvas.activeInHierarchy && !player.IsPlayerMoving() && !player.HasPlayerThrown())
        {
            canvas.SetActive(true);
        }
        else if (!player.IsPlayersTurn() && canvas.activeInHierarchy)
        {
            canvas.SetActive(false);
        }
    }

    public void SetPlayerRollMultiplier(float multiplier)
    {
        player.RollDice(multiplier * 5);
        
    }
}
