using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomStrike : MonoBehaviour
{
    public float speed=2;
    public int dammage;
    // Use this for initialization

    
    void Start ()
    {
        
    }

    void OnTriggerExit(Collider collision)
    {
        GameObject actor = collision.gameObject;

        if (actor.layer == 11)
        {
            Stats temp = actor.GetComponent<Stats>();
            temp.takeDamage(1);
        }
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
        transform.Translate(Vector3.forward*speed*Time.deltaTime);
	}
}
