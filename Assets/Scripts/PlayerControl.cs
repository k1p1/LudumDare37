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

	private bool isDead = false;

	private float freezeTimer = 1.0f;

	public bool IsDead
	{
		get { return isDead; }
	}

	void Awake()
	{
        _rigidbody = GetComponent<Rigidbody>();
        if (photonView.isMine)
		{
            GetComponent<Renderer>().material.color = MyColor;
			localPlayerInstance = this;
		}

		_rigidbody = GetComponent<Rigidbody>();
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
		if (freezeTimer > 0.0f)
		{
			freezeTimer -= Time.deltaTime;
		}
		else if (photonView.isMine && !isDead)
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
        StartCoroutine(DashCoroutine());
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
    private void OnTriggerEnter(Collider collider)
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

	public void Die()
	{
		isDead = true;
	}

    [PunRPC]
    public void RpcTakeHit(Vector3 position, Vector3 force)
    {        
        if (photonView.isMine)
        {
            _rigidbody.AddForceAtPosition(force, position, ForceMode.Force);
        }
    }

	[PunRPC]
	public void RpcSpawnPlayer(Vector3 position, float freezeTime)
	{
		Debug.Log(position);
		isDead = false;

		if (photonView.isMine)
		{
			transform.position = position;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
			_force = Vector2.zero;

			freezeTimer = freezeTime;
		}
	}
}