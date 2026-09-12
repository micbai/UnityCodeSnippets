using UnityEngine;  
using UnityEngine.InputSystem;  
  
/// <summary>  
/// Attach to a Camera for full 3D fly-cam movement using the Unity Input System.  
/// Requires a PlayerInput component on the same GameObject with actions:  
///   Move  (Vector2) - WASD / left stick  
///   Look  (Vector2) - mouse delta / right stick  
///   Elevate (float) - Q/E or triggers for up/down  
/// </summary>  

[RequireComponent(typeof(PlayerInput))]  
public class FlyThroughCameraController : MonoBehaviour  
{  
    [Header("Speed")]  
    public float moveSpeed = 5f;  
    public float elevateSpeed = 5f;  
    public float rotateSpeed = 0.15f;  
  
    [Header("Feel")]  
    [Tooltip("Smooths out movement. 0 = instant, higher = more lag.")]  
    public float moveSmoothTime = 0.1f;  
  
    private Vector2 m_Move;  
    private Vector2 m_Look;  
    private float m_Elevate;  
  
    private Vector2 m_Rotation;  
    private Vector3 m_CurrentVelocity;  
    private Vector3 m_TargetVelocity;  
  
    private void Start()  
    {        
		// Initialise rotation from the camera's starting orientation  
        m_Rotation = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y);  
    }  
	
    // --- Input callbacks (wired via PlayerInput "Send Messages" behaviour) ---    
    public void OnMove(InputValue value)  
    {        
	    m_Move = value.Get<Vector2>();  
    }
	
    public void OnLook(InputValue value)  
    {   
	    m_Look = value.Get<Vector2>();  
    }  
	
    public void OnElevate(InputValue value)  
    {   
	    m_Elevate = value.Get<float>();  
    }  
	
    // --- Update ---  
    private void Update()  
    {   
		ApplyLook();  
        ApplyMove();  
    }  
    
    private void ApplyLook()  
    {   
		if (m_Look.sqrMagnitude < 0.01f) return;  
  
        m_Rotation.y += m_Look.x * rotateSpeed;        m_Rotation.x = Mathf.Clamp(m_Rotation.x - m_Look.y * rotateSpeed, -89f, 89f);  
        transform.localEulerAngles = new Vector3(m_Rotation.x, m_Rotation.y, 0f);  
    }  
    
    private void ApplyMove()  
    {        
		// Build desired velocity in local camera space  
        var horizontal = new Vector3(m_Move.x, 0f, m_Move.y) * moveSpeed;  
        var vertical   = new Vector3(0f, m_Elevate, 0f) * elevateSpeed;  
        m_TargetVelocity = transform.TransformDirection(horizontal) + vertical;  
  
        // Smooth then apply  
        m_CurrentVelocity = Vector3.Lerp(m_CurrentVelocity, m_TargetVelocity, moveSmoothTime > 0 ? Time.deltaTime / moveSmoothTime : 1f);  
        transform.position += m_CurrentVelocity * Time.deltaTime;    		
	}
}
