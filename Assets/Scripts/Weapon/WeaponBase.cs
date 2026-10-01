using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, ILevelup
{
    protected Entity owner;
    protected Transform firePoint;

    protected SOWeapon data;
    public SOWeapon Data => data;
    [SerializeField]
    private SOWeapon origin_data;

    public virtual void Shot(ProjectileStorage storage)
    {
        // List로 복사본 생성 - 복사본 생성하지 않고 원본 수정 시 foreach 순회 중 건드려서 에러 발생
        foreach(var projectile in storage.Projectiles.ToList())
        {
            if((Time.time - projectile.Value) * 1000 < projectile.Key.Data.attackRate)
            {
                continue;
            }

            Projectile clone = Instantiate(projectile.Key, firePoint.position, firePoint.rotation);

            float levelupAmount = Mathf.Log(data.level + 1); // log2 =1,
            //if(owner is Player p)
            //{ //테스트 영상용 플레이어 임시 강화
            //    levelupAmount *= 5;
            //}
            clone.Setup(owner.gameObject, 
                Mathf.Round(data.damageMultiplier * levelupAmount), 
                data.speedMultiplier * levelupAmount, 
                data.rangeMultiplier * levelupAmount,
                data.durationMultiplier * levelupAmount,
                data.attackRateMultiplier * levelupAmount,   
                data.sizeMultiplier * levelupAmount);

            // clone은 사본이기에 원본 projectile 넘기기
            storage.SetTimeCurrent(projectile.Key);
        }
    }

    public virtual void Setup(Entity owner, Transform firePoint, SOWeapon data = null)
    {
        this.owner = owner;
        this.firePoint = firePoint;

        this.data = (data == null) ? Instantiate(origin_data) : Instantiate(data); 
    }

    public void LevelUp()
    {
        Debug.Log($"{data.level} Prev WeaponLevel");
        data.level += 1;
        Debug.Log($"{data.level} After WeaponLevel");
    }
}
