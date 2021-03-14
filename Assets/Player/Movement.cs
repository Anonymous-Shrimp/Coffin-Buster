using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Movement : NetworkBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float gravity = 20.0f;
    public float rotSpeed = 10;

    public KeyCode dashKeybind = KeyCode.Space;
    public float dashSpeed;
    public float dashTime;
    public float pushPower = 2f;
    private Vector3 moveDirection = Vector3.zero;
    [Space]
    public KeyCode pickupKeybind = KeyCode.Mouse0;
    public string pickupTag = "PickupItem";
    public float pickupDistance = 3f;
    public GameObject itemHeld;
    public Transform FakeParent;
    public Vector3 _positionOffset;
    public Quaternion _rotationOffset;
    bool pickedUp = false;
    public bool canPickup = true;
    public PlayerPickup pickupObject;
    public Color playerColor;

    public Outline.Mode outlineMode = Outline.Mode.OutlineAll;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        pickupObject = GetComponentInChildren<PlayerPickup>();
    }

    [Client]
    void Update()
    {
        if (!hasAuthority) { return; }
        #region Movement
        moveDirection.y -= gravity * Time.deltaTime;
        
        characterController.Move(moveDirection * Time.deltaTime);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if(characterController.isGrounded)
        {
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

        #region Dash
        if (Input.GetKeyDown(dashKeybind))
        {
            StartCoroutine(Dash());
        }
        #endregion

        #region Pickup
        GameObject targeted = null;
        if (!pickedUp)
        {
            foreach (GameObject item in pickupObject.possiblePickupItems)
            {
                if (Vector3.Distance(item.transform.position, transform.position) <= pickupDistance)
                {
                    Debug.DrawLine(transform.position, item.transform.position, Color.red);
                    if (targeted != null)
                    {
                        if (Vector3.Distance(item.transform.position, transform.position) < Vector3.Distance(targeted.transform.position, transform.position))
                        {
                            targeted = item;
                        }
                    }
                    else
                    {
                        targeted = item;

                    }
                }
            }
        }
        if (targeted != null)
        {
            Debug.DrawLine(transform.position, targeted.transform.position, Color.blue);
            if (targeted.GetComponent<Outline>() != null)
            {
                targeted.GetComponent<Outline>().enabled = true;
                targeted.GetComponent<Outline>().OutlineColor = playerColor;
                targeted.GetComponent<Outline>().OutlineMode = outlineMode;
            }
            if (Vector3.Distance(targeted.transform.position, transform.position) > pickupDistance)
            {
                if (targeted.GetComponent<Outline>() != null)
                {
                    targeted.GetComponent<Outline>().OutlineColor = Color.white;
                    targeted.GetComponent<Outline>().enabled = false;
                }
                targeted = null;

            }
            print(targeted);
        }
        if (Input.GetKeyDown(pickupKeybind))
        {
            if (pickedUp)
            {
                if (itemHeld.GetComponent<Rigidbody>() != null)
                {
                    itemHeld.GetComponent<Rigidbody>().isKinematic = false;
                }
                itemHeld = null;
                pickedUp = false;

            }
            else if (canPickup)
            {
                if (targeted != null)
                {
                    itemHeld = targeted;

                    if (targeted.GetComponent<Outline>() != null)
                    {
                        targeted.GetComponent<Outline>().OutlineColor = Color.white;
                        targeted.GetComponent<Outline>().enabled = false;
                    }
                    pickedUp = true;
                    
                    if (itemHeld.GetComponent<Rigidbody>() != null)
                    {
                        itemHeld.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    targeted = null;
                }
            }
        }
        if(itemHeld != null)
        {
            var newpos = transform.TransformPoint(_positionOffset);
            var newfw = transform.TransformDirection(transform.forward);
            var newup = transform.TransformDirection(transform.up);
            var newrot = Quaternion.LookRotation(newfw, newup);
            itemHeld.transform.position = newpos;
            itemHeld.transform.rotation = newrot;
        }
        #endregion
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
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
}
