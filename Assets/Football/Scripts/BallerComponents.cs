using UnityEngine;
using UnityEngine.UI;

public class BallerComponents : MonoBehaviour
{
	[Header("References")]
	public KeyCode powerShotKeyCode = KeyCode.Mouse0;
	public KeyCode getNewBallKeyCode = KeyCode.Mouse2;
	public GameObject player;
	public Camera playerCamera;
	public Rigidbody ballRigidbody;
	public AudioSource footballSound;
	public bool isDribbling;
	public Slider shootSlider;
	public float shootPower;
	public Vector3 footPos;
	public CharacterController characterController;
	public Animator ballerAnimator;
	public TrajectoryLineRenderer trajectoryLineRenderer;
	public Transform shootingCenter;
}