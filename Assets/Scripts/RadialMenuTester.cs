using UnityEngine;

public class RadialMenuTester : MonoBehaviour
{
    [Header("Masukkan UI Quest Window di sini")]
    public GameObject questWindow;

    [Header("Masukkan UI Exit Confirm Window di sini")]
    public GameObject exitConfirmWindow;

    public GameObject rightHandLaser;

    [Header("Masukkan Audio Source Ambient Hutan di sini")]
    public AudioSource ambientAudioSource;

    private bool ambientMuted = false;

    void Start()
    {
        if (questWindow != null) questWindow.SetActive(false);
        if (exitConfirmWindow != null) exitConfirmWindow.SetActive(false);

        if (rightHandLaser != null) rightHandLaser.SetActive(false);

        if (ambientAudioSource != null)
        {
            ambientAudioSource.mute = false;
            ambientMuted = false;
        }
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
                ToggleAmbientSound();
                break;

            case 3:
                break;

            default:
                break;
        }
    }

    public void ToggleAmbientSound()
    {
        if (ambientAudioSource == null)
        {
            Debug.Log("Ambient Audio Source belum dimasukkan.");
            return;
        }

        ambientMuted = !ambientMuted;
        ambientAudioSource.mute = ambientMuted;

        if (ambientMuted)
        {
            Debug.Log("Suara ambient dimatikan.");
        }
        else
        {
            Debug.Log("Suara ambient dinyalakan kembali.");
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