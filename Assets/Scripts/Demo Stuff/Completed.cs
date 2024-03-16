using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Completed : MonoBehaviour
{
    public bool Win;

    void OnTriggerEnter(Collider collider)
    {
        PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();

        if(playerMovement != null)
        {
            Win = true;
            transform.position -= new Vector3(0, 100, 0);
        }
    }
}

