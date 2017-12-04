using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public GameObject PassableCube;
    public GameObject targetPlane;

    public int num_Enemies = 5;
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    //not yet implimented public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] enemyTiles;

    private Transform boardHolder;

    //places the ground tiles on the map, calls build map base to get an array to do this with.
    void BoardSetup()
    {
        bool passable = false;
        targetPlane = Resources.Load("Plane") as GameObject;

        boardHolder = new GameObject("Board").transform;
        int[,,] ground = BuildMapBase(floorTiles);
        PassableCube = Resources.Load("PassableBlock") as GameObject;
        for (int z = 0; z < 3; z++)
        {
            GameObject planeInstance = Instantiate(targetPlane, new Vector3(0, (z * 8)+.9f, 0), Quaternion.identity) as GameObject;
            planeInstance.transform.SetParent(boardHolder);
            //still hits above planes.
            planeInstance.GetComponent<MeshRenderer>().enabled = false;
            this.GetComponentInParent<GameManager>().planes[z] = planeInstance;

            for (int x = -1; x < columns + 1; x++)
            {
                for (int y = -1; y < rows + 1; y++)
                {
                    passable = false;
                    GameObject toInstantiate;
                    if (x == -1 || x == columns || y == -1 || y == rows)
                    {
                        toInstantiate = outerWallTiles[0];
                    }
                    else
                    {
                        //if this ground tile is not in the list of tiles
                        if (ground[x, y, z] >= floorTiles.Length)
                        {
                            toInstantiate = PassableCube;
                            GameObject passableInstance = Instantiate(toInstantiate, new Vector3(x, (z * 8)+.5f, y), Quaternion.identity) as GameObject;
                            passableInstance.transform.SetParent(boardHolder);
                            continue;
                        }
                        toInstantiate = floorTiles[ground[x, y, z]];
                        if (z<2 && ground[x, y, z+1] >= floorTiles.Length)
                        {
                            passable = true;
                        }
                    }
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, z * 8, y), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);
                    
                    if (passable)
                    {
                        
                        //attach particle effect to instance.
                        GameObject particleEffect = Instantiate(Resources.Load("HoleAbove"), new Vector3(x-.5f, (z*8)+1.5f, y), Quaternion.identity) as GameObject;
                        particleEffect.transform.SetParent(instance.transform);
                    }
                    
                }
            }
        }
    }

    //build the ground tile setup in an array of ints
    private int[,,] BuildMapBase(GameObject[] tileArray)
    {
        int[,,] map = new int[columns, rows, 3];

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < columns - 1; y++)
            {
                map[x, y, 0] = Random.Range(0, tileArray.Length);
            }
        }
        for (int z = 1; z < 3; z++)
        {
            for (int x = 1; x < columns - 1; x++)
            {
                for (int y = 1; y < columns - 1; y++)
                {
                    map[x, y, z] = Random.Range(0, tileArray.Length + 1 + z);
                    //if (z == 2) print("map val:"+map[x, y, z]);
                }
            }
        }

        //pearlin noise goes here be sure to add constraints to prevent null tiles being placed.
        return map;
    }

    //builds an array with the given tile set, used for pickups enemies ect.
    private int[,,] BuildMap(GameObject[] tileArray, int minimum, int maximum, int objectCount)
    {
        objectCount = 5; //Random.Range(minimum, maximum);
        int[,,] map = new int[columns, rows, 3];
        for (int z = 0; z < 3; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {

                    map[x, y, z] = -1;
                }
            }
        }
        while (objectCount > 0)
        {
            int z = 0;// Random.Range(0, 2);
            int x = Random.Range(1, columns - 1);
            int y = Random.Range(1, rows - 1);
            //check to make sure there's nothing else on the tiles
            if (map[x, y, z] == -1)
            {
                int tileVersion = Random.Range(0, tileArray.Length);
                map[x, y, z] = tileVersion;
                objectCount--;
            }

        }
        return map;
    }

    //uses arrays to place the tiles in their proper location.
    void LayoutObjectAtRandom(GameObject[] tileArray, int[,,] map, List<GameObject> tracker)
    {

        //int objectCount = Random.Range(minimum, maximum + 1);
        //for (int z = 0; z < 3; z++)
        //{
        for (int z = 0; z < 3; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (map[x, y, z] == -1) continue;

                    //Vector3 randomPosition = RandomPosition();
                    GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                    tracker.Add(Instantiate(tileChoice, new Vector3(x, 2.0f + z * 8, y), Quaternion.identity));
                }
            }
        }
        //}
    }

    //level not instatiated designed for dungeon level type rogue like.
    public void SetupScene(int level, List<GameObject> tracker)
    {

        BoardSetup();
        int[,,] map = BuildMap(enemyTiles, 1, 2, num_Enemies); //enemies being placed
        LayoutObjectAtRandom(enemyTiles, map, tracker);
        //depreciated
        //InitializeList();


        //not yet in use, this is the format for filling the random landscape.
        //LayoutObjectAtRandom(outerWallTiles, WallCount.minimum, wallCount.maximum);
    }
}
