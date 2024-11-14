using EntityStates;
using LunarConstruct.Projectiles;
using RoR2.Projectile;

namespace LunarConstruct.States
{
    public class FireShardsState : BaseState
    {
        [SerializeField]
        public GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/MuzzleflashLunarNeedle.prefab").WaitForCompletion();

        [SerializeField]
        public GameObject projectilePrefab = LunarShard.projectile;

        [SerializeField]
        public float damageCoefficient = 1.5f;

        [SerializeField]
        public float force = 1800f;

        [SerializeField]
        public float minSpread = 4f;

        [SerializeField]
        public float maxSpread = 10f;

        [SerializeField]
        public float baseDuration = 1f / 60f;

        [SerializeField]
        public float recoilAmplitude = 1f;

        [SerializeField]
        public string attackSoundString = "Play_minorConstruct_attack_shoot";

        [SerializeField]
        public string targetMuzzle = "Muzzle";

        [SerializeField]
        public float bloom = 1f;

        [SerializeField]
        public string animationLayerName = "Weapon";

        [SerializeField]
        public string animationStateName = "FireConstructBeam";

        protected float stopwatch;

        protected float duration;

        protected bool firedProjectile;

        public override void OnEnter()
        {
            base.OnEnter();
            stopwatch = 0f;
            duration = baseDuration / attackSpeedStat;
            if (characterBody)
            {
                characterBody.SetAimTimer(2f);
            }
            PlayAnimation(animationLayerName, animationStateName);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void PlayAnimation(string layerName, string animationStateName)
        {
            Animator modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                PlayAnimationOnAnimator(modelAnimator, layerName, animationStateName);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (!firedProjectile)
            {
                firedProjectile = true;
                FireProjectile();
                DoFireEffects();
            }
            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public void FireProjectile()
        {
            if (isAuthority)
            {
                Ray ray = GetAimRay();
                ray.direction = Util.ApplySpread(ray.direction, minSpread, maxSpread, 1f, 1f);
                ProjectileManager.instance.FireProjectile(projectilePrefab, ray.origin, Util.QuaternionSafeLookRotation(ray.direction), gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }

        public void DoFireEffects()
        {
            Util.PlaySound(attackSoundString, gameObject);
            AddRecoil(-2f * recoilAmplitude, -3f * recoilAmplitude, -1f * recoilAmplitude, 1f * recoilAmplitude);
            if (effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, targetMuzzle, false);
            }
            characterBody.AddSpreadBloom(bloom);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}