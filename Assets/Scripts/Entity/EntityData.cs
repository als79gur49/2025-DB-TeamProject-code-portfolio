using UnityEngine;
using UnityEngine.Events;

public class EntityData
{
    private int level;
    private float maxHp;
    private float hp;
    private int damage;
    private float defense;

    private int score;

    public int Level => level;
    public float MaxHp => maxHp;
    public float HP => hp;
    public int Score => score;

    public float HPPercent => hp / maxHp * 100;

    public EntityData(int level, int hp, int damage, float defense)
    {
        Setup(level, hp, damage,defense);
    }

    public void Setup(EntityData data)
    {
        Setup(data.level, data.HP, data.damage, data.defense);
    }

    public void Setup(int level, float hp, int damage, float defense)
    {
        this.level = level;
        this.maxHp = hp;
        this.hp = hp;
        this.damage = damage;
        this.defense = defense;

        score = 0;
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
    }
    public void AddHp(float amount)
    {
        if (hp + amount > maxHp)
        {
            hp = maxHp;
        }
        else
        {
            hp += amount;
        }
    }

    public void Levelup()
    {
        level++;
    }


    public void IncreaseMaxHp(float amount)
    {
        maxHp += amount;
        hp += amount;
    }

    public void AddScore(int amount)
    {
        score += amount;
    }
};
