using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameControl : MonoBehaviour
{
    // Start is called before the first frame update
    int c;
    float timer;
    private void Start()
    {
        
    }
    void Update ()
    {

        if(c==0)
        {
            timer = Time.time;
            
        }
        c++;
        
        if (Time.time-timer>1)
        {
            
            Debug.Log(c);
            c = 0;  
        }
     
    }

    // Update is called once per frame
    
}
