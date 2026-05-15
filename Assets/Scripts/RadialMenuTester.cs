using UnityEngine;

public class RadialMenuTester : MonoBehaviour
{
    [Header("Masukkan UI Quest Window di sini")]
    public GameObject questWindow;

    [Header("Masukkan UI Exit Confirm Window di sini")]
    public GameObject exitConfirmWindow;

    [Header("Masukkan Objek Laser Kanan di sini")]
    public GameObject rightHandLaser;

    void Start()
    {
        if (questWindow != null) questWindow.SetActive(false);
        if (exitConfirmWindow != null) exitConfirmWindow.SetActive(false);

        if (rightHandLaser != null) rightHandLaser.SetActive(false);
    }

    public void ChangeGroundColor(int menuIndex)
    {
        switch (menuIndex)
        {
            case 0:
                if (exitConfirmWindow != null) exitConfirmWindow.SetActive(true);
                if (questWindow != null) questWindow.SetActive(false);
                if (rightHandLaser != null) rightHandLaser.SetActive(true);
                break;

            case 1:
                if (questWindow != null) questWindow.SetActive(true);
                if (exitConfirmWindow != null) exitConfirmWindow.SetActive(false);
                if (rightHandLaser != null) rightHandLaser.SetActive(true);
                break;

            case 2:
                break;

            case 3:
                break;

            default:
                break;
        }
    }

    public void TutupQuestWindow()
    {
        if (questWindow != null) questWindow.SetActive(false);
        if (rightHandLaser != null) rightHandLaser.SetActive(false);

        Debug.Log("Quest Window ditutup, Laser dimatikan.");
    }

    public void TutupExitConfirmWindow()
    {
        if (exitConfirmWindow != null) exitConfirmWindow.SetActive(false);
        if (rightHandLaser != null) rightHandLaser.SetActive(false);

        Debug.Log("Exit Confirm Window ditutup, Laser dimatikan.");
    }

    public void KeluarGame()
    {
        Debug.Log("Game keluar.");

        #if UNITY_EDITOR
            // Menghentikan mode Play di dalam Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Menutup aplikasi final (Build)
            Application.Quit();
        #endif
    }
}