using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    public delegate void OnTriggerCustom();
    public delegate void OnTriggerAddPointCustom(float p);
    public delegate void OnTriggerSpeedCustom(bool isSPDown);
    public delegate void OnTriggerExitCus(GameObject obj);
    public OnTriggerCustom onTrigger;
    public OnTriggerAddPointCustom onTriggerAddpoint;
    public OnTriggerSpeedCustom onTriggerSpeed;
    public OnTriggerExitCus onTriggerExit;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COL " +collision.gameObject.name);
        if (collision.gameObject.layer == 13)
        { 
            onTrigger?.Invoke();
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if ((other.gameObject.name.Contains("Answers") && !other.gameObject.name.Contains("True")) || other.gameObject.name.Contains("Obstacle") || other.gameObject.name.Contains("Trap") || other.gameObject.name.Contains("Obtacle") || other.gameObject.layer ==13)
        {
            if (other.gameObject.name.Contains("Trap"))
            {
                other.GetComponent<Trap>().Impacted();
            }
            onTrigger?.Invoke();
        }
        if (other.gameObject.name.Contains("plusPoint10"))
        {
            Destroy(other.transform.parent.gameObject);
            onTriggerAddpoint?.Invoke(10);
        }

        if (other.gameObject.name.Contains("plusPoint5"))
        {
            Destroy(other.transform.parent.gameObject);
            onTriggerAddpoint?.Invoke(5);
        }

        //if (other.gameObject.name.Contains("ObtacleSPOut"))
        //{
        //    onTriggerSpeed?.Invoke(false);
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Out"))
        {
            GameObject par = other.gameObject.GetComponentInParent<Trap>().gameObject;
            onTriggerExit?.Invoke(par);
        }
    }
}
