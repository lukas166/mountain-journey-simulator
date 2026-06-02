using UnityEngine;

public class NPCZoneTrigger : MonoBehaviour
{
    [Header("NPC Window Manager")]
    public NPCWindowManager npcWindowManager;

    [Header("Isi NPC Per Halaman")]
    [TextArea(3, 10)]
    public string[] halamanNPC;

    [Header("Gambar NPC Per Halaman")]
    public Sprite[] gambarNPC;

    [Header("Pengaturan Trigger")]
    public bool hancurkanSetelahTrigger = true;

    private bool sudahTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (sudahTrigger) return;

        if (other.CompareTag("Player"))
        {
            sudahTrigger = true;

            if (npcWindowManager != null)
            {
                npcWindowManager.UpdateNPCHalaman(halamanNPC, gambarNPC);
            }

            Debug.Log("NPC Window muncul via Trigger!");

            if (hancurkanSetelahTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}