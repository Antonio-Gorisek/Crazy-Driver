using System.Collections;
using UnityEngine;

public class SpawningVehicles : Singleton<SpawningVehicles>
{
    public float minTimeToSpawn = 2;
    public float maxTimeToSpawn = 7;

    public void StartGenerating() => StartCoroutine(SpawnVehiclesCoroutine());

    private IEnumerator SpawnVehiclesCoroutine()
    {
        // Infinite loop to keep spawning vehicles
        while (true)
        {
            // Spawn a vehicle
            SpawnIntersectionRoad();

            // Generate a random wait time.
            float randomWaitTime = Random.Range(minTimeToSpawn, maxTimeToSpawn);

            // Wait for the randomly generated amount of time before spawning the next vehicle
            yield return new WaitForSeconds(randomWaitTime);
        }
    }

    private void SpawnIntersectionRoad()
    {
        // Load a random vehicle prefab from Resources folder (Vehicle_0 to Vehicle_20)
        GameObject vehicle = Resources.Load<GameObject>("Environment/Vehicles/Vehicle_" + Random.Range(0, 21));

        // Calculate spawn position based on the last spawn position in MapGenerator
        Vector3 spawnPosition = GetComponent<MapGenerator>().lastSpawnPosition + new Vector3(0, 1.8f, 3);

        Instantiate(vehicle, spawnPosition, Quaternion.Euler(0, -90, 0));
    }
}
