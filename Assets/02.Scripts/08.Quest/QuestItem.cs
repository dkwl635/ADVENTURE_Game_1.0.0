using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    [HideInInspector] public Quest m_Quest;

    public Button m_QuestInfoBtn;

    public Text m_QuestName;
    public Text m_QuestStatus;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Quest == null)
            return;

        m_QuestName.text = m_Quest.m_QuestName;
        m_QuestStatus.text = m_Quest.m_QuestStatus;
    }

   


}
