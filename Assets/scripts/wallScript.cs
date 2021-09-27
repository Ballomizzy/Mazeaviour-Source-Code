using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallScript : MonoBehaviour
{
    public Transform myLight = null;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform item in transform)
        {
            if(item.gameObject.tag == "light")
            {
                myLight = item;
                Debug.Log("found light");
            }
            else 
                Debug.Log("did'nt find light");
        }
    }

    public void switchOffLights()
    {
        myLight.gameObject.SetActive(false);
        Debug.Log("yurned off light");
    }

    public void switchOnLights()
    {
        myLight.gameObject.SetActive(true);
        Debug.Log("turned on light");
    }
}
