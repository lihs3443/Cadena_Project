using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance = null;

    [HideInInspector]
    public InputActionMap actionMap_Player;
    [HideInInspector]
    public InputAction player_move;
    [HideInInspector]
    public InputAction player_jump;
    [HideInInspector]
    public InputAction player_pressW;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        actionMap_Player = GetComponent<PlayerInput>().actions.FindActionMap("Player");
    }

    private void OnEnable()
    {
        player_move = actionMap_Player.FindAction("Move");
        player_jump = actionMap_Player.FindAction("Jump");
        player_pressW = actionMap_Player.FindAction("PressW");
    }
}
