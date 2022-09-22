using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
    [SerializeField]
    Button buttonPosition;
    Vector3 direction;
    void Start()
    {
        Vector3 pos = buttonPosition.transform.position;
        direction = new Vector3(pos.x - transform.position.x, pos.y - transform.position.y);
        direction = Vector3.Normalize(direction);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * 2f * Time.deltaTime);
    }
}
