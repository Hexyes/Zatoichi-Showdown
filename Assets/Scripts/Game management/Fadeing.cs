using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadeing : MonoBehaviour {

    private Texture2D fadeOutTexture; 
    public float fadeSpeed;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1;
    //private float timer = 0.0f;

    private void Start()
    {
       fadeOutTexture = Resources.Load<Texture2D>("Black");
    }

    void OnGUI()
    {
       
        alpha += fadeDir*fadeSpeed * Time.deltaTime;
        //alpha *= fadeDir;

        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);

        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);

        GUI.enabled = false;

    }

   
    //set fade dir
    public float BeginFade(int dir)
    {
        fadeDir = dir;
        return 1/fadeSpeed;
    }
    private void OnLevelWasLoaded(int level)
    {
        BeginFade(-1);
    }
}
