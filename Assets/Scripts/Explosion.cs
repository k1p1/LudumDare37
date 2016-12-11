using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Photon.PunBehaviour
{

    [SerializeField]
    private float Force;
    [SerializeField]
    private Vector3 StartSize;
    [SerializeField]
    private Vector3 EndSize;
    [SerializeField]
    private float ExplodeTime;
    [SerializeField]
    private float DirectionThreshold;
    [SerializeField]
    private float Distance;

    private int ownerId = -1;
    private Vector3 growVelocity;
    private Vector3 moveVeloctiy;
    private Vector3 goal;
    
    void Start ()
    {
        goal = transform.position;
        if (photonView.instantiationData != null && photonView.instantiationData.Length > 0)
        {
            ownerId = (int)(photonView.instantiationData[0]);
            if (photonView.instantiationData.Length > 1)
            {
                var dir = (Vector3)photonView.instantiationData[1];
                if (dir.magnitude > DirectionThreshold)
                {
                    goal = transform.position + dir.normalized * Distance;
                }
            }
        }
        Invoke("Kill", ExplodeTime );
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, EndSize, ref growVelocity, ExplodeTime);
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref moveVeloctiy, ExplodeTime);
	}

    private void OnTriggerEnter(Collider collider)
    {
        var view = collider.GetComponent<PlayerControl>().photonView;
        if (collider.CompareTag("Bumper") && view.photonView.isMine && view.ownerId != ownerId)
        {
            collider.GetComponent<Rigidbody>().AddExplosionForce(Force, transform.position, transform.localScale.magnitude);
        }
    }

    private void Kill()
    {
        if (PhotonNetwork.player.ID == photonView.ownerId)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
