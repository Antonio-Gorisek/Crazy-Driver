using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void Restart() => GameManager.Instance.RestartGame();
    public void PlaySlapSound() => AudioManager.PlayFromResources("Slap", 1, 1);
}
