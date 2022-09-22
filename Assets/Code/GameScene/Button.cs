using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField]
    KeyCode keyCode;

    

    private void Update()
    {
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
       if (Input.GetKeyDown(keyCode))
        {
            Transform point = collision.transform;

            float distance = Vector3.Distance(transform.position, point.position);
            if (distance <= 0.128f)
            {
                Debug.Log("Perfect hit!");
            }
            else if (distance <= 0.384f)
            {
                Debug.Log("Very good hit");
            }
            else if (distance <= 0.768f)
            {
                Debug.Log("Good hit");
            }
            else
            {
                Debug.Log("Fine hit");
            }
            Debug.Log(point.name);
            Destroy(point.gameObject);
        }
    }
}
