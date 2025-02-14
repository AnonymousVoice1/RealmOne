﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace RealmOne.Projectiles.Magic
{

    public class GobPortal : ModProjectile
    {
        private static Asset<Texture2D> GobTexture;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sorcerer's Rift");

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -2;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 280;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.penetrate = -1;
            Projectile.stepSpeed = 1f;
            Projectile.alpha = 255;

        }

        public override void AI()
        {

            Projectile.rotation += 0.12f;
            Projectile.velocity.X *= 0.0f;
            Projectile.velocity.Y *= 0.01f;
            Lighting.AddLight(Projectile.position, 1.5f, 0.7f, 2.5f);
            Lighting.Brightness(2, 2);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && !npc.boss)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance <= 200)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        npc.velocity -= direction * 0.5f;
                    }
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
            Projectile.ownerHitCheck = true;

            int radius = 230;

            // Damage enemies within the splash radius
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];
                if (target.active && !target.friendly && Vector2.Distance(Projectile.Center, target.Center) < radius)
                {
                    int damage = Projectile.damage * 2;
                    target.SimpleStrikeNPC(damage: 12, 0);
                }
            }

            for (int i = 0; i < 150; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.ShadowbeamStaff, speed * 11, Scale: 3f);
                ;
                d.noGravity = true;
            }
        }

        public override void Load()
        { // This is called once on mod (re)load when this piece of content is being loaded.
          // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
            GobTexture = Request<Texture2D>("RealmOne/Assets/Effects/GobTexture");
        }

        public override void Unload()
        { // This is called once on mod reload when this piece of content is being unloaded.
          // It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.
            GobTexture = null;
        }


        public override bool PreDraw(ref Color lightColor)
        {

            Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16));

            Main.EntitySpriteDraw(GobTexture.Value, Projectile.Center - Main.screenPosition,
                          GobTexture.Value.Bounds, Color.MediumPurple, Projectile.rotation,
                          GobTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            return true;
        }




    }
}

