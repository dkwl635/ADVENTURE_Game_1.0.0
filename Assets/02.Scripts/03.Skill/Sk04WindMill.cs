using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sk04WindMill : Skill
{
   
    private float m_radus = 3.0f;                //반지름
    private Vector3 m_tempPos = Vector3.zero;    //계산용
   
    string m_AnimationName = "WindMill";

    public ParticleSystem m_SkillEffect;


    float timer = 0.0f;
    float attackTimer = 0.0f;

    public override void Init(Player a_player)   //기본 셋팅
    {
        base.Init(a_player);     

    }

    private float m_SkillDamage
    {
        get { return (m_Damage  * m_Lv) + player.m_PlayerAttPw * 0.4f; }
    }

    public override string SkillInfoTxt()
    {
        return "\n기본 스킬데미지 : " + m_SkillDamage.ToString()
            + "\n총 7회 피격"
            + "\n총 스킬 데미지 : " + (m_SkillDamage * 7).ToString();
    }


    public override IEnumerator SkillStart()
    {
        player.bIsAttack = true ;

        m_SkillEffect.transform.SetParent(playerTr);
        m_SkillEffect.transform.localPosition = Vector3.zero;
        m_SkillEffect.gameObject.SetActive(true);

        playerTr.LookAt(SkillMgr.Inst.m_MousePos);
        animator.SetTrigger(m_SkillName);
    
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(m_AnimationName))//교체 시간 대기                   
            yield return null;

        animator.speed = 0.7f;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)  // 특정 구간 전까지 대기
            yield return null;

        animator.speed = 0.0f;

        SoundMgr.Inst.PlaySound("Sk_04");
        while(timer < 3.5f)
        {
            yield return null;
            playerTr.Rotate(new Vector3(0, 600 * Time.deltaTime, 0));
            timer += Time.deltaTime;

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0.0f)
            {
                // 공격범위안에 있는 콜리더 가져오기
                RaycastHit[] hits = Physics.SphereCastAll(playerTr.position, m_radus, Vector3.up, 0, m_SkillTargetLayer);
                rigidbody.mass = 1;
                if (hits.Length > 0) // 콜리더가 있다면
                {                          //데미지 적용
                    for (int i = 0; i < hits.Length; i++)
                    {
                        hits[i].collider.GetComponent<MonsterCtrl>().OnDamge((int)m_SkillDamage, player);
                    }
                }
                attackTimer = 0.5f;
            }
        
        }
        timer = 0.0f;
        animator.speed = 1.0f;
        m_SkillEffect.gameObject.SetActive(false);

        while (animator.GetCurrentAnimatorStateInfo(0).IsName(m_AnimationName))//종료 까지 대기                   
            yield return null;

        player.bIsAttack = false;
        //애니메이션 종료시
       
        yield return new WaitForSeconds(0.5f);
       
                                          //공격끝남
        yield return null;
    }
}
