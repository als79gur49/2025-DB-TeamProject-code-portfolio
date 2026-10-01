using System.Collections.Generic;
using UnityEngine;

public class ProjectileStorage : MonoBehaviour
{
    // 투사체 저장 및 발사한 마지막 시간 저장. (현재 - 마지막) > cooldown
    private Dictionary<Projectile, float> projectiles;
    public Dictionary<Projectile, float> Projectiles => projectiles;

    public void Setup(List<Projectile> p)
    {
        projectiles = new Dictionary<Projectile, float>();

        foreach(var projectile in p)
        {
            projectile.Initialize();
            projectiles.Add(projectile, 0);
        }
    }

    public void SetTimeCurrent(Projectile p)
    {
        if(projectiles.ContainsKey(p))
        {
            projectiles[p] = Time.time;
        }
    }

    public void AddProjectile(Projectile p)
    {
        projectiles.Add(p, Time.time);
    }
}
