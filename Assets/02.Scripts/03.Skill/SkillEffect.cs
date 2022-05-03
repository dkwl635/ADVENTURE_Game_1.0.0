using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Player m_Owner = null;        //누구로부터
    public int m_Damage = 0;                    //데미지


    public void InitSkillEffect(int a_Damage = 0, Player a_Owner = null, float a_LifeTime=1.0f)
    {
        m_Damage = a_Damage;
        if (a_Owner != null)
            m_Owner = a_Owner;

        Destroy(this.gameObject, a_LifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        MonsterCtrl target = other.GetComponentInChildren<MonsterCtrl>();
        if (target != null)
        {
           target.OnDamge(m_Damage, m_Owner);
        }
    }

}
