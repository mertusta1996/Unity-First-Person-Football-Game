using UnityEngine;

public class BallerKick : MonoBehaviour
{
    [Header("References")]
    public BallerComponents ballerComponents;

    // Private parameters
    private Vector3 verticalDirection;
    private Vector3 horizontalDirection;
    private Vector3 powerDirection;
    private bool isPowerShotPressing;
    private bool isPowerShotPressed;
    private const float MinAngle = 70f;
    private const float MaxAngle = 110f;
    private const float DirectionMultiplier = 5f;
    private const float PowerMultiplier = 15f;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ballerComponents.isDribbling = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ballerComponents.isDribbling = false;
        }
    }

    public void Update()
    {
        // Update foot position relative to the ball
        ballerComponents.footPos = ballerComponents.player.transform.InverseTransformPoint(ballerComponents.ballRigidbody.position);

        // Get world up vector and calculate the angle between the player and camera
        Vector3 worldUp = ballerComponents.player.transform.up;
        float angle = Vector3.Angle(worldUp, ballerComponents.playerCamera.transform.forward);

        // Calculate up-down value based on angle
        float upDownValue = CalculateUpDownValue(angle);

        // Calculate direction vectors
        verticalDirection = DirectionMultiplier * upDownValue * ballerComponents.player.transform.up;
        horizontalDirection = DirectionMultiplier * -ballerComponents.footPos.x * ballerComponents.player.transform.right;
        powerDirection = (1 - upDownValue) * PowerMultiplier * ballerComponents.player.transform.forward;

        HandleBallReset();
        HandlePowerShotInput();
        HandleTrajectoryRendering();
    }

    private float CalculateUpDownValue(float angle)
    {
        if (angle <= MinAngle)
        {
            return 1f;
        }
        else if (angle >= MaxAngle)
        {
            return 0f;
        }
        else
        {
            return Mathf.InverseLerp(MaxAngle, MinAngle, angle);
        }
    }

    private void HandleBallReset()
    {
        // Reset ball position when the reset key is pressed
        if (Input.GetKeyDown(ballerComponents.getNewBallKeyCode))
        {
            ballerComponents.ballRigidbody.Sleep();
            ballerComponents.ballRigidbody.transform.position = ballerComponents.shootingCenter.position + ballerComponents.shootingCenter.forward * 1f;
        }
    }

    private void HandlePowerShotInput()
    {
        // Power shot key events
        if (Input.GetKeyUp(ballerComponents.powerShotKeyCode))
        {
            if (ballerComponents.isDribbling)
            {
                isPowerShotPressed = true;
            }
        }

        if (Input.GetKey(ballerComponents.powerShotKeyCode))
        {
            isPowerShotPressing = true;
        }
        else
        {
            isPowerShotPressing = false;
        }
    }

    private void HandleTrajectoryRendering()
    {
        // Handle trajectory line rendering during power shot pressing
        if (isPowerShotPressing && ballerComponents.isDribbling)
        {
            if (!ballerComponents.trajectoryLineRenderer.gameObject.activeInHierarchy)
            {
                ballerComponents.trajectoryLineRenderer.gameObject.SetActive(true);
            }

            Vector3 futurePosition = ballerComponents.ballRigidbody.transform.position + verticalDirection + horizontalDirection + powerDirection;
            ballerComponents.trajectoryLineRenderer.CreateTrajectoryLine(ballerComponents.shootingCenter.position, ballerComponents.ballRigidbody.position, futurePosition);
        }
        else
        {
            if (ballerComponents.trajectoryLineRenderer.gameObject.activeInHierarchy)
            {
                ballerComponents.trajectoryLineRenderer.gameObject.SetActive(false);
            }
        }
    }

    public void FixedUpdate()
    {
        // Execute power shot when the button is pressed
        if (isPowerShotPressed)
        {
            PerformPowerShot();
        }

        // Handle shoot power increase while pressing the power shot key
        if (isPowerShotPressing)
        {
            IncreaseShootPower();
        }
        else
        {
            ResetShootPower();
        }
    }

    private void PerformPowerShot()
    {
        ballerComponents.ballerAnimator.SetBool("shooting", true);
        Invoke(nameof(CompleteShooting), 0.4f);

        // Apply forces for power shot
        ballerComponents.ballRigidbody.AddForce(ballerComponents.shootPower * Time.fixedDeltaTime * verticalDirection, ForceMode.Impulse);
        ballerComponents.ballRigidbody.AddForce(ballerComponents.shootPower * Time.fixedDeltaTime * powerDirection, ForceMode.Impulse);
        ballerComponents.ballRigidbody.AddForce(ballerComponents.shootPower * Time.fixedDeltaTime * horizontalDirection, ForceMode.Impulse);

        // Play shooting sound
        ballerComponents.footballSound.pitch = 1f;
        ballerComponents.footballSound.volume = 1f;
        ballerComponents.footballSound.Play();

        // Reset power shot flag
        isPowerShotPressed = false;
    }

    private void IncreaseShootPower()
    {
        if (ballerComponents.shootPower < 100)
        {
            ballerComponents.shootPower += 5;
            ballerComponents.shootSlider.value = ballerComponents.shootPower / 100f;
        }
    }

    private void ResetShootPower()
    {
        ballerComponents.shootPower = 0;
        ballerComponents.shootSlider.value = 0;
    }

    public void CompleteShooting()
    {
        ballerComponents.ballerAnimator.SetBool("shooting", false);

        if (ballerComponents.trajectoryLineRenderer.gameObject.activeInHierarchy)
        {
            ballerComponents.trajectoryLineRenderer.gameObject.SetActive(false);
        }
    }
}
