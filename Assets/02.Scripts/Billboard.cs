using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private void LateUpdate()
    {

        Quaternion rot = Quaternion.LookRotation(Camera.main.transform.position);
        transform.localRotation = rot;
    }



}
