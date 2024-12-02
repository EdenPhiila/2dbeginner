using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UltraGear : MonoBehaviour
{
    public AudioClip collectedClip;
    public AudioClip robotAwaken;

    void OnTriggerEnter2D(Collider2D other)
    {
        BossController boss = GameObject.FindWithTag("Boss").GetComponent<BossController>();
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.score >= controller.robots)
        {
            boss.Awaken();
            controller.ultraGear = true;
            Destroy(gameObject);
            controller.PlaySound(collectedClip);
            controller.PlaySound(robotAwaken);
        }
    }

}