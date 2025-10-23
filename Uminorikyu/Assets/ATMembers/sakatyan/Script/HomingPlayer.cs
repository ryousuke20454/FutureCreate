using UnityEngine;

public class HomingPlayer : MonoBehaviour
{
    Transform playerTr;
    [SerializeField] float speed = 2f;

    [Header("�T�C�Y�ݒ�")]
    [SerializeField] float growAmount = 0.05f; // �g���
    [SerializeField] float maxScale = 1f;     // �ő�X�P�[��
    private Vector3 baseScale;                 // �����X�P�[����ێ�

    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        // �C���X�y�N�^�[�Őݒ肵�������X�P�[����ۑ�
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, playerTr.position) < 0.1f)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTr.position,
            speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash"))
        {
            Destroy(collision.gameObject);

            // ���݃X�P�[������Ɋg��i�����l����̑��Ίg��j
            float newScale = transform.localScale.x + growAmount;
            newScale = Mathf.Min(newScale, maxScale); // �ő�l����
            transform.localScale = new Vector3(newScale, newScale, 1f);
        }
    }
}
