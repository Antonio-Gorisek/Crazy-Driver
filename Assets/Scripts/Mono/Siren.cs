using UnityEngine;

/// <summary>
/// Simulates a siren effect by alternating the intensity of two lights.
/// </summary>
public class Siren : MonoBehaviour
{
    // Reference to the red light component
    [SerializeField] private Light redLight;

    // Reference to the blue light component
    [SerializeField] private Light blueLight;

    // Speed at which the light intensity changes
    [SerializeField] private float lightChangeSpeed = 5f;

    // Maximum intensity that the lights can reach
    [SerializeField] private float maxIntensity = 1.3f;

    // Minimum intensity that the lights can reach
    [SerializeField] private float minIntensity = 0.55f;

    // Flag to track if the red light intensity is increasing
    private bool redLightIncreasing = true;

    // Flag to track if the blue light intensity is increasing
    private bool blueLightIncreasing = false;

    /// <summary>
    /// Called every frame to update the intensity of both lights.
    /// </summary>
    private void Update()
    {
        // Update the intensity of the red and blue lights
        UpdateLightIntensity(redLight, ref redLightIncreasing);
        UpdateLightIntensity(blueLight, ref blueLightIncreasing);

        // Toggle between red and blue lights based on their intensity
        if (redLight.range >= maxIntensity)
        {
            redLightIncreasing = false;
            blueLightIncreasing = true;
        }
        else if (blueLight.range >= maxIntensity)
        {
            blueLightIncreasing = false;
            redLightIncreasing = true;
        }
    }

    /// <summary>
    /// Updates the intensity of the specified light based on the increasing flag.
    /// </summary>
    /// <param name="light">The light to update.</param>
    /// <param name="increasing">Whether the light's intensity is increasing or decreasing.</param>
    private void UpdateLightIntensity(Light light, ref bool increasing)
    {
        if (increasing)
        {
            // Increase light intensity
            light.range += lightChangeSpeed * Time.deltaTime;
            if (light.range > maxIntensity)
            {
                light.range = maxIntensity; // Cap at maximum intensity
            }
        }
        else
        {
            // Decrease light intensity
            light.range -= lightChangeSpeed * Time.deltaTime;
            if (light.range < minIntensity)
            {
                light.range = minIntensity; // Cap at minimum intensity
            }
        }
    }
}
