using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // speed at which the player moves
    public float speed = 0;

    // UI text component to display count of "PickUp" objects collected and "Lives".
    public TextMeshProUGUI countText;
    public TextMeshProUGUI livesText;

    // UI object to display winning text.
    public GameObject winTextObject;

    // UI object to display loosing text
    public GameObject looseTextObject;

    // Reference to the GameObject buttonsCanvas
    public GameObject buttonsCanvas;

    // Reference for the countdown text
    public TextMeshProUGUI countdownText;

    // Countdown time in seconds
    public float countdownTime = 60f;

    private AudioSource audioSource;
    public AudioClip pickUpSound;    // Sound for when a pickup is collected
    public AudioClip winSound;       // Sound for when the player wins
    public AudioClip loseSound;      // Sound for when the player loses

    // rigidbody of the player
    private Rigidbody rb;

    // Variable to keep track of collected "PickUp" objects.
    private int count;

    private int lives;
    
    // movement along X and Y axes
    private float movementX;
    private float movementY;
    
    // Start is called before the first frame update
    void Start()
    {
        // get and store the rigidbody component attached to the player
        rb = GetComponent <Rigidbody>();

        audioSource = GetComponent<AudioSource>();

        // Initialize count to zero.
        count = 0;

        // Initialize lives to one.
        lives = 2;

        // Update the count and lives display.
        SetCountText();
        SetLivesText();

        // Initially set the win text to be inactive.
        winTextObject.SetActive(false);

        looseTextObject.SetActive(false);

        buttonsCanvas.SetActive(false);

        // Start the countdown timer
        StartCoroutine(CountdownTimer());

        // Update the countdown display initially
        UpdateCountdownDisplay();
    }

    // this function is called when a move input is detected
    void OnMove(InputValue movementValue)
    {
        // convert the input value into a vector2 for movement
        Vector2 movementVector = movementValue.Get<Vector2>();

        // store the X and Y components of the movement
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // Update the count display.
    void SetCountText() 
    {
        // Update the count text with the current count.
        countText.text =  "Count: " + count.ToString();
       
        // Check if the count has reached or exceeded the win condition.
        if (count >= 34)
        {
            audioSource.PlayOneShot(winSound);
            
            // Display the win text.
            winTextObject.SetActive(true);

            // Trigger the next scene or quit using ButtonsManager
            buttonsCanvas.SetActive(true);

            StopBall();
        }
    }

    // Update the count display.
    void SetLivesText() 
    {
        // Update the count text with the current count.
        livesText.text =  "Lives: " + lives.ToString();
       
        // Check if the count has reached or exceeded the win condition.
        if (lives <= 0)
        {
            GameOver();
        }
    }

    // this function is called once per fixed frame-rate frame
    void FixedUpdate()
    {
        // create a 3D movement vector using the X and Y inputs
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        
        // apply force to the rigidbody to move the player
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object the player collided with has the "PickUp" tag.
        if (other.gameObject.CompareTag("PickUp")) 
        {
            // Deactivate the collided object (making it disappear).
            other.gameObject.SetActive(false);

            // Play the pick-up sound effect
            audioSource.PlayOneShot(pickUpSound);

            // Increment the count of "PickUp" objects collected.
            count = count + 1;

            // Update the count display.
            SetCountText();
        }

        // Check if the object player collided with the "Cactus" tag.
        if (other.gameObject.CompareTag("Cactus")) 
        {
            other.gameObject.SetActive(true);
            if (count < 34)
            {
                lives = lives - 1;
                SetLivesText();
            }
        }
    }

    void StopBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        speed = 0;
    }

    IEnumerator CountdownTimer()
    {
        while (countdownTime > 0 && count < 34)
        {
            yield return new WaitForSeconds(1f);
            countdownTime--;
            UpdateCountdownDisplay();
        }
        if (count < 34)
        {
            GameOver();
        }
        
    }

    // Update the countdown display
    void UpdateCountdownDisplay()
    {
        int minutes = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        countdownText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    // Trigger Game Over
    void GameOver()
    {
        audioSource.PlayOneShot(loseSound);
        looseTextObject.SetActive(true);
        buttonsCanvas.SetActive(true);
        StopBall();
    }

}
