using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEngine.UI.GridLayoutGroup;

public class SkillIconManager : MonoBehaviour
{
    // ILeveup인터페이스, 추상 클래스 직렬화가 안 되어서 수동으로 합치기.
    [SerializeField]
    private WeaponBase[] weapons;
    [SerializeField]
    private BaseSkill[] skills;

    [SerializeField]
    private GameObject iconPanel;
    [SerializeField] // 최대 크기 5
    private IconData[] icons;
    [SerializeField]
    private IconData iconPrefab;

    [SerializeField]
    private Sprite defaultIconSprite;
    private string defaultName = "존재하지 않음.";
    private string defaultDescription = "세부 내용 없음.";

    public UnityEvent<ILevelup> onSkillSelected;

    public List<ILevelup> GetLevelupable()
    {
        // 깊은 복사
        List<ILevelup> lists = new List<ILevelup>();
        foreach(var weapon in weapons)
        {
            lists.Add(Instantiate(weapon));
        }
        foreach(var skill in skills)
        {
            lists.Add(Instantiate(skill));
        }

        return lists;
    }

    public void ShowSkillIcons(List<ILevelup> list)
    {
        List<IconData> iconLists = new List<IconData>();

        for(int i = 0; i<list.Count; ++i)
        {
            IconData clone = Instantiate(iconPrefab);
            clone.transform.SetParent(iconPanel.transform);
            clone.Setup(defaultIconSprite, defaultName, defaultDescription);

            if (list[i] is BaseSkill skill)
            {
                clone.Setup(skill.Data.sprite, skill.Data.skillName, skill.Data.description);
            }
            else if (list[i] is WeaponBase weapon)
            {
                clone.Setup(weapon.Data.sprite, weapon.Data.weaponName, defaultDescription);
            }


            // i 캡쳐해서 사용
            int capturedIndex = i;
            clone.SelectButton.onClick.AddListener(() =>
            {
                // 레벨업 기능
                list[capturedIndex].LevelUp();

                onSkillSelected?.Invoke(list[capturedIndex]);

                // 모든 아이콘 삭제
                foreach (var icon in iconLists)
                {
                    Destroy(icon.gameObject);
                }
            });

            iconLists.Add(clone);
        }
    }
}
