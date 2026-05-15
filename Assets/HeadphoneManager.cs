using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HeadphoneManager : MonoBehaviour
{
    [Header("Visual & Audio")]
    public MeshRenderer headphoneRenderer;
    public AudioSource musicSource;    
    public AudioSource sfxSource;      
    public AudioClip clickSound;       

    [Header("Input Controller (Mute)")]
    public InputActionReference rightTapAction; 
    public InputActionReference leftTapAction;

    [Header("Input Controller (Lepas / Ambil Headphone)")]
    public InputActionReference rightGripAction;
    public InputActionReference leftGripAction;
    
    [Header("Tracking Posisi")]
    public Transform headTransform;        
    public Transform leftHandTransform;    
    public Transform rightHandTransform;   
    public float jarakKetuk = 0.35f;

    [Header("Physics Headphone")]
    public Rigidbody headphoneRigidbody;
    public Collider[] headphoneColliders;

    [Header("Socket Kepala")]
    public XRSocketInteractor headSocket;

    [Header("Anti Bug Cepat Lepas Pasang")]
    public float waktuBatalAmbil = 1.5f;

    private XRGrabInteractable grabInteractable;
    private bool isWearing = false;
    private bool isReadyToTake = false;
    private bool sedangDipegangTangan = false;
    private Coroutine batalAmbilCoroutine;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (headphoneRigidbody == null)
        {
            headphoneRigidbody = GetComponent<Rigidbody>();
        }

        if (headphoneColliders == null || headphoneColliders.Length == 0)
        {
            headphoneColliders = GetComponentsInChildren<Collider>();
        }
    }

    private void OnEnable()
    {
        if (rightTapAction != null) rightTapAction.action.performed += KetukKanan;
        if (leftTapAction != null) leftTapAction.action.performed += KetukKiri;

        if (rightGripAction != null) rightGripAction.action.performed += AmbilKanan;
        if (leftGripAction != null) leftGripAction.action.performed += AmbilKiri;

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnHeadphoneSelected);
            grabInteractable.selectExited.AddListener(OnHeadphoneReleased);
        }
    }

    private void OnDisable()
    {
        if (rightTapAction != null) rightTapAction.action.performed -= KetukKanan;
        if (leftTapAction != null) leftTapAction.action.performed -= KetukKiri;

        if (rightGripAction != null) rightGripAction.action.performed -= AmbilKanan;
        if (leftGripAction != null) leftGripAction.action.performed -= AmbilKiri;

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnHeadphoneSelected);
            grabInteractable.selectExited.RemoveListener(OnHeadphoneReleased);
        }
    }

    private void OnHeadphoneSelected(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor || args.interactorObject is XRRayInteractor)
        {
            sedangDipegangTangan = true;

            if (batalAmbilCoroutine != null)
            {
                StopCoroutine(batalAmbilCoroutine);
                batalAmbilCoroutine = null;
            }

            if (headSocket != null)
            {
                headSocket.socketActive = false;
            }

            isWearing = false;
            isReadyToTake = false;

            if (headphoneRenderer != null)
            {
                headphoneRenderer.enabled = true;
            }

            AturColliderSolid();

            if (headphoneRigidbody != null)
            {
                headphoneRigidbody.isKinematic = false;
                headphoneRigidbody.useGravity = true;
                headphoneRigidbody.linearVelocity = Vector3.zero;
                headphoneRigidbody.angularVelocity = Vector3.zero;
            }

            if (musicSource != null)
            {
                musicSource.Pause();
            }
        }
    }

    private void OnHeadphoneReleased(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor || args.interactorObject is XRRayInteractor)
        {
            sedangDipegangTangan = false;

            if (!isWearing && headSocket != null)
            {
                headSocket.socketActive = true;
            }
        }
    }

    public void ValidasiPakaiHeadphone()
    {
        Pakai();
    }

    private void Pakai()
    {
        isWearing = true;
        isReadyToTake = false;
        sedangDipegangTangan = false;

        if (batalAmbilCoroutine != null)
        {
            StopCoroutine(batalAmbilCoroutine);
            batalAmbilCoroutine = null;
        }

        if (headSocket != null)
        {
            headSocket.socketActive = true;
        }

        if (headphoneRenderer != null)
        {
            headphoneRenderer.enabled = false;
        }

        if (headphoneRigidbody != null)
        {
            headphoneRigidbody.isKinematic = true;
            headphoneRigidbody.useGravity = false;
            headphoneRigidbody.linearVelocity = Vector3.zero;
            headphoneRigidbody.angularVelocity = Vector3.zero;
        }

        AturColliderTrigger();

        if (clickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }

        if (musicSource != null)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }

            musicSource.mute = false;
        }
    }

    public void LepasHeadphone()
    {
        isWearing = false;
        isReadyToTake = false;

        if (batalAmbilCoroutine != null)
        {
            StopCoroutine(batalAmbilCoroutine);
            batalAmbilCoroutine = null;
        }

        PaksaKeluarDariSocket();

        if (headphoneRenderer != null)
        {
            headphoneRenderer.enabled = true;
        }

        AturColliderSolid();

        if (headphoneRigidbody != null)
        {
            headphoneRigidbody.isKinematic = false;
            headphoneRigidbody.useGravity = true;
            headphoneRigidbody.linearVelocity = Vector3.zero;
            headphoneRigidbody.angularVelocity = Vector3.zero;
        }

        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    private void AmbilKanan(InputAction.CallbackContext context)
    {
        if (!isWearing) return;

        float jarak = Vector3.Distance(headTransform.position, rightHandTransform.position);

        if (jarak <= jarakKetuk)
        {
            SiapkanHeadphoneUntukDiambil();
        }
    }

    private void AmbilKiri(InputAction.CallbackContext context)
    {
        if (!isWearing) return;

        float jarak = Vector3.Distance(headTransform.position, leftHandTransform.position);

        if (jarak <= jarakKetuk)
        {
            SiapkanHeadphoneUntukDiambil();
        }
    }

    private void SiapkanHeadphoneUntukDiambil()
    {
        if (isReadyToTake) return;

        isReadyToTake = true;
        isWearing = false;

        PaksaKeluarDariSocket();

        if (headphoneRenderer != null)
        {
            headphoneRenderer.enabled = true;
        }

        AturColliderSolid();

        if (headphoneRigidbody != null)
        {
            headphoneRigidbody.isKinematic = true;
            headphoneRigidbody.useGravity = false;
            headphoneRigidbody.linearVelocity = Vector3.zero;
            headphoneRigidbody.angularVelocity = Vector3.zero;
        }

        if (musicSource != null)
        {
            musicSource.Pause();
        }

        if (clickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }

        if (batalAmbilCoroutine != null)
        {
            StopCoroutine(batalAmbilCoroutine);
        }

        batalAmbilCoroutine = StartCoroutine(BatalAmbilJikaTidakDipegang());
    }

    private IEnumerator BatalAmbilJikaTidakDipegang()
    {
        yield return new WaitForSeconds(waktuBatalAmbil);

        if (isReadyToTake && !sedangDipegangTangan)
        {
            Pakai();
        }

        batalAmbilCoroutine = null;
    }

    private void PaksaKeluarDariSocket()
    {
        if (headSocket == null)
        {
            return;
        }

        headSocket.socketActive = false;

        if (grabInteractable != null && headSocket.interactionManager != null)
        {
            headSocket.interactionManager.SelectExit(
                (IXRSelectInteractor)headSocket,
                (IXRSelectInteractable)grabInteractable
            );
        }
    }

    private void AturColliderTrigger()
    {
        if (headphoneColliders == null)
        {
            return;
        }

        foreach (Collider col in headphoneColliders)
        {
            if (col != null)
            {
                col.enabled = true;
                col.isTrigger = true;
            }
        }
    }

    private void AturColliderSolid()
    {
        if (headphoneColliders == null)
        {
            return;
        }

        foreach (Collider col in headphoneColliders)
        {
            if (col != null)
            {
                col.enabled = true;
                col.isTrigger = false;
            }
        }
    }

    private void KetukKanan(InputAction.CallbackContext context)
    {
        if (!isWearing) return;

        float jarak = Vector3.Distance(headTransform.position, rightHandTransform.position);

        if (jarak <= jarakKetuk)
        {
            EksekusiMute();
        }
    }

    private void KetukKiri(InputAction.CallbackContext context)
    {
        if (!isWearing) return;

        float jarak = Vector3.Distance(headTransform.position, leftHandTransform.position);

        if (jarak <= jarakKetuk)
        {
            EksekusiMute();
        }
    }

    private void EksekusiMute()
    {
        if (clickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }

        if (musicSource != null)
        {
            musicSource.mute = !musicSource.mute;
        }
    }
}