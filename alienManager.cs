using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class alienManager : MonoBehaviour
{
    public GameObject alien, defBlocksPrefab,currentDefBlocks;
    public List<alienController> aliens;
    public Transform leftAlien, rightAlien;
    public int alienRows = 5;
    public int alienPerRow = 8;
    bool _lastLimit;
    public enum _state { loading, playing, win};
    public _state current = _state.loading;
    public Text info, winText;
    public int hp = 3;
    public playerMovement player;
    void Start()
    {
        StartCoroutine(init());
    }

    // Update is called once per frame
    void Update()
    {
        if (current == _state.playing)
        {
            if (leftAlien.position.x <= -270) reachedLimit(false);
            if (rightAlien.position.x >= 270) reachedLimit(true);
        }
        if (Input.GetKeyDown(KeyCode.Space) && current == _state.win)
        {
            StartCoroutine(init());
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(init());
        }
    }
    IEnumerator init()
    {
        current = _state.loading;
        hp = 3;
        _lastLimit = false;
        winText.gameObject.SetActive(false);
        Vector3 aPos = new Vector3(-305, 150, 0);
        player.transform.position = new Vector3(0, -140, 0);
        if (currentDefBlocks != null) Destroy(currentDefBlocks);
        currentDefBlocks = Instantiate(defBlocksPrefab);
        foreach(alienController a in aliens)
        {
            Destroy(a.gameObject);
        }
        aliens = new List<alienController>();
        //to do: crear grupo de aliens, moviment baixant, aument de la velocitat conforme se eliminen, bala en dos direccions
        int index = 0;
        for (int i = 0; i < alienRows; i++)
        {
            aPos.y -= 30;
            for (int a = 0; a < alienPerRow; a++)
            {
                aPos.x += 35;
                GameObject ali = Instantiate(alien, aPos, Quaternion.identity);
                ali.GetComponent<alienController>().manager = this;
                ali.GetComponent<alienController>().index = index;
                index++;
                aliens.Add(ali.GetComponent<alienController>());
                //print($"Created alien {i},{a} on {aPos}");
                yield return new WaitForSeconds(0.1f);
            }
            aPos.x = -305;
        }
        findLimitAliens();
        updateInfo();
        yield return new WaitForSeconds(0.2f);
        foreach(alienController a in aliens)
        {
            a.initMovement();
        }
        player.move = true;
        current = _state.playing;
    }
    public void reachedLimit(bool _limit)
    {
        if (_limit != _lastLimit)
        {
            _lastLimit = _limit;
            foreach (alienController a in aliens)
            {
                a.goDown();
            }
        } 
        foreach(alienController a in aliens)
        {
            if (a.transform.position.y < player.transform.position.y+20) lostGame();
        }
    }
    public void killedAlien(int index)
    {
        GameObject d = aliens[index].gameObject;
        aliens.RemoveAt(index);
        Destroy(d);
        if (aliens.Count == 0)
        {
            current = _state.win;
            winText.gameObject.SetActive(true);
            winText.text = "You won! \n Press SPACE to Restart.";
        }
        else
        {
            foreach (alienController a in aliens)
            {
                a.tic /= 1.2f;
            }
            findLimitAliens();
            refillIndexs();
        }
        updateInfo();
    }
    void findLimitAliens()
    {
        int left = 0;
        int right = 0;
        for (int i = 0; i < aliens.Count; i++)
        {
            if (aliens[i].transform.position.x < aliens[left].transform.position.x)
            {
                left = i;
            }
            if (aliens[i].transform.position.x > aliens[right].transform.position.x)
            {
                right = i;
            }
            
        }
        leftAlien = aliens[left].transform;
        rightAlien = aliens[right].transform;
    }
    void refillIndexs()
    {
        int index = 0;
        foreach(alienController a in aliens)
        {
            a.index = index;
            index++;
        }
    }
    public void updateInfo()
    {
        info.text = $"Aliens: {aliens.Count}  -  Lifes: {hp}";
    }
    public void lostGame()
    {
        current = _state.win;
        winText.text = "You lost! \n Press SPACE to Restart";
        winText.gameObject.SetActive(true);
        foreach(alienController a in aliens)
        {
            a.move = false;
        }
        player.move = false;
    }
}
