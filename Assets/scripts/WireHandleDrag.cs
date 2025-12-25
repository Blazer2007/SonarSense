using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WireHandleDrag : MonoBehaviour
{
    public WireController wire;     // referência ao script do fio
    public float snapRadius = 0.5f; // distância máxima para encaixar num pino

    private Camera cam;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
        Debug.Log("Iniciou arrastar");
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Procura pino de destino próximo
        Transform closestPin = FindClosestPin();
        if (closestPin != null)
        {
            wire.SnapToPin(closestPin);
        }
        else
        {
            // Volta ao início se não encaixou
            wire.ResetToStart();
        }
    }

    void Update()
    {
        if (!isDragging) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 pos = hitInfo.point;
            transform.position = pos;
        }
        else
        {
            // alternativa simples: plano na frente da câmara
            Vector3 pos = ray.origin + ray.direction * 5f;
            transform.position = pos;
        }
    }

    Transform FindClosestPin()
    {
        GameObject[] pins = GameObject.FindGameObjectsWithTag("EndPin");
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var p in pins)
        {
            float d = Vector3.Distance(transform.position, p.transform.position);
            if (d < closestDist && d <= snapRadius)
            {
                closestDist = d;
                closest = p.transform;
            }
        }
        return closest;
    }
}
 