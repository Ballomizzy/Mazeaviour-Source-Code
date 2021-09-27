using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class simpleMove : MonoBehaviour
{
    public float move_speed = 40f;
    public float sensitivity = 100f;
    private CharacterController characterController;

    public float shakeTimer = 0f;
    public float shakeTimerThresold = 20f;
    public float shakeTime = 2f;
    public GameObject lightInfoTxt;
    public Transform cam;
    private Text lightText;
    private GameObject gameControlGO;
    public mazeRenderer gameControl;

    public AudioClip switchAudio;
    public AudioClip explosion;

    private AudioSource playerAudio;

    public bool onPause;

    public GameObject confetti = null;
    

    // Start is called before the first frame update
    void Start()
    {
        playerAudio = transform.GetComponent<AudioSource>();
        gameControlGO = GameObject.Find("mazeGen");
        if (gameControlGO != null)
        {
            gameControl = gameControlGO.transform.GetComponent<mazeRenderer>();
        }
        characterController = GetComponent<CharacterController>();
        lightInfoTxt = GameObject.Find("LightLogs");
        if (lightInfoTxt != null)
        {
            lightText = lightInfoTxt.transform.GetComponent<Text>();
        }
        else{Debug.Log("did not find light logs GO");}
    }

    // Update is called once per frame
    void Update()
    {
        if (!onPause)
        {
            shakeTimer += Time.deltaTime;
            //Debug.Log(shakeTimer);

            if (shakeTimer >= shakeTimerThresold){
                shakeTimer = 0f;
                StartCoroutine(shake(shakeTime, .2f));
            }

            //get inputs for moving on x and z axes
            float horizontal_move = Input.GetAxis("Horizontal");
            float vertical_move = Input.GetAxis("Vertical");

            //get input for rotating
            float rotate_x = Input.GetAxis("Mouse X");
            
            //store the directions for moving on x and z axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            //rotate according to sensitivity and time
            transform.Rotate(new Vector3(0, rotate_x, 0) * sensitivity * Time.deltaTime, Space.World);

            //move
            characterController.SimpleMove(forward * move_speed * vertical_move * Time.deltaTime);
            characterController.SimpleMove(right * move_speed * horizontal_move * Time.deltaTime);

            //switch off lights
            if(Input.GetKeyDown(KeyCode.Space))
            {
                switchOff();
            }
            if(Input.GetKeyDown(KeyCode.V))
            {
                switchOn();
            }
        }
        /* if(onPause)
        {

        } */
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Finish")
        {
            Instantiate(confetti, other.transform);
            finish();
        }
    }

    private void finish(){
        Debug.Log("finished");
        // stop game, timer
        playerAudio.Stop();
        gameControl.finishGame();

        //show gameover screen 
            //congratulate
            //tell time spent
            //play again
            //quit


    }

    public void switchOff(){
        Vector3 overlapWallSize = new Vector3(.5f, .5f, .5f);
        Collider[] wallPrefabs = Physics.OverlapBox(transform.position, overlapWallSize, Quaternion.identity);
        foreach (Collider item in wallPrefabs)
        {
            if(item.gameObject.tag == "wall")
            {
                wallScript wallscript = item.transform.GetComponent<wallScript>();
                if (wallscript != null)
                {
                    playerAudio.clip = switchAudio;
                    playerAudio.Play();
                    wallscript.switchOffLights();
                    StartCoroutine(logLightInfo("Switched Off Neighbour Lights!"));
                }
                else
                {
                    Debug.Log("error loading wallscript");
                } 
            }
        }
    }

    public void switchOn(){
        Vector3 overlapWallSize = new Vector3(.5f, .5f, .5f);
        Collider[] wallPrefabs = Physics.OverlapBox(transform.position, overlapWallSize, Quaternion.identity);
        foreach (Collider item in wallPrefabs)
        {
            if(item.gameObject.tag == "wall")
            {
                wallScript wallscript = item.transform.GetComponent<wallScript>();
                if (wallscript != null)
                {
                    wallscript.switchOnLights();
                }
                else
                {
                    Debug.Log("error loading wallscript");
                } 
            }
        }
    }

    public IEnumerator shake(float duration, float magnitude)
    {
        Debug.Log("shook");
        playerAudio.clip = explosion;
        playerAudio.Play();
        Vector3 initPos = cam.transform.localPosition;
        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            // transform.localPosition = transform.localPosition + new Vector3(Random.Range(-.1f, .1f), Random.Range(-0.1f, .1f), 0);
            float x = Random.Range(-.25f, .25f) * magnitude;
            float y = Random.Range(-.25f, .25f) * magnitude;

            cam.transform.localPosition = new Vector3(initPos.x, y, x);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.localPosition = initPos;
    }

    public IEnumerator logLightInfo(string info)
    {
        lightText.text = info;

        yield return new WaitForSeconds(1f);

        lightText.text = " ";
    }

    public void pauseGame()
    {
        onPause = true;
    }

    public void playGameForPlayer()
    {
        onPause = false;
    }
}
