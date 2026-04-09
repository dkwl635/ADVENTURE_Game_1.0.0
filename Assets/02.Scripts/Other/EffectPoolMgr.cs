using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolMgr : MonoBehaviour
{
    [Header("DamageTxt_ObjectPool")]
    public Canvas m_DamageCanvas = null;
    public GameObject m_DamageTxtObj;
    public int m_ObjPoolInit = 20;
    private Stack<DamageTxt> m_DamageTxtPool = new Stack<DamageTxt>();


    private void Awake()
    {
            
    }

    void InitObjPool()
    {
        if (m_DamageTxtObj != null)
        {
            for (int i = 0; i < m_ObjPoolInit; i++)
            {
                DamageTxt txtObj = Instantiate(m_DamageTxtObj, m_DamageCanvas.transform).GetComponent<DamageTxt>();
                txtObj.gameObject.SetActive(false);
                m_DamageTxtPool.Push(txtObj);
            }
        }
    }
    public void SpanwDamageTxt(Vector2 a_Pos, TxtType a_TxtType, int a_Value = 0)
    {
        DamageTxt text = null;
        if (m_DamageTxtPool.Count > 0)
            text = m_DamageTxtPool.Pop();
        else
            text = Instantiate(m_DamageTxtObj, m_DamageCanvas.transform).GetComponent<DamageTxt>();
        text.transform.position = a_Pos;
        text.OnDamageText(a_Value, a_TxtType);
    }
    public void PushBackDamageTxt(DamageTxt a_text)
    {
        m_DamageTxtPool.Push(a_text);
    }

}
