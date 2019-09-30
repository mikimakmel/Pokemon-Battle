using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_ShakeScreen : MonoBehaviour
{
    Vector3 cameraInitialPosition;
    public float shakeMagnetude = 0.05f;
    public float shakeTime = 0.05f;
    public Camera Camera;
    public Canvas Canvas_Battle;

    public IEnumerator ShakeIt()
    {
        //Canvas_Battle.renderMode = RenderMode.WorldSpace;
        cameraInitialPosition = Camera.transform.position;
        InvokeRepeating("StartCameraShaking", 0f, 0.005f);
        Invoke("StopCameraShaking", shakeTime);
        yield return new WaitForSeconds(0.06f);
        //Canvas_Battle.renderMode = RenderMode.ScreenSpaceCamera;
    }

    void StartCameraShaking()
    {
        float cameraShakingOffsetX = Random.value * shakeMagnetude * 2 - shakeMagnetude;
        float cameraShakingOffsetY = Random.value * shakeMagnetude * 2 - shakeMagnetude;
        Vector3 cameraIntermadiatePosition = Camera.transform.position;
        cameraIntermadiatePosition.x += cameraShakingOffsetX;
        cameraIntermadiatePosition.y += cameraShakingOffsetY;
        Camera.transform.position = cameraIntermadiatePosition;
    }

    void StopCameraShaking()
    {
        CancelInvoke("StartCameraShaking");
        Camera.transform.position = cameraInitialPosition;
    }
}
