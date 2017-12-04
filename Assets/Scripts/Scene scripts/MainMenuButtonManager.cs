using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviour {

public void NewGameBtn(string newGameLevel)
    {
        float fadeTime = GameObject.Find("Main Camera").GetComponent<Fadeing>().BeginFade(1);
        IEnumerator co = fade(newGameLevel, fadeTime);
        StartCoroutine(co);
    }

    public void ExitGameBtn()
    {
        float fadeTime = GameObject.Find("Main Camera").GetComponent<Fadeing>().BeginFade(1);
        IEnumerator co = fade(-1, fadeTime);
        StartCoroutine(co);
    }

    IEnumerator fade(string scene, float fadeTime)
    {
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(scene);
    }

    IEnumerator fade(float scene, float fadeTime)
    {
        yield return new WaitForSeconds(fadeTime);
        Application.Quit();
    }

}
