using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {

        if (col.gameObject.name == "Player")
        {
            col.attachedRigidbody.useGravity = false;
            col.gameObject.GetComponent<Stats>().onRope = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.name == "Player")
        {
            col.gameObject.GetComponent<Stats>().onRope = false;
            col.attachedRigidbody.useGravity = true;
            Destroy(this.gameObject);
        }
    }
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
