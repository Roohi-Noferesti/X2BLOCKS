using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    // Start is called before the first frame update
    LayerMask mask;
    public static TouchManager instance { get; private set; }
    private void Awake()
    {
        if(instance!=null && instance!=this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        mask = LayerMask.GetMask("Touch");
        ReloadDelay = new WaitForSeconds(0.6f);
        CanTouch = true;
        
    }
    public delegate void NewBlockRequest(int  column);
    public static event NewBlockRequest NewBlockRequestEvent;
    private bool canTouch;

    public bool CanTouch
    {
        get { return canTouch; }
        set {
            canTouch = value;
            if(!canTouch)
            {
                StartCoroutine(ReloadBlock());
            }
        
        }
    }

    WaitForSeconds ReloadDelay;
    IEnumerator ReloadBlock()
    {
        yield return ReloadDelay;
        CanTouch = true;
    }

    bool _hammer, _swap,_tutorial;

    public void ActivateTutorial()
    {
        _tutorial = true;
    }

    public void DeActivateTutorial()
    {
        _tutorial=false;
    }


    public void ActivateHammer()
    {
        StartCoroutine(Co_enableHammer());   
    }

    IEnumerator Co_enableHammer()
    {
        yield return new WaitForSeconds(0.1f);
        _hammer = true;
    }
    IEnumerator Co_enableSwap()
    {
        yield return new WaitForSeconds(0.1f);
        _swap = true;
    }
    public void DeActivateHammer()
    {
        _hammer = false;
    }
    public void ActivateSwap()
    {
        StartCoroutine(Co_enableSwap()); 
    }
    public void DeActivateSwap()
    {
        _swap=false;
    }

    private bool paused,cursePause;
    public void StopTouch(bool curse=false)
    {
        if(curse)
        {
            cursePause = true;
        }
        else
        {
            paused = true;
        }
       
    }


    IEnumerator Co_ResumeTouch()
    {
        yield return new WaitForSeconds(0.1f);
        paused = false;
    }
    public void ResumeTouch(bool curse =false)
    {
        if (curse)
        {
            cursePause=false;   
        }
       StartCoroutine(Co_ResumeTouch());
    }
    Vector2 TouchStart;
    void Update()
    {
        if (paused) return;
       if(Input.touchCount==1)
        {
            
            Touch touch = Input.GetTouch(0);
            Ray ray = UiManager.instance.MainCamera.ScreenPointToRay(touch.position);
            if(touch.phase==TouchPhase.Began)
            {
                TouchStart=touch.position;
            }
            if (touch.phase==TouchPhase.Ended)
            {
                Vector2 delta = touch.position - TouchStart;
                
                if (delta.magnitude>100) return;
                
                if(CanTouch && Physics.Raycast(ray,out RaycastHit hitInfo,mask))
                {
                    Vector2 HitPoint = UiManager.instance.MainCamera.WorldToScreenPoint(hitInfo.point);
                    
                    int row = GridManager.instance.chooseGridRow(HitPoint.y);
                   
                    int column = GridManager.instance.ChooseGridColumn(HitPoint.x);
                    if(HitPoint.y<GridManager.instance.panel.position.y 
                        || HitPoint.y > GridManager.instance.panel.position.y+GridManager.instance.PanelHeight)
                    {
                        return;
                    }
                    if (_hammer)
                    {
                        if (GridManager.instance.GridArray[row, column] == null) return;
                        PowerManager.instance.Hammer(row,column);
                    }else if(_swap)
                    {
                        if(GridManager.instance.GridArray[row, column] == null) return; 
                        PowerManager.instance.Swap(row,column);
                    }
                    else
                    {
                        if (_tutorial)
                        {
                            if (column != Tutorial.Instance.activeColumn) return;
                            Tutorial.Instance.FinishTutorial();
                        }
                        //normal state
                        CanTouch = false;
                        if (NewBlockRequestEvent != null)
                        {
                            Application.targetFrameRate = -1;
                            if (cursePause) return;
                            StartCoroutine(Touch(column));
                        }
                    }


                    
                   
                }
                
            }
        }
    }

    IEnumerator Touch(int column)
    {
        
        yield return null;
        NewBlockRequestEvent(column);//event is subscribed in blockPool object

        GameOrganizer.instance.addToGeneratedBlocks();
        AudiManager.instance.ResetPitch();
    }
}
