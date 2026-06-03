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

    private bool dataSudahDikirim = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (npcWindowManager != null)
            {
                if (!dataSudahDikirim)
                {
                    npcWindowManager.SetNPCHalaman(halamanNPC, gambarNPC);
                    dataSudahDikirim = true;
                }

                npcWindowManager.MasukAreaNPC();
            }

            Debug.Log("Player masuk area NPC.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (npcWindowManager != null)
            {
                npcWindowManager.KeluarAreaNPC();
            }

            Debug.Log("Player keluar area NPC.");
        }
    }
}