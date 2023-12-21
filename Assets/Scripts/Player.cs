using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.Oculus;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private OVRHand rightHand;
    [SerializeField] private Transform playerInitTransform;
    private Transform vrCamera;
    private int hp = 0;

    private AudioSource slapsound;
    
    private Vector3 newPosRight;
    private Vector3 prevPosRight;
    private Vector3 rightHandVelocity;
    void PlaySlapSound(int volume)
    {
        this.slapsound.volume = volume * 0.01f;
        this.slapsound.Play();
    }

    public int Hp
    {
        get => hp;
    }

    private void Start()
    {
        hp = maxHp;
        slapsound = GetComponent<AudioSource>();
        vrCamera = transform.GetChild(0);
    }
    private void Update()
    {
        newPosRight = rightHand.transform.position;
        rightHandVelocity = (newPosRight - prevPosRight);
        prevPosRight = newPosRight;
    }
    public Vector3 GetRightHandVelocity()
    {
        return rightHandVelocity;
    }
    public void OnV()
    {//핸드트래킹 V 입력시 실행
        InitPosition();
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Player");
        hp--;

        // HP 0 => 패배
        if (hp <= 0)
        {
            //hp가 0이 될 시
            Debug.Log("패배");
            GameManager.Instance.isGameOver = true;
        }

        // Sound
        PlaySlapSound(100);

        GameManager.Instance.isStopTimer = true;
        StartCoroutine(CameraRotateAfterSlaped());
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

            Vector3 newPos = new Vector3(
                playerInitTransform.position.x - centerEyeAnchor.transform.position.x,
                playerInitTransform.position.y - centerEyeAnchor.transform.position.y,
                playerInitTransform.position.z - centerEyeAnchor.transform.position.z
            );
            gameObject.transform.position += newPos;
        }
    }

    IEnumerator CameraRotateAfterSlaped()
    {
        float elapsedTime = 0;
        while (elapsedTime < 0.2f)
        {
            vrCamera.transform.Rotate(Vector3.up, 90 * Time.deltaTime * 5);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        elapsedTime = 0;
        while (elapsedTime < 1.0f)
        {
            vrCamera.transform.Rotate(Vector3.down, 90 * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
