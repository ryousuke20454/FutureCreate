using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createObject : MonoBehaviour
{

    public GameObject enemy;
    
    int num = 0;//�b��

    int count = 0; 
    int countMax = 40;//�o�鐔

    //Range�̒�
    int randomSecond = 0;//�����_���b
    int min = 50;//�ŏ�
    int max = 100;//�ő�
    
    void Start()
    {
        //60�`120=1�`2�b��:120�`180=2�`3�b��
        randomSecond = Random.Range(min, max);
    }
    void FixedUpdate()
    {
        //�����ɑ���
        num++;

        //num��Range��+1
        if (num % max+1 == randomSecond && count <= countMax) 
        {
            //�^�񒆂�Vector3��Position������
            Instantiate(enemy, new Vector3(0, 0+count*0.1f, 0), Quaternion.identity);
            
            //�o�鐔���J�E���g���Ă���
            count++;

            //���ڈȍ~��[randomSecond]�̒��I
            randomSecond = Random.Range(min, max);

            //if���ʂ�����[num]��0�ɂ���if�O�ɏo��
            num = 0;

            //���s�ł������Ƃ����o�I�\��
            Debug.Log("�ł���");
        }
    }
}