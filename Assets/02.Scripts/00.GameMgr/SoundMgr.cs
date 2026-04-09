using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundMgr : MonoBehaviour, IAudioService
{
    public AudioClip[] BGM_Clips = null;
    public AudioClip[] Effect_Clip = null;
    Dictionary<string, AudioClip> DicEffectClip = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> DicBGMClip = new Dictionary<string, AudioClip>();

    [HideInInspector] public AudioSource m_BgmAudio = null;
    [HideInInspector] public AudioSource m_EffectAudio = null;

    private float m_BgmVolume;
    private float m_EffectVolume;

    private void Awake()
    {
        //등록된 서비스가 있다면 파괴
       if(ServiceLocator.IsRegistered<IAudioService>())
       {
           Destroy(this.gameObject);
           return;
       }

        DontDestroyOnLoad(this.gameObject);
        ServiceLocator.Register<IAudioService>(this);

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
        m_BgmVolume = 0.02f;
        m_EffectAudio.volume = 0.02f;
        m_EffectVolume = 0.02f;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (ServiceLocator.IsRegistered<IAudioService>() && 
            ServiceLocator.Get<IAudioService>() == (IAudioService)this)
        {
            ServiceLocator.Unregister<IAudioService>();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LoadingScene")
            return;

        m_BgmAudio = Camera.main.GetComponents<AudioSource>()[0];
        m_EffectAudio = Camera.main.GetComponents<AudioSource>()[1];
        
        m_BgmAudio.volume = m_BgmVolume;
        m_EffectAudio.volume = m_EffectVolume;

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

        m_EffectAudio.PlayOneShot(DicEffectClip[a_Name]);

        Debug.Log($"PlaySound_ {a_Name}");
    }

    public void ChangeBGMVolume(float Volume)
    {
 
        m_BgmAudio.volume = Volume * 0.1f;
        m_BgmVolume = m_BgmAudio.volume;

    }

    public void ChangeEffectVolume(float Volume)
    {

        m_EffectAudio.volume = Volume * 0.1f;
        m_EffectVolume = m_EffectAudio.volume;
    }

    public float GetBGMVolume()
    {
        return m_BgmVolume;
    }

    public float GetEffectVolume()
    {
        return m_EffectVolume;
    }
}
