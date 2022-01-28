using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Script : MonoBehaviour
{
    public SpriteRenderer fader_renderer;
    public AudioClip leaveSound;

    protected AudioSource r_audioSource;
    protected bool hasLeft = false;
    protected float current_alpha = 0;

    // Start is called before the first frame update
    void Start()
    {
        r_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Escape) ){
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
        }
        else if ( Input.anyKeyDown && !hasLeft ){
            hasLeft = true;
            StartCoroutine( LoadScene_Game() );
        }
    }

    IEnumerator LoadScene_Game()
    {
        //Stop music and play the exit sound
        r_audioSource.Stop();
        r_audioSource.clip = leaveSound;
        r_audioSource.loop = false;
        r_audioSource.Play();

        //Wait for that sound to end (with a margin)
        yield return new WaitForSeconds(0.8f);

        //Fade the white fader into "existence"
        while ( current_alpha < 1)
        {
            current_alpha += Time.deltaTime / 2;
            fader_renderer.color = new Color(1, 1, 1, current_alpha);
            yield return null;
        }

        //Wait a tiny bit
        yield return new WaitForSeconds(0.5f);

        //Load game scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("AppleCatcher");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
