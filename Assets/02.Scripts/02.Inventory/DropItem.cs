using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{

    [HideInInspector] public ItemData m_ItemData;
    public GameObject m_Mesh;
    public MeshFilter m_ItemMesh;
    public MeshRenderer m_ItemMeshRenderer;

    public SphereCollider m_SphereCollider;
    public Rigidbody m_Rigidbody;

    Vector3 m_Force = Vector3.zero;


    public void InitDropItem(ItemData a_ItemData)
    {
        m_ItemData = a_ItemData;    //가지고 있는 아이템 적용
        m_ItemMesh.mesh = a_ItemData.m_ItemMesh;    //메쉬 적용
        m_ItemMeshRenderer.material = a_ItemData.m_ItemMatrl;   //메테리얼 적용
        m_Mesh.transform.localPosition = a_ItemData.m_DropMeshPos;

        m_Force.y = 400.0f;
        m_Force.x = Random.Range(-1,2) * Random.Range(80, 150);
        m_Force.z = Random.Range(-1,2) * Random.Range(80, 150);
        m_Rigidbody.AddForce(m_Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.CompareTag("GROUND"))
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_SphereCollider.isTrigger = true;
            m_Rigidbody.useGravity = false;
        }
    }


}
