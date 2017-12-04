using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public float timer = 0.0f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > 5)
        {
            transform.Find("PUText").GetComponent<PopUpText>().fadeIn=true;
            if (Input.anyKey)
            {
                float fadeTime = GameObject.Find("Main Camera").GetComponent<Fadeing>().BeginFade(1);
                IEnumerator co = ChangeLevel(fadeTime);
                StartCoroutine(co);
            }
        }    
	}
    IEnumerator ChangeLevel(float fadeTime)
    {

        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("MainMenu");
    }
}
