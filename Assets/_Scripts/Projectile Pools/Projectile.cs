using UnityEngine;

public class Projectile : MonoBehaviour {

    public ParticleSystem ProjectileParticles;
    public ParticleSystem ImpactParticles;
    public ProjectilePool MyPool { get; set; }

    private Rigidbody _rb;
    private Parade _targetParade = null;
    private int _damage;
    private bool _impact = false;
    private Vector3 _ballisticVelocity = new Vector3();
    private void Awake() { _rb = this.GetComponent<Rigidbody>(); }

    private void OnEnable() //reset gameObject completely
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = false;
        _impact = false;
        if(ImpactParticles!=null) ImpactParticles.Stop();
    }

    public void Init(Vector3 spawnPosition, Parade targetParade = null, int targetError = 0, bool seekStrongest = false, int damage = 10)
    {
        _damage = damage;
        transform.localPosition = spawnPosition;
        _targetParade = targetParade;
        var errorOffset = new Vector3(Random.Range(-targetError, targetError + 1), 0, Random.Range(-targetError, targetError + 1));
        if (_targetParade != null)
        {
            var target = _targetParade.transform.position;
            if (seekStrongest)
            {
                var maxHealth = 0;
                foreach (Person p in _targetParade.People)
                    if (p.Health > maxHealth) target = p.transform.position;
            }
            SetBallisticVelocity(_targetParade.transform);
            _rb.velocity = _ballisticVelocity;
        }
        else //no parade target, just randomly fire
        {
            _rb.velocity = new Vector3(Random.Range(-4, 4), Random.Range(2, 10), Random.Range(-4, 4));
        }
    }


    public void Update()
    {
        if (gameObject.activeInHierarchy && !_impact)
            if (transform.position.y < 0)
            {
                _impact = true;
                _rb.isKinematic = true;
                if(ImpactParticles!=null) ImpactParticles.Play();
                if (_targetParade != null) _targetParade.HitParade(transform, _damage);
                Invoke("ReturnToPool", 1.5f);
            }
    }

    private void ReturnToPool()
    {
        MyPool.ReturnProjectile(this);
    }


    private void SetBallisticVelocity(Transform target)
    {
        var duration = 3f;
        _ballisticVelocity = target.position - transform.position;
        _ballisticVelocity /= duration;
        _ballisticVelocity.y = .5f * Physics.gravity.magnitude * duration; //assume both y values are ground zero


        //long version:
        //var vX = (target.position.x-transform.position.x)/duration;
        //var vZ = (target.position.z-transform.position.z)/duration;
        //var vY = ( target.position.y  + (.5f* Physics.gravity.magnitude * duration * duration) - transform.position.y) / duration;
        //_ballisticVelocity.x = vX;
        //_ballisticVelocity.y = vY;
        //_ballisticVelocity.z = vZ;
    }

}
