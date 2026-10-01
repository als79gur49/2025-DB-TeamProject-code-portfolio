using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LevelupStorage : MonoBehaviour
{
    private List<ILevelup> levelupable;
    public List<ILevelup> Levelupable => levelupable;

    private SkillIconManager skillIconManager;
    private const int maxSelectedIcons = 3;

    private Entity owner;
    private Transform firePoint;

    public void Setup(SkillIconManager skillIconManager, Entity owner, Transform firePoint)
    {
        this.skillIconManager = skillIconManager;

        this.owner = owner;
        this.firePoint = firePoint;

        levelupable = new List<ILevelup>();
        levelupable = skillIconManager.GetLevelupable();
        foreach(ILevelup obj in  levelupable)
        {
            if (obj is BaseSkill skill)
            {
                skill.Setup(owner);
            }
            else if (obj is WeaponBase weapon)
            {
                weapon.Setup(owner, firePoint);
            }
        }

        skillIconManager.onSkillSelected.AddListener(Calculate);
    }

    public void Calculate(ILevelup obj)
    {
        Debug.Log("Calculate");
        if(obj is  BaseSkill skill)
        {

        }
        else if(obj is  WeaponBase weapon)
        {
            // 현재는 이전 무기의 값 유지
            owner.ChangeWeapon(weapon);
        }
    }

    public void Levelup()
    {
        if(levelupable == null || levelupable.Count == 0)
        {
            Debug.Log("레벨 업 가능한 스킬 없음");
            return;
        }

        List<ILevelup> randomSkills = new List<ILevelup>(levelupable);
        // 랜덤하게 섞기
        int count = Mathf.Min(maxSelectedIcons, randomSkills.Count);
        List<ILevelup> selected = randomSkills.GetRange(0, count);

        skillIconManager.ShowSkillIcons(selected);

        
    }

    public void AddLevelupable(ILevelup item)
    {
        // levelupable.Any(x => x.GetType() == item.GetType())

        // 참조 비교가 아닌 클래스 타입으로 비교
        if (levelupable == null || levelupable.Count <= 0)
        {
            return;
        }

        ILevelup sameTypeClassItem = levelupable.FirstOrDefault(x => x.GetType() == item.GetType());
        if(sameTypeClassItem != null)
        {
            levelupable.Remove(sameTypeClassItem);
            item.LevelUp();
            levelupable.Add(item);

        }
        else
        {
            levelupable.Add(item);
        }
    }
}
