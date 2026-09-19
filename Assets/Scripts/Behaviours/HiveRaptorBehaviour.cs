using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HiveRaptorBehaviour : MonoBehaviour
{
    private HiveRaptor hiveRaptor;    
    private float speed = 6f, followDelay = 1.5f;        
    private float rotationSpeed = 5f;

    private bool isInKamikazeMode = false;
    public float kamikazeRadius = 8f;
    public float kamikazeSpeedMultiplier = 3f;
    private int layerMask;

    private Collider2D[] neighbors;
    private Rigidbody2D rigidbody;
    private Player player;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        gameObject.layer = layerMask;
    }

    public void Activate(HiveRaptor hiveRaptor)
    {
        this.hiveRaptor = hiveRaptor;

        isInKamikazeMode = false;

        speed = 5;
    }

    private void Start()
    {
        player = ServiceLocator.Instance.GetService<Player>();
    }

    private void FixedUpdate()
    {
        float distToPlayer = (player.Transform.position - transform.position).sqrMagnitude;
        if (!isInKamikazeMode && distToPlayer < kamikazeRadius * kamikazeRadius)
        {
            isInKamikazeMode = true;

            hiveRaptor.EnterKamikazeMode();
            StartCoroutine(Charge());
            StartCoroutine(DestroyAfterWait());
        }
    }

    private IEnumerator Charge()
    {
        float t = 0;
        while (true)
        {
            float speed = Mathf.Lerp(UnitStats.HiveRaptorNormalSpeed, UnitStats.HiveRaptorChaseSpeed, t);
            t += Time.deltaTime;
            hiveRaptor.SetSpeed(speed);

            yield return null;
        }
    }

    private IEnumerator DestroyAfterWait()
    {
        yield return new WaitForSeconds(1.5f);

        hiveRaptor.DestroySelf();
    }
}