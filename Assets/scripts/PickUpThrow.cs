using UnityEngine;

public class PickUpThrow : MonoBehaviour
{
    public Camera cam;                // Camara do jogador
    public Transform holdPoint;       // Empty a frente da camara
    public float detectDistance = 3f; // Distancia maxima para apanhar
    public float throwForce = 10f;    // Forca do lancamento
    public LayerMask pickupLayer;
    public GameObject pickupHint; // texto/icone de pegar
    public bool canThrow = true;
    Transform currentTarget;
    [SerializeField] private PlayerStamina playerStamina;

    //Raio maior
    Rigidbody heldRb;

    void Update()
    {
        DetectPickup(); // raycast so para ver se ha algo a frente e mostrar a dica

        // Apanhar / largar com tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldRb == null)
                TryPickUpCurrent();
            else
                Drop();
        }

        // Manter o objeto na mao
        if (heldRb != null)
        {
            heldRb.MovePosition(holdPoint.position);
        }

        // Atirar com a tecla T
        if (heldRb != null && Input.GetKeyDown(KeyCode.T) && canThrow)
        {
            Throw();
        }
    }
    void DetectPickup()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectDistance, pickupLayer))
        {
            currentTarget = hit.collider.transform;
            if (!pickupHint.activeSelf)
                pickupHint.SetActive(true);
        }
        else
        {
            currentTarget = null;
            if (pickupHint.activeSelf)
                pickupHint.SetActive(false);
        }
    }
    void TryPickUpCurrent()
    {
        if (currentTarget == null) return;

        Transform root = currentTarget.GetComponent<Rigidbody>()
                         ? currentTarget
                         : currentTarget.GetComponentInParent<Rigidbody>()?.transform;
        if (root == null) return;

        Rigidbody rb = root.GetComponent<Rigidbody>();
        if (rb == null) return;

        heldRb = rb;
        heldRb.useGravity = false;
        heldRb.freezeRotation = true;

        SoundEmitter emitter = root.GetComponent<SoundEmitter>();
        if (emitter != null)
            emitter.OnPickedUp();
    }
    void Drop()
    {
        heldRb.useGravity = true;
        heldRb.freezeRotation = false;
        heldRb = null;
    }

    void Throw()
    {
        if (heldRb == null) return;

        SoundEmitter emitter = heldRb.GetComponent<SoundEmitter>();
        if (emitter != null)
            emitter.OnThrown();

        heldRb.useGravity = true;
        heldRb.freezeRotation = false;
        heldRb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

        heldRb = null;

        playerStamina.SpendStamina(10f); // Reduz a stamina do jogador ao lancar um objeto

    }

}
