using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum TxtType
{
    PlayerDamage,
    Damage,
    Heal,
    LvUp,

}

public class DamageTxt : MonoBehaviour
{
    private int m_Value;
    private float m_LifeTime = 1.3f;
    private float m_InitLifeTime = 1.3f;
    public Text m_DamageValueTxt = null;
    


    private void Update()
    {
        if(m_LifeTime > 0)
        {
            m_LifeTime -= Time.deltaTime;
        }
        else if(m_LifeTime < 0)
        {
            OffDamageText();
        }

    }



    public void OnDamageText(int a_Value , TxtType a_Damage = TxtType.Damage)
    {
        m_Value = a_Value;
        if (a_Damage == TxtType.Damage)
            m_DamageValueTxt.color = Color.black;
        else if (a_Damage == TxtType.Heal)
            m_DamageValueTxt.color = Color.green;
        else if (a_Damage == TxtType.PlayerDamage)
            m_DamageValueTxt.color = Color.gray;

        m_DamageValueTxt.text = m_Value.ToString();
        gameObject.SetActive(true); 
    }

    public void OffDamageText()
    {
        InGameMgr.Inst.PushBackDamageTxt(this);
        m_LifeTime = m_InitLifeTime;   
        gameObject.SetActive(false);
    }



}
