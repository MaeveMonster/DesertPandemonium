using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldFollower : MonoBehaviour {

    Vector3[,] flowField = new Vector3[1000,1000];

    public Material forwardLineMatt;
    public Material rightLineMatt;
    public Material averageDirLineMatt;
    public Material fieldLineMat;

    GameObject terrain;
    GameObject closestFlocker;
    public GameObject averagePosMarker;

    public Vector3 futurePos;
    public Vector3 vehiclePosition;
    public Vector3 direction;
    public Vector3 velocity;
    public Vector3 acceleration;
    Vector3 averagePos;
    Vector3 averageDirection;

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
    public float fleeWeight;

    List<GameObject> avoidList;
    List<GameObject> flockersList;
    List<GameObject> mountainsList;
    List<GameObject> pondAvoidList;

    // Use this for initialization
    void Start () {

        float x = 0; float z = 0;

		for(int i = 0; i < 200; i++)
        {
            for(int j = 0; j < 200; j++)
            {
                flowField[i, j] = new Vector3 (Mathf.PerlinNoise(i/20f, j/20f), 0, Mathf.PerlinNoise(i/20f, j/20f));
               
            }
        }

        vehiclePosition = transform.position;
        pondAvoidList = GameObject.Find("Manager").GetComponent<Manager>().pondAvoidList;
        terrain = GameObject.Find("Manager").GetComponent<Manager>().terrain;
        avoidList = GameObject.Find("Manager").GetComponent<Manager>().avoidList;
        flockersList = GameObject.Find("Manager").GetComponent<Manager>().flockersList;
        mountainsList = GameObject.Find("Manager").GetComponent<Manager>().mountainsList;
        closestFlocker = flockersList[0];
    }
    
    // Update is called once per frame
    void Update()
    {
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

    public Vector3 Flow()
    {
        return flowField[(int)futurePos.x, (int)futurePos.z];
    }

    public void CalcSteeringForces()
    {
        Vector3 finalForce = new Vector3();
        finalForce += StayWithinBounds() * bounceWeight;
        finalForce += SinkHole();

        foreach(GameObject flocker in flockersList)
        {
            if(CircleCollision(this.gameObject, flocker, 5, 5))
            {
                finalForce += Flee(flocker.transform.position) * fleeWeight;
            }
        }

        finalForce += new Vector3(10, 0, 10);

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

        finalForce += Flow().normalized*maxSpeed-velocity;

        Vector3.ClampMagnitude(finalForce, maxForce);
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

    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    public void Rotate()
    {
        float angle = (Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg) - 90;
        this.gameObject.transform.rotation = Quaternion.Euler(0, -angle, 0);
    }

    public Vector3 StayWithinBounds()
    {
        float height = (terrain.transform.position.x + terrain.transform.localScale.x / 2) * (terrain.GetComponent<TerrainCollider>().terrainData.size.x) + (terrain.GetComponent<TerrainCollider>().terrainData.size.x / 2);
        float width = (terrain.transform.position.z + terrain.transform.localScale.z / 2) * (terrain.GetComponent<TerrainCollider>().terrainData.size.z) + (terrain.GetComponent<TerrainCollider>().terrainData.size.z / 2);
        Vector3 terrainCenter = new Vector3(height / 2, 0, width / 2);

        if (vehiclePosition.z > width - 50)
        {
            return Seek(terrainCenter);
        }
        else if (vehiclePosition.z < 50)
        {
            return Seek(terrainCenter);
        }

        if (vehiclePosition.x > height - 50)
        {
            return Seek(terrainCenter);
        }
        else if (vehiclePosition.x < 50)
        {
            return Seek(terrainCenter);
        }
        else return Vector3.zero;
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

    public Vector3 Flee(Vector3 targetPosition) //where targetPosition is the seeker's future position
    {
        //Vector3 desiredVelocity = Vector3.MoveTowards(vehiclePosition, targetPosition, maxSpeed);
        Vector3 desiredVelocity = vehiclePosition - targetPosition;

        // Scale desired to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Calc steering force = desired - current velocity
        Vector3 fleeingForce = desiredVelocity - velocity;

        // Return the steering force to be applied
        return fleeingForce;
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

    public Vector3 Wander()
    {
        Vector3 circleCenter = vehiclePosition + this.gameObject.transform.forward;
        float radius = maxSpeed / 2;
        float angle = Random.Range(0f, 2f * Mathf.PI);

        float randX = radius * Mathf.Cos(angle);
        float yLoc = 0f;
        float randZ = radius * Mathf.Sin(angle);

        Vector3 target = circleCenter + new Vector3(randX, yLoc, randZ);

        return Seek(target);
    }

    bool CircleCollision(GameObject obj1, GameObject obj2, float rad1, float rad2)
    {
        Vector3 pos1 = obj1.transform.position;
        Vector3 pos2 = obj2.transform.position;

        return (pos2 - pos1).magnitude < (rad1 + rad2);
    }

    public Vector3 SinkHole()
    {
        float coDrag = 0.09f;
        float dragMag = coDrag * maxSpeed * maxSpeed;
        Vector3 drag = new Vector3();
        drag.x = velocity.x;
        drag.y = velocity.y;
        drag.z = velocity.z;
        drag *= -1;
        drag.Normalize();
        drag *= dragMag;

        if(this.transform.position.x >= 90 && this.transform.position.x <= 130)
        {
            if(this.transform.position.z >= 100 && this.transform.position.z <= 140)
            {
                return drag;
            }
        }
        else
        {
            return Vector3.zero;
        }
        return Vector3.zero;

    }

    public void OnRenderObject()
    {
        if (drawDebugTools)
        {
            fieldLineMat.SetPass(0);

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    Vector3 line = new Vector3(i, 40, j);
                    GL.Begin(GL.LINES);
                    GL.Vertex(line);
                    GL.Vertex(line + flowField[i, j].normalized);
                    GL.End();
                }
            }
        }   
    }
}


