using Microsoft.Xna.Framework;
using RealmOne.RealmPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RealmOne.Projectiles.Explosive
{
    public class C5Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("C4 explosive");

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {

            // while the sprite is actually bigger than 15x15, we use 15x15 since it lets the projectile clip into tiles as it bounces. It looks better.
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;

            // 4 second defuse
            Projectile.timeLeft = 1200;

            // These 2 help the projectile hitbox be centered on the projectile sprite.
            DrawOffsetX = 5;
            DrawOriginOffsetY = 5;
            Projectile.ownerHitCheck = false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Die immediately if ai[1] isn't 0 (We set this to 1 for the 5 extra explosives we spawn in Kill)
            if (Projectile.ai[1] != 0)
                return true;

            return false;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 1f, Projectile.velocity.Y * 1f);//spawns dust behind it, this is a spectral light blue dust
            if (Main.mouseRight && Main.myPlayer == Projectile.owner)
                Projectile.ai[1] = 7201;
            if (Projectile.localAI[0] >= 10f)
            {
                Projectile.localAI[0] = 0f;
                int num416 = 0;
                int num419 = Projectile.type;
                for (int num420 = 0; num420 < 1000; num420++)
                {
                    if (Main.projectile[num420].active && Main.projectile[num420].owner == Projectile.owner && Main.projectile[num420].type == num419)
                        num416++;

                    if (num416 > 5)
                    {
                        Projectile.netUpdate = true;
                        Projectile.Kill();
                        return;
                    }
                }
            }

            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = true;
                // Set to transparant. This projectile technically lives as  transparant for about 3 frames
                Projectile.alpha = 255;
                // change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
                Projectile.position.X = Projectile.position.X + Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y + Projectile.height / 2;
                Projectile.width = 250;
                Projectile.height = 250;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.damage = 45;
                Projectile.knockBack = 4f;
                Projectile.penetrate = 2;
                Projectile.ownerHitCheck = false;
            }
            else
                // Smoke and fuse dust spawn.
                if (Main.rand.NextBool(2))
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
                Main.dust[dustIndex].scale = 0.1f + Main.rand.Next(5) * 0.1f;
                Main.dust[dustIndex].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2)).RotatedBy(Projectile.rotation, default) * 1.1f;
                dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 0.5f);
                Main.dust[dustIndex].scale = 1f + Main.rand.Next(5) * 0.1f;
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2 - 6)).RotatedBy(Projectile.rotation, default) * 1.1f;
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 10f;
                // Roll speed dampening.
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.9f;
                    //if (projectile.type == 29 || projectile.type == 470 || projectile.type == 637)
                    {
                        Projectile.velocity.X = Projectile.velocity.X * 0.7f;
                    }

                    if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.1)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }

                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            // Rotation increased by velocity.X
            Projectile.rotation += Projectile.velocity.X * 0.09f;
            return;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<Screenshake>().SmallScreenshake = true;
            Projectile.penetrate = 2;

            // These gores work by simply existing as a texture inside any folder which path contains "Gores/"
            int RumGore1 = Mod.Find<ModGore>("SawGore1").Type;
            int RumGore2 = Mod.Find<ModGore>("SawGore2").Type;
            int RumGore3 = Mod.Find<ModGore>("SawGore3").Type;

            Terraria.DataStructures.IEntitySource entitySource = Projectile.GetSource_Death();

            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), RumGore1);
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), RumGore2);
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), RumGore3);

            }

            for (float i = 0; i <= 3f; i += Main.rand.NextFloat(0.5f, 2))
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, i.ToRotationVector2() * Main.rand.NextFloat(), ProjectileID.ClusterFragmentsI, (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);

            // If we are the original projectile, spawn the 5 child projectiles
            if (Projectile.ai[1] == 0)
                for (int i = 0; i < 5; i++)
                    // Random upward vector.
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
            // Smoke Dust spawn
            for (int i = 0; i < 50; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            // Fire Dust spawn
            for (int i = 0; i < 80; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 1f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            // Large Smoke Gore spawn

            // reset size to normal width and height.
            Projectile.position.X = Projectile.position.X + Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y + Projectile.height / 2;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;

            // TODO, tmodloader helper method
            {
                int explosionRadius = 150;
                //if (projectile.type == 29 || projectile.type == 470 || projectile.type == 637)
                //{
                //  explosionRadius = 15;
                //}
                int minTileX = (int)(Projectile.position.X / 16f - explosionRadius);
                int maxTileX = (int)(Projectile.position.X / 16f + explosionRadius);
                int minTileY = (int)(Projectile.position.Y / 16f - explosionRadius);
                int maxTileY = (int)(Projectile.position.Y / 16f + explosionRadius);
                if (minTileX < 0)
                    minTileX = 0;
                if (maxTileX > Main.maxTilesX)
                    maxTileX = Main.maxTilesX;
                if (minTileY < 0)
                    minTileY = 0;
                if (maxTileY > Main.maxTilesY)
                    maxTileY = Main.maxTilesY;
                bool canKillWalls = false;
                for (int x = minTileX; x <= maxTileX; x++)
                    for (int y = minTileY; y <= maxTileY; y++)
                    {
                        float diffX = Math.Abs(x - Projectile.position.X / 16f);
                        float diffY = Math.Abs(y - Projectile.position.Y / 16f);
                        double distance = Math.Sqrt((double)(diffX * diffX + diffY * diffY));
                        if (distance < explosionRadius && Main.tile[x, y] != null && Main.tile[x, y].WallType == 0)
                        {
                            canKillWalls = true;
                            break;
                        }
                    }
            }
        }
    }
}

