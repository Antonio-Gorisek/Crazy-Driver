using System.Collections;
using UnityEngine;

public class TalkingEffect : MonoBehaviour
{
    private void Start() => StartCoroutine(RandomTalk());

    private IEnumerator RandomTalk()
    {
        while (true)
        {
            float waitTime = Random.Range(4f, 8f);
            yield return new WaitForSeconds(waitTime);
            AudioManager.PlayFromResources("talkin", 0.5f, Random.Range(0.6f, 1.6f));
        }
    }
}
