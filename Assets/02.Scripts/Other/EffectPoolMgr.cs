using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolMgr : MonoBehaviour
{
    static public EffectPoolMgr Inst;

    public int m_EffectCount = 20;  //처음 생성 오브젝트
    public GameObject m_HitEffectPrefab = null;   //맞을때 나오는 이펙트 프리팹

    Dictionary<string, Stack<GameObject>> m_EffectPool = new Dictionary<string, Stack<GameObject>>();
   

    private void Awake()
    {
        if (Inst == null)
            Inst = this;

        m_EffectPool["HitEffect"] = new Stack<GameObject>();
    }
}
