using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

// Determine the behaviour of the bird
public class BirdBehaviour : MonoBehaviour{

// HUD
    public TextMeshPro ref_gameOver;
    public TextMeshPro ref_highscore_1;
    public TextMeshPro ref_highscore_2;

// References to extern elements
    public ObstacleSpawner spawner;
    public Sprite dead_bird;

// Elements to get from the object
    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer image;

// Audio
    private AudioSource ref_audio;
    public AudioClip death_sound;
    public AudioClip fly_sound;
    public AudioClip highscore_sound;

// Highscore Elements
    // path to the file
    private string highscore_file_path = "Assets/Resources/Highscores.txt";
    // Temporary string to save the content of the file
    private string temp_highscore_AC, temp_highscore_BB, temp_highscore_FB;

// Bird movement attributes
    const float strength = 4f;
    const float angle = 45f;
    const float speed = 5f;
    const float initial_height = 0f, initial_x_position = -6.5f;

    // Points management
    private int points = 0, highscore = 0;
    
    // Fader variable
    private float delta;
    
    // boolean to disable bird control
    public bool can_play;

    void Start(){
        // Get components from the object carrying the script
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        image = GetComponent<SpriteRenderer>();
        ref_audio = GetComponent<AudioSource>();
        // Initialize the audio clip to the fly soud
        ref_audio.clip = fly_sound;
        // disable player input
        can_play = false;
        // Gets the highscore from the file
        get_highscore();
        StartCoroutine(start_game());
    }

    void Update(){
        
        // Fly when the player presses 'Up' and is able to move
        if (Input.GetKeyDown(KeyCode.UpArrow) && can_play){
            // Play the fly sound
            ref_audio.Play();
            // move bird up, but not with a translation to avoid infinite speed gain
            body.velocity = new Vector3(0, strength, 0);
        }
        // rotate the bird according to its velocity
        
        if(body.velocity.y > 0){
            // while going up, instant rotation but low angle -> hard to climb
            transform.eulerAngles = new Vector3(0, 0, angle/2);
        }else{
            // while going down, increasing rotation to increase the speed feeling
            transform.eulerAngles = new Vector3(0, 0,transform.rotation.z + body.velocity.y*speed);    
        }
        // Restart the game when the restart key is pressed
        if (Input.GetKeyDown(KeyCode.R)){
            StopAllCoroutines();
            ref_gameOver.color = new Color(1, 1, 1, 0);
            ref_highscore_1.color = new Color(0, 0, 0, 0);
            restart(); 
        }
        // Go to the menu when the exit key is pressed
        if (Input.GetKeyDown(KeyCode.Escape)) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu"); 
        }
    }
    private void OnCollisionEnter2D(Collision2D collision){
        // Start dying routine if the bird isn't already dead
        if (can_play) { StartCoroutine(game_over()); }
        // Disable the dynamic aspect of the body to avoid weird behaviours (rebounds, infinite fall, etc...)
        // Don't disable it otherwise to have the bird fall through the screen
        if(collision.collider.name == "Lower Wall") { body.isKinematic = true; }
    }

    // Reinitialize the elements of the game
    private void restart(){
        // Initialize position, rotation and velocity
        transform.position = new Vector3(initial_x_position, initial_height, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);
        body.velocity = new Vector3(0,0,0);
        points = 0;
        // Enable the animation
        anim.enabled = true;
        // Set the audio clip sound to 'fly'
        ref_audio.clip = fly_sound;
        // Start the coroutine beginning a new game
        StartCoroutine(start_game());
    }

    // Start the game
    IEnumerator start_game(){
        ref_highscore_2.SetText("Highscore : " + highscore);
        // Disable gravity effect in case it wasn't already done
        body.isKinematic = true;
        // fade out the game over and highscore messages
        delta = ref_gameOver.color.a;
        if (delta > 0){
            while (ref_gameOver.color.a > 0){
                delta -= Time.deltaTime;
                ref_gameOver.color = new Color(1, 1, 1, delta);
                yield return null;
            }
            ref_highscore_1.color = new Color(0, 0, 0, 0);
        }
        yield return new WaitForSeconds(2f);
        ref_gameOver.color = new Color(1,1,1,0);
        // Enable the player to move and activate gravity
        body.isKinematic = false;
        can_play = true;
    }

    // Game over routine
    IEnumerator game_over(){
        // Play the death sound
        ref_audio.clip = death_sound;
        ref_audio.Play();
        // Disable the player input and the animation
        anim.enabled = false;
        can_play = false;
        // Changes the sprite of the bird to the death sprite
        image.sprite = dead_bird;
        // fade in the game over message 
        delta = 0;
        while (delta < 1){
            delta += Time.deltaTime;
            ref_gameOver.color = new Color(1, 1, 1, delta);
            yield return null;
        }
        // Get the points from the spawner script
        points = spawner.points;
        // Change the highscore if it's a new highscore
        if (points > highscore) {
            // Display highscore message
            ref_highscore_1.color = new Color(0, 0, 0, 1);
            highscore = points;
            // Play highscore sound
            ref_audio.clip = highscore_sound;
            ref_audio.Play();
            // Update the highscore file and the in game message
            update_highscore();
            ref_highscore_2.SetText("Highscore : " + highscore);
        }
    }

    // Get the highscore from the file
    void get_highscore(){
        // Open the file in read mode
        StreamReader reader = new StreamReader(highscore_file_path);
        // Read and save line by line the content of the file
        temp_highscore_FB = reader.ReadLine();
        temp_highscore_BB = reader.ReadLine();
        temp_highscore_AC = reader.ReadLine();
        reader.Close();
        // Convert the 'Furapi Bird' line into an integer
        highscore = int.Parse(temp_highscore_FB);
    }

    // Print the temporary values into the file to update it
    void update_highscore()
    {
        // Open file in vriting mode, false to ovewrite the file instead of addind text
        StreamWriter writer = new StreamWriter(highscore_file_path, false);
        // print the new highscore
        writer.WriteLine(""+highscore);
        // Print other games highscore
        writer.WriteLine(temp_highscore_BB);
        writer.WriteLine(temp_highscore_AC);
        writer.Close();
    }
}
