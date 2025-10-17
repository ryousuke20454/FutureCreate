using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("�v���C���[�ݒ�")]
    public Transform player1;
    public Transform player2;

    [Header("�J��������")]
    public float smoothSpeed = 5f;       // �ړ��̃X���[�Y���i�傫���قǑ����j
    public float minZoom = 5f;           // �ߋ����̃Y�[��
    public float maxZoom = 15f;          // �������̃Y�[��
    public float zoomLimiter = 10f;      // �Y�[���̔����x

    private Camera cam;
    private Vector3 targetPosition;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera()
    {
        // 2�l�̒��ԓ_���v�Z
        Vector3 centerPoint = (player1.position + player2.position) / 2f;

        // Z���W�͌Œ�
        targetPosition = new Vector3(centerPoint.x, centerPoint.y, transform.position.z);

        // �X���[�Y�ɒǏ]�iLerp�ŕ�ԁj
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }

    void ZoomCamera()
    {
        // �v���C���[�Ԃ̋���
        float distance = Vector2.Distance(player1.position, player2.position);

        // �����ɉ����ăY�[������
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);

        // �X���[�Y�ȃY�[��
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * smoothSpeed);
    }
}
