using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private Player player;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyHand")
        {
            Debug.Log("Detect Enemy Hand Collisin On Player");
            player.GetSlapped();
        }
    }
}
