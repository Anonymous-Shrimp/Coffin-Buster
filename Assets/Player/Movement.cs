using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 3f;
    Rigidbody rigid;
    public KeyCode dashKeybind = KeyCode.Space;
    public float dashForce = 10;
    public float dashTime;
    float dashTimeUsed;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        float mH = Input.GetAxis("Horizontal");
        float mV = Input.GetAxis("Vertical");
        rigid.velocity = new Vector3(mH * speed, rigid.velocity.y, mV * speed);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            transform.LookAt(hit.point);
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        }
        if (Input.GetKeyDown(dashKeybind))
        {
            dashTimeUsed = 0;
        }
        if(dashTimeUsed < dashTime)
        {
            dashTimeUsed += Time.deltaTime;
            rigid.AddRelativeForce(new Vector3(0, 0, dashForce * Time.deltaTime));
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
