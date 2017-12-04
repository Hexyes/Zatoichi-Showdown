using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_first : MonoBehaviour {

    public Transform target;
    Rigidbody theRigidBody;
    Renderer myRenderer;

    private Stats stats;
    private float targetDistance;
    // Use this for initialization
    void Start () {
        stats = GetComponent<Stats>();
        myRenderer = GetComponent<Renderer>();
        theRigidBody = GetComponent<Rigidbody>();
        target = GameObject.Find("Player").transform;
        InvokeRepeating("shoot", 0, stats.fireRate);

        Vector3 randDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f,1f));
        transform.forward = randDir;
        theRigidBody.freezeRotation = true;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            stats.isGrounded = true;
        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            stats.isGrounded = false;
        }
    }

    
    void FixedUpdate () {
        if (target == null) return;
        targetDistance = Vector3.Distance(target.position, transform.position);
        if (targetDistance > stats.viewRadius) stats.agroing = false;

        if (stats.agroing)
        {
            if (targetDistance > stats.maximumEngadge)
            {
                myRenderer.material.color = Color.red;
                move_To_Attack();
            }
            else if (targetDistance < stats.minimumEngadge)
            {
                myRenderer.material.color = Color.red;
                evade();
            }
        }
        else if (!stats.agroing)
        {
            if (targetDistance < stats.hearingDistance)
            {
                myRenderer.material.color = Color.yellow;
            }
            else
            {
                myRenderer.material.color = Color.blue;
            }
        }
	}

    void move_To_Attack()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        if (target.position.y > transform.position.y+.5f && stats.isGrounded)
        {
            jump();
        }
        
        if (stats.isGrounded)
        {
            theRigidBody.velocity = new Vector3(theRigidBody.velocity.x + direction.x * stats.moveSpeed, theRigidBody.velocity.y, theRigidBody.velocity.z + direction.z * stats.moveSpeed);

            if (theRigidBody.velocity.x > stats.moveSpeedMax) theRigidBody.velocity.Set(stats.moveSpeedMax, theRigidBody.velocity.y, theRigidBody.velocity.z);
            if (theRigidBody.velocity.z > stats.moveSpeedMax) theRigidBody.velocity.Set(theRigidBody.velocity.x, theRigidBody.velocity.y, stats.moveSpeedMax);
        }
        else
        {
            theRigidBody.velocity = new Vector3(theRigidBody.velocity.x + direction.x * stats.airSpeed, theRigidBody.velocity.y, theRigidBody.velocity.z + direction.z * stats.airSpeed);
        }

        Vector3 targetlock = new Vector3(target.position.x, theRigidBody.position.y, target.position.z);
        transform.LookAt(targetlock);


    }

    void evade()
    {
        if (target == null) return;
        Vector3 direction = transform.position - target.position;
        direction.Normalize();
        if (stats.isGrounded)
        {
            theRigidBody.velocity = new Vector3(theRigidBody.velocity.x + direction.x * stats.moveSpeed, theRigidBody.velocity.y, theRigidBody.velocity.z + direction.z * stats.moveSpeed);

            if (theRigidBody.velocity.x > stats.moveSpeedMax) theRigidBody.velocity.Set(stats.moveSpeedMax, theRigidBody.velocity.y, theRigidBody.velocity.z);
            if (theRigidBody.velocity.z > stats.moveSpeedMax) theRigidBody.velocity.Set(theRigidBody.velocity.x, theRigidBody.velocity.y, stats.moveSpeedMax);
        }
        else
        {
            theRigidBody.velocity = new Vector3(theRigidBody.velocity.x + direction.x * stats.airSpeed, theRigidBody.velocity.y, theRigidBody.velocity.z + direction.z * stats.airSpeed);
        }

        Vector3 targetlock = new Vector3(target.position.x, theRigidBody.position.y, target.position.z);
        transform.LookAt(targetlock);
        
        

    }

    void shoot()
    {
        if (target == null) return;
        if (!stats.agroing) return;

        Vector3 targetPos = target.position;

        
        GameObject shuriken = (GameObject)Instantiate(Resources.Load("projectile"), transform.position, Quaternion.identity);
        targetPos.y = transform.position.y;
        shuriken.transform.LookAt(targetPos);

        Physics.IgnoreCollision(shuriken.GetComponent<MeshCollider>(), this.GetComponent<CapsuleCollider>());
        Physics.IgnoreCollision(shuriken.GetComponent<MeshCollider>(), this.GetComponent<BoxCollider>());

        Destroy(shuriken, stats.shurikenDuration);

        // never use this again! System.Threading.Thread.Sleep(fireRate);
        
}

    void jump() {
        theRigidBody.velocity = new Vector3(theRigidBody.velocity.x, stats.jumpHeight, theRigidBody.velocity.z);
    }
    
}
