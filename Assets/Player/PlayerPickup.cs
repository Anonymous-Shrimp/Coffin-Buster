using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerPickup : NetworkBehaviour
{
    public List<GameObject> possiblePickupItems;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }
        
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GetComponentInParent<Movement>().pickupTag))
        {
            if (!possiblePickupItems.Contains(other.gameObject))
            {
                possiblePickupItems.Add(other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(GetComponentInParent<Movement>().pickupTag))
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
