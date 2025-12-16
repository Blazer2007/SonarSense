using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    public float baseVolume = 1f;

    void OnCollisionEnter(Collision collision)
    {
        float intensity = collision.relativeVelocity.magnitude * baseVolume;
        Vector3 pos = collision.contacts[0].point;

        // Encontra todos os inimigos no raio (Physics.OverlapSphere)
        Collider[] hits = Physics.OverlapSphere(pos, intensity * 5f);
        foreach (var hit in hits)
        {
            var ai = hit.GetComponent<EnemyEcholocationAI>();
            if (ai != null)
                ai.HearCollision(pos, intensity);
        }
    }
}
