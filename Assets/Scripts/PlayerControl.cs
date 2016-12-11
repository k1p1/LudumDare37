using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Photon.PunBehaviour
{
	#region Network
	private static PlayerControl localPlayerInstance;

	public static PlayerControl LocalPlayerInstance
	{
		get { return localPlayerInstance; }
	}
	#endregion
 
    public float DashPowerup { get { return _dashTimer / DashCooldown ; } }
    public bool CanDash { get { return DashCooldown - _dashTimer < 0.1f; } }
    private bool isGrounded
    { get { return Mathf.Abs(_rigidbody.velocity.y) < 0.01f || _rigidbody.velocity.magnitude > ThresholdVelocity; } }

    [SerializeField]
    private GameObject Explosion;
    [SerializeField]
    private GameObject NameCanvas;
    [SerializeField]
    private Color TheirColor;
    [SerializeField]
    private Color MyColor;
    [SerializeField]
    private float MoveForce;
    [SerializeField]
    private float HitForce;
    [SerializeField]
    private float DashForce;
    [SerializeField]
    private float DashCooldown;
    [SerializeField]
    private float GroundedDrag;
    [SerializeField]
    private float FallingDrag;
    [SerializeField]
    private float ThresholdVelocity;
    [SerializeField]
    private float DashTime;

    private Vector3 _force = new Vector2();
    private Rigidbody _rigidbody;
    private float _dashTimer;
    private bool _dash = false;

	void Awake()
	{
        _rigidbody = GetComponent<Rigidbody>();
        if (photonView.isMine)
		{
            GetComponent<Renderer>().material.color = MyColor;
			localPlayerInstance = this;
		}
    }
    private void Start ()
    {        
        var canvas = Instantiate(NameCanvas, transform.position, Quaternion.identity).GetComponent<NameCanvas>();
        canvas.Target = gameObject;
        canvas.PlayerName = photonView.owner.customProperties[GameManager.NameKey].ToString();
    }

    // Update is called once per frame
    private void Update ()
    {
		if (photonView.isMine)
		{
			this.ProcessInputs();
		}        
	}

    private void ProcessInputs()
    {
        _dashTimer = Mathf.Min(_dashTimer + Time.unscaledDeltaTime, DashCooldown);
        _force = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
        if (Input.GetKeyDown(KeyCode.Space) && CanDash)
        {
            Dash();
        }
    }

    private void Dash()
    {
        _dashTimer = 0;
        PhotonNetwork.Instantiate(Explosion.name, transform.position, Quaternion.identity, 0, new object[] { photonView.ownerId, _force });

        //StartCoroutine(DashCoroutine());
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            _rigidbody.AddForce(_force * MoveForce + (_dash ? DashForce * _force.normalized : Vector3.zero), ForceMode.Force);
            if (!_dash)
            {
                _rigidbody.drag = GroundedDrag;
            }
        }
        else
        {
            _rigidbody.drag = FallingDrag;
        }
        
    }

    private IEnumerator DashCoroutine()
    {
        _dash = true;

        Vector3 targetScale = 6 * Vector3.one;
        Vector3 velocity = new Vector3();
        float time = DashTime;
        while (time > 0)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref velocity, DashTime);
            time -= Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * 3;
        _dash = false;
    }
    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag(gameObject.tag) && photonView.isMine)
        {
            Rigidbody otherBody = collider.GetComponent<Rigidbody>();
            Vector3 direction = -(collider.transform.position - transform.position).normalized;
            float factor = Mathf.Max(0, Vector3.Dot(otherBody.velocity, direction));
            Vector3 hitForce = direction * factor * HitForce;// (_dash ?  : 1.0f);
            RpcTakeHit(transform.position, hitForce);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitForce = (collision.collider.transform.position - transform.position).normalized * _rigidbody.velocity.magnitude;
        Vector3 hitPos = collision.contacts[0].point;
        if (collision.collider.CompareTag("Obsticle") && PhotonNetwork.isMasterClient)
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(hitForce, hitPos, ForceMode.Impulse);
        }
    }

    [PunRPC]
    public void RpcTakeHit(Vector3 position, Vector3 force)
    {        
        if (photonView.isMine)
        {
            _rigidbody.AddForceAtPosition(force, position, ForceMode.Force);
        }
    }

}