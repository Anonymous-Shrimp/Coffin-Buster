using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float gravity = 20.0f;
    public float rotSpeed = 10;

    public KeyCode dashKeybind = KeyCode.Space;
    public float dashSpeed;
    public float dashTime;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        #region Movement
        moveDirection.y -= gravity * Time.deltaTime;
        
        characterController.Move(moveDirection * Time.deltaTime);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection = Camera.main.transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            
        }
        if (v != 0 || h != 0)
        {
            Vector3 fwd = new Vector3(h, 0, v);
            fwd = Camera.main.transform.TransformDirection(fwd);
            fwd.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(fwd), rotSpeed * Time.deltaTime);
        }
        #endregion
        
        if (Input.GetKeyDown(dashKeybind))
        {
            StartCoroutine(Dash());
        }
    }
    private IEnumerator Dash()
    {
        float startTime = Time.time; 
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(transform.forward * dashSpeed * Time.deltaTime);
            
            yield return null; 
        }
    }
}
