using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
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
        StartCoroutine(KeluarGameRoutine());
    }

    IEnumerator KeluarGameRoutine()
    {
        Debug.Log("Kembali ke Menu Scene dengan fade.");

        yield return StartCoroutine(FadeOutOtomatis());

        int sceneSekarang = SceneManager.GetActiveScene().buildIndex;
        int sceneSebelumnya = sceneSekarang - 1;

        if (sceneSebelumnya >= 0)
        {
            SceneManager.LoadScene(sceneSebelumnya);
        }
        else
        {
            Debug.LogWarning("Tidak ada scene sebelumnya.");
        }
    }

    IEnumerator FadeOutOtomatis()
    {
        GameObject canvasObject = new GameObject("Auto Fade Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasGroup canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        GameObject imageObject = new GameObject("Fade Image");
        imageObject.transform.SetParent(canvasObject.transform, false);

        Image image = imageObject.AddComponent<Image>();
        image.color = Color.black;

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        float durasiFade = 1f;
        float timer = 0f;

        while (timer < durasiFade)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = timer / durasiFade;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}