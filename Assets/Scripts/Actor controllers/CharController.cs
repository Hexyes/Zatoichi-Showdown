using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharController : MonoBehaviour {

    //players character sheet basically
    private Stats stats;
  
    //used for camera vs controlls setup
    Vector3 forward, right;
    
    //general player controll variables
    Rigidbody rb;
    GameObject view;
    private float nextFire = 0.0f;

    //for UI controlling:
    [SerializeField]
    private Image healthBar;

    public Text enemyText;
    private float maxHealth;
    public GameManager GM;

    //for player passing through blocks
    private List<Collider> listFeels;
    private List<Collider> turnON;

    // Use this for initialization this is run when the player is spawned in.
    void Start () {
        stats = this.GetComponent<Stats>();// the players stats

        //for passable terrain
        listFeels = new List<Collider>();//things the player "feels" or, is touching
        turnON = new List<Collider>();//things the player is no longer touching and need to be turned back on.

        //UI elements startup:
        GM = FindObjectOfType<GameManager>();
        maxHealth = stats.maxHitPoints;

       

        //movement set up based on camera angle so that "up" key moves player towards the top of the screen.
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        rb = GetComponent<Rigidbody>();
	}



    // Update is called once per frame
    void Update() {

        //always up for players who are dying in menu
        HandleHealth();
        //player can still look around while in menu
        LookToMouse();

        //if condition this to determine if the player is in the menu or not.
        character_controlls();

	}

    void character_controlls()
    {
        //input names handled in unity input function
        //movements, grounded state is handled in the functions.
        if (Input.GetButton("HorizontalKey") || Input.GetButton("VerticalKey"))
        {
            Move();
        }
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        //if left click
        if (Input.GetButton("Shuriken"))
        {
            if (nextFire < Time.time)
            {
                GetComponent<NinjaStar>().ThrowStar();
                nextFire = Time.time + stats.fireRate;
            }
        }
        //if right click use grappling hook function
        if (Input.GetButtonDown("GrapplingHook"))
        {
            grapplingHook();
        }
        //controlls for passing through blocks
        if (Input.GetButton("PassThrough"))
        {
            passthrough();
        }
        else
        {
            //i'm not sure where i could move this structure. this is used to re set passable blocks to solid
            for (int i = 0; i < turnON.Count; i++)
            {
                Physics.IgnoreCollision(turnON[i], GetComponent<CapsuleCollider>(), false);
            }
            turnON.Clear();
        }

        //enter menu
        if (Input.GetButtonDown("Menu"))
        {
            print("menu pressed");
        }
    }

    //used for moving through blocks that are above the player when on a grapple line
    void passthrough()
    {
        for (int i = 0; i < listFeels.Count; i++)
        {
            if (listFeels[i].transform.name == ("PassableBlock(Clone)"))
            {
                Physics.IgnoreCollision(listFeels[i], GetComponent<CapsuleCollider>());
                turnON.Add(listFeels[i]);
            }
        }
    }

    //used to lay a grappling line
    void grapplingHook()
    {
        if (!stats.onRope)
        {
            GameObject rope = (GameObject)Instantiate(stats.pre_Rope, transform.position, Quaternion.identity);
        }else if (stats.onRope)
        {
            GameObject rope = GameObject.Find("Rope(Clone)");
            stats.onRope = false;
            rb.useGravity = true;
            Destroy(rope);
        }
    }

    //used to set player view angle toward the mouse at the current z plane
    void LookToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
        Vector3 targetPos;
        int layerMask = LayerMask.GetMask("Plane");
        if (Physics.Raycast(ray, out Hit, 100, layerMask))
        {
            targetPos = Hit.point;
            targetPos.y = 0;
            Vector3 lookPos = Vector3.Normalize(targetPos - transform.position);
            lookPos.y = 0;
            transform.forward = lookPos;
        }
    }

    //move function handles grounded,ongrapple, and non grounded player states
    void Move()
    {
        if (stats.onRope)
        {
            if (Input.GetButton("VerticalKey")){
                float velocity = stats.moveSpeed * Input.GetAxis("VerticalKey");
                rb.velocity = new Vector3(0, velocity, 0);
            }
        }
        else if(stats.isGrounded)
        {
            Vector3 rightMovement = right * stats.moveSpeed * Input.GetAxis("HorizontalKey");
            Vector3 upMovement = forward * stats.moveSpeed * Input.GetAxis("VerticalKey");
            rb.velocity = new Vector3(rightMovement.x + upMovement.x + rb.velocity.x, rb.velocity.y, rightMovement.z + upMovement.z + rb.velocity.z);
            if (rb.velocity.x > stats.moveSpeedMax) rb.velocity.Set(stats.moveSpeedMax, rb.velocity.y, rb.velocity.z);
            if (rb.velocity.z > stats.moveSpeedMax) rb.velocity.Set(rb.velocity.x, rb.velocity.y, stats.moveSpeedMax);
        }
        else
        {
            Vector3 rightMovement = right * stats.airSpeed * Input.GetAxis("HorizontalKey");
            Vector3 upMovement = forward * stats.airSpeed * Input.GetAxis("VerticalKey");
            rb.velocity = new Vector3(rightMovement.x + upMovement.x + rb.velocity.x, rb.velocity.y, rightMovement.z + upMovement.z + rb.velocity.z);
            if (rb.velocity.x > stats.moveSpeedMax) rb.velocity.Set(stats.moveSpeedMax, rb.velocity.y, rb.velocity.z);
            if (rb.velocity.z > stats.moveSpeedMax) rb.velocity.Set(rb.velocity.x, rb.velocity.y, stats.moveSpeedMax);
        }
    }

    //used for passable blocks in order to determine if a block is passable
    void OnCollisionEnter(Collision collision)
    {
        listFeels.Add(collision.collider);
    }

    //used to determine if the player is currently grounded
    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.layer == 8)
        {
            stats.isGrounded = true;
        }

    }

    //used to reset player's grounded state.
    void OnCollisionExit(Collision collision)
    {
        listFeels.Remove(collision.collider);
        if (collision.gameObject.layer == 8)
        {
            stats.isGrounded = false;
        }
    }

    //jump function determines if player is on a rope or the ground and then executes movement based on that determination
    void Jump()
    {
        if (stats.onRope)
        {
            //i'm not entirely convinced this is working as intended. it should "fling" the player in the direction of the mouse cursor.
            rb.velocity = new Vector3(transform.forward.x*stats.moveSpeedMax, stats.jumpHeight, transform.forward.y*stats.moveSpeed);
        }
        else if(stats.isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, stats.jumpHeight, rb.velocity.z);
        }
    }

    //used for the HUD to properly display players current health and the number of enemies currently in play
    void HandleHealth()
    {
        float currentHealth = stats.currhitpoints;
        enemyText.text = ("Goon AI Left: " + GM.enemyList.Count);
        healthBar.fillAmount = (currentHealth/maxHealth);
    }

    //i think this is depreciated, and was used for the field of view script (now seperated
    float Map_Values(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
