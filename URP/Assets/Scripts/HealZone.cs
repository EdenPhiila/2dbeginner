using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealZone : MonoBehaviour
{

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.maxHealth > controller.health)
            {
                controller.ChangeHealth(1);
            }
    }
}
