using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeathCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.maxHealth > controller.health)
        {
            controller.ChangeHealth(1);
            Destroy(gameObject);

            controller.PlaySound(collectedClip);
        }
    }

}