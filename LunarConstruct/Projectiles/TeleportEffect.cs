using R2API;

namespace LunarConstruct.Projectiles
{
    public static class TeleportEffect
    {
        public static GameObject prefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Parent/ParentTeleportEffect.prefab").WaitForCompletion(), "LunarConstructTeleport", false);
            var particles = prefab.transform.GetChild(0);
            var ringParticle = particles.GetChild(0).GetComponent<ParticleSystemRenderer>();

            var moonRamp = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampLunarWispFire.png").WaitForCompletion();

            var newRing = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Parent/matParentTeleportPortal.mat").WaitForCompletion());
            newRing.SetTexture("_RemapTex", moonRamp);

            ringParticle.sharedMaterial = newRing;

            particles.GetChild(1).gameObject.SetActive(false);

            var energyInitialParticle = particles.GetChild(3).GetComponent<ParticleSystemRenderer>();
            energyInitialParticle.sharedMaterial = newRing;
            energyInitialParticle.gameObject.transform.localScale = Vector3.one * 0.25f;

            var eps = particles.GetChild(3).GetComponent<ParticleSystem>().main;
            eps.duration = 0.17f;

            particles.GetChild(4).gameObject.SetActive(false);

            ContentAddition.AddEffect(prefab);
        }
    }
}