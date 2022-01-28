using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Determine the behaviour of obstacles
// Obstacles can be moving and have random sprites
public class PipeObstacle_Script : MonoBehaviour
{
    const float despawn_posX = -12f;
    
// Used for initialisation
    // Represent the different sprites
    public Sprite[] pipes;
    public bool is_moving = false;
    public GameObject upper_pipe, lower_pipe;
    
// Constant values
    const float moving_coef = 0.2f;
    const float min_height = -3.5f, max_height = 3.5f;

// In game modified    
    public float pipeSpeed;   
    public float angle = 0;
    private int moving_sign = 1;
    

    void Start(){
        // Give each pipe a random sprite
        random_sprites();
        //transform.eulerAngles = new Vector3(0, 0, angle); Wasn't working
    }

    void Update(){
        // Move pipes constantly towards the player         
        transform.Translate(-pipeSpeed * Time.deltaTime, 0, 0);
        // Destroy in-game obstacles if they reach their despawn position or if the player restarts the game
        if (transform.position.x < despawn_posX || Input.GetKey(KeyCode.R)){
            Destroy(gameObject);
        }
        // Move the moving pipes up and down
        if (is_moving){
            if (transform.position.y > max_height){ moving_sign = -1; }
            if (transform.position.y < min_height){moving_sign = 1; }
            transform.Translate(0, moving_sign * pipeSpeed * moving_coef * Time.deltaTime, 0);
        }
    }
    
    // Give each pipe a random sprite
    private void random_sprites(){
        upper_pipe.GetComponent<SpriteRenderer>().sprite = pipes[Random.Range(0, 4)];
        lower_pipe.GetComponent<SpriteRenderer>().sprite = pipes[Random.Range(0, 4)];
    }
}
