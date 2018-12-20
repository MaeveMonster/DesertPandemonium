using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public GameObject terrain;
    public GameObject flocker;
    public GameObject flowFieldFollower;
    public GameObject pathFollower;
    public GameObject grass1;
    public GameObject grass2;
    public GameObject log1;
    public GameObject log2;
    public GameObject log3;
    public GameObject plant1;
    public GameObject plant2;
    public GameObject plant3;
    public GameObject plant4;
    public GameObject plant5;
    public GameObject plant6;
    public GameObject plant7;
    public GameObject rock1;
    public GameObject rock2;
    public GameObject rock3;
    public GameObject rock4;
    public GameObject rock5;
    public GameObject rock6;
    public GameObject stone;
    public GameObject tree1;
    public GameObject tree2;
    public GameObject tree3;

    public GameObject flockTracker;

    public List<GameObject> flockersList = new List<GameObject>();
    List<GameObject> flowFieldFollowersList = new List<GameObject>();
    public List<GameObject> objectsList = new List<GameObject>();
    public List<GameObject> avoidList = new List<GameObject>();
    public List<GameObject> pondAvoidList = new List<GameObject>();

    public List<GameObject> mountainsList;

    public bool drawDebugTools;

	// Use this for initialization
	void Start () {

        flockTracker = Instantiate(flockTracker, Vector3.zero, Quaternion.Euler(Vector3.zero));
        flockTracker.SetActive(false);

        for (int i = 0; i < 100; i++)
        {
            objectsList.Add(RandSpawn(grass1));
            objectsList.Add(RandSpawn(grass2));
            objectsList.Add(RandSpawn(stone));
        }

        for (int i = 0; i < 5; i++)
        {
            GameObject p1 = RandSpawn(plant1);
            GameObject p2 = RandSpawn(plant2);
            GameObject p3 = RandSpawn(plant3);
            GameObject p4 = RandSpawn(plant4);
            GameObject p5 = RandSpawn(plant5);
            GameObject p6 = RandSpawn(plant6);
            GameObject p7 = RandSpawn(plant7);


            objectsList.Add(p1);
            objectsList.Add(p2);
            objectsList.Add(p3);
            objectsList.Add(p4);
            objectsList.Add(p5);
            objectsList.Add(p6);
            objectsList.Add(p7);

            avoidList.Add(p1);
            avoidList.Add(p2);
            avoidList.Add(p3);
            avoidList.Add(p4);
            avoidList.Add(p5);
            avoidList.Add(p6);
            avoidList.Add(p7);
        }

        for(int i = 0; i < 3; i++)
        {
            GameObject t1 = RandSpawn(tree1);
            GameObject t2 = RandSpawn(tree2);
            GameObject t3 = RandSpawn(tree3);

            objectsList.Add(t1);
            objectsList.Add(t2);
            objectsList.Add(t3);

            avoidList.Add(t1);
            avoidList.Add(t2);
            avoidList.Add(t3);
        }

        for(int i = 0; i < 10; i++)
        {
            GameObject r1 = RandSpawn(rock1);
            GameObject r2 = RandSpawn(rock2);
            GameObject r3 = RandSpawn(rock3);
            GameObject r4 = RandSpawn(rock4);
            GameObject r5 = RandSpawn(rock5);
            GameObject r6 = RandSpawn(rock6);

            GameObject l1 = RandSpawn(log1);
            GameObject l2 = RandSpawn(log2);
            GameObject l3 = RandSpawn(log3);

            objectsList.Add(r1);
            objectsList.Add(r2);
            objectsList.Add(r3);
            objectsList.Add(r4);
            objectsList.Add(r5);
            objectsList.Add(r6);

            objectsList.Add(l1);
            objectsList.Add(l2);
            objectsList.Add(l3);

            avoidList.Add(r1);
            avoidList.Add(r2);
            avoidList.Add(r3);
            avoidList.Add(r4);
            avoidList.Add(r5);
            avoidList.Add(r6);

            avoidList.Add(l1);
            avoidList.Add(l2);
            avoidList.Add(l3);
        }

        foreach(GameObject item in objectsList)
        {
            if (item.transform.position.x >= 90 && item.transform.position.x <= 130)
            {
                if (item.transform.position.z >= 100 && item.transform.position.z <= 140)
                {
                    item.SetActive(false);
                }
            }
        }

        for (float i = 0; i < 20f; i++)
        {
            int randZ = Random.Range(0, 10);
            int randX = Random.Range(0, 10);
            int randSign = Random.Range(0, 16);

            Vector3 pos;

            if(randSign == 0)
            {
                pos = new Vector3(126f + i + (float)randX, 0, 126f + i + (float)randZ);
            }
            else if(randSign == 1)
            {
                pos = new Vector3(126f + i - (float)randX, 0, 126f + i + (float)randZ);
            }
            else if(randSign == 2)
            {
                pos = new Vector3(126f + i + (float)randX, 0, 126f - i + (float)randZ);
            }
            else if (randSign == 3)
            {
                pos = new Vector3(126f + i + (float)randX, 0, 126f + i - (float)randZ);
            }
            else if (randSign == 4)
            {
                pos = new Vector3(126f - i + (float)randX, 0, 126f + i + (float)randZ);
            }
            else if (randSign == 5)
            {
                pos = new Vector3(126f - i - (float)randX, 0, 126f - i - (float)randZ);
            }
            else if (randSign == 6)
            {
                pos = new Vector3(126f + i - (float)randX, 0, 126f - i - (float)randZ);
            }
            else if (randSign == 7)
            {
                pos = new Vector3(126f - i + (float)randX, 0, 126f - i - (float)randZ);
            }
            else if (randSign == 8)
            {
                pos = new Vector3(126f - i - (float)randX, 0, 126f + i - (float)randZ);
            }
            else if (randSign == 9)
            {
                pos = new Vector3(126f - i - (float)randX, 0, 126f - i + (float)randZ);
            }
            else if (randSign == 10)
            {
                pos = new Vector3(126f - i - (float)randX, 0, 126f + i + (float)randZ);
            }
            else if (randSign == 11)
            {
                pos = new Vector3(126f + i + (float)randX, 0, 126f - i - (float)randZ);
            }
            else if (randSign == 12)
            {
                pos = new Vector3(126f + i - (float)randX, 0, 126f - i + (float)randZ);
            }
            else if (randSign == 13)
            {
                pos = new Vector3(126f - i + (float)randX, 0, 126f + i - (float)randZ);
            }
            else if (randSign == 14)
            {
                pos = new Vector3(126f - i + (float)randX, 0, 126f - i + (float)randZ);
            }
            else
            {
                pos = new Vector3(126f + i - (float)randX, 0, 126f + i - (float)randZ);
            }

            float yValue = terrain.GetComponent<Terrain>().SampleHeight(pos) + 0.3f;
            pos.y = yValue;
            GameObject spawned = Instantiate(flocker, pos, Quaternion.Euler(Vector3.zero));
            flockersList.Add(spawned);
            avoidList.Add(spawned);
        }

        flowFieldFollower = RandSpawn(flowFieldFollower);
        avoidList.Add(flowFieldFollower);

        pathFollower = Instantiate(pathFollower, new Vector3(32, Terrain.activeTerrain.SampleHeight(new Vector3(31, 0, 117)), 118), Quaternion.Euler(Vector3.zero));
        avoidList.Add(pathFollower);
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 direction = flockersList[0].GetComponent<Flocker>().averageDirection;

        flockTracker.transform.position = flockersList[0].GetComponent<Flocker>().flockPos;
        float angle = (Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg) - 90;
        flockTracker.transform.rotation = Quaternion.Euler(0, -angle, 0);

        if (Input.GetKeyDown(KeyCode.D))
        {
            drawDebugTools = !drawDebugTools;
            flockTracker.SetActive(drawDebugTools);
        }

    }

    GameObject RandSpawn(GameObject original)
    {
        float randX = (Random.Range(terrain.transform.position.x - terrain.transform.localScale.x / 2, terrain.transform.position.x + terrain.transform.localScale.x / 2) * terrain.GetComponent<TerrainCollider>().terrainData.size.x) + (terrain.GetComponent<TerrainCollider>().terrainData.size.x / 2);
        float randZ = (Random.Range(terrain.transform.position.z - terrain.transform.localScale.z / 2, terrain.transform.position.z + terrain.transform.localScale.z / 2) * terrain.GetComponent<TerrainCollider>().terrainData.size.z) + (terrain.GetComponent<TerrainCollider>().terrainData.size.x / 2);
        float yLoc = Terrain.activeTerrain.SampleHeight(new Vector3(randX, 0f, randZ));

        float randRot = Random.Range(0, 360);

        GameObject spawned = Instantiate(original, new Vector3(randX, yLoc, randZ), Quaternion.Euler(new Vector3(0, randRot, 0)));
        return spawned;
    }

    bool CircleCollision(GameObject obj1, GameObject obj2, float rad1, float rad2)
    {
        Vector3 pos1 = obj1.transform.position;
        Vector3 pos2 = obj2.transform.position;

        return (pos2 - pos1).magnitude < (rad1 + rad2);
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(150, 50, 250, 40), "Press D to toggle debug lines.\nPress C to cycle through the cameras");
    }
}
