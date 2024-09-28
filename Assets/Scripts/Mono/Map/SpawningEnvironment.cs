using UnityEngine;

public class SpawningEnvironment : MonoBehaviour
{
    [SerializeField] private Transform _leftSidePosition;
    [SerializeField] private Transform _rightSidePosition;

    private GameObject[] _buildings;

    private void Start()
    {
        // Load all buildings from the Environment/Buildings folder
        _buildings = Resources.LoadAll<GameObject>("Environment/Buildings");

        // Check if there are any buildings loaded
        if (_buildings.Length > 0)
        {
            InstantiateBuilding(_leftSidePosition);
            InstantiateBuilding(_rightSidePosition);
        }
        else
        {
            Debug.LogWarning("No available buildings in the Environment/Buildings folder.");
        }
    }

    private void InstantiateBuilding(Transform position)
    {
        // Select a random building from the loaded objects
        GameObject randomBuilding = _buildings[Random.Range(0, _buildings.Length)];

        Instantiate(randomBuilding, position.position, Quaternion.identity, position);
    }
}
