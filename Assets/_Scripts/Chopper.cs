using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Chopper : MonoBehaviourPunCallbacks
{
    public GameObject health;
    public GameObject grenade;

    float lerpTime = 1f;
    float currentLerpTime;
    float waitTime = 2f;
    float destroyTimer = 5f;

    float moveDistance = 30f;
    bool leaving;


    Vector2 startPos;
    Vector2 endPos;

    void Start()
    {
        startPos = transform.position;
        endPos = Vector2.zero;
    }

    void Update()
    {
        destroyTimer -= Time.deltaTime;

        if(destroyTimer < 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        //increment timer once per frame
        currentLerpTime += Time.deltaTime;


        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }

        //lerp!
        //float perc = currentLerpTime / lerpTime;
        float t = currentLerpTime / lerpTime;
        t = Mathf.Sin(t * Mathf.PI * 0.5f);
        transform.position = Vector2.Lerp(startPos, endPos, t);
        
        if(transform.position == Vector3.zero)
        {
            waitTime -= Time.deltaTime;
        }

        if(waitTime < 0 && !leaving)
        {
            DropCrate();
            leaving = true;
            startPos = Vector2.zero;
            endPos = transform.position + transform.right * moveDistance;
            currentLerpTime = 0f;
        }
    }

    void DropCrate()
    {
        int numberOfPlayers = PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.IsMasterClient)
        {
            for(int i = 0; i < numberOfPlayers; i++)
            {
                float randomHealthX = Random.Range(-5, 5);
                float randomHealthY = Random.Range(-5, 5);
                float randomNadeX = Random.Range(-5, 5);
                float randomNadeY = Random.Range(-5, 5);

                PhotonNetwork.Instantiate(health.name, new Vector2(randomHealthX, randomHealthY), Quaternion.identity);
                PhotonNetwork.Instantiate(grenade.name, new Vector2(randomNadeX, randomNadeY), Quaternion.identity);
            }
        }
    }
}
