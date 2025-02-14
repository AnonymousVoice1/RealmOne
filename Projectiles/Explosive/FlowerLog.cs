using RealmOne.Common.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RealmOne.Projectiles.Explosive
{
    public class FlowerLog : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("log");

        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 26;

            Projectile.damage = 38;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = 0;
            Projectile.penetrate = -2;
            Projectile.timeLeft = 120;
            Projectile.CloneDefaults(ProjectileID.RocketIV);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)

        {
            SoundEngine.PlaySound(rorAudio.SFX_CrossbowHit);

        }

        public override void Kill(int timeleft)
        {

            Collision.AnyCollision(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(rorAudio.SFX_CrossbowImpact);

        }
    }
}