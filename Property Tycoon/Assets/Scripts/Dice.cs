using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Dice : MonoBehaviour
{
    enum Status
    {
        Standby,
        Rolling,
        Rolled
    }
    Status status = Status.Standby;
    [SerializeField] int standardForce = 15;
    [SerializeField] int forceNoiseUpperLimit = 4;
    [SerializeField] int forceNoiseLowerLimit = 2;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (status == Status.Rolling && rb.velocity == Vector3.zero && rb.angularVelocity == Vector3.zero)
        {
            status = Status.Rolled;
        }
        else if ((rb.velocity != Vector3.zero || rb.angularVelocity != Vector3.zero) && status == Status.Rolled)
        {
            Debug.Log("nudged");
            status = Status.Rolling;
        }
        
        if (status == Status.Rolled)
        {
            
            Debug.Log($"{gameObject.name} Rolled: {GetSideFacingUp()}");

        }


    }

    /// <summary>
    /// Adds a forward and rotational force on the dice based on a constant value * random noise * a multiplier (how long the user held down the roll button). 
    /// Sets status of the dice to 'Rolling'
    /// </summary>
    public void Roll(float multiplier)
    {
        System.Random rnd = new();
        float rX = (float)rnd.NextDouble() * rnd.Next(forceNoiseLowerLimit, forceNoiseUpperLimit);
        float rY = (float)rnd.NextDouble() * rnd.Next(forceNoiseLowerLimit, forceNoiseUpperLimit);
        float rZ = (float)rnd.NextDouble() * rnd.Next(forceNoiseLowerLimit, forceNoiseUpperLimit);

        Vector3 force = rnd.Next(forceNoiseLowerLimit, forceNoiseUpperLimit) * multiplier * transform.forward * standardForce;
        Vector3 rotationForce = new Vector3(rX,rY,rZ) * multiplier * standardForce;

        rb.AddForce(force);
        rb.AddTorque(rotationForce);

        status = Status.Rolling;
        Debug.Log($"{gameObject.name} Rolling...");
    }

    /// <summary>
    /// Calculates which face on the die is facing upwards by comparing the angle between the face and Vector3(0,1,0) (representing up) for every face.
    /// </summary>
    /// <returns>Integer value on the face of the dice facing upwards (1 - 6)</returns>
    public int GetSideFacingUp()
    {
        Vector3[] sides = new Vector3[] { 
            -transform.up, transform.right,         // 1 | 2
            -transform.forward, -transform.right,   // 3 | 4
            transform.forward, transform.up };      // 5 | 6
        float highestAngle = 0;
        int upFacingSide = -1;
        for (int i = 0; i < sides.Length; i++)
        {
            float angle = Vector3.Angle(sides[i], Vector3.up);
            if (angle > highestAngle)
            {
                highestAngle = angle;
                upFacingSide = i + 1;
            }
        }
        status = Status.Standby;
        return upFacingSide;
    }

    public bool isRolling()
    {
        return status == Status.Rolling;
    }
}
