using UnityEngine;

public static class ChanceCalculator
{
    public static bool GetChance(float percentage)
    {
        float randomValue = Random.Range(0f, 100f);
        return randomValue < percentage;
    }
}