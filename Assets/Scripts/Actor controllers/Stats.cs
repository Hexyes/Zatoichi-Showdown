using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stats : MonoBehaviour {

    //based on character
    public int currhitpoints;
    public int maxHitPoints;
    public int shurikenDammage;
    public float jumpHeight;
    public float moveSpeedMax = 4f;
    public float moveSpeed = 1f;
    public float airSpeed = .1f;
    public float fireRate = 1;
    public float shurikenDuration = 1;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public GameObject pre_Rope;

    //AI only
    [HideInInspector]
    public bool agroing=false;

    public float maximumEngadge = 1.75f;
    public float minimumEngadge = 1.25f;
    public float hearingDistance;
    

    //current states
    public bool isGrounded = true;
    public bool onRope = false;

    private List<Transform> targ;

    void Start () {
        pre_Rope = Resources.Load("Rope") as GameObject;
        targ = new List<Transform>();
        currhitpoints = maxHitPoints;
        //initialize stats for enemy
        if (gameObject.name == "Enemy(Clone)")
        {
            Invisible();
            //shurikenDammage = 1;
            //currhitpoints = 10;
        }
        if (gameObject.name == "Player")
        {
            //shurikenDammage = 1;
            //currhitpoints = 10;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (targ.Count>0)
        {
            for(int i=targ.Count-1; i>=0; i--)
            {
                if (targ[i] == null)
                {
                    targ.RemoveAt(i);
                    targ.TrimExcess();
                    continue;
                }
                float dstToTarget = Vector3.Distance(transform.position, targ[i].position);
                if (dstToTarget > targ[i].GetComponent<Stats>().viewRadius)
                {
                    if(this.name=="Enemy(Clone)")Invisible();
                    if (this.name == "Player") targ[i].GetComponent<Stats>().agroing=false;
                    targ.RemoveAt(i);
                    targ.TrimExcess();
                }
            }
        }
    }

    public void checkVis(Transform newTarg)
    {
        if (!targ.Contains(newTarg)) targ.Add(newTarg);
    }

    public void takeDamage(int dmg)
    {
        
        currhitpoints -= dmg;
        if(this.name == "Enemy(Clone)")
        {
            
            agroing = true;
            if (currhitpoints < 1)
            {
                FindObjectOfType<GameManager>().enemyList.Remove(gameObject);
                Destroy(gameObject);
            }
        }
        else if(currhitpoints < 1 && this.name == "Player")
        {
            Destroy(gameObject);

        }
    }

    public void Visible()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        transform.Find("ViewVisualization").GetComponent<MeshRenderer>().enabled = true;
    }
    public void Invisible()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        transform.Find("ViewVisualization").GetComponent<MeshRenderer>().enabled = false;
    }
}
