using UnityEngine;

public class MoveRightTest : MonoBehaviour
{
    [SerializeField] private float speed = 2f; // �ړ����x

    private void Update()
    {
        // �E�����Ɉړ��iX���W�𑝉�������j
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
