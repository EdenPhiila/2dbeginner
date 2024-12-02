using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WrenchCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.meleeAction.Enable();
            Destroy(gameObject);

            controller.PlaySound(collectedClip);
        }
    }

}