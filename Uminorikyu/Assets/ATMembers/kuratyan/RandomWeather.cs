using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using Unity.VisualScripting;


public class RandomWeather : MonoBehaviour
{
    //�����_���擾�p
    int rnd;

    //���g�̎擾�p
    Image image;

    // �V�����X�v���C�g���A�^�b�`
    [SerializeField] Sprite sunnySprite;    //����
    [SerializeField] Sprite cloudySprite;   //�܂�
    [SerializeField] Sprite rainySprite;    //�J

    void Start()
    {
        rnd = Random.Range(1, 4); // �� 1�`3�͈̔͂Ń����_���Ȑ����l���Ԃ�
        image = GetComponent<Image>();

        //����
        if (rnd == 1)
        {
            image.sprite = sunnySprite;
            PlayerControllerManager.controllerManager.round.weatherNum = rnd;
        }
        //�܂�
        if (rnd == 2)
        {
            image.sprite = cloudySprite;
            PlayerControllerManager.controllerManager.round.weatherNum = rnd;
        }
        //�J
        if (rnd == 3)
        {
            image.sprite = rainySprite;
            PlayerControllerManager.controllerManager.round.weatherNum = rnd;
        }
    }
}
