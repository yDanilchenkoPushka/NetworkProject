using UnityEngine;

namespace Damage
{
    public class DamageZone : MonoBehaviour, IBoundable
    {
        public Bounds Bound => _boxCollider.bounds;
        
        [SerializeField]
        private BoxCollider _boxCollider;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable)) 
                damageable.TakeDamage();
        }
    }
}