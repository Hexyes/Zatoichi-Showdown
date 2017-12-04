using System.Collections;
using UnityEngine;

public class NinjaStar : MonoBehaviour {
    
    //public int numStars=3;
    public float duration = 3;
    GameObject prefab;

    //private float angle;

    // Use this for initialization
    void Start () {

        prefab = Resources.Load("projectile") as GameObject;	
	}

    public void ThrowStar () {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit Hit;
            Vector3 targetPos;
            int layerMask = LayerMask.GetMask("Plane");
            if (Physics.Raycast(ray, out Hit, 100, layerMask))
            {
                targetPos = Hit.point;
        }else
        {
            return;
        }

        //targetPos = Hit.point;
        //targetPos.y = (float)(transform.position.y+1);
        //targetPos.z -= 1;

        Vector3 spawnPos = transform.position;//new Vector3((float)(transform.position.x), (float)(transform.position.y+1), transform.position.z);
            //spawnPos.y;
            GameObject shuriken = (GameObject)Instantiate(prefab, spawnPos, Quaternion.identity);
            shuriken.GetComponent<CustomStrike>().dammage = GetComponent<Stats>().shurikenDammage;
            targetPos.y = spawnPos.y; // transform.position.y;
            shuriken.transform.LookAt(targetPos);
   
            Physics.IgnoreCollision(shuriken.GetComponent<MeshCollider>(), GetComponent<CapsuleCollider>());
            //Physics.IgnoreCollision(shuriken.GetComponent<MeshCollider>(), GetComponent<BoxCollider>());

            Destroy(shuriken, duration);
        
        
        }
}
