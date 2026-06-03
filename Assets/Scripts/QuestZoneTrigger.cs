using UnityEngine;

public class QuestZoneTrigger : MonoBehaviour
{
    [Header("Quest Manager")]
    public QuestManager questManager;

    [Header("Isi Quest Per Halaman")]
    [TextArea(3, 10)]
    public string[] halamanQuest;

    [Header("Gambar Quest Per Halaman")]
    public Sprite[] gambarQuest;

    [Header("Visual Barrier")]
    public GameObject cubeVisualBarrier;
    public bool matikanCubeVisualDiAwal = true;
    public bool aktifkanCubeVisualSaatTrigger = true;

    [Header("Barrier Yang Dihilangkan")]
    public GameObject barrierParentUntukDihilangkan;
    public bool hilangkanBarrierSaatTrigger = false;

    [Header("Pengaturan Trigger")]
    public bool hancurkanSetelahTrigger = true;

    private bool sudahTrigger = false;

    private void Start()
    {
        if (cubeVisualBarrier != null && matikanCubeVisualDiAwal)
        {
            cubeVisualBarrier.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sudahTrigger) return;

        if (other.CompareTag("Player"))
        {
            sudahTrigger = true;

            if (questManager != null)
            {
                questManager.UpdateQuestHalaman(halamanQuest, gambarQuest);
            }

            if (cubeVisualBarrier != null && aktifkanCubeVisualSaatTrigger)
            {
                cubeVisualBarrier.SetActive(true);
            }

            if (barrierParentUntukDihilangkan != null && hilangkanBarrierSaatTrigger)
            {
                barrierParentUntukDihilangkan.SetActive(false);
            }

            Debug.Log("Quest muncul via Trigger!");

            if (hancurkanSetelahTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}