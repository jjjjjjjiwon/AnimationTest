using System.Collections.Generic;

public class BossRuntime
{
    public string BossName { get; private set; }

    public float MaxHp { get; private set; }
    public float Damage { get; private set; }
    public float MoveSpeed { get; private set; }
    public float Armor { get; private set; }

    public readonly List<BossUpgrade> upgrades = new();

    public BossRuntime(BossDefinition def, List<BossUpgrade> selected)
    {
        BossName = def != null ? def.bossName : "";

        if (def != null && def.baseStats != null)
        {
            MaxHp = def.baseStats.maxHp;
            Damage = def.baseStats.damage;
            MoveSpeed = def.baseStats.moveSpeed;
            Armor = def.baseStats.armor;
        }
        else
        {
            MaxHp = 0f;
            Damage = 0f;
            MoveSpeed = 0f;
            Armor = 0f;
        }

        if (selected != null)
            upgrades.AddRange(selected);

        ApplyUpgrades();
    }

    private void ApplyUpgrades()
    {
        foreach (var up in upgrades)
        {
            if (up == null) continue;

            string stat = (up.statType ?? "").Trim().ToLower();
            float v = up.value;

            if (stat == "health" || stat == "hp" || stat == "maxhp")
                MaxHp *= (1f + v);
            else if (stat == "damage")
                Damage *= (1f + v);
            else if (stat == "speed" || stat == "movespeed")
                MoveSpeed *= (1f + v);
            else if (stat == "armor")
                Armor *= (1f + v);
        }
    }
}
