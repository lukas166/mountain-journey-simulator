using UnityEngine;

public class FootSocketFollowCamera : MonoBehaviour
{
    [Header("Referensi")]
    public Transform mainCamera;

    [Header("Pengaturan Rotasi")]
    public bool ikutRotasiKamera = true;

    void LateUpdate()
    {
        if (mainCamera == null)
        {
            return;
        }

        if (!ikutRotasiKamera)
        {
            return;
        }

        Vector3 forward = mainCamera.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
        {
            return;
        }

        forward.Normalize();

        Quaternion rotasiArahKamera = Quaternion.LookRotation(forward, Vector3.up);

        transform.rotation = rotasiArahKamera;
    }
}