using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Photon.PunBehaviour
{
	#region Network
	private static GameObject localPlayerInstance;

	public static GameObject LocalPlayerInstance
	{
		get { return localPlayerInstance; }
	}
	#endregion

    public static event Action<PlayerControl> PlayerSpawned;
    public float DashPowerup { get { return _dashTimer / DashCooldown ; } }
    public bool CanDash { get { return Mathf.Approximately(_dashTimer, DashCooldown); } }

    [SerializeField]
    private float MoveForce;
    [SerializeField]
    private float HitForce;
    [SerializeField]
    private float DashForce;
    [SerializeField]
    private float DashCooldown;

    private Vector3 _force = new Vector2();
    private Rigidbody _rigidbody;
    private float _dashTimer;
    private bool _dash = false;

	void Awake()
	{
		if (photonView.isMine)
		{
			localPlayerInstance = gameObject;
		}
	}

    private void Start ()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (PlayerSpawned != null)
        {
            PlayerSpawned(this);
        }
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
		_force = new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
		_dashTimer = Mathf.Max(_dashTimer + Time.unscaledDeltaTime, DashCooldown);
		if (Input.GetKeyDown(KeyCode.Space) )
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
        _rigidbody.AddForce(_force * MoveForce * (_dash ? DashForce : 1.0f), ForceMode.Force);
        _dash = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(gameObject.tag))
        {
            collision.collider.GetComponent<Rigidbody>().AddForceAtPosition(-collision.contacts[0].normal * HitForce, collision.contacts[0].point, ForceMode.Impulse);
        }
    }
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Ground"))
    //    {
    //        _rigidbody.drag = 0;
    //        _rigidbody.mass = 20;
    //    }
    //}
}