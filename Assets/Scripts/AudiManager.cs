using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudiManager : MonoBehaviour
{
    public static AudiManager instance { get; private set; }
    AudioSource audioSource;
    private void Awake()
    {
        if(instance!=null&& instance!=this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }


    IEnumerator Co_PithcUp()
    {
        while(audioSource.isPlaying)
        {
            yield return null;
        }
        if (audioSource != null)
        {
            if (audioSource.pitch < 0.85f)
            {
                yield break; ;
            }
            audioSource.pitch += 0.06f;
        }

    }
    void PitchUp()
    {
        StartCoroutine(Co_PithcUp());   
        
    }

    public void ResetPitch()
    {
        audioSource.pitch = 1;
        ComboManager.instance.ResetComboVal();
    }
    [SerializeField]
    private AudioClip shoot,gem,merge,owl;

    private AudioSource m_AudioSource;
    public AudioSource owl_audioSource;
    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlayGem()
    {
        m_AudioSource.clip = gem;
        m_AudioSource.Play();
    }

    public void Playshoot()
    {
        m_AudioSource.clip = shoot;
        m_AudioSource.Play();
    }

    public void PlayOwl()
    {
        owl_audioSource.clip=owl;
        owl_audioSource.Play();   
    }
    public void PlayMerge()
    {
        //if (audioSource.isPlaying) return;

        m_AudioSource.clip = merge;
        m_AudioSource.Play();
        PitchUp();
    }
}
