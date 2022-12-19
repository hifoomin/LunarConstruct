using HBT;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFUBanditTweaks.Misc
{
    public class Reload : MiscBase
    {
        public static float ReloadDur;

        public override string Name => ": Primary ::: Reload";

        public override void Init()
        {
            ReloadDur = ConfigOption(0.3f, "Reload Duration", "Vanilla is ??");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Bandit2.Weapon.EnterReload.OnEnter += EnterReload_OnEnter;
            On.EntityStates.Bandit2.Weapon.Reload.OnEnter += Reload_OnEnter;
        }

        private void Reload_OnEnter(On.EntityStates.Bandit2.Weapon.Reload.orig_OnEnter orig, EntityStates.Bandit2.Weapon.Reload self)
        {
            EntityStates.Bandit2.Weapon.Reload.baseDuration = ReloadDur;
            orig(self);
        }

        private void EnterReload_OnEnter(On.EntityStates.Bandit2.Weapon.EnterReload.orig_OnEnter orig, EntityStates.Bandit2.Weapon.EnterReload self)
        {
            EntityStates.Bandit2.Weapon.EnterReload.baseDuration = ReloadDur;
            orig(self);
        }
    }
}