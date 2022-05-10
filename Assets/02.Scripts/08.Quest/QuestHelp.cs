using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHelp : MonoBehaviour
{
    NPC m_MyNPC;
    public List<int> m_Quest_IdList = new List<int>();
    public int m_Quest_ID = -1;
    public GameObject m_QuestObj;


    void Start()
    {
    
        QuestSet();

        m_MyNPC = GetComponent<NPC>();
        if (m_MyNPC != null)
        {
            m_MyNPC.Talk += OpenQuest;
        }
    }

    public void QuestSet()
    {
        m_QuestObj.SetActive(false);
        if (m_Quest_IdList.Count <= 0)
            return;

        m_Quest_ID = -1;

        for (int i = 0; i < m_Quest_IdList.Count; i++)
        {
         
                      
            if (!QuestMgr.Inst.ClearCheckQuest(m_Quest_IdList[i]))//선행퀘 확인                    
                continue;
           

            if (QuestMgr.Inst.CheckQuest(m_Quest_IdList[i]))//받은 퀘스트인지 확인                          
                continue;

            m_Quest_ID = m_Quest_IdList[i];
        }



        if (m_Quest_ID.Equals(-1))
            return;

      
        m_QuestObj.SetActive(true);

    }


    public void OpenQuest(Player a_player)
    {      
        if (m_Quest_ID.Equals(-1))
            return;

        TalkMgr.Inst.OnQuestBtn(m_Quest_ID);
    }

   public void OffQuestObj()
   {
       m_QuestObj.SetActive(false);
   }

}
