using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject player;
    public float speed;

    Animator animator;

    public bool passive = true;
    private float distance;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }


    // FixedUpdate has the same call rate as the physics system
    void Update()
    {
        //Turns the boss off to start with
        if (passive) 
        {
            return; 
        }

        distance = Vector2.Distance(transform.position, player.transform.position);

        //If the player is too far away, the boss won't chase them, which helps because otherwise he clips all the way back to spawn.
        if ( distance > 20f )
        {
            return;
        }

        animator.enabled = !animator.enabled;

        //All of this makes the boss animatedly chase the player.
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();

        transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);

        animator.SetFloat("Move X", direction.x);
        animator.SetFloat("Move Y", direction.y);
    } 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (passive)
        {
            return;
        }

        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-2);
            
        }
    }

    //Turns the boss on. Public because the Ultra Wrench's code calls it.
    public void Awaken() 
    { 
        audioSource.Play();
        passive = false;
    }

}