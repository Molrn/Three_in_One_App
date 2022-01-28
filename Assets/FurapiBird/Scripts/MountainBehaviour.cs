using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Determine the behaviour of the mountains in the background
// There are four mountains in the background which loops around once they reach their despawn position
public class MountainBehaviour : MonoBehaviour{

    // personalizable Attributes
    const float speed = 0.3f;
    const float height_rate = 1.5f;
    
    // Position attributes
    const float despawn_position = -16f, spawn_position = 15f, y_axis_value = -3.5f;

    void Start()
    {
        // Initialize the scale of the mountain with a random value on the y axis
        transform.localScale = new Vector3(1, Random.value * height_rate + 1f);
    }

    void Update()
    {
        // Move the mountain at a constant speed on the x axis
        transform.Translate(-speed * Time.deltaTime, 0, 0);
        // Reinitialize the mountain position once it reaches its despawn position
        if(transform.position.x < despawn_position)
        {
            // Randomize the scale
            transform.localScale = new Vector3(1, Random.value*height_rate + 1f);
            // initialize the position, random value on the z axis to organise mountains between values 0 and 1 of the background
            transform.transform.position = new Vector3(spawn_position, y_axis_value,Random.value);
        }
    }
}
