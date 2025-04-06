using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    GameObject target;
    int forwardOffset = 4;
    float heightOffset = 3.5f;

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
        Vector3 position = target.transform.position;
        Vector3 offset;
        if (position.x <= 0 && position.z == 0)
        {
            offset=  new Vector3(0, heightOffset, -forwardOffset);
        }
        else if (position.x == -11 && position.z >= 0)
        {
            offset = new Vector3(-forwardOffset, heightOffset, 0);
        }else if(position.x <= 0 && position.z == 11)
        {
            offset = new Vector3(0, heightOffset, forwardOffset);
        }
        else
        {
            offset = new Vector3(forwardOffset, heightOffset, 0);
        }
        transform.position = target.transform.position + offset;
        transform.LookAt(target.transform.position);
    }

    public void SetTarget(GameObject player)
    {
        target = player;
    }
}
