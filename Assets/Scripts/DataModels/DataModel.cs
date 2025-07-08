using System.Collections.Generic;

namespace DataModels
{
    public class CardData
    {
        public int id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string grade { get; set; }
        public int value { get; set; }
        public int pct { get; set; }
        public int need_percent { get; set; }
        public string color { get; set; }
        public string border_path { get; set; }
        public string icon_path  { get; set; }
    }

    public class PlayerData
    {
        public int id { get; set; }
        public string job_type { get; set; }
        public int hp { get; set; }
        public float ult_gauge { get; set; }
        public int attack_power { get; set; }
        public float attack_speed { get; set; }
        public float move_speed { get; set; }
        public int critical_pct { get; set; }
        public int critical_dmg { get; set; }
    }
    public class EnemyData
    {
        public int id { get; set; }
        public string type { get; set; }
        public string prefab_path { get; set; }
    }
    public class WaveData
    {
        public int id { get; set; }
        public string title { get; set; }
        public string difficulty { get; set; }
        public string background { get; set; }
        public int shared_hp_id { get; set; }
    }
    public class SharedData
    {
        public int id { get; set; }
        public string prefab_path { get; set; }
    }
    public class BulletData
    {
        public int id { get; set; }
        public string prefab_path { get; set; }
    }
}