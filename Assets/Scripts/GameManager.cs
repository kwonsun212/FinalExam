using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float gameTime;
    public float maxGametime = 2 * 10f;
    public PoolManager pool;
    public Player player;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        if(gameTime > maxGametime)
        {
            gameTime = maxGametime;
           
        }
    }
}
