using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyAgent : MonoBehaviour
{
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("AI INSTANTIATED");
        player = gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMyTurn()
    {

    }
}
