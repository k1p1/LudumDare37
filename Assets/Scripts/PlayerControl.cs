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

    public static event Action<PlayerControl> PlayerSpawned;
    public static event Action<PlayerControl> PlayerDead;

    public float DashPowerup { get { return _dashTimer / DashCooldown ; } }
    public bool CanDash { get { return DashCooldown - _dashTimer < 0.1f; } }
    private bool isGrounded { get { return Mathf.Abs(_rigidbody.velocity.y) < 0.01f || _rigidbody.velocity.magnitude > ThresholdVelocity; } }
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
		if (photonView.isMine)
		{
            GetComponent<Renderer>().material.color = MyColor;
			localPlayerInstance = this;
		}

		_rigidbody = GetComponent<Rigidbody>();
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
        _dash = true;
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            _rigidbody.AddForce(_force * MoveForce * (_dash ? DashForce : 1.0f), ForceMode.Force);
            _dash = false;
            _rigidbody.drag = GroundedDrag;
        }
        else
        {
            _rigidbody.drag = FallingDrag;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitForce = (collision.collider.transform.position - transform.position).normalized * HitForce;
        Vector3 hitPos = collision.contacts[0].point;
        if (collision.collider.CompareTag(gameObject.tag) && photonView.isMine)
        {
            collision.gameObject.GetComponent<PlayerControl>().photonView.RPC("RpcTakeHit", PhotonTargets.All, hitPos, hitForce);
        }
        else if (collision.collider.CompareTag("Obsticle") && PhotonNetwork.isMasterClient)
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
        Debug.Log(force);
        if (photonView.isMine)
        {
            _rigidbody.AddForceAtPosition(force, position, ForceMode.Impulse);
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