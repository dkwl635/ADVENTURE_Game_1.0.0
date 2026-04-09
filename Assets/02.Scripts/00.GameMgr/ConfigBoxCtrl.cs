using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigBoxCtrl : MonoBehaviour
{
    public Button m_BackBtn = null;

    public Slider m_BGM_Slider;
    public Slider m_Eff_Slider;

    public Toggle m_BGM_Toggle;
    public Toggle m_Eff_Toggle;

    public Button m_SaveBtn = null;
    public Button m_SceneBackBtn = null;
    public Button m_DestoryBtn = null;

    [Header("DestoryLogBox")]
    public GameObject m_DestoryLogBox = null;
    public Button m_OkBtn = null;
    public Button m_CancelBtn = null;


    // Start is called before the first frame update
    void Start()
    {
        m_BGM_Slider.onValueChanged.AddListener(ChangeBGMVolume);
        m_Eff_Slider.onValueChanged.AddListener(ChangeEffectVolume);
        m_BGM_Toggle.onValueChanged.AddListener(ChangeBGMToggle);
        m_Eff_Toggle.onValueChanged.AddListener(ChangeEffToggle);

        m_BackBtn.onClick.AddListener(OffPanel);
        m_SaveBtn.onClick.AddListener(Save);
        m_SceneBackBtn.onClick.AddListener(SceneBack);
        m_DestoryBtn.onClick.AddListener(DestoryBtn);

        m_OkBtn.onClick.AddListener(() => { SelectDestory(true); });
        m_CancelBtn.onClick.AddListener(() => { SelectDestory(false); });

        float bgm = ServiceLocator.Get<IAudioService>()?.GetBGMVolume() ?? 0f;
        float eff = ServiceLocator.Get<IAudioService>()?.GetEffectVolume() ?? 0f;
        m_BGM_Slider.value = bgm * 10;
        m_Eff_Slider.value = eff * 10;
    }

   void OffPanel()
    {
        this.gameObject.SetActive(false);
    }

    void ChangeBGMVolume(float a_Volume)
    {
        ServiceLocator.Get<IAudioService>()?.ChangeBGMVolume(a_Volume);
        if (a_Volume <= 0.0f)
        {
            m_BGM_Toggle.isOn = true;
        }
        else
            m_BGM_Toggle.isOn = false;

    }

    void ChangeBGMToggle(bool sel)
    {
        if (sel)
        {
            ServiceLocator.Get<IAudioService>()?.ChangeBGMVolume(0.0f);
            m_BGM_Slider.value = 0;
        }
        else
        {
            ServiceLocator.Get<IAudioService>()?.ChangeBGMVolume(0.2f);
            m_BGM_Slider.value = 0.2f;
        }

    }


    void ChangeEffectVolume(float a_Volume)
    {
        ServiceLocator.Get<IAudioService>()?.ChangeEffectVolume(a_Volume);
        if (a_Volume <= 0.0f)
        {
            m_Eff_Toggle.isOn = true;
        }
        else
            m_Eff_Toggle.isOn = false;

    }

    void ChangeEffToggle(bool sel)
    {
        if (sel)
        {
            ServiceLocator.Get<IAudioService>()?.ChangeEffectVolume(0.0f);
            m_Eff_Slider.value = 0;
        }
        else
        {
            ServiceLocator.Get<IAudioService>()?.ChangeEffectVolume(0.2f);
            m_Eff_Slider.value = 0.2f;
        }

    }


    void Save()
    {
        ServiceLocator.Get<IAudioService>()?.PlaySound("Button");
        ServiceLocator.Get<ISaveLoadService>()?.SaveData();
    }

    void SceneBack()
    {
        ServiceLocator.Get<IAudioService>()?.PlaySound("Button");
        DontDestroyOnLoadMgr.inst.AllDestory();
        LoadingSceneMgr.LoadScene("TitleScene");
    }

    void DestoryBtn()
    {
        ServiceLocator.Get<IAudioService>()?.PlaySound("Button");
        m_DestoryLogBox.SetActive(true);
    }

    void SelectDestory(bool sel)
    {
        ServiceLocator.Get<IAudioService>()?.PlaySound("Button");

        if (sel)
        {     
            ServiceLocator.Get<ISaveLoadService>()?.DestorySaveData();
            DontDestroyOnLoadMgr.inst.AllDestory();
            LoadingSceneMgr.LoadScene("TitleScene");
        }
        m_DestoryLogBox.SetActive(false);

    }

}
