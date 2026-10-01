public class KillLog
{
    public string Self {  get; set; }
    public string Weapon {  get; set; }
    public string Enemy {  get; set; }

    public KillLog(string self, string weapon, string enemy)
    {
        Self = self;
        Weapon = weapon;
        Enemy = enemy;
    }
}
