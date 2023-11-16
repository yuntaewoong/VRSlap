using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.Oculus;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Transform centerEyeAnchor;
    private int hp = 0;
    // private AudioSource slapsound;
    public int Hp
    {
        get => hp;
    }
    void Start()
    {
        hp = maxHp;
       // slapsound = GetComponent<AudioSource>();
    }
    public void OnClap()
    {//핸드트래킹 박수입력시 실행
        InitPosition();
    }
    private void OnTriggerEnter(Collider other)
    {//플레이어가 싸대기 맞은 경우
        GetSlapped();
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Player");
        hp--;
     //   this.slapsound.Play();
        GameManager.Instance.isStopTimer = true;
    }
    private void InitPosition()
    {
        RecenterHeadset();
    }
    private void RecenterHeadset()
    {
        if (OVRManager.display != null)
        {
            float currentRotY = centerEyeAnchor.transform.eulerAngles.y; //This refence a CenterEyeAnchor
            float difference = 0 - currentRotY;
            gameObject.transform.Rotate(0, difference, 0); // InteractionSystem.Anchor This refence a Player

            Vector3 newPos = new Vector3(0 - centerEyeAnchor.transform.position.x, 0, 0 - centerEyeAnchor.transform.position.z);
            gameObject.transform.position += newPos;
        }
    }
}
