using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPart : MonoBehaviour
{
    public PartType m_PartType; //파츠가 어떤 타입인지
    public EquipmentItemData m_Equipment; //착용중인 장비 아이템 정보
    public SkinnedMeshRenderer m_SkinMesh; //보여주는 메쉬 정보

    public int m_EquipmentDef
    {
        get { return m_Equipment != null ?  m_Equipment.m_DefPw : 0; }
    }

    Mesh m_OrgInMesh = null;
    Material m_OrgInMatrl = null;

    private void Awake()
    {
        m_Equipment = null;
        m_SkinMesh = GetComponent<SkinnedMeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    { 
     
        m_OrgInMesh = m_SkinMesh.sharedMesh;
        m_OrgInMatrl = m_SkinMesh.material;
    }

    public void ChangeEquipment(EquipmentItemData a_NewEquipment)
    {
        m_Equipment = a_NewEquipment;
        m_SkinMesh.sharedMesh = a_NewEquipment.m_ItemMesh;
        m_SkinMesh.material = a_NewEquipment.m_ItemMatrl;
    }

    public void OffEquipment()
    {
        m_Equipment = null;
        m_SkinMesh.sharedMesh = m_OrgInMesh;
        m_SkinMesh.material = m_OrgInMatrl;
    }

}
