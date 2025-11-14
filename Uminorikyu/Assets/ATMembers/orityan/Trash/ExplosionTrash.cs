using KanKikuchi.AudioManager;
using UnityEngine;

public class ExplosionTrash : MonoBehaviour
{
    [SerializeField] GameObject particle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerAndStaminaInfo>() != null)
        {
            collision.gameObject.GetComponent<PlayerAndStaminaInfo>().GetExplosionTrash();
            SEManager.Instance.Play(SEPath.EXPLOSION);
            Instantiate(particle, collision.gameObject.transform.position, Quaternion.identity);
        }
    }
}
