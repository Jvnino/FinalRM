﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip soundFall;
    public AudioClip soundBumb;
    public AudioClip soundTalk;
    // Update is called once per frame
    public bool isLeft = false;
    public int status = 0;
    private float scale;
    void Start()
    {
        scale = gameObject.transform.localScale.y;
    }

    void Update()
    {

        Vector3 pos = gameObject.transform.localPosition;
        //Debug.Log(pos.x);
        float speed = 8;
        if (status == 0)
        {
            float y_ = Time.deltaTime * 3;
            if (pos.y > 3) // fly random
            {
                y_ = -Time.deltaTime * 3;
            }
            if (gameObject.transform.localPosition.x < 12 && !isLeft)
            {

                gameObject.transform.localPosition = new Vector3(pos.x + Time.deltaTime * speed, pos.y + y_, pos.z);
                gameObject.transform.localScale = new Vector3(scale, scale, scale);
                isLeft = false;
            }
            else if (gameObject.transform.localPosition.x > 12 && !isLeft)
            {

                randomFly();
            }
            else if (gameObject.transform.localPosition.x > -12 && isLeft)
            {
                gameObject.transform.localPosition = new Vector3(pos.x - Time.deltaTime * speed, pos.y + y_, pos.z);
                gameObject.transform.localScale = new Vector3(-scale, scale, scale);
                isLeft = true;
            }
            else if (gameObject.transform.localPosition.x < -12 && isLeft)
            {
                randomFly();
            }

        }
        else if (status == 2)
        {
            if (pos.y > -8) // fall
            {
                gameObject.transform.localPosition = new Vector3(pos.x, pos.y - Time.deltaTime * speed * 1.5f, pos.z);
            }
            else // destroy
            {
                status = 3;
                Destroy(gameObject);
            }

        }

    }

    public void randomFly()
    {
        if(Random.Range(0, 10) > 5) // talk
        {
            GetComponent<AudioSource>().clip = soundTalk;
            GetComponent<AudioSource>().Play();
        }
        //Fly from left or right
        if (Random.Range(0, 10) > 5)
        {
            transform.localPosition = new Vector3(-15, Random.Range(-5, 2), 0);
            isLeft = false;
        }
        else
        {
            transform.localPosition = new Vector3(15, Random.Range(-5, 2), 0);
            isLeft = true;
        }
    }
    public void die()
    {
        GetComponent<AudioSource>().clip = soundBumb;
        GetComponent<AudioSource>().Play();
        status = 1;
        GetComponent<Animator>().Play("hit");
    }
    public void fall()
    {
        status = 2;
        GetComponent<AudioSource>().clip = soundFall;
        GetComponent<AudioSource>().Play();
        GetComponent<Animator>().Play("fall");
    }
}
