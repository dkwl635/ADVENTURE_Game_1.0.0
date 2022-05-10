﻿using UnityEngine;
using UnityEngine.UI;

public class SpawnMgr : MonoBehaviour
{
    public GameObject m_MonsterPrefab = null;       //몬스터 프리팹
    public GameObject m_BossPrefab = null;       //보스 몬스터 프리팹

    private int m_CurMonCount = 0;      //현재 소환된 몬스터의 수
    public int m_MaxMonConunt = 1;  //최대 소환 몬스터의 수

    public Transform[] m_MonsterSpawnPos = null;    //몬스터들의 스폰 위치 정보
    public Transform m_BossSpawnPos = null;    //보스몬스터들의 스폰 위치 정보

    private NomalMonster[] m_MonterList;             //소환된 노말몬스터를 관리하기 위한 리스트
    private MonsterCtrl m_Boss;             //소환된 보스몬스터를 관리

    private GameObject m_Player; //플레이어 감지 
    bool bSpawnMon = false;                                 // 일반몬스터가 스폰 되었는지
   
    public Button m_BossSpawnBtn = null;
    public GameObject m_KillGageObj = null;
    public Image m_KillGage = null;
    UnityEngine.UI.Outline outline = null;

    public GameObject m_BossHpBar = null;
    public MonHpBarCtrl m_BossHpCtrl = null;
    bool bSpawnBoss = false;

    int m_MonseterKillCount = 0;
    [SerializeField] int m_BossSpawnCount = 10;

    public GameObject m_OutRing = null;
    public GameObject m_InRing = null;


    void Start()
    {
        GameObject[] spanwpos = GameObject.FindGameObjectsWithTag("MONSTERSPAWN");
        m_MonsterSpawnPos = new Transform[spanwpos.Length];
        for (int i = 0; i < spanwpos.Length; i++)
        {
            m_MonsterSpawnPos[i] = spanwpos[i].transform;
        }

        if (m_BossSpawnBtn != null)
        {
            m_BossSpawnBtn.onClick.AddListener(SpawnBoss);
            outline = m_BossSpawnBtn.GetComponent<UnityEngine.UI.Outline>();
            outline.enabled = false;
        }

        m_KillGage.fillAmount = (float)m_MonseterKillCount / (float)m_BossSpawnCount;

        SetMonster();

   

    }

    private void FixedUpdate()
    {
        if(bSpawnMon && !bSpawnBoss)
        for (int i = 0; i < m_MonterList.Length; i++)
        {
            m_MonterList[i].Think_FixedUpdate();
            m_MonterList[i].Action_FixedUpdate();
        }
    }

    void AddMonsterKillCount()
    {
        m_MonseterKillCount++;
     
        m_KillGage.fillAmount = (float)m_MonseterKillCount / (float)m_BossSpawnCount;

        if (m_MonseterKillCount >= m_BossSpawnCount)
        {
            outline.enabled = false;
        }
    }


    
   void SetMonster()
    {
        m_MonterList = new NomalMonster[m_MaxMonConunt];
        for (int i = 0; i < m_MaxMonConunt; i++)
        {
            m_MonterList[i] = Instantiate(m_MonsterPrefab, transform).GetComponent<NomalMonster>();
            m_MonterList[i].transform.position = m_MonsterSpawnPos[i + 1].position;
            m_MonterList[i].m_SpawnPos = m_MonsterSpawnPos[i + 1].position;
            m_MonterList[i].DieEvent += AddMonsterKillCount;
            m_MonterList[i].Init();
            m_MonterList[i].gameObject.SetActive(false);

        }
    }

    void SpawnMonster()
    {
        m_KillGageObj.SetActive(true);
        m_MonseterKillCount = 0;
        m_KillGage.fillAmount = (float)m_MonseterKillCount / (float)m_BossSpawnCount;

        if (m_MonterList.Length > 0)
            for (int i = 0; i < m_MonterList.Length; i++)
            {
                m_MonterList[i].gameObject.SetActive(true);               
                m_MonterList[i].Spawn();
            }
    }

    void DeSpawnMonster()
    {
        for (int i = 0; i < m_MaxMonConunt; i++)
        {
            m_MonterList[i].gameObject.SetActive(false);
        }     
    }

    void SpawnBoss()
    {
        Debug.Log("보스스폰 시도");

        if (m_MonseterKillCount < m_BossSpawnCount)
            return;

        m_Boss = Instantiate(m_BossPrefab, transform).GetComponent<BossMonster>();
        m_Boss.transform.position = m_BossSpawnPos.transform.position;
        m_Boss.m_SpawnPos = m_BossSpawnPos.transform.position;
        m_Boss.m_HpBarCtrl = this.m_BossHpCtrl;
        m_BossHpCtrl.SetHpBar(m_Boss.m_MonsterStatus.m_CurHp, m_Boss.m_MonsterStatus.m_MaxHp);
        
        m_Boss.DieEvent = DieBoss;

        m_MonseterKillCount = 0;
        m_KillGage.fillAmount = (float)m_MonseterKillCount / (float)m_BossSpawnCount;
        m_KillGageObj.SetActive(false);

        DeSpawnMonster();
        bSpawnMon = false;
        bSpawnBoss = true;

    }

    void DieBoss()
    {      
        bSpawnBoss = false;    
        m_Boss = null;

        m_OutRing.SetActive(true);
        m_InRing.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (bSpawnBoss)
            return;

        if(other.CompareTag("Player") && !bSpawnMon)
        {
            m_OutRing.SetActive(false);
            m_InRing.SetActive(true);
            SpawnMonster();
            bSpawnMon = true;
           
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!bSpawnMon)
            return;
    
        if (other.CompareTag("Player"))
        {
            m_OutRing.SetActive(true);
            m_InRing.SetActive(false);

            DeSpawnMonster();
            bSpawnMon = false;
            m_KillGageObj.SetActive(false);
        }
    }

}
