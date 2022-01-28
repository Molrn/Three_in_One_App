using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    

    // Position attributes
    const float x0 = -5.5f, y0 = 3.3f, sizeX = 1f, sizeY = 0.4f;
    const int bricks_per_half_line = 6;
    
// Brick attributes
    public int brick_nb;
    // List of all the bricks to be able to destroy a level easily (restart feature)
    public List<GameObject> bricks;
    // Game object to initialize by instatiating the prefab
    private GameObject Brick;
    // reference to the brick prefab
    public GameObject brick_prefab;

    void Start(){ }

    void Update(){ }
    
    // Create a new stage with line_nb lines and a spawn_prob spawning probability
    public void newStage(float spawn_prob, int line_nb) {
        destroy_level();
        brick_nb = 0;
        // go through every case of the brick grid
        for(int i = 0; i<line_nb; i++){
            for(int j = 0; j < bricks_per_half_line; j++){
                // Spawn symetric bricks with a spawn_prob probabilty
                if (Random.Range(0f,1f) > spawn_prob){
                    create_brick(x0 + j * sizeX, y0 - i * sizeY);
                    create_brick(-x0 - j * sizeX, y0 - i * sizeY);
                    brick_nb += 2;
                }
            }
        }
    }

    // Destroy all the brick
    public void destroy_level(){
        // Go through the brick list 
        for (int i = 0; i < bricks.Count; i++){
            // Destroy every brick
            Destroy(bricks[i]);
        }
        // Clear the list
        bricks.Clear();
    }

    // Create a brick with x and y position and add it to the list
    private void create_brick(float x, float y){
        // create a brick from the prefab
        Brick = Instantiate(brick_prefab);
        // Add it to the list
        bricks.Add(Brick);
        //Change its position
        Brick.transform.position = new Vector3(x, y, 0);
    }
}
