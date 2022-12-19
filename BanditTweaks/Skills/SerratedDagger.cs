using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HBT.Skills
{
    public class SerratedDagger : TweakBase
    {
        public static float Damage;
        public static float Cooldown;
        public static bool Lunge;

        public override string Name => ": Secondary : Serrated Dagger";

        public override string SkillToken => "secondary";

        public override string DescText => "Lunge and slash for <style=cIsDamage>" + d(Damage) + " damage</style>. Critical Strikes also cause <style=cIsHealth>hemorrhaging</style>.";

        public override void Init()
        {
            Damage = ConfigOption(6f, "Damage", "Decimal. Vanilla is 3.6");
            Cooldown = ConfigOption(7f, "Cooldown", "Vanilla is 4");
            Lunge = ConfigOption(true, "Enable lunge?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Bandit2.Weapon.SlashBlade.OnEnter += SlashBlade_OnEnter;
            Changes();
        }

        private void SlashBlade_OnEnter(On.EntityStates.Bandit2.Weapon.SlashBlade.orig_OnEnter orig, EntityStates.Bandit2.Weapon.SlashBlade self)
        {
            self.damageCoefficient = Damage;
            self.procCoefficient = 0;
            orig(self);
            if (Lunge && self.isAuthority)
            {
                Vector3 direction = self.GetAimRay().direction;
                Vector3 a = direction.normalized * 3f * self.moveSpeedStat;
                Vector3 b = Vector3.up * 2f;
                Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * 3f;
                self.characterMotor.Motor.ForceUnground();
                self.characterMotor.velocity = a + b + b2;
            }
        }

        private void Changes()
        {
            var shiv = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/SlashBlade.asset").WaitForCompletion();
            shiv.baseRechargeInterval = Cooldown;
        }
    }
}