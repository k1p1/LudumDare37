using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperControl : MonoBehaviour
{

    public static event Action<BumperControl> PlayerSpawned;
    public static event Action<BumperControl> PlayerDead;
    public float DashPowerup { get { return _dashTimer / DashCooldown ; } }
    public bool CanDash { get { return DashCooldown - _dashTimer < 0.1f; } }
    private bool isGrounded { get { return Mathf.Abs(_rigidbody.velocity.y) < 0.01f || _rigidbody.velocity.magnitude > ThresholdVelocity; } }
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
        if (collision.collider.CompareTag(gameObject.tag))
        {
            collision.collider.GetComponent<Rigidbody>().AddForceAtPosition(-collision.contacts[0].normal * HitForce, collision.contacts[0].point, ForceMode.Impulse);
        }
    }

    private void OnDestroy()
    {
        if (PlayerDead != null)
        {
            PlayerDead(this);
        }
    }
}