using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    static public SoundMgr Inst;

    public AudioClip[] BGM_Clips = null;
    public AudioClip[] Effect_Clip = null;
    Dictionary<string, AudioClip> DicEffectClip = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> DicBGMClip = new Dictionary<string, AudioClip>();

    AudioSource m_BgmAudio = null;
    AudioSource m_EffectAudio = null;

    float m_Volume = 1.0f;
    private void Awake()
    {
        if (!Inst)
        {
            Inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
         

        m_BgmAudio = Camera.main.GetComponents<AudioSource>()[0];
        m_EffectAudio = Camera.main.GetComponents<AudioSource>()[1];

        for (int i = 0; i < BGM_Clips.Length; i++)
        {
            DicBGMClip.Add(BGM_Clips[i].name, BGM_Clips[i]);
        }

        for (int i = 0; i < Effect_Clip.Length; i++)
        {
            DicEffectClip.Add(Effect_Clip[i].name, Effect_Clip[i]);
        }

        m_BgmAudio.volume = 0.02f;
        m_Volume = 0.02f;
    }

    private void Start()
    {
       
    }

    public void OffSound()
    {
        m_BgmAudio.Stop();
    }

    public void ChangeBGM(string a_BgmName)
    {
        if (!DicBGMClip.ContainsKey(a_BgmName))
            return;

        m_BgmAudio.clip = DicBGMClip[a_BgmName];
        m_BgmAudio.Play();
    }

    public void PlaySound(string a_Name)
    {
        if (!DicEffectClip.ContainsKey(a_Name))
            return;

        m_EffectAudio.PlayOneShot(DicEffectClip[a_Name], m_Volume);
    }

    public void ChangeBGMVolume(float Volume)
    {
       // m_Volume = Volume;
        m_BgmAudio.volume = Volume * 0.1f;

    }

    public void ChangeEffectVolume(float Volume)
    {
        m_Volume = Volume * 0.1f;    
    }   
}
