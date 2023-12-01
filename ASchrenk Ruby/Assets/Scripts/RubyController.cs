using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 0f;

    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; } }
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;

    public ParticleSystem hitEffect;
    public ParticleSystem healEffect;

    AudioSource audioSource;
    public AudioClip throwingClip;
    public AudioClip hitClip;

    //Variables to track the score
    public TextMeshProUGUI robotCount;
    public int fixedRobots = 0;

    //Variables to track win/loss
    public GameObject gameOverUI;
    public TextMeshProUGUI gameOverText;
    public GameObject group11;
    bool victory = false;
    bool gameIsOver = false;

    //New modding variables for:

    //Win/loss audio cues -Astraea
    public AudioClip winClip;
    public AudioClip lossClip;

    //Time attack mode -Astraea
    float attackTimer = 30.0f;
    string displayTime;
    bool timeMode = false;
    public GameObject modeUI;
    public TextMeshProUGUI timerUI;

    //Ken- please put your new variables here


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        //Ends game if health hits 0
        if (currentHealth == 0)
        {
            GameOver();
        }

        //Ends game with victory if four robots are fixed
        if (fixedRobots >= 4)
        {
            victory = true;
            GameOver();
        }
        
        //Restarts the game if the game is over and R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameIsOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        //If in time attack mode, updates timer -Astraea
        if (timeMode == true)
        {
            attackTimer -= Time.deltaTime;

            displayTime = attackTimer.ToString("F0");
            if (attackTimer <= 9.49999f)
            {
                displayTime = "0" + displayTime;
            }

            timerUI.SetText(":" + displayTime);

            //If the timer reaches zero without all the robots fixed, end the game -Astraea
            if (attackTimer <= 0f && fixedRobots < 4)
            {
                GameOver();
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            hitEffect.Play();

            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitClip);
        }
        else
        {
            healEffect.Play();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300f);

        animator.SetTrigger("Launch");
        PlaySound(throwingClip);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    //Updates the score UI text
    public void IncrementRobotText()
    {
        fixedRobots++;
        robotCount.SetText("{0}", fixedRobots);
    }

    //Shows the win/loss message and allows player to restart the game
    void GameOver()
    {
        //Sets message to You Win or You Lose
        string message = "Lose";

        //Stops the timer -Astraea
        timeMode = false;

        if (victory == true)
        {
            message = "Win";
            group11.SetActive(true);
            //Plays win audio cue -Astraea
            PlaySound(winClip);
        }
        else
        {
            //Plays loss audio cue -Astraea
            PlaySound(lossClip);
        }

        gameOverText.SetText("You "+message+"!");

        //Halts player movement and displays message
        speed = 0f;
        gameOverUI.SetActive(true);
        gameIsOver = true;
    }

    //Hides mode UI and starts game -Astraea
    public void StartGame()
    {
        modeUI.SetActive(false);
        speed = 3.0f;
    }

    //Starts game in time attack mode -Astraea
    public void TimeAttackMode()
    {
        timerUI.SetText(":30");
        timeMode = true;

        StartGame();
    }
}


