using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Variables related to player character movement
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    public float speed = 3.0f;


    // Variables related to the health and mana systems
    public int maxHealth = 5;
    int currentHealth;
    public int health { get { return currentHealth; } }

    //Variables relating to mana storage and cooldowns (Eden's Add)
    public int maxMana = 3;
    int currentMana;
    public int mana { get { return currentMana; } }

    public float timeManaSick = 0.5f;
    bool isManaSick;
    float manaCooldown;

    // Variables related to temporary invincibility [and vulnerability (Eden's Add)]
    public float timeInvincible = 2.0f;
    public float timeVulnerable = 2.0f;
    bool isInvincible;
    bool isVulnerable;
    float damageCooldown;

    // Variables related to animation
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    //Variables for casting spells
    public GameObject projectilePrefab;
    public InputAction launchAction;

    // Variable for talking to ally
    public InputAction talkAction;

    AudioSource audioSource;

    // Audio Clip for winning (Eden's Add)
    public AudioClip win;
    bool winSound;

    // Audio Clip for damage 
    public AudioClip damage;

    // Particle effects
    public ParticleSystem healingEffect;
    public ParticleSystem damageEffect;

    //Setting win conditions
    public int score = 0;
    public int robots = 6;
    public bool ultraGear = false;

    //Registering the Game Over
    public GameObject gameOverText;
    bool gameOver = false;

    //Parameters to teleport (Eden's Add)
    public InputAction teleportAction;
    bool readyToWarp = false;
    Vector2 warpLocation;

    //Variables for the Melee Strike (Sydney's Add)
    public InputAction meleeAction;
    public AudioClip meleeSwing;

    // Start is called before the first frame update
    void Start()
    {
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();


        currentHealth = maxHealth;
        currentMana = maxMana;

        launchAction.Enable();
        launchAction.performed += Launch;

        talkAction.Enable();
        talkAction.performed += FindFriend;

        teleportAction.Enable();
        teleportAction.performed += Teleport;

        meleeAction.Disable();
        meleeAction.performed += MeleeHit;

        audioSource = GetComponent<AudioSource>();

        winSound = false;
    }

    // Update is called once per frame
    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();


        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat("Move X", moveDirection.x);
        animator.SetFloat("Move Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (isVulnerable)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isVulnerable = false;
            }
        }

        // One of the many aspects of the mana system. Keeps it from recharging too fast (Eden's Add)
        if (isManaSick)
        { 
            manaCooldown -= Time.deltaTime;
            if (manaCooldown < 0)
            { 
                isManaSick = false;
            }
        }

        // Lose Condition
        if (health <= 0) {
            gameOverText.SetActive(true);
            gameOver = true;
            speed = 0;
        }
        if (Input.GetKey(KeyCode.R)) // check to see if the player is pressing R
        {
            if (gameOver == true) // check to see if the game is over
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene, which results in a restart of whatever scene the player is currently in
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            Instantiate(damageEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit"); 
            PlaySound(damage); //Eden's Add
        }

        //Keeps healing and audio stable again, and gives a place to put the particle effect on heal.
        if (amount > 0)
        {
            if (isVulnerable)
            {
                return;
            }
            Instantiate(healingEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            isVulnerable = true;
            damageCooldown = timeVulnerable;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    public void ChangeMana(int amount)  //MANA SYSTEM! Works similarly to heal system, but off a differnt resource (Eden's Add)
    {
        if (amount > 0) 
        {
            if (isManaSick) 
            {
                return;
            }
            isManaSick = true;
            manaCooldown = timeManaSick;
        }
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        UIHandler.instance.SetManaValue(currentMana / (float)maxMana);
    }

    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    void Launch(InputAction.CallbackContext context)
    {
        // Deplete mana when you fire magic. If you don't have any more mana, no more projectiles.
        if (currentMana > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 1.0f, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(moveDirection, 300);
            animator.SetTrigger("Launch");
            ChangeMana(-1);
        }
        else 
        {
            print("No Mana ;~;");
                return;
        }
    }

    void OnTriggerEnter2D(Collider2D other) //Eden's add. Helps to trigger the vent's particle effects to let the player know they can teleport
    {
        Teleporter portal = other.GetComponent<Teleporter>();

        if (portal != null)
        {
            portal.spellEffect.Play();
            warpLocation = portal.warpLocation;
            readyToWarp = true;
        }
    }

    void Teleport(InputAction.CallbackContext context)
    {
        if (readyToWarp && mana > 0) 
        {
            rigidbody2d.position = warpLocation;
        }
        if (rigidbody2d.position == warpLocation)
        {
            readyToWarp = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) //Turns off the particle effects and kills the player's ability to teleport.
    {
        Teleporter portal = other.GetComponent<Teleporter>();

        if (portal != null)
        {
            portal.spellEffect.Stop();
            readyToWarp = false;
        }
    }

    void FindFriend(InputAction.CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        print(hit.collider);

        if (hit.collider != null)
        {
            NonPlayerCharacter character1 = hit.collider.GetComponent<NonPlayerCharacter>();
            NonPlayerCharacter1 character2 = hit.collider.GetComponent<NonPlayerCharacter1>();

            // Win Condition and Eden's add. Requires the player to go back to Duck and talk to them in order to end the game.
            if (character1 != null && score >= robots && ultraGear)
            {
                gameOverText.SetActive(true);
                gameOver = true;
                speed = 0;
                if (!winSound)
                {
                    PlaySound(win); //Eden's Add
                    winSound = true;
                }
            }

            else if (character2 != null)
            {
                UIHandler.instance.DisplayFrogDialogue();
            }

            else if (character1 != null)
            {
                UIHandler.instance.DisplayDuckDialogue();
            }

        }

    }

    void MeleeHit(InputAction.CallbackContext context) //Sydney's Add
    {
        PlaySound(meleeSwing);
        animator.SetTrigger("Melee");
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));

        if (hit.collider != null)
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.Fix();
            }
        } 
    }
    public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }
    // Gain score to win!
    public void ChangeScore(int amount)
    {
        if (amount > 0)
        {
            score = score + amount;
        }
    }
    }

