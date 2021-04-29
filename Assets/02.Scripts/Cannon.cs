using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject Effect;
    public float speed = 2000.0f;
    public string shooter;
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward*speed); 
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject obj = Instantiate(Effect,transform.position,Quaternion.identity);
        Destroy(obj, 5.0f);
    }

    
}
