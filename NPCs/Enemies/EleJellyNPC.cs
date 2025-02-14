﻿using Microsoft.Xna.Framework;
using RealmOne.Items.Weapons.PreHM.Throwing;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace RealmOne.NPCs.Enemies
{
    public class EleJellyNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ele-Jelly");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueJellyfish];

            var value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            { // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 20;
            NPC.damage = 30;
            NPC.lifeMax = 60;
            NPC.value = Item.buyPrice(0, 0, 1, 65);
            NPC.aiStyle = NPCAIStyleID.Jellyfish;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            AIType = NPCID.BlueJellyfish;
            AnimationType = NPCID.BlueJellyfish;
            NPC.npcSlots = 0;

        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Water ? 0.6f : 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                   BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A more reckless and more conductive jellyfish, this jellyfish does a major amount of damage compared to other jellies."),

				// By default the last added IBestiaryBackgroundImagePathAndColorProvider will be used to show the background image.
				// The ExampleSurfaceBiome ModBiomeBestiaryInfoElement is automatically populated into bestiaryEntry.Info prior to this method being called
				// so we use this line to tell the game to prioritize a specific InfoElement for sourcing the background image.
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EleJelly>(), 1, 5, 10));
            npcLoot.Add(ItemDropRule.Common(ItemID.Glowstick, 2, 3, 6));

        }

        public override void HitEffect(NPC.HitInfo hit)
        {

            for (int i = 0; i < 26; i++)
            {

                Vector2 speed = Main.rand.NextVector2Square(1f, 1f);

                var d = Dust.NewDustPerfect(NPC.position, DustID.Electric, speed * 5, Scale: 1f);
                d.noLight = false;
                d.noGravity = true;

            }
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.position, r: 0.1f, g: 0.2f, b: 1.1f);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            // Here we can make things happen if this NPC hits a player via its hitbox (not projectiles it shoots, this is handled in the projectile code usually)
            // Common use is applying buffs/debuffs:

            int buffType = BuffID.Electrified;
            // Alternatively, you can use a vanilla buff: int buffType = BuffID.Slow;

            int timeToAdd = 2 * 60; //This makes it 5 seconds, one second is 60 ticks
            target.AddBuff(buffType, timeToAdd);
        }
    }
}
