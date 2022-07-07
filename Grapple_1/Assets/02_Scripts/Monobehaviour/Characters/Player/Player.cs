using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    [System.Serializable]
    public class PlayerStat
    {
        public float movementSpeed = 4f;
    }
    public PlayerStat playerStat = new PlayerStat();

    private void Awake()
    {
        currentState = new State_Idle();
    }

    protected override void DeathAction()
    {
        StageManager.instance.tempStage0.RespawnPlayer(gameObject);
        transform.GetComponent<Grappling>().Hookoff();                  // ¸®½ºÆù½Ã ÈÅ ²ô±â
    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if (_collision.gameObject.CompareTag("InstantKillTrap"))
        {
            DeathAction();
        }
    }
}
