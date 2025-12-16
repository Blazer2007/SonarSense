using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public float stepVolume = 1f;

    public void PlayFootstep()
    {
        Vector3 pos = transform.position;

        Collider[] hits = Physics.OverlapSphere(pos, stepVolume * 5f);
        foreach (var hit in hits)
        {
            var ai = hit.GetComponent<EnemyEcholocationAI>();
            if (ai != null)
                ai.HearFootstep(pos, transform);
        }
    }
}
