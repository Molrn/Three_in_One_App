using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class button_management_script : MonoBehaviour
{
    public SpriteRenderer fader;
    public VideoPlayer ref_space_video;
    private float delta;
    

    void Start(){
        StartCoroutine(load_screen());
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }
    public void launch_AppleCatcher(){ StartCoroutine(launch_game(0)); }
    public void launch_BrickBreaker(){ StartCoroutine(launch_game(1)); }
    public void launch_FurapiBird() { StartCoroutine(launch_game(2)); }
    IEnumerator load_screen(){
        while (!ref_space_video.isPlaying)
        {
            yield return null;
        }
        delta = 1;
        while (delta > 0)
        {
            delta -= Time.deltaTime/1.5f;
            fader.color = new Color(0, 0, 0, delta);
            yield return null;
        }
    }
    IEnumerator launch_game(int game)
    {
        //Fade the white fader into "existence"
        delta = 0;
        while (delta < 1)
        {
            delta += Time.deltaTime;
            fader.color = new Color(1, 1, 1, delta);
            yield return null;
        }

        AsyncOperation asyncLoad;
        switch (game){
            case 0 : asyncLoad = SceneManager.LoadSceneAsync("AC_Title");
                break;
            case 1 : asyncLoad = SceneManager.LoadSceneAsync("BrickBreaker");
                break;
            case 2 : asyncLoad = SceneManager.LoadSceneAsync("FurapiBird");
                break;
            default: asyncLoad = SceneManager.LoadSceneAsync("Menu");
                break;
        }
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone){
            yield return null;
        }
    }
}
