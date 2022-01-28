using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnerScript : MonoBehaviour
{
    public AudioClip ref_audioClip;
    public SpriteRenderer fader_renderer;
    

    protected GameObject apple_prefab;
    protected float timer = 3f;
    protected AudioSource ref_audioSource;
    protected float current_alpha = 1;
    public bool can_play;

    // Start is called before the first frame update
    void Start()
    {
        
        apple_prefab = Resources.Load<GameObject>("Apple_prefab");
        ref_audioSource = gameObject.AddComponent<AudioSource>();
        ref_audioSource.loop = true;
        ref_audioSource.volume = 0.5f;
        ref_audioSource.clip = ref_audioClip;

        StartCoroutine( FadeOutFromWhite() );

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if ( timer <= 0)
        {
            float randomX = Random.value * 17f - 8.5f;

            GameObject newApple = Instantiate(apple_prefab);
            newApple.transform.position = new Vector3(randomX, 6.0f, 0);

            timer = 0.5f + Random.value*1f ;
        }
        
    }

    //Coroutine to fade out from white/launch music with a delay
    IEnumerator FadeOutFromWhite()
    {
        yield return new WaitForSeconds(0.5f);

        ref_audioSource.Play();

        while (current_alpha > 0)
        {
            current_alpha -= Time.deltaTime / 2;
            fader_renderer.color = new Color(1, 1, 1, current_alpha);
            yield return null;
        }

        Destroy(fader_renderer.gameObject);

    }
    
}
