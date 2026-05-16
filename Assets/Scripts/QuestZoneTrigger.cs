using UnityEngine;

public class QuestZoneTrigger : MonoBehaviour
{
    public QuestManager questManager; // Tarik skrip QuestManager ke sini
    public string judulBaru = "Jelajahi Hutan Terlarang"; // Judul yang ingin ditampilkan
    public bool hancurkanSetelahTrigger = true; // Agar trigger hanya sekali jalan

    private bool sudahTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (sudahTrigger) return;

        // Hanya jalan jika yang menabrak adalah Player
        if (other.CompareTag("Player"))
        {
            sudahTrigger = true;

            if (questManager != null)
            {
                questManager.UpdateJudulQuest(judulBaru);
            }

            Debug.Log("Quest Updated via Trigger!");

            // Jika hanya ingin sekali seumur hidup quest ini muncul
            if (hancurkanSetelahTrigger)
            {
                Destroy(gameObject); 
            }
        }
    }
}