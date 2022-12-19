using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HBT.Skills
{
    public class SerratedShiv : TweakBase
    {
        public static float Damage;
        public static float Cooldown;
        public static int Charges;
        public static int ChargesToRecharge;
        public static bool Sidestep;

        public override string Name => ": Secondary :: Serrated Shiv";

        public override string SkillToken => "secondary_alt";

        public override string DescText => (Sidestep ? "Sidestep and throw" : "Throw") +
                                           " a hidden blade for <style=cIsDamage>" + d(Damage) + " damage</style>. Critical Strikes also cause <style=cIsHealth>hemorrhaging</style>." +
                                           (Charges > 1 ? " Can hold up to " + Charges + " shivs." : "");

        public override void Init()
        {
            Damage = ConfigOption(2f, "Damage", "Decimal. Vanilla is 3.6");
            Cooldown = ConfigOption(12f, "Cooldown", "Vanilla is 4");
            Charges = ConfigOption(2, "Charges", "Vanilla is 1");
            ChargesToRecharge = ConfigOption(2, "Charges to Recharge", "Vanilla is 1");
            Sidestep = ConfigOption(true, "Enable sidestep?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Bandit2.Weapon.Bandit2FireShiv.OnEnter += Bandit2FireShiv_OnEnter;
            Changes();
        }

        private void Bandit2FireShiv_OnEnter(On.EntityStates.Bandit2.Weapon.Bandit2FireShiv.orig_OnEnter orig, EntityStates.Bandit2.Weapon.Bandit2FireShiv self)
        {
            self.damageCoefficient = Damage;
            orig(self);
            if (Sidestep && self.isAuthority)
            {
                Vector3 direction = (self.inputBank.moveVector == Vector3.zero ? Vector3.zero : self.inputBank.moveVector.normalized);
                Vector3 a = direction.normalized * 2.4f * self.moveSpeedStat;
                self.characterMotor.Motor.ForceUnground();
                self.characterMotor.velocity = a;
            }
        }

        private void Changes()
        {
            var shiv = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/Bandit2SerratedShivs.asset").WaitForCompletion();
            shiv.baseRechargeInterval = Cooldown;
            shiv.baseMaxStock = Charges;
            shiv.rechargeStock = ChargesToRecharge;
        }
    }
}