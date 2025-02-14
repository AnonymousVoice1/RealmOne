﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealmOne.Common.Systems;
using RealmOne.Projectiles.Explosive;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RealmOne.Projectiles.HeldProj
{
    public class BotanicLogLauncherH : ModProjectile
    {
        private float AimResponsiveness = 0.7f;
        private bool timerUp = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;//number of frames the animation has
        }
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.knockBack = 4;
            Projectile.ownerHitCheck = true;//so it cant attack through walls
            Projectile.scale = 1.2f;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        private bool recoilFX;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 90f)
            {
                Projectile.ai[0] = -1f;
                timerUp = true;
                Projectile.netUpdate = true;
            }

            if (timerUp == true)
                Projectile.ai[0] -= 1;

            if (player.channel == false)
            {
                Projectile.Kill();
            }

            //calling shootbullets and recoil
            if (Projectile.ai[0] == 80)
                ShootBullets();
            if (Projectile.ai[0] == 89)
                ShootBullets();
            if (Projectile.ai[0] > 79 && Projectile.ai[0] < 86)
                recoilFX = true;
            else
                recoilFX = false;

            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            bool stillInUse = player.channel && !player.noItems && !player.CCed;
            if (Projectile.owner == Main.myPlayer)
            {
                UpdatePlayerVisuals(player, rrp);

                UpdateAim(rrp, player.HeldItem.shootSpeed);

            }
            else if (!stillInUse)
                Projectile.timeLeft = 2;

        }
        private void UpdatePlayerVisuals(Player player, Vector2 playerhandpos)
        {
            Projectile.Center = playerhandpos;
            Projectile.spriteDirection = Projectile.direction;

            // Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 12;
            player.itemAnimation = 12;

            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

        }
        private void UpdateAim(Vector2 source, float speed)
        {
            Player player = Main.player[Projectile.owner];
            var aim = Vector2.Normalize(Main.MouseWorld - source);
            if (aim.HasNaNs())
                aim = -Vector2.UnitY;
            Vector2 DirAndVel = new(Projectile.velocity.X * player.direction, Projectile.velocity.Y * player.direction);
            Projectile.rotation = DirAndVel.ToRotation();
            //lerp = to, from, speed to go
            aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, AimResponsiveness));
            aim *= speed;

            if (aim != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aim;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // get texture
            var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            var sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            // (0,0) is upper-left corner

            //recoil
            float offsetX = 30f;
            if (recoilFX == true)
            {
                Main.instance.LoadProjectile(Projectile.type);
                var drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
                for (int k = 0; k < Projectile.oldPos.Length; k++) // old pos
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }

                offsetX -= 8;
                if (offsetX == 32)
                    offsetX += 4;

            }

            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;

            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            //return false to not draw original texture
            return false;
        }

        private void ShootBullets()
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(rorAudio.blunderbussShot);
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Rocket), Projectile.Center, Projectile.velocity * 1f, ModContent.ProjectileType<FlowerLogProj>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            //screenshake
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 0, default, 2f);
            }
        }
    }
}