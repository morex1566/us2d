using UnityEngine;

public class HitBox : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private LayerMask TriggerLayer;

    [SerializeField] private float knockback = 0f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((TriggerLayer.value & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }

        HurtBox hurtBox = collision.GetComponent<HurtBox>();

        if (hurtBox == null)
        {
            hurtBox = collision.GetComponentInParent<HurtBox>();
        }

        if (hurtBox == null)
        {
            return;
        }

        if (hurtBox.OwnerGameObject == gameObject)
        {
            return;
        }

        Vector2 hitDirection = collision.transform.position - transform.position;

        if (hitDirection.sqrMagnitude > 0.0001f)
        {
            hitDirection = hitDirection.normalized;
        }
        else
        {
            hitDirection = Vector2.zero;
        }

        HitContext hitContext = new HitContext
        {
            Attacker = collision.gameObject,
            HitPoint = collision.ClosestPoint(transform.position),
            HitDirection = hitDirection,
            Damage = 0f,
            Knockback = knockback
        };

        hurtBox.ReceiveHit(hitContext);
    }
}
