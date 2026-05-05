using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// REMOVE this to avoid conflict
// using UnityEngine.UIElements;

public class RadialSelection : MonoBehaviour
{
    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject RadialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;

    public Transform handTransform; // ✅ add this

    public UnityEvent<int> OnPartSelected; 

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = 0; // ✅ fix init

    void Start()
    {
        SpawnRadialPart();
    }

    void Update()
    {
        GetSelectedRadialPart();
    }

    public void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    public void GetSelectedRadialPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, radialPartCanvas.forward);
        angle = Mathf.Repeat(360f - angle, 360f);

        Debug.Log("ANGLE" + angle);
        currentSelectedRadialPart = (int)(angle * numberOfRadialPart / 360); // ✅ fix typo

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = Color.blue;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = Color.white;
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void SpawnRadialPart()
    {
        radialPartCanvas.gameObject.SetActive(true);
        radialPartCanvas.position = handTransform.position;
        radialPartCanvas.rotation = handTransform.rotation;

        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }

        spawnedParts.Clear();

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle =- i * 360 / numberOfRadialPart - angleBetweenPart / 2;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

            GameObject spawnedRadialPart = Instantiate(RadialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

            spawnedRadialPart.GetComponent<Image>().fillAmount =
                (1f / numberOfRadialPart) - (angleBetweenPart / 360f);

            spawnedParts.Add(spawnedRadialPart);
        }
    }
}