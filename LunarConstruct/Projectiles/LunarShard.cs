using RoR2.Projectile;
using R2API;

namespace LunarConstruct.Projectiles
{
    public static class LunarShard
    {
        public static GameObject projectile;

        public static void Init()
        {
            projectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/MinorConstructProjectile.prefab").WaitForCompletion(), "LunarConstructLunarShard");

            var curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(2.5f, 0.5f), new Keyframe(5f, 0.25f))
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever
            };

            var projectileSimple = projectile.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 6f;
            projectileSimple.desiredForwardSpeed = 45f;
            projectileSimple.updateAfterFiring = true;
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.velocityOverLifetime = curve;
            projectileSimple.lifetimeExpiredEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/MuzzleflashLunarGolemTwinShot.prefab").WaitForCompletion();

            var projectileSingleTargetImpact = projectile.GetComponent<ProjectileSingleTargetImpact>();
            projectileSingleTargetImpact.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/OmniImpactVFXBrotherLunarShardExplosion.prefab").WaitForCompletion();

            var projectileController = projectile.GetComponent<ProjectileController>();
            projectileController.flightSoundLoop = null;

            var newGhost = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarExploder/LunarExploderShardGhost.prefab").WaitForCompletion(), "LunarConstructLunarShardGhost", false);
            var trail = newGhost.transform.GetChild(1).GetComponent<TrailRenderer>();
            trail.widthMultiplier = 1.25f;
            trail.material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/LunarSun/matLunarSunProjectileTrail.mat").WaitForCompletion();

            var sparks = newGhost.transform.GetChild(5).GetComponent<ParticleSystem>();
            var main = sparks.main.startColor;
            main.mode = ParticleSystemGradientMode.Color;
            main.color = new Color32(0, 12, 238, 255);

            projectileController.ghostPrefab = newGhost;

            PrefabAPI.RegisterNetworkPrefab(projectile);
        }
    }
}