using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketDoorBridgeFinal : MonoBehaviour {
    
    [Header("Identification")]
    public string acceptsTag; // Ej: "Key_Bronze"
    public string lockName; // Ej: "Bronze" (opcional para LockManager)

    [Header("References")]
    public Collider blockingCollider; // Assign BlockingPlate's Collider
    public XRSocketInteractor socket; // Assign the XR Socket Interactor (same GO or child)
    public Animator doorAnimator; // optional
    public AudioSource audioSource; // optional
    public AudioClip openClip;
    public AudioClip wrongClip;
    public LockManager lockManager; // arrastrar en inspector

    private bool isUnlocked = false;

    void Start() {

        Debug.Log($"[SocketDoorBridge] Start. socket={(socket != null)}, blockingCollider={(blockingCollider != null)}");
        if (blockingCollider != null)
            Debug.Log($"[SocketDoorBridge] Blocking collider isTrigger={blockingCollider.isTrigger}, enabled={blockingCollider.enabled}, name={blockingCollider.name}");
    }
    void OnEnable() {
        if (socket == null) {
            Debug.LogError("[SocketDoorBridge] socket is null — assign the XRSocketInteractor in the inspector.");
            return;
        }
        socket.selectEntered.AddListener(OnSelectEntered);
        socket.selectExited.AddListener(OnSelectExited);
    }
    void OnDisable() {
        if (socket != null) {
            socket.selectEntered.RemoveListener(OnSelectEntered);
            socket.selectExited.RemoveListener(OnSelectExited);
        }
    }
    void OnSelectEntered(SelectEnterEventArgs args) {
        Debug.Log($"[SocketDoorBridge] OnSelectEntered called. interactable={(args.interactableObject != null ? args.interactableObject.transform.name : "null")}");
        if (args.interactableObject == null) 
            return;
        GameObject keyGO = args.interactableObject.transform.gameObject;
        Debug.Log($"[SocketDoorBridge] Interactable GameObject: {keyGO.name}, tag={keyGO.tag}");

        if (keyGO.CompareTag(acceptsTag)) {
            Debug.Log("[SocketDoorBridge] Correct key detected. Unlocking...");
            Unlock(keyGO);
        }
        else {
            Debug.LogWarning("[SocketDoorBridge] Wrong key entered.");
            WrongKey(args);
        }
    }
    void OnSelectExited(SelectExitEventArgs args) {
        if (args.interactableObject == null) return;
        GameObject keyGO = args.interactableObject.transform.gameObject;
        if (keyGO.CompareTag(acceptsTag)) {
            var rb = keyGO.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = false;
            }
            if (keyGO.transform.parent == socket.attachTransform)
                keyGO.transform.SetParent(null, worldPositionStays: true);
        }
    }
    void Unlock(GameObject key) {
        if (isUnlocked) return;
        isUnlocked = true;

        if (blockingCollider != null) {
            blockingCollider.enabled = false;
            Debug.Log($"[SocketDoorBridge] blockingCollider.enabled set to false for {blockingCollider.name}");
        }
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (audioSource != null && openClip != null)
            audioSource.PlayOneShot(openClip);
        // Notificar al LockManager
        if (lockManager != null) {
            string nameToSend = string.IsNullOrEmpty(lockName) ? acceptsTag : lockName;
            lockManager.Unlock(nameToSend);
        }
        // Mantener la llave en el socket: parentearla al attach point y hacerla kinematic
        Transform attach = socket.attachTransform;
        if (attach != null && key != null) {
            var rb = key.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            key.transform.SetParent(attach, worldPositionStays: false);
            key.transform.localPosition = Vector3.zero;
            key.transform.localRotation = Quaternion.identity;
        }
    }
    void WrongKey(SelectEnterEventArgs args) {
        if (audioSource != null && wrongClip != null)
            audioSource.PlayOneShot(wrongClip);

        var interactorObject = args.interactorObject;
        if (interactorObject is XRBaseInputInteractor baseInter)
            baseInter.SendHapticImpulse(0.2f, 0.1f);

        var selected = args.interactableObject;
        if (selected != null && socket != null && socket.interactionManager != null) {
            socket.interactionManager.SelectExit(socket, selected);
            var socketCollider = socket.GetComponent<Collider>();
            if (socketCollider != null) {
                socketCollider.enabled = false;
                StartCoroutine(ReenableCollider(socketCollider, 0.5f));
            }
        }
    }
    System.Collections.IEnumerator ReenableCollider(Collider c, float delay) {
        yield return new WaitForSeconds(delay);
        if (c != null) c.enabled = true;
    }
}
/*
public class SocketDoorBridgeDiagnostic : MonoBehaviour {

    [Header("Identification")]
    public string acceptsTag; // Ej: "Key_Bronze"

    [Header("References")]
    public Collider blockingCollider; // Assign BlockingPlate's Collider
    public XRSocketInteractor socket; // Assign the XR Socket Interactor (same GO or child)
    public Animator doorAnimator; // optional
    public AudioSource audioSource; // optional
    public AudioClip openClip;
    public AudioClip wrongClip;
    public LockManager lockManager; // arrastrar en inspector
    private bool isUnlocked = false;

    void Start() {
        // diagnstico inicial
        Debug.Log($"[SocketDoorBridge] Start. socket={(socket != null)}, blockingCollider={(blockingCollider != null)}");
        if (blockingCollider != null)
            Debug.Log($"[SocketDoorBridge] Blocking collider isTrigger={blockingCollider.isTrigger}, enabled={blockingCollider.enabled}, name={blockingCollider.name}");
    }
    void OnEnable() {
        if (socket == null) { 
            Debug.LogError("[SocketDoorBridge] socket is null � assign the XRSocketInteractor in the inspector."); 
            return; 
        }
        socket.selectEntered.AddListener(OnSelectEntered);
    }
    void OnDisable() {
        if (socket != null) 
            socket.selectEntered.RemoveListener(OnSelectEntered);
    }
    void OnSelectEntered(SelectEnterEventArgs args) {
        Debug.Log($"[SocketDoorBridge] OnSelectEntered called. interactable={(args.interactableObject != null ? args.interactableObject.transform.name : "null")}");
        if (args.interactableObject == null) 
            return;
        // obtener el GameObject que corresponde a la llave real
        GameObject keyGO = args.interactableObject.transform.gameObject;
        Debug.Log($"[SocketDoorBridge] Interactable GameObject: {keyGO.name}, tag={keyGO.tag}");
        // comparar tag
        if (keyGO.CompareTag(acceptsTag)) {
            Debug.Log("[SocketDoorBridge] Correct key detected. Unlocking...");
            Unlock(keyGO);
        }
        else {
            Debug.LogWarning("[SocketDoorBridge] Wrong key entered.");
            WrongKey(args);
        }
    }
    void Unlock(GameObject key) {
        if (isUnlocked) return;
            isUnlocked = true;
        if (blockingCollider != null) {
            blockingCollider.enabled = false;
            Debug.Log($"[SocketDoorBridge] blockingCollider.enabled set to false for {blockingCollider.name}");
        }
        if (doorAnimator != null) 
            doorAnimator.SetTrigger("Open");

        if (audioSource != null && openClip != null) 
            audioSource.PlayOneShot(openClip);

        if (lockManager != null) 
            lockManager.Unlock(acceptsTag);
        // Mantener la llave en el socket: parentearla al attach point y hacerla kinematic
        Transform attach = socket.attachTransform; // XR Socket Interactor expone attachTransform
        if (attach != null && key != null) {
            // fijar física
            var rb = key.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            // parentear y alinear
            key.transform.SetParent(attach, worldPositionStays: false);
            key.transform.localPosition = Vector3.zero;
            key.transform.localRotation = Quaternion.identity;
        }
        // NO desactivar el socket; si lo haces, la llave podría soltarse
        // if (socket != null) socket.enabled = false;
    }
    void WrongKey(SelectEnterEventArgs args) {
        if (audioSource != null && wrongClip != null) audioSource.PlayOneShot(wrongClip);

        var interactorObject = args.interactorObject;
        if (interactorObject is XRBaseInputInteractor baseInter)
            baseInter.SendHapticImpulse(0.2f, 0.1f);

        // Fuerza la eyecci�n si qued� seleccionado
        var selected = args.interactableObject;
        if (selected != null && socket != null && socket.interactionManager != null) {
            socket.interactionManager.SelectExit(socket, selected);
            // desactivar collider del socket un instante para evitar re-socket inmediato
            var socketCollider = socket.GetComponent<Collider>();
            if (socketCollider != null) {
                socketCollider.enabled = false;
                StartCoroutine(ReenableCollider(socketCollider, 0.5f));
            }
        }
    }
    System.Collections.IEnumerator ReenableCollider(Collider c, float delay) {
        yield return new WaitForSeconds(delay);
        if (c != null) c.enabled = true;
    }
}*/