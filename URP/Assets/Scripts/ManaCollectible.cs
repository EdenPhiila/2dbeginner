using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ManaCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.maxMana > controller.mana)
        {
            controller.ChangeMana(1);
            Destroy(gameObject);

            controller.PlaySound(collectedClip);
        }
    }

}