using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBoxCtrl : MonoBehaviour
{
    static public LogBoxCtrl Inst;

    Text m_LogMsg;

    float m_ActiveTimer = 3.0f;
    float m_Timer = 0.0f;

    Color m_Color;

    private void Awake()
    {
        Inst = this ;
        m_LogMsg = GetComponent<Text>();
    }

    private void Update()
    {
        if(m_Timer > 0)
        {
            m_Color.a = Mathf.PingPong(Time.time, 1);
            m_LogMsg.color = m_Color;

            m_Timer -= Time.deltaTime;
            if (m_Timer <= 0)
                m_LogMsg.gameObject.SetActive(false);
        }      
    }

    public void LogBox(string a_Msg)
    {
        m_LogMsg.text = a_Msg;

        m_Timer = m_ActiveTimer;
        m_LogMsg.gameObject.SetActive(true);
        m_Color = m_LogMsg.color;
    }
}
