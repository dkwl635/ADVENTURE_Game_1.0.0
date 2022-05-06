using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NPC : MonoBehaviour
{

    Animator animator;
    //NPC ID
    public int m_NpcId = -1;
    //NPC 이름
    public string m_NpcName = "";
    //NPC 초상화
    public List<Sprite> m_NpcSprite;
    
    public delegate void TalkAction (Player player);
    public TalkAction Talk;
    public TalkAction TalkEnd;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        Talk += OpenTalk;
        TalkEnd += CloseTalk;
    }
    


    //NPC 대화 시작
    void OpenTalk(Player a_Player)
    {
       

        TalkMgr.Inst.SetTalkMgr(m_NpcId, m_NpcName, m_NpcSprite,a_Player); 
        TalkMgr.Inst.OnTalkBtnGroup();

        TalkMgr.Inst.OnTalkBtn();
    }

    void CloseTalk(Player a_Player)
    {     
        TalkMgr.Inst.OffTalkBox();
    }

}
