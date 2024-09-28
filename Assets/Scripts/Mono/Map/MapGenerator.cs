using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    private int MinStraightRoadsBeforeIntersection = 2;
    private float MaxDistanceBehindPlayer = 1000f;  // Distance after which road pieces behind the player will be destroyed
    private float MinDistanceAheadPlayer = 1000f;  // Minimum distance ahead of the player for generating new road pieces

    private List<GameObject> spawnedRoads = new List<GameObject>(); // List to track spawned road pieces
    private List<string> roadHistory = new List<string>(); // List to track road generation history
    private Transform _player; // Reference to the player’s transform

    [HideInInspector] public Vector3 lastSpawnPosition; // Tracks the position of the last generated road piece

    
    public void StartMapGenerate()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        lastSpawnPosition = transform.position;
        // Initial map generation of 20 road pieces
        for (int i = 0; i < 20; i++)
        {
            Generate();
        }

        // Start regularly generating new road pieces every second
        InvokeRepeating(nameof(Generate), 0.2f, 0.2f);
    }

    // <summary>
    // Called every frame to check and remove road pieces that are far behind the player.
    // </summary>
    private void Update()
    {
        RemoveRoadsBehindPlayer();
    }

    // <summary>
    // Generates a new road piece if the player is approaching the end of the current generated map.
    // </summary>
    private void Generate()
    {
        // Check if the last spawned piece is far enough ahead of the player
        if (Vector3.Distance(lastSpawnPosition, _player.position) < MinDistanceAheadPlayer)
        {
            if (roadHistory.Count >= MinStraightRoadsBeforeIntersection && ChanceCalculator.GetChance(30))
            {
                SpawnIntersectionRoad();
                lastSpawnPosition += new Vector3(20, 0, 0); // Move spawn position by 20 units
            }
            else
            {
                SpawnLineRoad();
                lastSpawnPosition += new Vector3(40, 0, 0); // Move spawn position by 40 units
            }
        }
    }

    // <summary>
    // Spawns a straight road and adds it to the list of spawned road pieces.
    // Also tracks the road history for determining when to spawn intersections.
    // </summary>
    private void SpawnLineRoad()
    {
        GameObject road = Instantiate(Resources.Load<GameObject>("Road/Line"), lastSpawnPosition, Quaternion.identity);
        spawnedRoads.Add(road);  // Add the newly spawned road to the list
        roadHistory.Add("Line");

        // Maintain the road history by removing old entries if needed
        if (roadHistory.Count > MinStraightRoadsBeforeIntersection)
        {
            roadHistory.RemoveAt(0);
        }
    }

    // <summary>
    // Spawns an intersection and adds it to the list of spawned road pieces.
    // Clears the road history after spawning an intersection.
    // </summary>
    private void SpawnIntersectionRoad()
    {
        GameObject intersection = Instantiate(Resources.Load<GameObject>("Road/Intersection"), lastSpawnPosition, Quaternion.identity);
        spawnedRoads.Add(intersection);  // Add the newly spawned intersection to the list
        roadHistory.Clear();  // Clear the road history after spawning an intersection
    }

    // <summary>
    // Removes any road pieces that are more than the set distance behind the player.
    // This is called in the Update method to ensure old road pieces are removed.
    // </summary>
    private void RemoveRoadsBehindPlayer()
    {
        // Loop through the spawned roads and remove those too far behind the player
        for (int i = spawnedRoads.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(spawnedRoads[i].transform.position, _player.position) > MaxDistanceBehindPlayer)
            {
                Destroy(spawnedRoads[i]);  // Destroy the road piece
                spawnedRoads.RemoveAt(i);  // Remove it from the list
            }
        }
    }
}
