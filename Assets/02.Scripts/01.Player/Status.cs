using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Status
{
    public int m_Lv;    //레벨    
    public int m_CurExp;   //현재 경험치
    public int m_NextExp;   //다음레벨까지의 경험치

    public int m_CurHp; //현재 체력
    public int m_MaxHp; //최대 체력
    public int m_AttPw; //공격력
    public int m_DefPw; //방어력

    public float m_Critical;    //크리티컬 확률

   public void SetStatue(int a_Lv,int a_NextExp, int a_MaxHp, int a_AttPw,int a_DefPw, float a_Critical = 0.0f)
    {
        m_Lv = a_Lv;
        m_CurExp = 0;
        m_NextExp = a_NextExp;

        m_MaxHp = a_MaxHp;
        m_CurHp = a_MaxHp;
        m_AttPw = a_AttPw;
        m_DefPw = a_DefPw;
        m_Critical = a_Critical;
    }

  


}
