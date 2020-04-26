using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update

    private float lookSpeed, mouseX, mouseY;
    public float moveSpeed;
    public float dist;
    public float yAngleMin;
    public float yAngleMax;
    void Start()
    {
        dist = 7f;
        yAngleMin = 20f;
        yAngleMax = 50f;
        lookSpeed = 7.0f;
        mouseX = mouseY = 0.0f;
        moveSpeed = 10;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement();

        ThirdPerson(GameObject.FindGameObjectsWithTag("goodGuy")[0]);
    }

    private void ThirdPerson(GameObject target)
    {
        transform.eulerAngles = new Vector3(mouseY, mouseX, 0.0f);

        Vector3 input = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

        transform.position += input * Time.deltaTime * moveSpeed;

        mouseY = Mathf.Clamp(mouseY, yAngleMin, yAngleMax);

        Vector3 thirdPersonDist = new Vector3(14f, 10f, -4f);
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        transform.position = target.transform.position + rotation * thirdPersonDist;
        transform.LookAt(target.transform.position);
    }
    // Camera free-look, moves where camera is pointing
    private void Movement()
    {

        mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;
        transform.localRotation = Quaternion.Euler(new Vector4(-1f * (mouseY * 180f), mouseX * 360f, transform.localRotation.z));

        Vector3 input = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));


        transform.position += input * Time.deltaTime * moveSpeed;

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Only if we are above ground
            if (transform.position.y > 0.0f)
            {
                transform.position -= Vector3.up * Time.deltaTime * moveSpeed;
            }
        }
    }
}
