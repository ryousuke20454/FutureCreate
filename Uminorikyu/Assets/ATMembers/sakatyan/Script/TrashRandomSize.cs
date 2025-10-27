using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TrashRandomSize : MonoBehaviour
{
    [Header("�����_���X�P�[���͈�")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;

    void Start()
    {
        // �����_���ȃX�P�[��������
        float randomScale = Random.Range(minScale, maxScale);

        // X, Y�i�܂���Z���܂߂�j�𓯂��䗦�Ŋg��k��
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }
}
