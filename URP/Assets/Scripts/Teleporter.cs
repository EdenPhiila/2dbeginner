using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public GameObject teleporterPrefab;
    public ParticleSystem spellEffect;
    public Vector2 warpLocation;

    void Start() 
    {
        Rigidbody2D controller = teleporterPrefab.GetComponent<Rigidbody2D>();
        warpLocation = controller.position;
        warpLocation.y = warpLocation.y - 1;
    }
}
