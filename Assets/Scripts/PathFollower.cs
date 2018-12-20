using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour {

    public Vector3[] path;
    public int index;

    public Material pathLineMat;

    GameObject terrain;
    GameObject closestFlocker;
    public GameObject averagePosMarker;

    public Vector3 futurePos;
    public Vector3 vehiclePosition;
    public Vector3 direction;
    public Vector3 velocity;
    public Vector3 acceleration;

    public float mass;
    public float maxSpeed;
    public float maxForce;

    bool drawDebugTools;

    public float bounceWeight;
    public float avoidWeight;
    public float separationWeight;
    public float alignmentWeight;
    public float cohesionWeight;
    public float seekWeight;

    List<GameObject> avoidList;
    List<GameObject> flockersList;
    List<GameObject> mountainsList;
    List<GameObject> pondAvoidList;

    // Use this for initialization
    void Start () {

        index = 1;
        vehiclePosition = transform.position;
        pondAvoidList = GameObject.Find("Manager").GetComponent<Manager>().pondAvoidList;
        terrain = GameObject.Find("Manager").GetComponent<Manager>().terrain;
        avoidList = GameObject.Find("Manager").GetComponent<Manager>().avoidList;
        flockersList = GameObject.Find("Manager").GetComponent<Manager>().flockersList;
        mountainsList = GameObject.Find("Manager").GetComponent<Manager>().mountainsList;
        drawDebugTools = GameObject.Find("Manager").GetComponent<Manager>().drawDebugTools;;
        closestFlocker = flockersList[0];
    }

    // Update is called once per frame
    void Update () {
        CalcSteeringForces();
        UpdatePosition();
        Rotate();
        futurePos = vehiclePosition + velocity;
        drawDebugTools = GameObject.Find("Manager").GetComponent<Manager>().drawDebugTools;

        foreach (GameObject flocker in flockersList)
        {
            if ((this.transform.position - flocker.transform.position).sqrMagnitude < (this.transform.position - closestFlocker.transform.position).sqrMagnitude)
            {
                closestFlocker = flocker;
            }
        }
    }

    public void CalcSteeringForces()
    {
        Vector3 finalForce = new Vector3();

        foreach (GameObject tree in avoidList)
        {
            finalForce += AvoidObstacle(tree) * avoidWeight;
        }

        foreach (GameObject mountain in mountainsList)
        {
            finalForce += AvoidObstacle(mountain) * avoidWeight;
        }
        foreach (GameObject item in pondAvoidList)
        {
            finalForce += AvoidObstacle(item) * avoidWeight;
        }

        //
        //PATH FOLLOWING
        //
        finalForce += Seek(path[index]);
        if(CircleCollisionPositions(this.transform.position, path[index], 4, 4))
        {
            if(index == path.Length - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }

        finalForce *= maxForce;
        ApplyForce(finalForce);

    }

    void UpdatePosition()
    {
        vehiclePosition = transform.position;
        velocity += acceleration * Time.deltaTime;
        Vector3.ClampMagnitude(velocity, maxSpeed);
        vehiclePosition += velocity * Time.deltaTime;
        direction = velocity.normalized;
        acceleration = Vector3.zero;
        vehiclePosition.y = Terrain.activeTerrain.SampleHeight(this.gameObject.transform.position);
        transform.position = vehiclePosition;
        transform.forward = direction;
    }

    public Vector3 Seek(Vector3 targetPosition)
    {
        // Desired velocity = target's pos - my pos
        Vector3 desiredVelocity = targetPosition - vehiclePosition;

        // Scale desired to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Calc steering force = desired - current velocity
        Vector3 seekingForce = desiredVelocity - velocity;

        // Return the steering force to be applied
        return seekingForce;
    }

    public Vector3 StayWithinBounds()
    {
        float height = (terrain.transform.position.x + terrain.transform.localScale.x / 2) * (terrain.GetComponent<TerrainCollider>().terrainData.size.x) + (terrain.GetComponent<TerrainCollider>().terrainData.size.x / 2);
        float width = (terrain.transform.position.z + terrain.transform.localScale.z / 2) * (terrain.GetComponent<TerrainCollider>().terrainData.size.z) + (terrain.GetComponent<TerrainCollider>().terrainData.size.z / 2);
        Vector3 terrainCenter = new Vector3(height / 2, 0, width / 2);

        if (vehiclePosition.z > width - 75)
        {
            return Seek(terrainCenter);
        }
        else if (vehiclePosition.z < 75)
        {
            return Seek(terrainCenter);
        }

        if (vehiclePosition.x > height - 75)
        {
            return Seek(terrainCenter);
        }
        else if (vehiclePosition.x < 75)
        {
            return Seek(terrainCenter);
        }
        else return Vector3.zero;
    }
    public void Rotate()
    {
        float angle = (Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg) - 90;
        this.gameObject.transform.rotation = Quaternion.Euler(0, -angle, 0);
    }
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    public Vector3 AvoidObstacle(GameObject obstacle)
    {
        Vector3 desiredVelocity = Vector3.zero;
        Vector3 vtoc = obstacle.transform.position - vehiclePosition;
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;
        float dotProductSide = Vector3.Dot(vtoc, right);
        float dotProductFront = Vector3.Dot(vtoc, forward);

        //if the obstacle is behind the object
        if (dotProductFront < 0)
        {
            return Vector3.zero;
        }
        //if the obstacle is in front of the object but it is not within a 6 units radius of the object
        else if (dotProductFront > 0 && !CircleCollision(this.gameObject, obstacle, 3, 3))
        {
            return Vector3.zero;
        }
        //if the obstacle is in front of the object and within a 6 units radius of the object
        else
        {
            //if the dot product of the vtoc and the right vector is less than the distance from the vehicle to the obstacle
            if (dotProductSide < 6)
            {
                if (dotProductSide > 0)
                {
                    //the obstacle is on the right 
                    desiredVelocity = -this.gameObject.transform.right;
                }
                else if (dotProductSide < 0)
                {
                    //the obstacle is on the left
                    desiredVelocity = this.gameObject.transform.right;
                }
            }
            else
            {
                return Vector3.zero;
            }
        }

        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;
        Vector3 avoidingForce = desiredVelocity - velocity;
        return avoidingForce;
    }
    bool CircleCollision(GameObject obj1, GameObject obj2, float rad1, float rad2)
    {
        Vector3 pos1 = obj1.transform.position;
        Vector3 pos2 = obj2.transform.position;

        return (pos2 - pos1).magnitude < (rad1 + rad2);
    }

    bool CircleCollisionPositions(Vector3 pos1, Vector3 pos2, float rad1, float rad2)
    {
        return (pos2 - pos1).magnitude < (rad1 + rad2);
    }

    public void OnRenderObject()
    {
        if (drawDebugTools)
        {
            pathLineMat.SetPass(0);

            for (int i = 0; i < path.Length; i++)
            {
                if(i == path.Length - 1)
                {
                    GL.Begin(GL.LINES);
                    GL.Vertex(path[i]);
                    GL.Vertex(path[0]);
                    GL.End();
                }
                else
                {
                    GL.Begin(GL.LINES);
                    GL.Vertex(path[i]);
                    GL.Vertex(path[i + 1]);
                    GL.End();
                }
                
            }

            GL.Begin(GL.LINES);
            GL.Vertex(new Vector3(90, Terrain.activeTerrain.SampleHeight(new Vector3(90, 0f, 100)) + 6, 100));
            GL.Vertex(new Vector3(130, Terrain.activeTerrain.SampleHeight(new Vector3(130, 0f, 100)) + 6, 100));
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(new Vector3(130, Terrain.activeTerrain.SampleHeight(new Vector3(130, 0f, 100)) + 6, 100));
            GL.Vertex(new Vector3(130, Terrain.activeTerrain.SampleHeight(new Vector3(130, 0f, 140)) + 6, 140));
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(new Vector3(130, Terrain.activeTerrain.SampleHeight(new Vector3(130, 0f, 140)) + 6, 140));
            GL.Vertex(new Vector3(90, Terrain.activeTerrain.SampleHeight(new Vector3(90, 0f, 140)) + 6, 140));
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(new Vector3(90, Terrain.activeTerrain.SampleHeight(new Vector3(90, 0f, 140)) + 6, 140));
            GL.Vertex(new Vector3(90, Terrain.activeTerrain.SampleHeight(new Vector3(90, 0f, 100)) + 6, 100));
            GL.End();

        }
    }
}
