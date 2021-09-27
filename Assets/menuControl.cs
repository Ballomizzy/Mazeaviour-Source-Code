using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuControl : MonoBehaviour
{
    public int countdown = 3;
    public int countdownTimer = 0;
    public int difficultyLevel = 1;

    public GameObject mainMenuUI = null;
    public GameObject tutorialMenuUI = null;
    public GameObject countdownUI = null;
    //public GameObject sun;

    private GameObject countdownText = null;
    private Text ctText;
    
    private GameObject difficultySlider;

    //private GameObject settings;

    // Start is called before the first frame update
    public void Awake()
    {
        if (mainMenuUI != null)
        {
            //show main menu
            mainMenuUI.SetActive(true);
        }
        if (tutorialMenuUI != null)
        {
            tutorialMenuUI.SetActive(false);
        }  
        if (countdownUI != null)
        {
            countdownUI.SetActive(false);
        }    
    }

    // Update is called once per frame
    public void Update()
    {

    }

    //play
    public void playButton()
    {
        //show tutorial menu
        mainMenuUI.SetActive(false);
        tutorialMenuUI.SetActive(true);
        countdownUI.SetActive(false);
    }

    //confirm play
    public void confirmPlay()
    {
        //show main menu
        mainMenuUI.SetActive(false);
        tutorialMenuUI.SetActive(false);
        countdownUI.SetActive(true);
        StartCoroutine(startGame());
    }

    public void setDifficulty()
    {
        difficultySlider = GameObject.Find("difficulty");
        difficultyLevel = (int)difficultySlider.transform.GetComponent<Slider>().value;
        transform.GetComponent<dontDestroy>().levelSetter(difficultyLevel);
    }

    public void exit ()
    {
        Application.Quit();
    }

    public IEnumerator startGame()
    {
        
        //countdown -= Time.deltaTime;
        
        countdownText = GameObject.Find("countdownText");
        if (countdownText != null)
        {
            ctText = countdownText.transform.GetComponent<Text>();

            ctText.text = "3";
            yield return new WaitForSeconds(1f);
            ctText.text = "2";
            yield return new WaitForSeconds(1f);
            ctText.text = "1";
            yield return new WaitForSeconds(1f);
            ctText.text = "GO!";
            yield return new WaitForSeconds(1f);
        }

    
        

        yield return new WaitForSeconds(1f);
        DontDestroyOnLoad(this.gameObject);
        transform.GetComponent<AudioSource>().Stop();
        //Destroy(this);
        mainMenuUI.SetActive(false);
        tutorialMenuUI.SetActive(false);
        countdownUI.SetActive(false);
        SceneManager.LoadScene("game");
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        
    }
    
}
