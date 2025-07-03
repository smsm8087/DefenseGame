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
    }

    public class PlayerData
    {
        public int id { get; set; }
        public string job_type { get; set; }
        public int hp { get; set; }
        public float ult_gauge { get; set; }
        public int attack_power { get; set; }
        public float move_speed { get; set; }
        public int critical_pct { get; set; }
        public int critical_dmg { get; set; }
    }
    public class EnemyData
    {
        public int id { get; set; }
        public string type { get; set; }
        public string prefab { get; set; }
        public int hp { get; set; }
        public float speed { get; set; }
        public float attack { get; set; }
        public float defense { get; set; }
        public float base_width{ get; set; }
        public float base_height { get; set; }
        public float base_scale { get; set; }
        public float base_offsetx { get; set; }
        public float base_offsety { get; set; }
    }
    public class WaveData
    {
        public int id { get; set; }
        public string title { get; set; }
        public string difficulty { get; set; }
        public int max_wave { get; set; }
        public int settlement_phase_round { get; set; }
        public string background { get; set; }
        public string shared_hp { get; set; }
        public float shared_hp_radius { get; set; }
        public float shared_hp_posx { get; set; }
        public float shared_hp_posy { get; set; }
        public float spawn_left_posx { get; set; }
        public float spawn_left_posy { get; set; }
        public float spawn_right_posx { get; set; }
        public float spawn_right_posy { get; set; }
    }
}