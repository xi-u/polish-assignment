using ObjectPool;
using System.Collections;
using UnityEngine;

public class MirrageMantaBehaviour : MonoBehaviour
{
    private RoamingBehaviour roamingBehaviour;
    private float speedChangeDuration = 0.99f, moveSpeed;
    private MirrageManta mirrageManta;

    // Start is called before the first frame update
    void Start()
    {
        roamingBehaviour = GetComponent<RoamingBehaviour>();
        moveSpeed = roamingBehaviour.MovementSpeed;
    }

    public void Activate(MirrageManta mirrageManta)
    {
        this.mirrageManta = mirrageManta;
    }

    private void OnEnable()
    {
        StartCoroutine(SplitUp());
    }

    private IEnumerator SplitUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 6));

            for (float t = 0; t < speedChangeDuration; t += Time.deltaTime)
            {
                roamingBehaviour.MovementSpeed = Mathf.Lerp(moveSpeed, 0, t / speedChangeDuration);
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);

            MirrageMantaClone mirrageClone = null;

            PoolManager.Instance.GetEntity(PoolableType.MirrageMantaClone, transform.position, (Entity entity) =>
            { 
                mirrageClone = entity as MirrageMantaClone;
                mirrageManta.SetChildIds(mirrageClone.InstanceIds);
                mirrageClone.SetParentIds(mirrageManta.InstanceIds);
            });

            for (float t = 0; t < speedChangeDuration; t += Time.deltaTime)
            {
                roamingBehaviour.MovementSpeed = Mathf.Lerp(0, moveSpeed, t / speedChangeDuration);
                yield return null;
            }

            yield return new WaitForSeconds(2);
            mirrageClone.SetParentIds(EntityId.None);
            mirrageManta.SetChildIds(EntityId.None);
        }
    }
}
