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
    public Button openAgainButton;

    [Header("NPC Window dan Panel")]
    public GameObject npcWindow;
    public GameObject npcPanel;

    [Header("Animator NPC")]
    public Animator npcAnimator;
    public string idleAnimationStateName = "Idle";
    public string talkingAnimationStateName = "Talking";
    public float transitionDuration = 0.15f;

    [Header("NPC Default")]
    [TextArea(3, 10)]
    public string[] halamanDefaultNPC;

    [Header("Gambar Default NPC")]
    public Sprite[] gambarDefaultNPC;

    private string[] halamanNPC;
    private Sprite[] gambarNPC;
    private int halamanAktif = 0;

    private bool dataSudahDiisi = false;
    private bool sedangDiArea = false;

    private int halamanTersimpanSaatKeluar = 0;
    private bool panelTerbukaSaatKeluar = true;
    private bool sudahPernahKeluarArea = false;

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
            closeButton.onClick.RemoveListener(TutupNPCPanel);
            closeButton.onClick.AddListener(TutupNPCPanel);
            closeButton.gameObject.SetActive(false);
        }

        if (openAgainButton != null)
        {
            openAgainButton.onClick.RemoveListener(BukaKembaliNPCPanel);
            openAgainButton.onClick.AddListener(BukaKembaliNPCPanel);
            openAgainButton.gameObject.SetActive(false);
        }

        if (npcPanel != null)
        {
            npcPanel.SetActive(false);
        }

        if (npcWindow != null)
        {
            npcWindow.SetActive(false);
        }

        GantiAnimasiNPC(idleAnimationStateName);
    }

    public void SetNPCHalaman(string[] halamanBaru, Sprite[] gambarBaru)
    {
        halamanNPC = halamanBaru;
        gambarNPC = gambarBaru;
        dataSudahDiisi = true;

        if (halamanNPC == null || halamanNPC.Length == 0)
        {
            halamanAktif = 0;
            halamanTersimpanSaatKeluar = 0;
            return;
        }

        if (halamanAktif >= halamanNPC.Length)
        {
            halamanAktif = halamanNPC.Length - 1;
        }

        if (halamanAktif < 0)
        {
            halamanAktif = 0;
        }
    }

    public void MasukAreaNPC()
    {
        sedangDiArea = true;

        if (!dataSudahDiisi)
        {
            halamanNPC = halamanDefaultNPC;
            gambarNPC = gambarDefaultNPC;
            dataSudahDiisi = true;
        }

        if (npcWindow != null)
        {
            npcWindow.SetActive(true);
        }

        if (sudahPernahKeluarArea)
        {
            halamanAktif = halamanTersimpanSaatKeluar;

            if (panelTerbukaSaatKeluar)
            {
                BukaNPCPanelTanpaResetHalaman();
            }
            else
            {
                TampilkanTombolBukaSaja();
            }
        }
        else
        {
            halamanAktif = 0;
            BukaNPCPanelTanpaResetHalaman();
        }
    }

    public void KeluarAreaNPC()
    {
        sedangDiArea = false;

        halamanTersimpanSaatKeluar = halamanAktif;

        if (npcPanel != null && npcPanel.activeSelf)
        {
            panelTerbukaSaatKeluar = true;
        }
        else
        {
            panelTerbukaSaatKeluar = false;
        }

        sudahPernahKeluarArea = true;

        if (npcPanel != null)
        {
            npcPanel.SetActive(false);
        }

        if (openAgainButton != null)
        {
            openAgainButton.gameObject.SetActive(false);
        }

        if (npcWindow != null)
        {
            npcWindow.SetActive(false);
        }

        GantiAnimasiNPC(idleAnimationStateName);
    }

    private void BukaNPCPanelTanpaResetHalaman()
    {
        if (!sedangDiArea) return;

        if (npcWindow != null)
        {
            npcWindow.SetActive(true);
        }

        if (npcPanel != null)
        {
            npcPanel.SetActive(true);
        }

        if (openAgainButton != null)
        {
            openAgainButton.gameObject.SetActive(false);
        }

        GantiAnimasiNPC(talkingAnimationStateName);

        TampilkanHalaman();
    }

    public void BukaKembaliNPCPanel()
    {
        if (!sedangDiArea) return;

        halamanAktif = 0;

        BukaNPCPanelTanpaResetHalaman();
    }

    public void TutupNPCPanel()
    {
        if (!sedangDiArea) return;

        if (npcPanel != null)
        {
            npcPanel.SetActive(false);
        }

        if (openAgainButton != null)
        {
            openAgainButton.gameObject.SetActive(true);
        }

        GantiAnimasiNPC(idleAnimationStateName);
    }

    private void TampilkanTombolBukaSaja()
    {
        if (!sedangDiArea) return;

        if (npcWindow != null)
        {
            npcWindow.SetActive(true);
        }

        if (npcPanel != null)
        {
            npcPanel.SetActive(false);
        }

        if (openAgainButton != null)
        {
            openAgainButton.gameObject.SetActive(true);
        }

        GantiAnimasiNPC(idleAnimationStateName);
    }

    private void TampilkanHalaman()
    {
        if (halamanNPC == null || halamanNPC.Length == 0)
        {
            halamanNPC = new string[]
            {
                "Tidak ada teks NPC."
            };

            gambarNPC = null;
            halamanAktif = 0;
        }

        if (halamanAktif < 0)
        {
            halamanAktif = 0;
        }

        if (halamanAktif >= halamanNPC.Length)
        {
            halamanAktif = halamanNPC.Length - 1;
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

    private void GantiAnimasiNPC(string namaAnimasi)
    {
        if (npcAnimator != null && namaAnimasi != "")
        {
            npcAnimator.CrossFade(namaAnimasi, transitionDuration);
        }
    }
}