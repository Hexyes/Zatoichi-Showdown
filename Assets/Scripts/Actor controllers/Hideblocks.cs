using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hideblocks : MonoBehaviour {
    private GameObject player;
    private ParticleSystem[] particles=new ParticleSystem[2];
    //private Transform currBlock;
    //Use this for initialization

    void Start() {
        player = GameObject.Find("Player");
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        

        if (gameObject.transform.parent.childCount == 2)
        {
            particles[0] = gameObject.transform.parent.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();//GetComponent("Particle System") as ParticleSystem;
            particles[1] = gameObject.transform.parent.GetChild(1).GetChild(1).GetComponent<ParticleSystem>();
            particles[0].Stop();
        }
    }

    // Update is called once per frame
    void LateUpdate () {




        //find player y height
        if (player == null) return;
        if (player.transform.position.y <= transform.position.y-3.5 || player.transform.position.y >= transform.position.y+9)
        {

            //print("player pos=" + player.transform.position.y + " this blocks position:" + transform.position.y);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            if (particles[0]!=null)
            {
                for (int i = 0; i < particles.Length; i++) particles[i].Stop();
            }
        }else
        {
            GetComponent<Renderer>().enabled = true;
            if (particles[0]!=null)
            {
                for (int i = 0; i < particles.Length; i++) particles[i].Play();
            }
        }
	}
}
