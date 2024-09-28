using UnityEngine;

public class SpawningBarriers : MonoBehaviour
{
    public float spawnChense = 20;

    private void Start() => SpawnRandomBarrier();

    private void SpawnRandomBarrier()
    {
        // 20% chance to spawn a barrier
        // Checks if the map section is at least 320 on the X-axis 
        // to avoid generating barriers within the area where the cutscene takes place at the start.
        if (ChanceCalculator.GetChance(spawnChense) && transform.parent.position.x >= 320)
        {
            // Get direct children including inactive ones and pick one at random
            Transform[] directChildren = System.Array.FindAll(
                GetComponentsInChildren<Transform>(true),
                child => child.parent == transform
            );

            if (directChildren.Length > 0)
            {
                GameObject obj = directChildren[Random.Range(0, directChildren.Length)].gameObject;
                obj.SetActive(true);
                Debug.Log("Spawn barrier: " + obj.name);
            }

        }
    }
}
