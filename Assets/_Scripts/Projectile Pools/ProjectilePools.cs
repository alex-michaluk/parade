using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectilePoolName { Standard = 0, Poison = 1, Magic = 2, Techie = 3 }

public class ProjectilePools : MonoBehaviour{


    [Header("Standard = 0, Fire = 1, Magic = 2, Tech = 3 ")]
    [SerializeField]
    private ProjectilePool[] _projectilePools;
    public static ProjectilePools instance;

    private void Awake()
    {
        instance = this;
        var numberOfPools = System.Enum.GetNames(typeof(ProjectilePoolName)).Length;
        _projectilePools = new ProjectilePool[numberOfPools];
        for (var i = 0; i < numberOfPools; i++)
        {
            var go = new GameObject("Projectile Pool " + i);
            go.transform.SetParent(this.transform);
            _projectilePools[i] = go.AddComponent<ProjectilePool>();
            _projectilePools[i].Init(i,15);
        }
    }
    public Projectile GetProjectile(ProjectilePoolName poolName)
    {
        return _projectilePools[(int)poolName].GetProjectile();
    }

}
