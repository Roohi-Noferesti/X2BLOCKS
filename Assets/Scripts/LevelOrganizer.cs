using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOrganizer : MonoBehaviour
{
    public static LevelOrganizer instance { get; private set; }
    private void Awake()
    {
        if(instance != null && instance!=this)
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
        confetti.Stop();
    }

    ulong highestBlock;
    public ulong HighestBlock { get { return highestBlock; }
        private set { highestBlock = value;
        GameStateManager.instance.State.HighestBlock = value;
        
        } }
    

    public void InitHighestBlock()
    {
        HighestBlock = GameStateManager.instance.State.HighestBlock;
        if (HighestBlock == 0)
        {
            HighestBlock = 256;

        }
        LastHighBlock = highestBlock;
    }
    public delegate void CrownHigh();
    public static event CrownHigh CrownHighDelegate;


    ulong LastHighBlock;
    [SerializeField]
    ParticleSystem confetti;

    public delegate void FadeBlocks(bool fadeIn);
    public static event FadeBlocks FadeBlocksDelegate;

    //l
    [SerializeField]
    
    [ContextMenu("playConfetti")]
    public void playConfetti(bool unblockNext)
    {
        Application.targetFrameRate = -1;
        TouchManager.instance.StopTouch();
        FadeBlocksDelegate(false);
        confetti.gameObject.SetActive(true);
        confetti.Play();
        StartCoroutine(FadeInBlocks(unblockNext));
    }

    IEnumerator FadeInBlocks(bool unblockNext)
    {
        while(confetti.isPlaying)
        {
            yield return null;  
        }
        confetti.gameObject.SetActive(false);
        FadeBlocksDelegate(true);
        if(unblockNext)
        {
            BlockListManager.instance.UnblockNext();
        }
        else
        {
            BlockListManager.instance.NextGoal();
        }
        LastHighBlock = highestBlock;
       CrownHighDelegate();
        //b
    }


    public void CheckForNextGoal()
    {
        if (confetti.isPlaying) return;
        if (highestBlock>LastHighBlock)
        {
            if ((Mathf.Sqrt(HighestBlock / 512)) % 1 == 0 && highestBlock > 512)
            {

                playConfetti(true);

            }
            else if(highestBlock>256)
            {
                playConfetti(false);
                
            }
            
        }
    }


    public void UpdateHighestBlock(ulong val)
    {
        if (GameStateManager.instance.ifInSurvival()) return;

        if (HighestBlock == 0) return;

        if(val==0)
        {
            highestBlock=0;
            return;
        }
        if(val>HighestBlock)
        {
            HighestBlock=val;            
            
        }
    }




}
