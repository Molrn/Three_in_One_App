using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Determine the paddle behaviour
public class Paddle : MonoBehaviour {
    const float speed = 10;
    void Start(){ }

    void Update(){
        // Move right if the right key is pressed
        if (Input.GetKey(KeyCode.RightArrow)){
            if (transform.position.x < 4.48f){
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
        }
        // Move left if the left key is pressed
        if (Input.GetKey(KeyCode.LeftArrow)){
            if (transform.position.x > -4.48f){
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            }
        }
    }
}
