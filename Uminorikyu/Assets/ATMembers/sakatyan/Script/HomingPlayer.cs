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
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float speed = 2f;

    // =====================================================
    // �T�C�Y�ݒ�
    // =====================================================
    [Header("�T�C�Y�ݒ�")]
    [SerializeField] private float growAmount = 0.05f;
    [SerializeField] private float maxScale = 1f;

    // ���ύX�_�F�������x
    [SerializeField] private float growSpeed = 3f; // �X���[�Y�Ɋg�傷�鑬��

    // =====================================================
    // ��]�ݒ�
    // =====================================================
    [Header("��]�ݒ�")]
    [SerializeField] private float rotationSpeed = 180f;

    // =====================================================
    // ������ѐݒ�
    // =====================================================
    [Header("������ѐݒ�")]
    [SerializeField] private float bouncePower = 5f;
    [SerializeField] private float maxBounceDistance = 2f;
    [SerializeField] private float stopDuration = 1f;

    // =====================================================
    // �����ϐ�
    // =====================================================
    private Rigidbody2D rb;
    private Rigidbody2D targetRb;
    private MonoBehaviour targetController;

    private Vector3 baseScale;
    private bool isKnockback = false;
    private Vector3 knockbackStartPos;

    private CameraController camera;

    // ���ύX�_�FLerp�p�̃^�[�Q�b�g�X�P�[��
    private Vector3 targetScale;

    // =====================================================
    // ������
    // =====================================================
    private void Start()
    {
        baseScale = transform.localScale;
        targetScale = baseScale; // ���������i�����X�P�[���Ɠ����Ɂj
        rb = GetComponent<Rigidbody2D>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

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
        // ��ɉ�]
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // �X�P�[�������炩�ɕ��
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * growSpeed
        );

        // ������ђ��͒Ǐ]���Ȃ�
        if (isKnockback) return;

        // �Ǐ]����
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
        if (isKnockback) return;

        // ------------------------
        // �S�~�Ƃ̏Փ�
        // ------------------------
        if (collision.CompareTag("Trash"))
        {
            float vortexScale = transform.localScale.x;
            float trashScale = collision.transform.localScale.x;

            if (vortexScale >= trashScale)
            {
                Destroy(collision.gameObject);

                //�����g�傹���A�^�[�Q�b�g�X�P�[�����X�V
                float newScale = Mathf.Min(targetScale.x + growAmount, maxScale);
                targetScale = new Vector3(newScale, newScale, 1f);
            }
            return;
        }

        // ------------------------
        // ���̉Q�Ƃ̏Փ�
        // ------------------------
        Vortex otherVortex = collision.GetComponent<Vortex>();
        if (otherVortex == null || otherVortex == this) return;

        float myScale = transform.localScale.x;
        float otherScale = otherVortex.transform.localScale.x;

        Rigidbody2D otherRb = otherVortex.rb;
        Rigidbody2D otherTargetRb = otherVortex.targetRb;

        if (myScale > otherScale)
        {
            camera.ShakeCamera();
            Vector2 dir = (otherVortex.transform.position - transform.position).normalized;

            otherRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherTargetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);

            otherVortex.StartKnockback();
            if (otherVortex.targetController != null)
                otherVortex.targetController.enabled = false;
        }
        else if (myScale < otherScale)
        {
            camera.ShakeCamera();
            Vector2 dir = (transform.position - otherVortex.transform.position).normalized;

            rb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            targetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);

            StartKnockback();
            if (targetController != null)
                targetController.enabled = false;
        }
        else
        {
            camera.ShakeCamera();
            Vector2 dir = (otherVortex.transform.position - transform.position).normalized;

            rb?.AddForce(-dir * bouncePower, ForceMode2D.Impulse);
            targetRb?.AddForce(-dir * bouncePower, ForceMode2D.Impulse);
            StartKnockback();
            if (targetController != null)
                targetController.enabled = false;

            otherRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherTargetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherVortex.StartKnockback();
            if (otherVortex.targetController != null)
                otherVortex.targetController.enabled = false;
        }
    }

    // =====================================================
    // ������ъJ�n
    // =====================================================
    public void StartKnockback()
    {
        if (isKnockback) return;
        isKnockback = true;
        knockbackStartPos = transform.position;
        StartCoroutine(StopKnockbackAfterDelay());
    }

    // =====================================================
    // ������щ���
    // =====================================================
    private IEnumerator StopKnockbackAfterDelay()
    {
        yield return new WaitForSeconds(stopDuration);

        rb.Sleep();
        rb.angularVelocity = 0f;
        if (targetRb != null) targetRb.Sleep();

        if (targetController != null)
            targetController.enabled = true;

        isKnockback = false;
    }
}
