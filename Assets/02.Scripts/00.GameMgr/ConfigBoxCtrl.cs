using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBoxCtrl : MonoBehaviour
{
    public Button m_BackBtn = null;

    public Slider m_BGM_Slider;
    public Slider m_Eff_Slider;

    public Button m_SaveBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        m_BGM_Slider.onValueChanged.AddListener(ChangeBGMVolume);
        m_Eff_Slider.onValueChanged.AddListener(SoundMgr.Inst.ChangeEffectVolume);

        m_BackBtn.onClick.AddListener(OffPanel);
        m_SaveBtn.onClick.AddListener(Save);
    }

   void OffPanel()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    void ChangeBGMVolume(float a_Volume)
    {
        SoundMgr.Inst.ChangeBGMVolume(a_Volume);
        if(a_Volume <= 0.0f)
        {

        }

    }

    void Save()
    {
        InGameMgr.Inst.SaveData();
    }

}
