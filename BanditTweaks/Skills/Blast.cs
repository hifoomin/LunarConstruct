using EntityStates;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HBT.Skills
{
    public class Blast : TweakBase
    {
        public static float Damage;
        public static int AmmoCount;
        public static float AutofireDur;

        public override string Name => ": Primary :: Blast";

        public override string SkillToken => "primary_alt";

        public override string DescText => "Fire an automatic rifle blast for <style=cIsDamage>" + d(Damage) + " damage</style>. Can hold up to " + AmmoCount + " bullets.";

        public override void Init()
        {
            Damage = ConfigOption(2.4f, "Damage", "Decimal. Vanilla is 3.3");
            AmmoCount = ConfigOption(6, "Charges", "Vanilla is 4");
            AutofireDur = ConfigOption(0.12f, "Autofire Duration per Bullet", "");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.OnEnter += Bandit2FirePrimaryBase_OnEnter;
            On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.GetMinimumInterruptPriority += Bandit2FirePrimaryBase_GetMinimumInterruptPriority;
            Changes();
        }

        private EntityStates.InterruptPriority Bandit2FirePrimaryBase_GetMinimumInterruptPriority(On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.orig_GetMinimumInterruptPriority orig, EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase self)
        {
            if (self.fixedAge <= self.minimumDuration && self.inputBank.skill1.wasDown)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Any;
        }

        private void Bandit2FirePrimaryBase_OnEnter(On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.orig_OnEnter orig, EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase self)
        {
            if (self is EntityStates.Bandit2.Weapon.Bandit2FireRifle)
            {
                self.damageCoefficient = Damage;
                self.recoilAmplitudeX = 0.15f;
                self.recoilAmplitudeY = 0.7f;
                self.spreadYawScale = 0f;
                self.spreadPitchScale = 0f;
                self.bulletRadius = 0.4f;
                self.minimumBaseDuration = AutofireDur;
            }
            orig(self);
        }

        private void Changes()
        {
            var rifle = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/Bandit2Blast.asset").WaitForCompletion();
            rifle.baseMaxStock = AmmoCount;
            rifle.mustKeyPress = false;
            rifle.interruptPriority = InterruptPriority.Skill;
        }

        private void HaveAblastPeriphery()
        {
        }
    }
}