using UnityEngine;

public class BillboardText : MonoBehaviour
{
    public Camera cam;
    public float heightOffset = 0.8f; // altura fixa acima do objeto
    public Transform target;

    void LateUpdate()
    {
        if (cam == null || target == null) return;

        // 1) Posicao sempre acima do objeto na MESMA altura
        Vector3 pos = target.position;
        pos.y += heightOffset;
        transform.position = pos;

        // 2) Rotacao: olha para a camara, mas so no plano horizontal
        Vector3 dir = transform.position - cam.transform.position;
        dir.y = 0f; // impede inclinacao para cima/baixo

        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = lookRot;
    }
}
