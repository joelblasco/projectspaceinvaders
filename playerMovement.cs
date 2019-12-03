using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float speed;
    public GameObject bullet;
    public bool bulletActive;
    public Transform shotPoint;
    public alienManager manager;
    public bool move;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            int posInt = (int)transform.position.x;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                posInt -= 5;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                posInt += 5;
            }

            Vector3 v = new Vector3(
            Mathf.Clamp(posInt, -280, 280), -140, 0);
            transform.SetPositionAndRotation(v, Quaternion.identity);
        }
        //Disparo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!bulletActive)shot();
            //shot();
        }
    }
    private void shot()
    {
        GameObject b = Instantiate(bullet,shotPoint.transform.position, Quaternion.identity);
        b.GetComponent<bulletController>().p = this;
        b.GetComponent<bulletController>().shotByPlayer = true;
        bulletActive = true;
    }
    public void hitByBullet()
    {
        manager.hp--;
        manager.updateInfo();
        if (manager.hp == 0) manager.lostGame();
    }
}
