using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PopUpText : MonoBehaviour {


    private float alpha = 0;
    public bool fadeIn = false;
    void Start()
    {
        this.GetComponent<CanvasRenderer>().SetAlpha(alpha);
    }
    void Update()
    {
        if(fadeIn)fadeInFn();
    }
    public void fadeInFn()
    {
        alpha += 1 * .5f * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);
        this.GetComponent<CanvasRenderer>().SetAlpha(alpha);
    }
}
