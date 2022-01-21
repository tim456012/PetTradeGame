using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int hp;
    public int hitCount;
    public int damage;
    public int agility;
    public int xp;
    public int gold;

    public void Load(string line)
    {
        string[] lines = line.Split(",");
        name = lines[0];
        hp = Convert.ToInt32(lines[1]);
        hitCount = Convert.ToInt32(lines[2]);
        damage = Convert.ToInt32(lines[3]);
        agility = Convert.ToInt32(lines[4]);
        xp = Convert.ToInt32(lines[5]);
        gold = Convert.ToInt32(lines[6]);
    }
}
