using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    public bool cameraBob = true;
    float originalY;
    public float speed;
    public float magnitute;
    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(cameraBob)
        {
            float offsetY = magnitute * Mathf.Sin(speed * Time.time);

            transform.position = new Vector3(transform.position.x, offsetY + originalY, transform.position.z);
        }
    }
}
