using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Public variables
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    // Private variables
    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;

    bool aggressive = true;

    public ParticleSystem smokeEffect;

    private PlayerController rubyController; // this line of code creates a variable called "rubyController" to store information about the RubyController script!

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;

        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController"); //this line of code finds the RubyController script by looking for a "RubyController" tag on Ruby

        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<PlayerController>(); //and this line of code finds the rubyController and then stores it in a variable
            print("Found the RubyConroller Script!");
        }
        if (rubyController == null)
        {
            print("Cannot find GameController Script!");
        }

    }


    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {

        if (!aggressive)
        {
            return;
        }

        timer -= Time.deltaTime;


        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }


        rigidbody2d.MovePosition(position);
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();


        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        aggressive = false;
        GetComponent<Rigidbody2D>().simulated = false;
        animator.SetTrigger("Fixed");

        smokeEffect.Stop();

        if (rubyController != null) 
        {
            rubyController.ChangeScore(1);
        }
    }

}