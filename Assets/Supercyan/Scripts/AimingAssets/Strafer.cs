using System.Collections.Generic;
using UnityEngine;

public class Strafer : MonoBehaviour, IInitializable
{
    public void Initialize(GameObject character)
    {
        m_animator = character.GetComponent<Animator>();
        m_rigidBody = character.GetComponent<Rigidbody>();
    }
    MeshFilter mf;
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_jumpForce = 4 ;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;

    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_jumpInput = false;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    public Animator Animator { set { m_animator = value; } }
    public Rigidbody Rigidbody { set { m_rigidBody = value; } }

    public bool IsDead { get; set; }
    public bool IsZombie { get; set; }

    public bool CanMove = true;

    #region "Collision"
    private void OnCollisionEnter(Collision collision)
    {
        //ContactPoint[] contactPoints = collision.contacts;
        //for (int i = 0; i < contactPoints.Length; i++)
        //{
        //    if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
        //    {
        //        if (!m_collisions.Contains(collision.collider))
        //        {
        //            m_collisions.Add(collision.collider);
        //        }
        //        m_isGrounded = true;
        //    }
        //}
        if (collision.gameObject.tag == "Finish") 
            Application.Quit();
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }
    
    #endregion
    private void Start()
    {
        mf = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += new Vector3(-1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.W)&&transform.position.y<1)
        {
            transform.position+= new Vector3(0,2,0);
            m_rigidBody.useGravity = false;
        }
        if (Input.GetKeyDown(KeyCode.S) && transform.position.y > 1)
        {
            transform.position -= new Vector3(0, 2, 0);
            m_rigidBody.useGravity = true;
        }
        if (!m_jumpInput && Input.GetKey(KeyCode.Space) && CanMove)
        {
            m_jumpInput = true;
        }
    }

    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);

        if (!IsDead) { DirectUpdate(); }

        m_jumpInput = false;
    }

    private void DirectUpdate()
    {
        if (!CanMove) { return; }

        

        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && m_jumpInput && !IsZombie)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }
    }
}
