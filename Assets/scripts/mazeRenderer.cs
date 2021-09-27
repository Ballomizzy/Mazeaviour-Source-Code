using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class mazeRenderer : MonoBehaviour
{
    [SerializeField]
    [Range(1, 50)]
    public int width = 10;

    [SerializeField]
    [Range(1, 50)]
    public int height = 10;

    [SerializeField]
    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    public Transform world = null;
    public Transform start = null;
    public Transform end = null;
    public Transform player = null;

    public GameObject pauseMenuUI;
    public GameObject gameUI;

    public GameObject mapView;

    public GameObject wonUI;
    public GameObject lostUI;

    private bool count = true;

    public int level = 1;

    public Text didYouKnow;
    private string[] didYouKnows = {
        "Power your home with renewable energy!", 
        "Invest in energy-efficient appliances", 
        "Reduce water waste.", "Do not waste food!", 
        "Pull the inactive plugs/ lights", "Drive a fuel efficient vehicle",
        "Shrink your carbon profile!"
        };

    public Text timerTextHolder;

    public AudioClip timeTick;
    private AudioSource gameAudio;
    private Animator textAnim;
    

    private Transform playerUpdate = null;
    private Transform endUpdate = null;
    private Transform startUpdate = null;
    private GameObject playerCursor = null;

    private GameObject levelDecider = null;

    private GameObject winText = null;

    private float timerThres = 30f;
    public float currentTime = 1f;


    public void Awake()
    {
        levelDecider = GameObject.Find("menuController");
        if (levelDecider != null)
        {
            level = levelDecider.transform.GetComponent<dontDestroy>().levelSetting;
        }
        else{Debug.Log("level decider not found");}
    }
    // Start is called before the first frame update
    public void Start()
    {
        gameAudio = transform.GetComponent<AudioSource>();
        levelDecider = GameObject.Find("settings");
        if (levelDecider != null)
        {
            level = levelDecider.transform.GetComponent<dontDestroy>().levelSetting;
        }
        else{Debug.Log("level decider not found");}
        count = true;
        textAnim = timerTextHolder.transform.GetComponent<Animator>();
        timerThres = 40f * level;
        currentTime = timerThres;

        wonUI.SetActive(false);
        lostUI.SetActive(false);

        pauseMenuUI.SetActive(false);

        width = 4 + (level - 1);
        height = 4 + (level - 1);
        

        mapView.transform.localPosition = new Vector3(0, 8 + (level - 1), 0) ;

        var maze = mazeGen.Generate(width, height);
        Draw(maze);

        var worldGO = Instantiate(world, transform) as Transform;
        world.localScale = new Vector3(1, 1, 1);
        world.localPosition = new Vector3(0, -.5f, 0);

        var startGO = Instantiate(start, transform) as Transform;
        startUpdate = startGO;
        startGO.position = new Vector3((-world.localScale.x - world.localScale.x/2) - startGO.localScale.x/2, 0, (-world.localScale.z - world.localScale.z/2) - startGO.localScale.z/2);

        var endGO = Instantiate(end, transform) as Transform;
        endUpdate = endGO;
        endGO.position = new Vector3((world.localScale.x + world.localScale.x/2) - endGO.localScale.x/2, 0, (world.localScale.z + world.localScale.z/2) - endGO.localScale.z/2);

        var playerGO = Instantiate(player, transform) as Transform;
        playerUpdate = playerGO;
        playerGO.transform.forward = endGO.transform.forward;
        playerGO.position = new Vector3(startGO.position.x, -.2f, startGO.position.z);

        playerCursor = GameObject.Find("Quad");
        playerCursor.transform.localScale = new Vector3(level, level, level) * 20f;

    }

    private void Draw (WallState[,] maze)
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i,j];
                var position = new Vector3 (-width/2 + i, 0, -height/2 + j);

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as Transform;
                    topWall.position = position + new Vector3(0, 0, size /2);
                    topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(-size/2, 0, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                if(i == width - 1)
                {
                    if(cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(+size/2, 0, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j==0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var downWall = Instantiate(wallPrefab, transform) as Transform;
                        downWall.position = position + new Vector3(0, 0, -size /2);
                        downWall.localScale = new Vector3(size, downWall.localScale.y, downWall.localScale.z);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        endUpdate.forward = playerUpdate.forward;
        startUpdate.forward = playerUpdate.forward;
        if (count)
        {
            currentTime -= Time.deltaTime;
        }
        
        timerTextHolder.text = /*.transform.GetComponent<TextMesh>().text*/Mathf.Ceil(currentTime).ToString();
        
        if (currentTime <= 0)
        {
            Debug.Log("Time's up");
            lose();
        }
        if (currentTime <= 20)
        {
            gameAudio.clip = timeTick;
            gameAudio.Play();
            timerTextHolder.color = Color.red;
            textAnim.SetTrigger("textWarn");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BroadcastMessage("pauseGame");
        }
    }

    public void pauseGame()
    {
        gameUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        
        didYouKnow.text = "Protect the Planet in your own litte way: \n\n " + didYouKnows[(int)(Random.Range(0, didYouKnows.Length))];

        Time.timeScale = 0f;
    } 

    public void playGame()
    {
        gameUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        BroadcastMessage("playGameForPlayer");
    }

    public void finishGame()
    {
        gameAudio.Stop();
        Debug.Log("finished level " + level);
        //disable all other screens
        gameUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        lostUI.SetActive(false);
        //stop timer
        count = false;
        //congrats, display seconds used, ask to play again, quit, home
        wonUI.SetActive(true);

        winText = GameObject.Find("timeElapsed");
        winText.transform.GetComponent<Text>().text = "It took you " + (Mathf.Ceil((int)timerThres-currentTime)).ToString() + " seconds!";

    }

    public void lose()
    {
        gameAudio.Stop();
        gameUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        wonUI.SetActive(false);
        //stop timer
        count = false;
        //congrats, display seconds used, ask to play again, quit, home
        lostUI.SetActive(true);
    }

    public void quit()
    {
        Application.Quit();
    }
    public void restart()
    {
        SceneManager.LoadScene("game");
    }
    public void mainMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }
}


