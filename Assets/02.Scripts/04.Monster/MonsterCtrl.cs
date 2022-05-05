using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterState
{
    Idle, 
    AttackIdle,
    Attack,
    GoBack,
    Trace,
    Hit,
    Die,
    Respawn
}


 public class MonsterCtrl : MonoBehaviour
{
    [HideInInspector] public BoxCollider boxCollider = null;       //몬스터 콜라이더
    [HideInInspector] public Animator animator = null;             //몬스터 애니메이터
    [HideInInspector] public NavMeshAgent navMeshAgent = null;     //타켓 추적을 위한 네비
    [HideInInspector] public GameObject mesh = null;                      //몬스터 메쉬 오브젝트
    [HideInInspector] public Transform m_Target = null;                    //타겟      

    public int m_MonsterId = -1;

    //상태
    public MonsterState m_MonsterState;

    //체력 UI를 저장하는 변수
    //public GameObject m_HpBarObj = null;             //체력바 오브젝트
    public MonHpBarCtrl m_HpBarCtrl = null;         //체력바 UI 컨트롤용

    //몬스터 스탯
    public Status m_MonsterStatus;

    //데미지를 준 플레이어 저장
    [HideInInspector]  public Player m_Attacker = null;

    public float m_Speed = 3.0f;
    //공격 관련
    public float m_AttackDist = 1.0f; //공격 사거리
    public float m_TraceDist = 8.0f;  //추적 사거리

    [HideInInspector] public int m_AttackPower = 10;             //공격력
    [HideInInspector] public float m_Dist = 0.0f;                      //타겟과의 거리

    //스폰위치
    public Vector3 m_SpawnPos = Vector3.zero;          

    //드랍 아이템 관련
    public int m_MinCoin = 0;              //죽으면 드랍하는 코인의 갯수 최소
    public int m_MaxCoin = 0;             //최대

    //데미지 받을 시 효과를 주기 위한 변수들
    public SkinnedMeshRenderer m_Skin; //메쉬 스킨
    public Material m_OrginMtrl;              //처음으로 가지고 있는 머터리얼
    public Material m_HitMtrl;              //데미지 받을 시 바꿔줄 머터리얼

    public delegate void Event();
    public Event DieEvent;


    public virtual void Init() { }

    public virtual void Spawn() { }               //스폰 하는 함수 
    public virtual void OnDamge(int a_Damage = 0, Player a_Attacker = null) { }  //데미지를 받는 함수
    public virtual void Die() { }     //죽으면 호출되는 함수
    public virtual void ObjDestory() { }  //오브젝트를 파괴 할때 

    public IEnumerator SetHitMtrl()
    {
        if (m_HitMtrl == null && m_Skin == null)
            yield break;
        
        m_Skin.material = m_HitMtrl;

        yield return new WaitForSeconds(0.3f);

        m_Skin.material = m_OrginMtrl;
    }
}
