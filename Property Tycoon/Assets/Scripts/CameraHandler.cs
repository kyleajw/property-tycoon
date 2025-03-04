using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            FollowActivePlayer();

        }
    }

    void FollowActivePlayer()
    {
        //transform.LookAt(target.transform.position); //temporary
    }

    public void SetTarget(GameObject player)
    {
        target = player;
    }
}
