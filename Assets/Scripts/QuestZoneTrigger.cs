using UnityEngine;

public class QuestZoneTrigger : MonoBehaviour
{
    [Header("Quest Manager")]
    public QuestManager questManager;

    [Header("Isi Quest Per Halaman")]
    [TextArea(3, 10)]
    public string[] halamanQuest;

    [Header("Pengaturan Trigger")]
    public bool hancurkanSetelahTrigger = true;

    private bool sudahTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (sudahTrigger) return;

        if (other.CompareTag("Player"))
        {
            sudahTrigger = true;

            if (questManager != null)
            {
                questManager.UpdateQuestHalaman(halamanQuest);
            }

            Debug.Log("Quest muncul via Trigger!");

            if (hancurkanSetelahTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}