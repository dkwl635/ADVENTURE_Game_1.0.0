using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMgr : MonoBehaviour
{
    public Button m_PlayerBtn01;

    private void Start()
    {
        GlobalValue.LoadData();
        m_PlayerBtn01.GetComponentInChildren<Text>().text = GlobalValue.SetStartBtn(1);
        m_PlayerBtn01.onClick.AddListener(() => { SceneManager.LoadScene("rpgpp_lt_scene_1.0 1"); });
    }

}
