using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHp;
    private int hp = 0;
    // private AudioSource slapsound;
    void Start()
    {
        hp = maxHp;
       // slapsound = GetComponent<AudioSource>();
    }
    void OnCollisionEnter(Collision collision)
    {
        //GetSlapped();
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Player");
        hp--;
     //   this.slapsound.Play();
        GameManager.Instance.isStopTimer = true;
    }

}
