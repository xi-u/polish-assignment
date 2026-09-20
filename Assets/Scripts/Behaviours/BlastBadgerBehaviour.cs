using System.Collections;
using UnityEngine;

public class BlastBadgerBehaviour : MonoBehaviour
{
    private BlastWaveBehaviour blastWaveBehaivour;

    // Start is called before the first frame update
    void Awake()
    {
        blastWaveBehaivour = gameObject.AddComponent<BlastWaveBehaviour>();
    }

    public void Activate()
    {        
        StartCoroutine(CreateBlastWave());
    }

    private IEnumerator CreateBlastWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);

            yield return new WaitForSeconds(0.5f);

            blastWaveBehaivour.CreateBlast(5, true);
        }
    }
}
