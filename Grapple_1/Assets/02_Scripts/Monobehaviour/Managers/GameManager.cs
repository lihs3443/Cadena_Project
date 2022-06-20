using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [System.Serializable]
    public class DataContainer
    {
        public Player playerComp = null;
        public Camera mainCameraComp = null;
    }

    public DataContainer dataCont = new DataContainer();

    private void Awake()
    {
        MakeSingleton();
    }

    private void Start()
    {
        StuffDataContainer();
    }

    void StuffDataContainer()
    {
        dataCont.playerComp = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        dataCont.mainCameraComp = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void MakeSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}
