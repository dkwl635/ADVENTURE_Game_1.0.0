using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Player player;
    public WeaponData m_WeaponData; //착용중인 장비 아이템 정보
    public Transform m_WeaponHandTr; //무기 핸드 위치 
    public MeshFilter m_MeshFiter; //보여주는 메쉬 정보
    public MeshRenderer m_MeshRenderer; //메쉬머터리얼 바꾸기 위해
    public Transform m_EffPos;  //무기의 이펙트 위치
   
    
    public int m_WeaponPw { get { 
            
            return m_WeaponData != null ? m_WeaponData.m_AttPw : 0; } }


    private void Awake()
    {
        player = GetComponentInParent<Player>();
        m_MeshFiter = GetComponentInChildren<MeshFilter>();
        m_MeshRenderer = GetComponentInChildren<MeshRenderer>();
    }
    public void ChangeWeapon(WeaponData a_NewWeapon)    
    {
        m_WeaponHandTr.gameObject.SetActive(true);
        m_WeaponHandTr.localPosition = a_NewWeapon.m_HandPos;
        m_WeaponData = a_NewWeapon;
        m_MeshFiter.mesh = a_NewWeapon.m_ItemMesh;
        m_MeshRenderer.material = a_NewWeapon.m_ItemMatrl;
        player.bIsWeapon = true;
    }

    public void OffWepon()
    {
        m_WeaponData = null;
        m_WeaponHandTr.gameObject.SetActive(false);
        player.bIsWeapon = false;
    }


}
