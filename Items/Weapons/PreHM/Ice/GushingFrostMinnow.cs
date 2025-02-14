using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace RealmOne.Items.Weapons.PreHM.Ice
{
    public class GushingFrostMinnow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gushing Frost Minnow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'A rare fish of the snow that has been frozen over time, filled with icy water'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1;
            Item.value = 30000;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WaterStream;
            Item.shootSpeed = 14f;
            Item.questItem = true;
            Item.uniqueStack = true;
            Item.noMelee = true;

        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override bool IsQuestFish()
        {
            return true; // Makes the item a quest fish
        }

        public override bool IsAnglerQuestAvailable()
        {
            return !Main.hardMode; // Makes the quest only appear in hard mode. Adding a '!' before Main.hardMode makes it ONLY available in pre-hardmode.
        }

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            // How the angler describes the fish to the player.
            description = "Rumours of my old eskimo friend says there's a frigid and nimble fish swimming in the freezing waters of the tundra. Be careful, its filled with freezing water. Go fetch and don't get frostbite!";
            // What it says on the bottom of the angler's text box of how to catch the fish.
            catchLocation = "Caught in the tundra.";
        }
        public override Vector2? HoldoutOffset()
        {
            var offset = new Vector2(3, 0);
            return offset;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IceBlock, 30)
            .AddIngredient(ItemID.WaterBucket, 1)
            .AddIngredient(ItemID.SnowBlock, 30)

            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}