using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWallAnimation : MonoBehaviour
{
    // Use this for initialization
    public GameObject door;
    public bool doorUp;
    void Start()
    {
        doorUp = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void triggerDoor()
    {

    }
    public void opendoor()
    {
        if (!doorUp)
        {
            /* door.transform.Translate(door.transform.position + new Vector3(0, 2.5f, 0));
             doorUp = true;*/
            door.SetActive(false);
        }
    }
    public void closedoor()
    {
        if (doorUp)
        {
            door.transform.Translate(door.transform.position - new Vector3(0, 2.5f, 0));
            doorUp = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("goodGuy"))
            opendoor();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("goodGuy"))
            closedoor();
    }
}
