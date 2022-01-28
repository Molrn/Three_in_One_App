using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class Player_Script : MonoBehaviour
{

    //---------------------------------------------------------------------------------
    // ATTRIBUTES
    //---------------------------------------------------------------------------------
    public TextMeshPro displayed_text;
    public TextMeshPro ref_highscore;
    private string highscore_file_path = "Assets/Resources/Highscores.txt";
    private string temp_highscore_AC, temp_highscore_BB, temp_highscore_FB;
    private int highscore;

    protected int score = 0;
    protected AudioSource ref_audioSource;
    protected Animator ref_animator;

    //---------------------------------------------------------------------------------
    // METHODS
    //---------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        get_highscore();
        ref_audioSource = GetComponent<AudioSource>();
        ref_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        //Manage movement speed and animations
        float newSpeed = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            newSpeed = -10f;
            ref_animator.SetBool("isForwards", false);
        }
        else if ( Input.GetKey(KeyCode.RightArrow) )
        {
            newSpeed = 10f;
            ref_animator.SetBool("isForwards", true);
        }
        
        //Inform animator : Are we moving?
        ref_animator.SetBool("isMoving", newSpeed != 0);


        //Move with the speed found
        transform.Translate(newSpeed * Time.deltaTime, 0, 0);

        //We stop time if the spaceBar is pushed down
        if ( Input.GetKeyDown(KeyCode.Space) )
        {
            Time.timeScale = 0f;
        }
        else if ( Input.GetKeyUp(KeyCode.Space) )
        {
            Time.timeScale = 1.0f;
        }

        //Quit game
        if (Input.GetKeyDown(KeyCode.Escape)){
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
        }
    }

    //React to a collision (collision start)
    void OnCollisionEnter2D( Collision2D col )
    {
        score++;
        displayed_text.SetText("Score : " + score);
        if(highscore < score)
        {
            update_highscore();
            highscore = score;
            ref_highscore.SetText("Highscore : " + highscore);
        }
        ref_audioSource.Play();
    }

    void get_highscore()
    {
        StreamReader reader = new StreamReader(highscore_file_path);
        temp_highscore_FB = reader.ReadLine();
        temp_highscore_BB = reader.ReadLine();
        temp_highscore_AC = reader.ReadLine();
        reader.Close();
        highscore = int.Parse(temp_highscore_AC);
        ref_highscore.SetText("Highscore : " + highscore);
    }
    void update_highscore()
    {
        StreamWriter writer = new StreamWriter(highscore_file_path, false);
        writer.WriteLine(temp_highscore_FB);
        writer.WriteLine(temp_highscore_BB);
        writer.WriteLine("" + highscore);
        writer.Close();
    }
}
