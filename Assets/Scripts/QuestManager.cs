using UnityEngine;
using TMPro; // Penting untuk TextMeshPro

public class QuestManager : MonoBehaviour
{
    public TextMeshProUGUI judulQuestText; // Tarik objek JudulQuest ke sini di Inspector

    // Fungsi ini bisa dipanggil saat event tertentu terjadi
    public void UpdateJudulQuest(string judulBaru)
    {
        if (judulQuestText != null)
        {
            judulQuestText.text = judulBaru;
        }
    }

    [ContextMenu("Test Update Teks")]
    public void TestUpdate() 
    {
        UpdateJudulQuest("Quest Berhasil Diubah!");
    }
}