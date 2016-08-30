using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour {

    public GameObject myCamera;

    bool isAlive = true;
    Vector3 position;
    Quaternion rotation;
    float lerpSmoothing = 10f;

	// Use this for initialization
	void Start () {

        if (photonView.isMine)
        {
            gameObject.name = "Me";
            Debug.Log("IsMine");
            myCamera.SetActive(true);
            GetComponent<Player>().enabled = true;
            GetComponent<Controller2D>().enabled = true;
        }
        else
        {
            gameObject.name = "Network Player";
            StartCoroutine("Alive");
        }

	}
	
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    IEnumerator Alive()
    {
        while (isAlive)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lerpSmoothing);
            yield return null;
        }
    }
}
