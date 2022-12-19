using EntityStates;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HBT.Skills
{
    public class Burst : TweakBase
    {
        public static int PelletCount;
        public static float Damage;
        public static float ProcCoefficient;
        public static int AmmoCount;
        public static float AutofireDur;

        public override string Name => ": Primary : Burst";

        public override string SkillToken => "primary";

        public override string DescText => "Fire an automatic shotgun burst for <style=cIsDamage>" + PelletCount + "x" + d(Damage) + " damage</style>. Can hold up to " + AmmoCount + " shells.";

        public override void Init()
        {
            PelletCount = ConfigOption(5, "Pellet Count", "Vanilla is 5");
            Damage = ConfigOption(0.7f, "Damage", "Decimal. Vanilla is 1");
            ProcCoefficient = ConfigOption(0.6f, "Proc Coefficient", "Vanilla is 0.5");
            AmmoCount = ConfigOption(4, "Charges", "Vanilla is 4");
            AutofireDur = ConfigOption(0.18f, "Autofire Duration per Bullet", "");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.OnEnter += Bandit2FirePrimaryBase_OnEnter;
            Changes();
        }

        private void Bandit2FirePrimaryBase_OnEnter(On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.orig_OnEnter orig, EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase self)
        {
            if (self is EntityStates.Bandit2.Weapon.FireShotgun2)
            {
                self.bulletCount = PelletCount;
                self.damageCoefficient = Damage;
                self.procCoefficient = ProcCoefficient;
                self.bulletRadius = 0.3f;
                self.minimumBaseDuration = AutofireDur;
            }
            orig(self);
        }

        private void Changes()
        {
            var shotgun = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/FireShotgun2.asset").WaitForCompletion();
            shotgun.baseMaxStock = AmmoCount;
            shotgun.mustKeyPress = false;
            shotgun.interruptPriority = InterruptPriority.Skill;
        }
    }
}