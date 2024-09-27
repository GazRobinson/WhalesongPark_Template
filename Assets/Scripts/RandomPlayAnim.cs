using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayAnim : MonoBehaviour
{
    public Animator anim;

    public float randomMaxNum = 1000;
    public float randomMinNum = 1;
    private float randomNum;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        randomNum = Random.Range(randomMinNum, randomMaxNum);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= randomNum)
        {
            anim.SetTrigger("active");
            timer = 0;
            randomNum = Random.Range(randomMinNum, randomMaxNum);
        }
    }
}
