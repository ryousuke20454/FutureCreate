using UnityEngine;

public class TextMove : MonoBehaviour
{
    [SerializeField] float firstWaitTime;
    [SerializeField] float secondWaitTime;
    [SerializeField] Vector2 endPosition;
    [SerializeField] bool halfEnd = false;

    Vector2 firstPos;
    Vector2 direction;
    float countTime = 0;
    float moveTime = 0;
    float freezeTime = 0;

    bool freeze = false;
    bool end = false;
    
    RectTransform rect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rect = GetComponent<RectTransform>();
        firstPos = rect.anchoredPosition;
        
        direction = new Vector2(
            endPosition.x - firstPos.x,
            endPosition.y - firstPos.y
            );
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!end)
        {
            countTime += Time.deltaTime;

            if (countTime > firstWaitTime)
            {
                if (moveTime < 3.14f / 2f)
                {
                    moveTime += Time.deltaTime;

                    rect.anchoredPosition = new Vector2(
                        firstPos.x + direction.x * Mathf.Sin(moveTime),
                        firstPos.y + direction.y * Mathf.Sin(moveTime));
                }
                else
                {
                    if (halfEnd)
                    {
                        end = true;
                    }
                    freeze = true;
                }

                if (freeze)
                {
                    freezeTime += Time.deltaTime;
                }

                //–ß‚èØ‚Á‚½‚ç
                if (freezeTime > secondWaitTime && moveTime <= 3.14f)
                {
                    moveTime += Time.deltaTime;
                    rect.anchoredPosition = new Vector2(
                        firstPos.x + direction.x * Mathf.Sin(moveTime),
                        firstPos.y + direction.y * Mathf.Sin(moveTime));
                }
                else if(moveTime  > 3.14f)
                {
                    end = true;
                }
            }
        }
    }
}
