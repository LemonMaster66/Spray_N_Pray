using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();

        if(playerMovement != null)
        {
            Completed completed = FindAnyObjectByType<Completed>();
            if(!completed.Win)
            {
                Debug.Log(transform.GetChild(0));
                playerMovement.gameObject.transform.position = transform.GetChild(0).transform.position;
            }
            else
            {
                Debug.Log(transform.GetChild(1));
                playerMovement.gameObject.transform.position = transform.GetChild(1).transform.position;
            }
            
        }
    }
}

