using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;
using UnityEngine.Audio;




public class TankCtrl : MonoBehaviour,IPunObservable
{
    private Transform tr;
    public float speed = 10.0f;
    public float dumping = 100.0f;
    public float rot = 20.0f;
    private PhotonView pv;

    public Transform firePos;
    public GameObject cannon;

    public TMPro.TMP_Text userIdText;

    public Transform cannonMesh;
    public AudioClip fireSfx;
    public AudioSource soundSource;
    
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        pv =GetComponent<PhotonView>();
        soundSource = GetComponent<AudioSource>();
        userIdText.text = pv.Owner.NickName;
        if(pv.IsMine)
        {
            Camera.main.GetComponent<SmoothFollow>().target = tr.Find("CamPivot").transform;
            GetComponent<Rigidbody>().centerOfMass = new Vector3(0,-5.0f,0);
        }
        else{ GetComponent<Rigidbody>().isKinematic =  true;}

    }

    private int i=0;
    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine)
        {
            float v = Input.GetAxis("Vertical");        
            float h = Input.GetAxis("Horizontal");
            
            tr.Translate(Vector3.forward*Time.deltaTime*speed*v);
            tr.Rotate(Vector3.up*Time.deltaTime*dumping*h);


            if(Input.GetMouseButtonDown(0))
            {   
                pv.RPC("Fire",RpcTarget.All,pv.Owner.NickName);
                // pv.RPC("Fire",RpcTarget.AllViaServer,"^0^");
                i=0;
            }

            if(Input.GetMouseButton(0))
            {
                i++;
                if(i%4==3)
                {
                    pv.RPC("Fire",RpcTarget.All,pv.Owner.NickName);
                    // pv.RPC("Fire",RpcTarget.AllViaServer,"^0^");
                    i=0;
                }
            }
            if(Input.GetMouseButton(1))
            {
                pv.RPC("Fire",RpcTarget.All,pv.Owner.NickName);
            }
            float r = Input.GetAxis("Mouse ScrollWheel");
            cannonMesh.Rotate(Vector3.right*Time.deltaTime*rot*r);
        }
        else
        {
            if((tr.position-receivePos).sqrMagnitude>9.0f)
            {
                tr.position = receivePos;
            }
            else
            {
                tr.position = Vector3.Lerp(tr.position,receivePos,Time.deltaTime*10.0f);
            }
            
            tr.rotation = Quaternion.Slerp(tr.rotation,receiveRot,Time.deltaTime*10.0f);
        }

    }


    [PunRPC]
    void Fire(string shooterName)
    {
        soundSource?.PlayOneShot(fireSfx,0.8f);
        GameObject _cannon = Instantiate(cannon,firePos.position,firePos.rotation);
        _cannon.GetComponent<Cannon>().shooter = shooterName;
        //Destroy(_cannon,15.0f);
        
    }

    //네트워크를 통해 수신받을 변수
    Vector3 receivePos = Vector3.zero;          
    Quaternion receiveRot = Quaternion.identity;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)//자기꺼인지 확인, PhotonView.IsMine == true
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
    
}

