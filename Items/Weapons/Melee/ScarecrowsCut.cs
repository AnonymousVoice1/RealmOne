using Microsoft.Xna.Framework;
using RealmOne.Projectiles.Sword;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace RealmOne.Items.Weapons.Melee
{
    public class ScarecrowsCut : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scarecrow's Cut"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Always sharp'"
             + "\nSwings a sharp cleaver that pierces"
             + "\n'Why did the scarecrow win an award? Because he was outstanding in his field' Im so funny'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee;
            Item.width = 120;
            Item.height = 60;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 9f;
            Item.value = 90000;
            Item.rare = ItemRarityID.Master;
            Item.UseSound = SoundID.DD2_SkyDragonsFuryShot;
            Item.autoReuse = true;

            Item.crit = 30;

            Item.shoot = ModContent.ProjectileType<ScarecrowsCutProjectile>();

        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(ItemID.GoldBar, 40);
            recipe.AddIngredient(Mod, "HellishMembrane", 15);
            recipe.AddTile(TileID.Sawmill);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            var offset = new Vector2(6, 0);
            return offset;
        }
    }
}