using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using Unity.VisualScripting;


public class unko : MonoBehaviour
{

    int rnd;

    // Image�R���|�[�l���g���A�^�b�`
    //����z��
    public Image targetImage;

    // �V�����X�v���C�g���A�^�b�`
    //�܂�z��
    public Sprite newSprite;

    // �V�����X�v���C�g���A�^�b�`
    //�J�z��
    public Sprite newSprite2;

    void Start()
    {
        rnd = Random.Range(1, 4); // �� 1�`3�͈̔͂Ń����_���Ȑ����l���Ԃ�

        if (rnd == 2)
        {
            targetImage.sprite = newSprite;
        }
        if (rnd == 3)
        {
            targetImage.sprite = newSprite2;
        }
    }

   


    


    // �{�^����C�x���g�ŌĂяo�����\�b�h


}
