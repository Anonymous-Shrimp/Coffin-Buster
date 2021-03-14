using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerPickup : NetworkBehaviour
{
    public KeyCode pickupKeybind = KeyCode.Mouse0;
    public string pickupTag = "PickupItem";
    public float pickupDistance = 3f;
    public GameObject itemHeld;
    public GameObject releaseParent;
    public Vector3 offsetHold;
    bool pickedUp = false;
    public bool canPickup = true;
    public List<GameObject> possiblePickupItems;
    public Color playerColor;
    public Outline.Mode outlineMode = Outline.Mode.OutlineAll;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }
        GameObject targeted = null;
        if (!pickedUp)
        { 
            foreach (GameObject item in possiblePickupItems)
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
                if (releaseParent != null)
                {
                    itemHeld.transform.parent = releaseParent.transform;
                }
                else
                {
                    itemHeld.transform.parent = null;
                }
                if (itemHeld.GetComponent<Rigidbody>() != null)
                {
                    itemHeld.GetComponent<Rigidbody>().isKinematic = false;
                }
                itemHeld = null;
                pickedUp = false;
                
            }
            else if (canPickup)
            {
                if(targeted != null)
                {
                    itemHeld = targeted;
                    
                    if (targeted.GetComponent<Outline>() != null)
                    {
                        targeted.GetComponent<Outline>().OutlineColor = Color.white;
                        targeted.GetComponent<Outline>().enabled = false;
                    }
                    pickedUp = true;
                    
                    itemHeld.transform.parent = transform;
                    itemHeld.transform.localPosition = offsetHold;
                    itemHeld.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    if (itemHeld.GetComponent<Rigidbody>() != null)
                    {
                        itemHeld.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    targeted = null;
                }
            }
        } 
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(pickupTag))
        {
            if (!possiblePickupItems.Contains(other.gameObject))
            {
                possiblePickupItems.Add(other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(pickupTag))
        {
            if (possiblePickupItems.Contains(other.gameObject))
            {
                possiblePickupItems.Remove(other.gameObject);
                if (other.GetComponent<Outline>() != null)
                {
                    other.GetComponent<Outline>().OutlineColor = Color.white;
                    other.GetComponent<Outline>().enabled = false;
                }
            }
        }
    }
}
