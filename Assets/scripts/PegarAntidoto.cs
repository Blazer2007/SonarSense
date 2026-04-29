using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PegarAntidoto : MonoBehaviour
{
    
    [SerializeField] private VideoPlayer plottwistvideo;
    [SerializeField] private AudioSource plottwistsound;
    [SerializeField] private Camera cam;
    [SerializeField] private float detectDistance = 3f;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private GameObject pickupHint;
    private Transform currentTarget;
    [SerializeField] private float delayBeforeScene = 3f;
    //ATUALIZACAO FUTURA: RAIO DE PEGA MAIOR
    Rigidbody heldRb;

    void Update()
    {
        DetectPickup(); // raycast so para ver se ha algo a frente e mostrar a dica

        // Apanhar / largar com tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            //plottwistvideo.Play();
            //plottwistsound.Play();

            SceneManager.LoadScene("Credits");
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Ativar o vídeo e o som do plot twist
            plottwistvideo.Play();
            

            // Destruir o objeto do antídoto após ser apanhado
            Destroy(gameObject);
        }
    }
    void WaitForSeconds(float seconds = 3f)
    {
        float endTime = Time.time + seconds;
        while (Time.time < endTime)
        {
            // Aguarda até o tempo terminar
        }
    }
}
