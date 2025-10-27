using UnityEngine;
using System.Collections;

/// <summary>
/// �v���C���[��Ǐ]���A
/// �Փˎ��ɐ������ �� ��苗�� or ��莞�Ԍ�ɒ�~ �� �Ǐ]�ĊJ����Q�X�N���v�g
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Vortex : MonoBehaviour
{
    // =====================================================
    // �Ǐ]�ݒ�
    // =====================================================
    [Header("�Ǐ]�ݒ�")]
    [SerializeField] private Transform targetToFollow; // �Ǐ]����Ώہi�v���C���[�Ȃǁj
    [SerializeField] private float speed = 2f;         // �ʏ�̒Ǐ]���x

    // =====================================================
    // �T�C�Y�ݒ�
    // =====================================================
    [Header("�T�C�Y�ݒ�")]
    [SerializeField] private float growAmount = 0.05f; // �S�~�z�����̊g�嗦
    [SerializeField] private float maxScale = 1f;      // �ő�T�C�Y����

    // =====================================================
    // ��]�ݒ�
    // =====================================================
    [Header("��]�ݒ�")]
    [SerializeField] private float rotationSpeed = 180f; // ��]���x�i�x/�b�j

    // =====================================================
    // ������ѐݒ�
    // =====================================================
    [Header("������ѐݒ�")]
    [SerializeField] private float bouncePower = 5f;        // ������т̗�
    [SerializeField] private float maxBounceDistance = 2f;  // ������ы������
    [SerializeField] private float stopDuration = 1f;       // ������ь�ɒ�~���鎞�ԁi�b�j

    // =====================================================
    // �����ϐ�
    // =====================================================
    private Rigidbody2D rb;             // ������ Rigidbody2D
    private Rigidbody2D targetRb;       // �^�[�Q�b�g�� Rigidbody2D
    private MonoBehaviour targetController; // �v���C���[����X�N���v�g

    private Vector3 baseScale;          // �����X�P�[��
    private bool isKnockback = false;   // ������ђ����H
    private Vector3 knockbackStartPos;  // ������ъJ�n�ʒu

    // =====================================================
    // ����������
    // =====================================================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (targetToFollow != null)
        {
            targetRb = targetToFollow.GetComponent<Rigidbody2D>();
            targetController = targetToFollow.GetComponent<MonoBehaviour>();
        }
    }

    private void Start()
    {
        baseScale = transform.localScale;

        // �^�[�Q�b�g���ݒ�Ȃ玩���� Player �^�O����
        if (targetToFollow == null)
            targetToFollow = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (targetToFollow != null)
        {
            targetRb = targetToFollow.GetComponent<Rigidbody2D>();
            targetController = targetToFollow.GetComponent<MonoBehaviour>();
        }
    }

    // =====================================================
    // ���t���[������
    // =====================================================
    private void Update()
    {
        // ��ɉ�]�i�����ڗp�j
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // ������ђ��͒Ǐ]���Ȃ�
        if (isKnockback) return;

        // �^�[�Q�b�g�����݂���Ȃ�Ǐ]
        if (targetToFollow != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetToFollow.position,
                speed * Time.deltaTime
            );
        }
    }

    // =====================================================
    // �Փˏ���
    // =====================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ������ђ��͏Փˏ�������
        if (isKnockback) return;

        // ------------------------
        // �S�~�Ƃ̏Փˏ���
        // ------------------------
        if (collision.CompareTag("Trash"))
        {
            float vortexScale = transform.localScale.x;
            float trashScale = collision.transform.localScale.x;

            if (vortexScale >= trashScale)
            {
                Destroy(collision.gameObject);

                // ���������i�������j
                float newScale = Mathf.Min(transform.localScale.x + growAmount, maxScale);
                transform.localScale = new Vector3(newScale, newScale, 1f);
            }
            return;
        }

        // ------------------------
        // ���̉Q�Ƃ̏Փˏ���
        // ------------------------
        Vortex otherVortex = collision.GetComponent<Vortex>();
        if (otherVortex == null || otherVortex == this) return;

        float myScale = transform.localScale.x;
        float otherScale = otherVortex.transform.localScale.x;

        Rigidbody2D otherRb = otherVortex.rb;
        Rigidbody2D otherTargetRb = otherVortex.targetRb;

        // �召��r�ŏ�������
        if (myScale > otherScale)
        {
            // �������傫�� �� ����𐁂���΂�
            Vector2 dir = (otherVortex.transform.position - transform.position).normalized;

            otherRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherTargetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);

            otherVortex.StartKnockback();
            if (otherVortex.targetController != null)
                otherVortex.targetController.enabled = false;
        }
        else if (myScale < otherScale)
        {
            // ������������ �� �������������
            Vector2 dir = (transform.position - otherVortex.transform.position).normalized;

            rb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            targetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);

            StartKnockback();
            if (targetController != null)
                targetController.enabled = false;
        }
        else
        {
            // ���T�C�Y �� ���Ґ������
            Vector2 dir = (otherVortex.transform.position - transform.position).normalized;

            // ����
            rb?.AddForce(-dir * bouncePower, ForceMode2D.Impulse);
            targetRb?.AddForce(-dir * bouncePower, ForceMode2D.Impulse);
            StartKnockback();
            if (targetController != null)
                targetController.enabled = false;

            // ����
            otherRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherTargetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherVortex.StartKnockback();
            if (otherVortex.targetController != null)
                otherVortex.targetController.enabled = false;
        }
    }

    // =====================================================
    // ������ъJ�n����
    // =====================================================
    public void StartKnockback()
    {
        if (isKnockback) return;

        isKnockback = true;
        knockbackStartPos = transform.position;

        // ��莞�Ԍ�ɐ�����щ����{�Ǐ]�ĊJ
        StartCoroutine(StopKnockbackAfterDelay());
    }

    // =====================================================
    // ������яI������
    // =====================================================
    private IEnumerator StopKnockbackAfterDelay()
    {
        // ��莞�Ԓ�~
        yield return new WaitForSeconds(stopDuration);

        rb.Sleep();
        rb.angularVelocity = 0f;

        if (targetRb != null) targetRb.Sleep();

        // �v���C���[����ĊJ
        if (targetController != null)
            targetController.enabled = true;

        // ��ԃ��Z�b�g
        isKnockback = false;
    }
}
