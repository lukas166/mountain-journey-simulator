using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [Header("Text UI")]
    public TextMeshProUGUI judulQuestText;
    public TextMeshProUGUI pageNumberText;

    [Header("Image UI")]
    public Image questImage;

    [Header("Button UI")]
    public Button previousButton;
    public Button nextButton;
    public Button closeButton;

    [Header("Quest Window")]
    public GameObject questWindow;

    [Header("Ray Interactor / Laser")]
    public GameObject rightHandLaser;

    [Header("Quest Default Saat Dibuka Dari Radial")]
    [TextArea(3, 10)]
    public string[] halamanDefaultQuest;

    [Header("Gambar Default Quest")]
    public Sprite[] gambarDefaultQuest;

    private string[] halamanQuest;
    private Sprite[] gambarQuest;
    private int halamanAktif = 0;

    public void UpdateQuestHalaman(string[] halamanBaru)
    {
        halamanQuest = halamanBaru;
        gambarQuest = null;
        halamanAktif = 0;

        MunculkanQuestWindow();
    }

    public void UpdateQuestHalaman(string[] halamanBaru, Sprite[] gambarBaru)
    {
        halamanQuest = halamanBaru;
        gambarQuest = gambarBaru;
        halamanAktif = 0;

        MunculkanQuestWindow();
    }

    public void BukaQuestDariRadial()
    {
        halamanAktif = 0;

        if (halamanQuest == null || halamanQuest.Length == 0)
        {
            if (halamanDefaultQuest != null && halamanDefaultQuest.Length > 0)
            {
                halamanQuest = halamanDefaultQuest;
                gambarQuest = gambarDefaultQuest;
            }
            else
            {
                halamanQuest = new string[]
                {
                    "Belum ada quest yang aktif."
                };

                gambarQuest = null;
            }
        }

        MunculkanQuestWindow();
    }

    private void MunculkanQuestWindow()
    {
        if (questWindow != null)
        {
            questWindow.SetActive(true);
        }

        if (rightHandLaser != null)
        {
            rightHandLaser.SetActive(true);
        }

        TampilkanHalaman();
    }

    private void TampilkanHalaman()
    {
        if (halamanQuest == null || halamanQuest.Length == 0) return;

        if (judulQuestText != null)
        {
            judulQuestText.text = halamanQuest[halamanAktif];
        }

        if (pageNumberText != null)
        {
            pageNumberText.text = (halamanAktif + 1) + " / " + halamanQuest.Length;
        }

        if (questImage != null)
        {
            if (gambarQuest != null && halamanAktif < gambarQuest.Length && gambarQuest[halamanAktif] != null)
            {
                questImage.sprite = gambarQuest[halamanAktif];
                questImage.gameObject.SetActive(true);
            }
            else
            {
                questImage.sprite = null;
                questImage.gameObject.SetActive(false);
            }
        }

        bool halamanPertama = halamanAktif == 0;
        bool halamanTerakhir = halamanAktif >= halamanQuest.Length - 1;

        if (previousButton != null)
        {
            previousButton.gameObject.SetActive(!halamanPertama);
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(!halamanTerakhir);
        }

        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(halamanTerakhir);
        }
    }

    public void HalamanBerikutnya()
    {
        if (halamanQuest == null || halamanQuest.Length == 0) return;

        if (halamanAktif < halamanQuest.Length - 1)
        {
            halamanAktif++;
            TampilkanHalaman();
        }
    }

    public void HalamanSebelumnya()
    {
        if (halamanQuest == null || halamanQuest.Length == 0) return;

        if (halamanAktif > 0)
        {
            halamanAktif--;
            TampilkanHalaman();
        }
    }

    public void TutupQuestWindow()
    {
        if (questWindow != null)
        {
            questWindow.SetActive(false);
        }

        if (rightHandLaser != null)
        {
            rightHandLaser.SetActive(false);
        }
    }

    [ContextMenu("Test Update Quest Halaman")]
    public void TestUpdate()
    {
        string[] testHalaman = new string[]
        {
            "Halaman 1: Kamu baru saja memasuki area hutan.",
            "Halaman 2: Perhatikan tanda-tanda di sekitar jalan.",
            "Halaman 3: Lanjutkan perjalanan menuju basecamp."
        };

        UpdateQuestHalaman(testHalaman, gambarDefaultQuest);
    }
}
