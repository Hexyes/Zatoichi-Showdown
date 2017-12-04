using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public LevelManager boardScript;

    public GameObject[] planes = new GameObject[3];
    public List<GameObject> enemyList;
    private GameObject player;
    private float timer=0.0f;
    private string scene = "";
    void Awake()
    {
        boardScript = GetComponent<LevelManager>();
        player = GameObject.Find("Player");
        InitGame();
        Physics.IgnoreLayerCollision(11, 10);
        
	}

    void InitGame()
    {
        boardScript.SetupScene(1, enemyList);
    }
	// Update is called once per frame
	void Update () {
        PlaneCheck();
        if (timer == 0f)
        {
            if (player == null)
            {
                //print("this happened");
                scene = "GameOverScreen";
            }
            if (enemyList.Count == 0)
            {
                scene = "VictoryScreen";
            }
            
        }
        if (scene != "")
        {
            timer += Time.deltaTime;
        }
        if (timer > 5)
        {
            float fadeTime = GameObject.Find("Main Camera").GetComponent<Fadeing>().BeginFade(1);
            IEnumerator co = ChangeLevel(fadeTime, scene);
            StartCoroutine(co);
        }

    }
    void PlaneCheck()
    {
        if (player != null)
        {
            for (int i = 0; i < planes.Length; i++)
            {
                if (player.transform.position.y > planes[i].transform.position.y + .1) planes[i].SetActive(true);
                else planes[i].SetActive(false);
            }
        }
    }
    IEnumerator ChangeLevel(float fadeTime, string scene)
    {
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(scene);
    }
}
