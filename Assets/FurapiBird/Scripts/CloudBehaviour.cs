using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Determines the behaviour of the clouds
// they work the same as mountains
// There are 6 clouds (2 big, 4 small) which loop around once they reach their despawn position
public class CloudBehaviour : MonoBehaviour{   

    // Position variables
    private float height, x_axis, depth;
    const float max_depth_small = 10f, max_depth_big = 9.5f, depth_interval = 0.5f;
    const float max_height = 4f;
    const float x_axis_interval = 28f;
    
    // speed and size attributes
    private float speed, size;
    const float min_speed = 0.1f, speed_interval_small = 0.3f, speed_interval_big = 0.1f; 
    const float size_rate = 1f;
    
    // Type of the cloud. Big clouds are slower
    public bool is_big;
    
    // spawn and despawn position attributes
    private float despawn_position, spawn_position;
    const float x_axis_right_screen_side = 10f, x_axis_left_screen_side = -9.5f;
    const float regular_cloud_size = 4f;
    
    void Start(){
        // Generate random values for each variable
        random_values();
        // Apply the changes to the cloud
        transform.localScale = new Vector3(size, size, 1);
        transform.position = new Vector3(x_axis, height, depth);
    }

    // Update is called once per frame
    void Update()
    {
        // Move at a constant speed
        transform.Translate(-speed * Time.deltaTime, 0, 0);
        // reinitialize the cloud of its despawn position is reached
        if (transform.position.x < despawn_position){
            random_values();
            transform.localScale = new Vector3(size, size, 1);
            // Initialize with spawn_position value instead of x_axis so it doesn't spawn in the middle of the screen 
            transform.position = new Vector3(spawn_position, height, depth);
        }
    }

    // Give random values to each variable
    void random_values(){
        size = Random.value * size_rate + 1f;
        height = Random.value * max_height;
        if (is_big){ 
            speed = Random.value*speed_interval_big+min_speed;
            depth = max_depth_big - Random.value*depth_interval;
        } else { 
            speed = Random.value*speed_interval_small+min_speed; 
            depth = max_depth_small - Random.value*depth_interval;
        }
        x_axis = Random.value * x_axis_interval - x_axis_interval/2;
        despawn_position = x_axis_left_screen_side - regular_cloud_size * size;
        spawn_position = x_axis_right_screen_side + regular_cloud_size * size;
    }
}
