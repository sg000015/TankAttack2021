using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Damage : MonoBehaviour
{
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    public int hp = 100;
    private PhotonView pv;

    void Start()
    {
        pv=GetComponent<PhotonView>();
        GetComponentsInChildren<MeshRenderer>(renderers);

    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.CompareTag("CANNON"))
        {
            
            string shooter = coll.gameObject.GetComponent<Cannon>().shooter;
            hp-=10;
            Destroy(coll.gameObject);
            if(hp<=0){StartCoroutine(TankDestroy(shooter));}
            //StartCoroutine("TankDestroy",shooter);  
        }

    }

    IEnumerator TankDestroy(string shooter)
    {   
        string msg = $"\n<color=#00ff00>{pv.Owner.NickName}</color> is killed by <color=#ff0000>{shooter}</color>";
        GameManager.instance.msgText.text += msg;

        hp=100;
        GetComponent<BoxCollider>().enabled=false;
        if(pv.IsMine){GetComponent<Rigidbody>().isKinematic=true;}
        foreach(var mesh in renderers) mesh.enabled = false;
        yield return new WaitForSeconds(5.0f);
        Vector3 pos = new Vector3(Random.Range(-200.0f,200.0f),20.0f,Random.Range(-200.0f,200.0f));
        transform.position = pos;
        GetComponent<BoxCollider>().enabled=true;
        if(pv.IsMine){GetComponent<Rigidbody>().isKinematic=false;}
        foreach(var mesh in renderers) mesh.enabled = true;

    }
}
