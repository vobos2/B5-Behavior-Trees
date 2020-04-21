using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWallAnimation : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void triggerDoor()
    {
        if (transform.position.y == 2.5f)
        {
            transform.position += new Vector3(0, 3f, 0);
        }
        else if (transform.position.y == 5.5f)
        {
            transform.position -= new Vector3(0, 3f, 0);
        }

    }


}
