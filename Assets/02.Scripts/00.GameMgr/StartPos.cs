using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPos : MonoBehaviour
{
    public Player player;
   
    void Start()
    {

        StartCoroutine(SetPlayerPos());
    }

    IEnumerator SetPlayerPos()
    {
        yield return null;

        player = FindObjectOfType<Player>(true);

        if (player)
        {
            player.transform.position = this.transform.position;
            player.gameObject.SetActive(true);
        }

    }


}
