using UnityEngine;

public class GrassAnimationController : MonoBehaviour
{
    // Reference to the Animator component
    private Animator grassAnimator;

    // Speed multiplier for the animation
    public float animationSpeed = 0.5f;  // Default is slower, set to 0.5f (half-speed)

    void Start()
    {
        // Get the Animator component attached to the GameObject
        grassAnimator = GetComponent<Animator>();

        // Check if the Animator is found
        if (grassAnimator != null)
        {
            // Set the animation speed to the desired value
            grassAnimator.speed = animationSpeed;
        }
        else
        {
            Debug.LogError("Animator component not found on the object!");
        }
    }

    void Update()
    {
        // You can dynamically change the animation speed through code if needed
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Increase speed when the up arrow is pressed
            grassAnimator.speed += 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Decrease speed when the down arrow is pressed
            grassAnimator.speed -= 0.1f;
        }
    }
}