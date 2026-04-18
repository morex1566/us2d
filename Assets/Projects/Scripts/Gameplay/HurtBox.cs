using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Header("Internal Dependencies")]
    [SerializeField, ReadOnly] private Creature ownerCreature = null;

    [SerializeField, ReadOnly] private MonoBehaviour ownerBehaviour = null;


    private IDamageable owner = null;


    public GameObject OwnerGameObject => ownerBehaviour != null ? ownerBehaviour.gameObject : null;


    private void Awake()
    {
        ownerCreature = GetComponentInParent<Creature>();
        owner = GetComponentInParent<IDamageable>();
        ownerBehaviour = owner as MonoBehaviour;
    }

    public void ReceiveHit(HitContext hitContext)
    {
        if (owner == null)
        {
            return;
        }

        owner.ApplyHit(hitContext);
    }
}
