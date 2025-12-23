using System.Collections;
using UnityEngine;

public class ObjectSounds : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source; // fonte de áudio principal
    public AudioClip collisionClip; // som de colisão
    public AudioClip pickupClip;
    public AudioClip throwClip;

    void Awake()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) 
        {
            AudioSource.PlayClipAtPoint(collisionClip, transform.position);
        }
    }
    public void PlayPickupSound()
    {
        AudioSource.PlayClipAtPoint(pickupClip, transform.position);
    }
    public void PlayThrowSound() 
    {
        AudioSource.PlayClipAtPoint(throwClip, transform.position);
    }
}
