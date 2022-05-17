using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
//using System;

public class Player : MonoBehaviour
{    
    
    [HideInInspector] public Animator animator = null;               //애니메이터
    [HideInInspector] public Rigidbody rigidbody = null;             
    [HideInInspector] public PlayerInventory m_PlayerInventory = null;     //플레이어 아이템 관련

    Outline m_MeshOutline = null;


    public bool bIsAttack = false; // 지금 공격 중인지 판단
    public bool bIsHit = false; //피격중인지.
    public bool bIsMove = true; //움직일 수 있는지 
    public bool bIsWeapon = false; //무기를 장착 중인지

    #region ---------------KeyMode---------------
    //이동
    public float h; //감지된 움직임을 앞으로 갈지
    public float v;        //감지된 움직임을 회전 할지 

    public float m_MoveSpeed = 10.0f; // 이동속도
    public float m_RotSpeed = 180.0f; // 이동속도

    //이동관련
    private Vector3 m_DirVec = Vector3.zero; //방향백터 저장용 변수
    private float m_RotValue = 0.0f;
    private Vector3 m_Vec = Vector3.zero;
    #endregion
    #region ---------------Picking Mode---------------
    //Picking 관련 변수 
    public GameObject m_CursorMark = null; //마우스 피킹시 나타낼
    public Camera m_Camera = null;             //카메라
    bool m_IsPickMvOnOff = false;   //피킹 이동 OnOff

    NavMeshAgent nv;    //길이동   
    Vector3 m_CacLenVec = Vector3.zero; //계산용 변수


    Ray m_MousePos;
    RaycastHit hitInfo;
    private LayerMask m_LayerMask = -1;
    #endregion

    //장착중인 장비 아이템
    [HideInInspector] public Dictionary<PartType, EquipmentPart> m_PlayerPartItem = new Dictionary<PartType, EquipmentPart>();
    [HideInInspector] public Weapon weapon = null;                                 //지니고 있는 무기 정보 

    //능력치
    public Status m_PlayerStatus;
    //스킬 포인트
    public int m_SkillPoint = 0;

    public int m_PlayerAttPw { 
        get
        {
                return m_PlayerStatus.m_AttPw + weapon.m_WeaponPw;      
        } }  
    public int m_PlayerDefPw
    {
        get
        {
            int Hop = m_PlayerStatus.m_DefPw;
            if (m_PlayerPartItem.ContainsKey(PartType.Head))
                Hop += m_PlayerPartItem[PartType.Head].m_EquipmentDef;
            if (m_PlayerPartItem.ContainsKey(PartType.ShoulderPad))
                Hop += m_PlayerPartItem[PartType.ShoulderPad].m_EquipmentDef;
            if (m_PlayerPartItem.ContainsKey(PartType.Belt))
                Hop += m_PlayerPartItem[PartType.Belt].m_EquipmentDef;
            if (m_PlayerPartItem.ContainsKey(PartType.Cloth))
                Hop += m_PlayerPartItem[PartType.Cloth].m_EquipmentDef;
            if (m_PlayerPartItem.ContainsKey(PartType.Glove))
                Hop += m_PlayerPartItem[PartType.Glove].m_EquipmentDef;
            if (m_PlayerPartItem.ContainsKey(PartType.Shoe))
                Hop += m_PlayerPartItem[PartType.Shoe].m_EquipmentDef;
            return Hop;
        }

    }

    private int m_BuffDamage = 0;   //데미지 계산용

    //체력바 UI
    [Header("HpBarUI")]
    public GameObject m_HpBarObj = null; //체력바 오브젝트
    private Image m_HpBarImg = null; //체력바 이미지
    private Text m_HpBarText = null; //체력 수치 표시 텍스트
   

    [Header("ExpUI")]
    public GameObject m_ExpObj = null; //경험치바 오브젝트
    private Image m_ExpImg = null; //경험치바 이미지
    private Text m_LvText = null; //경험치 표시 텍스트

   [Header("Msg")]
    //데미지 텍스트 출력 
    public Transform m_MsgBoxTr = null;
    public Transform m_DamageTxtTr = null;
    public GameObject m_MsgBoxPrefab = null;
    public GameObject m_LvUpTxt = null; //레벨업 전용 텍스트
    //이펙트

    public ParticleSystem m_LevelEffect = null;

    float m_HitTimer = -1.0f;

    public GameObject m_NpcObj = null; //지금 보고 있는 NPC
    public NPC m_Npc= null; //지금 보고 있는 NPC
    LayerMask m_NpcLayer = 1;

    public  delegate void Event();
    public Event LevelUpEvent;

    private void Awake()
    {       
        weapon = gameObject.GetComponentInChildren<Weapon>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
       
        nv = GetComponent<NavMeshAgent>();
  

        m_HpBarImg = m_HpBarObj.GetComponentsInChildren<Image>()[1];
        m_HpBarText = m_HpBarObj.GetComponentInChildren<Text>();

        m_ExpImg = m_ExpObj.GetComponentsInChildren<Image>()[1];
        m_LvText = m_ExpObj.GetComponentInChildren<Text>();

        m_Camera = Camera.main;
        m_LayerMask = 1 << LayerMask.NameToLayer("GROUND");
        m_LayerMask |= 1 << LayerMask.NameToLayer("NPC");

        
        m_PlayerInventory = GetComponent<PlayerInventory>();
        //Coin
      
        m_NpcLayer = 1 << LayerMask.NameToLayer("NPC");

        //m_MeshOutline 설정
        m_MeshOutline = GetComponentInChildren<Outline>();

       
        
    }
    private void Start()
    {
        SetHpUI();
        SetExpUI();
    }

    private void OnEnable()
    {
        if(weapon.m_WeaponData != null)
        {
            animator.SetBool("UseWeapon", true);
        }
        else
            animator.SetBool("UseWeapon", false);

    }
    private void Update()
    {
     
        MouseInput();   //마우스 피킹
        CheckNpcUpdate();   //NPC 찾기

        //피격 연출 
        Hit_Update();

    }
    private void FixedUpdate()
    {
        MousePickMove(); //마우스로 이동 하는 함수               
    }

   
    #region ---------------MouseMode---------------
    public void MouseInput()
    {
        m_Vec = rigidbody.velocity;
        m_Vec.x = 0;
        m_Vec.z = 0;


        if (bIsAttack || bIsHit || !bIsMove)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) && !InGameMgr.IsPointerOverUIObject())
        {         
            m_MousePos = m_Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(m_MousePos, out hitInfo, Mathf.Infinity, m_LayerMask.value))
            {
               
                MousePicking(hitInfo);                         
            }
        }
    }

    public void MousePicking(RaycastHit hitInfo) //마우스 위치 마크
    {
        m_CacLenVec = hitInfo.point - transform.position;       //거리 계산을 위한
        //m_CacLenVec.y = 0;

        if (m_CacLenVec.magnitude < 0.5f)    // 현재 목표 까지의 거리가 0.5 보다 작으면
            return;
     
        if (hitInfo.collider.CompareTag("GROUND"))
        {   
            nv.SetDestination(hitInfo.point + (Vector3.up));                               
            CursorMarkOn(hitInfo.point);    //마크 커서 On          
        }
        else if(hitInfo.collider.CompareTag("NPC"))
        {
            nv.SetDestination(hitInfo.point);
            if (m_CursorMark.activeSelf)
                m_CursorMark.SetActive(false);

            m_NpcObj = hitInfo.collider.gameObject;
            m_Npc = null;
        }

        nv.nextPosition = transform.position;
        nv.isStopped = false;
        animator.SetBool("IsMove", true); // 애니메이션 적용  
        m_IsPickMvOnOff = true;  //마우스 이동 시작

    }
    void CursorMarkOn(Vector3 a_PickPos)
    {
        if (m_CursorMark == null) //게임오브젝트가 있는지
            return;
        m_CursorMark.SetActive(false);
        m_CursorMark.transform.position =
            new Vector3(a_PickPos.x, a_PickPos.y + 0.2f, a_PickPos.z);    //목표 위치에 마크 이동
        m_CursorMark.SetActive(true);                                                //마크 On

    }

    void MousePickMove()  //마우스로 실제 움직이게 하는 함수
    {
        if (m_IsPickMvOnOff == true)// 마우스 피킹 무브중일때 
        {
            if (bIsAttack || bIsHit || !bIsMove)    // 공격하거나 피격 당할실 
            {
                animator.SetBool("IsMove", false);      // 애니메이션 변경
                m_IsPickMvOnOff = false;                  // 마우스 이동 Off

                nv.isStopped = true;
                //nv.velocity = Vector3.zero;
                return;
            }          
            
            m_CacLenVec = nv.destination - transform.position; // 거리 계산
            //m_CacLenVec.y = 0.0f;
           
            if (m_CacLenVec.magnitude < 0.5f)
            {
                animator.SetBool("IsMove", false);
                m_IsPickMvOnOff = false;
                nv.isStopped = true;
                nv.velocity = Vector3.zero;
                return;
            }
        }
        else // m_IsPickMvOnOff ==false  면 이동마크 끄기
        {
            if (m_CursorMark.activeSelf)
                m_CursorMark.SetActive(false);
        }
    }
    #endregion

    private void OnDisable()
    {
        if (m_LvUpTxt)
            m_LvUpTxt.SetActive(false);
    }


    public void SetHpUI()
    {
        m_HpBarImg.fillAmount = (float)m_PlayerStatus.m_CurHp / (float)m_PlayerStatus.m_MaxHp;
        m_HpBarText.text = m_PlayerStatus.m_CurHp.ToString() + " / " + m_PlayerStatus.m_MaxHp.ToString();
    }

    void SetExpUI()
    {
        m_ExpImg.fillAmount = (float)m_PlayerStatus.m_CurExp / (float)m_PlayerStatus.m_NextExp;
        m_LvText.text = "Lv " +  m_PlayerStatus.m_Lv;
    }

    public void OnDamge(int a_Damage = 0)
    {
        if (m_PlayerStatus.m_CurHp <= 0)
            return;

        //들어온 데미지와 방어력 수치 계산
        m_BuffDamage = a_Damage - m_PlayerDefPw;
        if (m_BuffDamage <= 0)
        {
            m_BuffDamage = 0;
            return;
        }
            
        Vector2 pos = m_DamageTxtTr.position;
        pos.x = pos.x +(Random.Range(-20.0f,20.0f));

        InGameMgr.Inst.SpanwDamageTxt(pos, TxtType.PlayerDamage, m_BuffDamage ); //데미지 숫자 이펙트
        m_PlayerStatus.m_CurHp -= m_BuffDamage;//데미지 적용

        if(m_PlayerStatus.m_CurHp <= 0)
        {
            // 죽음
        }

        SetHpUI();


        m_MeshOutline.OutlineColor = Color.red;
        m_HitTimer = 0.5f;
        if (!bIsAttack && !bIsHit)
        {
            bIsHit = true;
            animator.SetTrigger("Hit");
        }
      
    }

    public void AddExp(int a_Exp)
    {
        m_PlayerStatus.m_CurExp += a_Exp;
        if(m_PlayerStatus.m_CurExp >= m_PlayerStatus.m_NextExp)
        {
            //레벨업
            m_PlayerStatus.LevelUp();
            m_SkillPoint += 10;
            SetHpUI();
            m_LevelEffect.Play();
            m_LvUpTxt.SetActive(false);
            m_LvUpTxt.SetActive(true);

            LevelUpEvent?.Invoke();
            SoundMgr.Inst.PlaySound("LevelUp");
        }      
        
        SetExpUI();
      
    }

    

    void Hit_Update()
    {
        if (m_HitTimer > 0)
        {
            m_HitTimer -= Time.deltaTime;

            if(m_HitTimer <= 0.3f)
                if (bIsHit)
                    bIsHit = false;

            if (m_HitTimer <= 0)
            {
               

                m_MeshOutline.OutlineColor = Color.white;
            }
        } 
    }

    void CheckNpcUpdate()
    {
        if (m_NpcObj != null  && m_Npc == null)
        {         
            if((m_NpcObj.transform.position - transform.position).magnitude < 3.0f)
            {
                transform.LookAt(m_NpcObj.transform);
                m_Npc = m_NpcObj.GetComponent<NPC>();
                m_Npc.Talk(this);

                animator.SetBool("IsMove", false);
                m_IsPickMvOnOff = false;
                nv.isStopped = true;
                nv.velocity = Vector3.zero;
            }     
        }

        else if(m_Npc != null && (m_NpcObj.transform.position - transform.position).magnitude > 4.0f)
        {
            m_Npc.TalkEnd(this);
            m_Npc = null;
            m_NpcObj = null;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

}
