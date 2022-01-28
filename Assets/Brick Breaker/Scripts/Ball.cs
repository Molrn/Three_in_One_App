using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

// Determine the behaviour of the ball
// Deal with the score and the audio
public class Ball : MonoBehaviour{

// HUD
    // In-game display
    public TextMeshPro ref_score;
    public TextMeshPro ref_highscore;
    public TextMeshPro ref_lives_display;
    // Messages
    public TextMeshPro ref_lives_message;
    public TextMeshPro ref_level;
    public TextMeshPro ref_gameOver;
    public TextMeshPro ref_highscore_message;
    // variables    
    private int points = 0, lives, highscore = 0, level = 1;

// Spawner management
    public GameObject brick_spawner;
    public List<GameObject> bricks;
    private Spawner s;
    private int brick_nb, line_nb = 3;
    private float spawn_prob;

// Audio
    private AudioSource ref_audio;
    public AudioClip paddle_bounce_sound;
    public AudioClip wall_bounce_sound;
    public AudioClip game_over_sound;
    public AudioClip intro_sound;
    public AudioClip point_loss_sound;
    public AudioClip point_win_sound;

// Highscore file management
    private string highscore_file_path = "Assets/Resources/Highscores.txt";
    private string temp_highscore_AC, temp_highscore_BB, temp_highscore_FB;

    // In-game ball modified properties
    private int coef_score = 1;
    private float speed = 5, diffx;

    
    // fader variable
    private float delta;

// constants
    const float death_y_position = -7.5f;
    const int lives_nb = 3;
    // Spawn probability attributes
    const float init_spawn_prob = 0.5f, max_spawn_prob = 0.8f, spawn_prob_interval = 0.05f;
    const float max_speed = 7f, speed_interval = 0.3f, init_speed = 5f;
    const int max_line_nb = 10, init_line_nb = 3;
    void Start(){
        // Get the spawner script from the brick_spawner object
        s = brick_spawner.GetComponent<Spawner>();
        ref_audio = GetComponent<AudioSource>();
        init_variables();
        StartCoroutine(play_clip(intro_sound));
        // Start level routine
        // shom then hide level message then start the ball
        StartCoroutine(start_level());
        // Create a new level
        new_level();
        // Reset the position and velocity of the ball
        reset_ball();
        // get highscore value from the file
        get_highscore();
        // Print the hud
        print_hud();
    }

    void Update(){
        // check if the ball is gone
        if (transform.position.y < death_y_position){
            if (lives == 0) { 
                // Player has no life remaining, so play game over routine
                StartCoroutine(game_over()); 
            }else {
                // player has lives remaining, keep playing
                lives--;
                coef_score = 1;
                StartCoroutine(play_clip(point_loss_sound));
                // Fade in and out lives message, then start the game
                StartCoroutine(lives_anouncer());
                print_hud();
            }
        }
        // Restart the game if the player presses the restart button
        if (Input.GetKeyDown(KeyCode.R)){
            StopAllCoroutines();
            ref_level.color = new Color(1, 1, 1, 0);
            StartCoroutine(restart());
        }
        // Go to the menu if 'esc' is pressed
        if (Input.GetKey(KeyCode.Escape)){
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision){
        // Destroy the collided object if it's a brick
        if (collision.collider.name == "Brick_prefab(Clone)"){
            // Add points to the total score
            points += 10 * coef_score;
            // Play winnig soud, without play_clip coroutine because
            // wait until the previous song is over makes a weird effect with close inputs
            ref_audio.clip = point_win_sound;
            ref_audio.Play();
            Destroy(collision.gameObject);
            brick_nb--;
            // If it was the last brick of the level, start a new one
            if (brick_nb == 0){
                coef_score += 1;
                level++;
                new_level();
                StartCoroutine(start_level());
            }
            print_hud();
        }else if (collision.collider.name == "Paddle") {
            // play paddle clip
            ref_audio.clip = paddle_bounce_sound;
            ref_audio.Play();
            // change ball velocity depending on the position of the ball on the paddle
            diffx = transform.position.x - collision.transform.position.x;
            GetComponent<Rigidbody2D>().velocity += new Vector2(2*diffx, 0);
        }else {
            // if it's not a paddle or a brick, it's a wall
            // Then play wall sound
            ref_audio.clip = wall_bounce_sound;
            ref_audio.Play();
        }
        if (velocity() < 2f) { 
            // increase the velocity if it's too low
            // --> avoid boring slow gameplay
            GetComponent<Rigidbody2D>().velocity *= 1.25f; 
        }
    }

    // Return the velocity of the ball
    private float velocity(){
        float x = GetComponent<Rigidbody2D>().velocity.x;
        float y = GetComponent<Rigidbody2D>().velocity.y;
        return Mathf.Sqrt(x*x + y*y);
    }

    // Reset position and velocity of the ball
    private void reset_ball(){
        transform.position = new Vector3(0, 0, 0);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    // increase one of the 3 parameters depending on the level
    // Then create a new level
    private void new_level(){
        ref_level.SetText("Level " + level);
        // parameters change
        switch (level % 3){
            case 0:
                if (speed < max_speed) { speed += speed_interval; }
                break;
            case 1:
                if (spawn_prob < max_spawn_prob) { spawn_prob += spawn_prob_interval; }
                break;
            case 2:
                if (line_nb < max_line_nb) { line_nb++; }
                break;
        }
        // Create a new stage 
        s.newStage(spawn_prob, line_nb);
        brick_nb = s.brick_nb;
        bricks = s.bricks;
    }

    // Game over routine
    IEnumerator game_over(){
        // Reset the position of the ball --> avoid starting the coroutine multiple times
        reset_ball();
        StartCoroutine(play_clip(game_over_sound));
        // Fade in game over message
        delta = 0;
        while (delta < 1){
            delta += Time.deltaTime;
            ref_gameOver.color = new Color(1, 1, 1, delta);
            yield return null;
        }
        // Modify highscore if there is a new highscore
        if (points > highscore){
            // Update highscore variable
            highscore = points;
            // update highscore file
            update_highscore();
            print_hud();
            // Display highscore message
            ref_highscore_message.color = new Color(1, 1, 1, 1);
        }
    }
    
    // Update HUD messages
    private void print_hud(){
        ref_highscore.SetText("Highscore " + highscore);
        ref_score.SetText("Score "+points+"  x"+coef_score);
        ref_lives_display.SetText(""+lives);
        ref_level.SetText("Level " + level);
    }

    // Get highscore value from the highscore file
    void get_highscore(){
        // Open the file in read mode
        StreamReader reader = new StreamReader(highscore_file_path);
        // Read and save line by line the content of the file
        temp_highscore_FB = reader.ReadLine();
        temp_highscore_BB = reader.ReadLine();
        temp_highscore_AC = reader.ReadLine();
        reader.Close();
        // Convert the 'Ball Breaker' line into an integer
        highscore = int.Parse(temp_highscore_BB);
    }

    // Print the temporary values into the file to update it
    void update_highscore(){
        // Open file in vriting mode, false to ovewrite the file instead of addind text
        StreamWriter writer = new StreamWriter(highscore_file_path, false);
        writer.WriteLine(temp_highscore_FB);
        // print the new highscore
        writer.WriteLine("" + highscore);
        // Print other games highscore
        writer.WriteLine(temp_highscore_AC);
        writer.Close();
    }
    
    // Initialize all variables
    void init_variables(){
        level = 1;
        points = 0;
        coef_score = 1;
        lives = lives_nb;
        spawn_prob = init_spawn_prob;
        line_nb = init_line_nb;
        speed = init_speed;
    }
    // Wait one second before launching the ball
    IEnumerator launch_ball(){
        yield return new WaitForSeconds(1f);
        // Initiate ball velocity
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -speed);
    }

    // Restart the game
    IEnumerator restart(){
        StopCoroutine(lives_anouncer());
        ref_lives_message.color = new Color(0,0,0,0); 
        // Play intro sound
        StartCoroutine(play_clip(intro_sound));
        // Initialize values
        init_variables();
        // fade out the Game Over message
        if (ref_gameOver.color.a > 0){
            delta = 1;
            while (delta > 0)
            {
                delta -= Time.deltaTime / 2;
                ref_gameOver.color = new Color(1, 1, 1, delta);
                yield return null;
            }
            // Hide the highscore message
            ref_highscore_message.color = new Color(1, 1, 1, 0);
        }
        //Fade out the lives message
        if (ref_lives_message.color.a > 0){
            delta = ref_lives_message.color.a;
            while (delta > 0)
            {
                delta -= Time.deltaTime / 2;
                ref_lives_message.color = new Color(0, 0, 0, delta);
                yield return null;
            }
        }
        print_hud();
        new_level();
        StartCoroutine(start_level());
    }

    // Start a new level
    IEnumerator start_level(){
        reset_ball();
        delta = 0;
        // Fade in the level message
        while (delta < 1){
            delta += Time.deltaTime;
            ref_level.color = new Color(0, 0, 0, delta);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        // Fade out the level Messaage
        while (delta > 0){
            delta -= Time.deltaTime;
            ref_level.color = new Color(0, 0, 0, delta);
            yield return null;
        }
        // Finally lanuches the ball
        StartCoroutine(launch_ball());
    }

    // Display the live message
    IEnumerator lives_anouncer(){
        reset_ball();
        // Set level text
        if(lives > 1){ ref_lives_message.SetText(lives + "  lives"); }
        else{ ref_lives_message.SetText("  " + lives + "  life"); }
        // Fade in lives message
        delta = 0;
        while (delta < 1){
            delta += Time.deltaTime;
            ref_lives_message.color = new Color(0, 0, 0, delta);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        // Fade out lives message
        while (delta > 0){
            delta -= Time.deltaTime;
            ref_lives_message.color = new Color(0, 0, 0, delta);
            yield return null;
        }
        // finally lanuch the ball
        StartCoroutine(launch_ball());
    }

    // Play the audio clip but first wait until the last one is finished
    IEnumerator play_clip(AudioClip clip){
        while (ref_audio.isPlaying){
            yield return null;
        }
        ref_audio.clip = clip;
        ref_audio.Play();
    }
}
