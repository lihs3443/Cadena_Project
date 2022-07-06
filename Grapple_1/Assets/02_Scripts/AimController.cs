using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    Grappling grappling;
    void Start()
    {
        grappling = GameObject.Find("Player").GetComponent<Grappling>();
    }

    void Update()
    {
        transform.GetComponent<SpriteRenderer>().enabled = !grappling.isHookActive;

        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 90));
    }
}
