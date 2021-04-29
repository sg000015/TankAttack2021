using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Turret : MonoBehaviour
{
    public float rotSpeed;
    private PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        this.enabled = pv.IsMine;
    }

    // Update is called once per frame
    void Update()
    {
        float r = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up*Time.deltaTime*r*rotSpeed);
    }
}
