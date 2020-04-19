using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {

    [SerializeField]
    private Projectile[] _prefabs;
    [SerializeField]
    private int _amountToPool;

    private CircleQueue<Projectile> _q;

    public static ProjectilePool instance;

    public void Init(int poolNumber,int amountToPool)
    {
        instance = this;
        _amountToPool = amountToPool;
        _q = new CircleQueue<Projectile>(_amountToPool);

        _prefabs = new Projectile[Util.Projectiles[poolNumber].Length];
        for (var i = 0; i < _prefabs.Length; i++) _prefabs[i] = Util.Projectiles[poolNumber][i].GetComponent<Projectile>();
        
        for (var i = 0; i < _amountToPool; i++)
        {
            var projectile = Instantiate(_prefabs[i % _prefabs.Length]);
            projectile.gameObject.SetActive(false);
            projectile.name = i.ToString();
            projectile.transform.SetParent(this.transform,false);
            projectile.MyPool = this;
            _q.Queue(projectile);
        }
    }

    public void ReturnProjectile(Projectile projectile)
    {
        if (projectile.gameObject.activeInHierarchy)
        {
            projectile.gameObject.SetActive(false);
            _q.Queue(projectile);
        }
    }

    public Projectile GetProjectile()
    {
        var projectile = _q.Dequeue();
        if (projectile)
        {
            projectile.gameObject.SetActive(true);
            return projectile;
        }
        else return null;
    }

}
