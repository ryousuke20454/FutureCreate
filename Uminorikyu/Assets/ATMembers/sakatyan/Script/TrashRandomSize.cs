using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TrashRandomSize : MonoBehaviour
{
    [Header("ランダムスケール範囲")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;

    void Start()
    {
        // ランダムなスケールを決定
        float randomScale = Random.Range(minScale, maxScale);

        // X, Y（またはZも含める）を同じ比率で拡大縮小
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }
}
