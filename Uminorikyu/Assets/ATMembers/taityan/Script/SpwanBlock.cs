using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createObject : MonoBehaviour
{

    public GameObject enemy;
    
    int num = 0;//•bŠÔ

    int count = 0; 
    int countMax = 40;//o‚é”

    //Range‚Ì’†
    int randomSecond = 0;//ƒ‰ƒ“ƒ_ƒ€•b
    int min = 50;//Å¬
    int max = 100;//Å‘å
    
    void Start()
    {
        //60`120=1`2•bŠÔ:120`180=2`3•bŠÔ
        randomSecond = Random.Range(min, max);
    }
    void FixedUpdate()
    {
        //–³ŒÀ‚É‘«‚·
        num++;

        //num‚ÍRange‚Ì+1
        if (num % max+1 == randomSecond && count <= countMax) 
        {
            //^‚ñ’†‚ÌVector3‚ÅPosition‚¢‚¶‚é
            Instantiate(enemy, new Vector3(0, 0+count*0.1f, 0), Quaternion.identity);
            
            //o‚é”‚ğƒJƒEƒ“ƒg‚µ‚Ä‚¢‚­
            count++;

            //“ñ‰ñ–ÚˆÈ~‚Ì[randomSecond]‚Ì’Š‘I
            randomSecond = Random.Range(min, max);

            //if‚ª’Ê‚Á‚½‚ç[num]‚ğ0‚É‚µ‚ÄifŠO‚Éo‚·
            num = 0;

            //Às‚Å‚«‚½‚±‚Æ‚ğ‹Šo“I•\¦
            Debug.Log("‚Å‚½‚Ÿ");
        }
    }
}