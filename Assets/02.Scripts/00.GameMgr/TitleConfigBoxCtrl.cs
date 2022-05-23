using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleConfigBoxCtrl : MonoBehaviour
{
    public Button m_BackBtn = null;

    public Slider m_BGM_Slider;
    public Toggle m_BGM_Toggle;

    public Slider m_Eff_Slider;
    public Toggle m_Eff_Toggle;

    public Button m_GameEscBtn = null;

    [Header("DestoryLogBox")]
    public GameObject m_GameEscLogBox = null;
    public Button m_OkBtn = null;
    public Button m_CancelBtn = null;


    // Start is called before the first frame update
    void Start()
    {
        m_BGM_Slider.onValueChanged.AddListener(ChangeBGMVolume);
        m_BGM_Toggle.onValueChanged.AddListener(ChangeBGMToggle);


        m_Eff_Slider.onValueChanged.AddListener(ChangeEffectVolume);
        m_Eff_Toggle.onValueChanged.AddListener(ChangeEffToggle);

        m_BGM_Slider.value = SoundMgr.Inst.m_BgmAudio.volume * 10.0f;
        m_Eff_Slider.value = SoundMgr.Inst.m_EffectAudio.volume * 10.0f;

        m_GameEscBtn.onClick.AddListener(GameEscBtn);
        m_OkBtn.onClick.AddListener(GameEsc);
        m_CancelBtn.onClick.AddListener(Cancel);


        m_BackBtn.onClick.AddListener(OffPanel);

    }

    void OffPanel()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    void ChangeBGMVolume(float a_Volume)
    {
        SoundMgr.Inst.ChangeBGMVolume(a_Volume);
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
            SoundMgr.Inst.ChangeBGMVolume(0.0f);
            m_BGM_Slider.value = 0;
        }
        else
        {
            SoundMgr.Inst.ChangeBGMVolume(0.2f);
            m_BGM_Slider.value = 0.2f;
        }

    }


    void ChangeEffectVolume(float a_Volume)
    {
        SoundMgr.Inst.ChangeEffectVolume(a_Volume);
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
            SoundMgr.Inst.ChangeEffectVolume(0.0f);
            m_Eff_Slider.value = 0;
        }
        else
        {
            SoundMgr.Inst.ChangeEffectVolume(0.2f);
            m_Eff_Slider.value = 0.2f;
        }

    }

    void GameEscBtn()
    {
        m_GameEscLogBox.SetActive(true);
    }

    void GameEsc()
    {
        Application.Quit();
    }

    void Cancel()
    {
        m_GameEscLogBox.SetActive(false);

    }

}
