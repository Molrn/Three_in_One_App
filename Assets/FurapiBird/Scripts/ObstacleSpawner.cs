using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Spawn obstacles and deals with levels and points
// Randomly spawn obstacles according to values which change according to the level
public class ObstacleSpawner : MonoBehaviour{
// objects generation
    protected GameObject obstacle_prefab;
    private GameObject newObstacle;

    //HUD
    public TextMeshPro ref_score;
    
    //reference to the script "BirdBehaviour"
    public BirdBehaviour bird_script;    
    
    // timers
    private float spawn_timer=0, level_timer = 0, points_timer;

// Level variables
    public float speed;
    private float height;
    private float difficulty;
    public float spawn_interval;
    private int series_length, temp_length;
    private float angle;
    public int points = 0;

// Constant values : level parameters
    // Difficulty parameters
    const float min_diff = 30, max_diff = 80;
    //Series parameters
    const int series_min = 2, series_max = 10;
    //speed parameters
    const float speed_interval = 0.1f, speed_min = 4, speed_max = 10;
    // spawn parameters
    const float min_spawn_time = 2, max_spawn_time = 3, init_spawn_interval = 0.2f, spawn_coef = 0.1f;
    // angle parameters
    const float angle_min = 5, angle_max = 30;
    // time parameters
    const float time_per_meter = 2f, time_per_level = 15;
    private bool do_spawn = true;

// other constant values     
    const float min_height = -3.5f, max_height = 3.5f;
    const float pipe_size = 2.1f;
    const float hauteur_marche = 0.5f;

    void Start(){
        //Get the obstacle prefab from the Resources folder
        obstacle_prefab = Resources.Load<GameObject>("Obstacle");
        initialize_values();
    }
 
    void Update(){
        // Update the timers only when the player can move / is alive
        if (bird_script.can_play){
            if (do_spawn) { spawn_timer -= Time.deltaTime; }
            level_timer -= Time.deltaTime;
            points_timer -= Time.deltaTime;
        }
        
        // Spawn an obstacle at the end of the timer
        if (spawn_timer <= 0){
            // Randomly spawn a difficult or a difficult pipe according to the difficulty of the game 
            if (Random.value*100 > difficulty) {
                spawn_regular();
            }else{
                spawn_difficult();
            }
            // Reinitialize the timer
            spawn_timer = max_spawn_time - Random.value*spawn_interval;
        }

        // change the level at the end of the timer
        // Modify each level variable until it reaches its limits
        if (level_timer <= 0){
            change_level();
        }
        
        // Add a point at the end of the timer
        if(points_timer <= 0){
            points++;
            ref_score.SetText(points + " m");
            points_timer = time_per_meter/ speed;
        }

        // Reinitialize every value when player hits restart key
        if (Input.GetKey(KeyCode.R)){
            initialize_values();
        }
    }
    
    // Increase level variables
    //Make sure values don't overcome their limits
    void change_level(){
        if(difficulty < max_diff) { difficulty += 1; }
        if(speed < speed_max) { speed += speed_interval; }
        if(spawn_interval < max_spawn_time - min_spawn_time){
            spawn_interval += spawn_coef;
        }
        if(series_length < series_max) { series_length++; }
        if(angle < angle_max) { angle++; }
        level_timer = time_per_level;
    }
    // Gives to each level value its initial value
    void initialize_values(){
        difficulty = min_diff;
        spawn_interval = init_spawn_interval;
        speed = speed_min;
        series_length = series_min;
        angle = angle_min;
        points = 0;
        points_timer = time_per_meter / speed;
        ref_score.SetText(points + " m");
    }

    // Spawn a regular obstacle
    void spawn_regular(){
        // Gives a random height
        height = max_height - Random.value * (max_height - min_height);
        // Creates a new object from the prefab
        newObstacle = Instantiate(obstacle_prefab);
        // Moves the pipe to its initinal position
        newObstacle.transform.position = new Vector3(12, height, 0);
        // Initialize its behaviour script's speed variable 
        newObstacle.GetComponent<PipeObstacle_Script>().pipeSpeed = speed;
    }

    // Spawn a difficult obstacle
    void spawn_difficult(){
        // Determine ramdomly which kind of obstacle to spawn
        switch (Random.Range(0, 3)) {
            case 0: StartCoroutine(spawn_series());
                break;
            case 1: spawn_moving();
                break;
            case 2:
                StartCoroutine(spawn_stairs());
                break;
        }
    }

    //doesn't work --> movement perpendicular to pipes
    void spawn_angle(){
        //newObstacle.GetComponent<PipeObstacle_Script>().angle = (Random.value - 0.5f) * angle * 2;
        newObstacle.transform.position = new Vector3(12, height, 0);
    }
    
    // Spawn close pipes at the same height
    IEnumerator spawn_series(){
        // Disable the spawning timer
        do_spawn = false;
        // Randomly select a height
        height = max_height - Random.value * (max_height - min_height);
        // Randomly select the length of the series
        temp_length = Random.Range(2, series_length);
        // Spawn temp_length pipes
        for(int i = 0; i < temp_length; i++){
            newObstacle = Instantiate(obstacle_prefab);
            newObstacle.GetComponent<PipeObstacle_Script>().pipeSpeed = speed;
            // spawn obstacles separated by a pipe_size value on the x axis 
            newObstacle.transform.position = new Vector3(12 + i*pipe_size, height, 0);
        }
        // Wait until the last obstacle of the series reach the starting position
        while (newObstacle.transform.position.x > 12f) { yield return null; }
        // Re-enable the spawning timer
        do_spawn = true;
    }
    
    // Spawn close pipes at a regular height interval
    IEnumerator spawn_stairs(){
        // disable the spawning timer
        do_spawn = false;
        temp_length = Random.Range(2, series_length);
        height=max_height-temp_length*hauteur_marche-Random.value*(max_height-min_height-2* temp_length*hauteur_marche);
        // Randomly spawn either an ascending or a descending stair
        if(Random.value > 0.5f) {
            for (int i = 0; i < temp_length; i++){
                newObstacle = Instantiate(obstacle_prefab);
                newObstacle.GetComponent<PipeObstacle_Script>().pipeSpeed = speed;
                newObstacle.transform.position = new Vector3(12 + i * pipe_size, height+hauteur_marche*i, 0);
            }
        }else{
            for (int i = 0; i < temp_length; i++){
                newObstacle = Instantiate(obstacle_prefab);
                newObstacle.GetComponent<PipeObstacle_Script>().pipeSpeed = speed;
                newObstacle.transform.position = new Vector3(12 + i * pipe_size, height-hauteur_marche*i, 0);
            }
        }
        while (newObstacle.transform.position.x > 12f) { yield return null; }
        do_spawn = true;
    }
    
    // Spawn a pipe moving up and down
    void spawn_moving(){
        // Spawna regular pipe
        height = max_height - Random.value * (max_height - min_height);
        newObstacle = Instantiate(obstacle_prefab);
        newObstacle.GetComponent<PipeObstacle_Script>().pipeSpeed = speed;
        newObstacle.transform.position = new Vector3(12, height, 0);
        // Enable the is_moving boolean of its script
        newObstacle.GetComponent<PipeObstacle_Script>().is_moving = true;
    }

}
