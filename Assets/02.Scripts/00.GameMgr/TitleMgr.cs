using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMgr : MonoBehaviour
{
    public Button[] m_PlayerBtn;
    public Button[] m_ClearBtn;
    
    public Button m_GameSetting;
    public GameObject m_GameSettingObj;
    public GameObject sky;

    private void Start()
    {
        Time.timeScale = 1.0f;

        GlobalValue.LoadData();
        m_PlayerBtn[0].GetComponentInChildren<Text>().text = GlobalValue.SetStartBtn(1);
        m_PlayerBtn[1].GetComponentInChildren<Text>().text = GlobalValue.SetStartBtn(2);
        m_PlayerBtn[2].GetComponentInChildren<Text>().text = GlobalValue.SetStartBtn(3);
                   
        m_PlayerBtn[0].onClick.AddListener(() => { StartBtn(1);  });
        m_PlayerBtn[1].onClick.AddListener(() => { StartBtn(2);  });
        m_PlayerBtn[2].onClick.AddListener(() => { StartBtn(3);  });
        
        m_ClearBtn[0].onClick.AddListener(() => { ResetData(1); });
        m_ClearBtn[1].onClick.AddListener(() => { ResetData(2); });
        m_ClearBtn[2].onClick.AddListener(() => { ResetData(3); });

        m_GameSetting.onClick.AddListener(() => { m_GameSettingObj.SetActive(true); });

        SoundMgr.Inst.ChangeBGM("TitleBGM");
    }

    private void Update()
    {
        sky.transform.Rotate(Vector3.up * Time.deltaTime * 3);

        if(Input.GetMouseButtonDown(0))
        {
            SoundMgr.Inst.PlaySound("Click");
        }

    }

    public void StartBtn(int num)
    {
        GlobalValue.StartTimer = Time.time;
        GlobalValue.playerNum = num;
    
        LoadingSceneMgr.LoadScene("rpgpp_lt_scene_1.0 1");
    }



    public void ResetData(int num)
    {
        GlobalValue.ResetData(num);
        m_PlayerBtn[num -1].GetComponentInChildren<Text>().text = GlobalValue.SetStartBtn(num);
    }


}
