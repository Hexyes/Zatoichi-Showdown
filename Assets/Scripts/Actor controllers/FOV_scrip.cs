using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV_scrip : MonoBehaviour {

    //private Stats stats;
    public LayerMask actors = 11;
    public LayerMask obstacleMask = 8;
    public float viewRadius;
    public float viewAngle;

    public float meshResolution;

    public MeshFilter viewMeshFilter;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();


 
    public Mesh viewMesh;
    //private List<GameObject> hideables;
    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = this.name+"View Mesh";
        viewMeshFilter.mesh = viewMesh;
        viewRadius = this.GetComponent<Stats>().viewRadius;
        viewAngle = this.GetComponent<Stats>().viewAngle;
        StartCoroutine("FindTargetsWithDelay", .2f);
        
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            
        }
    }


    void LateUpdate()
    {
        
        //if (gameObject.GetComponent<Renderer>().enabled) {
            DrawFieldOfView();
        //}
    }


    void FindVisibleTargets()
    {

        //need to pick list of all items that should be removed(shurikens, terrain ect)
        List<Collider> targetsInViewRadius = new List<Collider>(Physics.OverlapCapsule(transform.position, transform.position, viewRadius, actors));
        for (int i = 0; i < targetsInViewRadius.Count; i++)
        {
            //this block if for removing targets that have been killed and destroyed
            //this is for the editor
            visibleTargets.Clear();
            //if this target had been removed from scene
            if (targetsInViewRadius[i].transform == null)
            {
                visibleTargets.Remove(targetsInViewRadius[i].transform);
                targetsInViewRadius.Remove(targetsInViewRadius[i]);
                continue;
            }

            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float dstToTarget = Vector3.Distance(transform.position, target.position);
            if(this.name!=target.name) targetsInViewRadius[i].GetComponent<Stats>().checkVis(transform);
            //if this target is in the view radius
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
          
                //if there are no obstacles between this and target
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    
                    //target is visible here
                    visibleTargets.Add(target); // just add targets to list because of editor reasons
                    if (target.name == "Enemy(Clone)" && this.name == "Player") target.GetComponent<Stats>().Visible();
                    if (target.name == "Player") this.GetComponent<Stats>().agroing = true;
                }
                //if there is an obstacle
                else
                {
                    try
                    {
                        if (target.name == "Enemy(Clone)" && this.name == "Player") target.GetComponent<Stats>().Invisible();
                        if (target.name == "Player") this.GetComponent<Stats>().agroing = false;
                    }
                    catch(System.ArgumentException e)
                    {}
                    
                }
            }
            //if target is outside of view angle
            else
            {
                try
                {
                    if (target.name == "Enemy(Clone)" && this.name == "Player") target.GetComponent<Stats>().Invisible();
                    if (target.name == "Player") this.GetComponent<Stats>().agroing = false;
                }
                catch (System.ArgumentException e)
                { }
            }
            //if the target has become too far away
            if (dstToTarget > viewRadius) {
                try
                {
                    if (target.name == "Enemy(Clone)" && this.name == "Player") target.GetComponent<Stats>().Invisible();
                    if (target.name == "Player") this.GetComponent<Stats>().agroing = false;
                    //targetsInViewRadius.Remove(targetsInViewRadius[i]);
                }
                catch (System.ArgumentException e)
                { }
            }
        }
        /*
        for(int i=targetsInViewRadius.Count-1; i>=0; i--)
        {
            Collider targ = targetsInViewRadius[i];
            float dstToTarget = Vector3.Distance(transform.position, targ.transform.position);
            if (dstToTarget>viewRadius) {
                if(targ.transform.name=="Enemy(clone)")targ.transform.GetComponent<Stats>().Invisible();
                targetsInViewRadius.RemoveAt(i);
            }
        }
        */
    }

    void DrawFieldOfView()
    {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);
                viewPoints.Add(newViewCast.point);
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] verticies = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];
            verticies[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                verticies[i + 1] = this.transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    //print("object name:" + this.name + " vertacies:" +triangles[i*3]+","+triangles[i * 3 + 1]+","+triangles[i * 3+2]);
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();
            viewMesh.vertices = verticies;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        
        

    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }else
        {
            return new ViewCastInfo(false, transform.position+dir*viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDeg, bool angleIsGlobal) 
    {
        //if (!angleIsGlobal) angleInDeg += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDeg * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

}
