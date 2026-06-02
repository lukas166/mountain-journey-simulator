using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCWindowManager : MonoBehaviour
{
    [Header("Text UI")]
    public TextMeshProUGUI judulNPCText;
    public TextMeshProUGUI pageNumberText;

    [Header("Image UI")]
    public Image npcImage;

    [Header("Button UI")]
    public Button previousButton;
    public Button nextButton;
    public Button closeButton;

    [Header("NPC Window")]
    public GameObject npcWindow;

    [Header("Ray Interactor / Laser")]
    public GameObject rightHandLaser;

    [Header("NPC Default")]
    [TextArea(3, 10)]
    public string[] halamanDefaultNPC;

    [Header("Gambar Default NPC")]
    public Sprite[] gambarDefaultNPC;

    private string[] halamanNPC;
    private Sprite[] gambarNPC;
    private int halamanAktif = 0;

    private void Start()
    {
        if (previousButton != null)
        {
            previousButton.onClick.RemoveListener(HalamanSebelumnya);
            previousButton.onClick.AddListener(HalamanSebelumnya);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(HalamanBerikutnya);
            nextButton.onClick.AddListener(HalamanBerikutnya);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(TutupNPCWindow);
            closeButton.onClick.AddListener(TutupNPCWindow);
        }

        if (npcWindow != null)
        {
            npcWindow.SetActive(false);
        }
    }

    public void UpdateNPCHalaman(string[] halamanBaru)
    {
        halamanNPC = halamanBaru;
        gambarNPC = null;
        halamanAktif = 0;

        MunculkanNPCWindow();
    }

    public void UpdateNPCHalaman(string[] halamanBaru, Sprite[] gambarBaru)
    {
        halamanNPC = halamanBaru;
        gambarNPC = gambarBaru;
        halamanAktif = 0;

        MunculkanNPCWindow();
    }

    private void MunculkanNPCWindow()
    {
        if (npcWindow != null)
        {
            npcWindow.SetActive(true);
        }

        if (rightHandLaser != null)
        {
            rightHandLaser.SetActive(true);
        }

        TampilkanHalaman();
    }

    private void TampilkanHalaman()
    {
        if (halamanNPC == null || halamanNPC.Length == 0)
        {
            if (halamanDefaultNPC != null && halamanDefaultNPC.Length > 0)
            {
                halamanNPC = halamanDefaultNPC;
                gambarNPC = gambarDefaultNPC;
            }
            else
            {
                halamanNPC = new string[]
                {
                    "Tidak ada teks NPC."
                };

                gambarNPC = null;
            }

            halamanAktif = 0;
        }

        if (judulNPCText != null)
        {
            judulNPCText.text = halamanNPC[halamanAktif];
        }

        if (pageNumberText != null)
        {
            pageNumberText.text = (halamanAktif + 1) + " / " + halamanNPC.Length;
        }

        if (npcImage != null)
        {
            if (gambarNPC != null && halamanAktif < gambarNPC.Length && gambarNPC[halamanAktif] != null)
            {
                npcImage.sprite = gambarNPC[halamanAktif];
                npcImage.gameObject.SetActive(true);
            }
            else
            {
                npcImage.sprite = null;
                npcImage.gameObject.SetActive(false);
            }
        }

        bool halamanPertama = halamanAktif == 0;
        bool halamanTerakhir = halamanAktif >= halamanNPC.Length - 1;

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
        if (halamanNPC == null || halamanNPC.Length == 0) return;

        if (halamanAktif < halamanNPC.Length - 1)
        {
            halamanAktif++;
            TampilkanHalaman();
        }
    }

    public void HalamanSebelumnya()
    {
        if (halamanNPC == null || halamanNPC.Length == 0) return;

        if (halamanAktif > 0)
        {
            halamanAktif--;
            TampilkanHalaman();
        }
    }

    public void TutupNPCWindow()
    {
        if (npcWindow != null)
        {
            npcWindow.SetActive(false);
        }

        if (rightHandLaser != null)
        {
            rightHandLaser.SetActive(false);
        }
    }

    [ContextMenu("Test Update NPC Halaman")]
    public void TestUpdate()
    {
        string[] testHalaman = new string[]
        {
            "Halaman 1: Kamu bertemu dengan NPC.",
            "Halaman 2: NPC memberikan petunjuk perjalanan.",
            "Halaman 3: Lanjutkan perjalananmu."
        };

        UpdateNPCHalaman(testHalaman, gambarDefaultNPC);
    }
}