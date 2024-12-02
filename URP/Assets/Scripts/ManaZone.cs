using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ManaZone : MonoBehaviour
{
    // Same thing as heal zone, just mana.

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.maxMana > controller.mana)
            {
                controller.ChangeMana(1);
        }
    }
}
