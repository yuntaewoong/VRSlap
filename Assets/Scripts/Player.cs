using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHp;
    private int hp = 0;
    void Start()
    {
        hp = maxHp;
    }
    void OnCollisionEnter(Collision collision)
    {
        //GetSlapped();
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Player");
        hp--;
        
        GameManager.Instance.isStopTimer = true;
    }

}
