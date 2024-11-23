using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvaivalMode : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    int owlInterval;
    public static SurvaivalMode Instance { get; private set; }


    public void StartSurvival()
    {
        StartCoroutine(Co_Owl());
    }

    IEnumerator Co_Owl()
    {
         
        GameBrain.Instance.SetCurrentBLockToCache();
        
        TouchManager.instance.StopTouch(true);
        AudiManager.instance.PlayOwl();
        yield return new WaitForSeconds(3f);
        GridActionManager.instance.CurseFill();        
        GameBrain.Instance.SetCurrentBlockFromCach();
        yield return new WaitForSeconds(1);
        TouchManager.instance.ResumeTouch(true);


        yield break;
    }



    private void Awake()
    {
        if(Instance!=this && Instance!=null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
