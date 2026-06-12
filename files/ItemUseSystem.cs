// ItemUseSystem.cs — ports UseSelectedItem + Use914 from Main.bb

using System;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class ItemUseSystem
    {
        private static readonly Random Rng = new();

        public static Item WearItem;
        public static bool IsWearingInProgress => WearItem != null;

        public static void UpdateWearProgress()
        {
            if (WearItem?.Template == null) return;

            string temp = WearItem.Template.TempName;
            float rate = temp is "hazmatsuit" or "hazmatsuit2" or "hazmatsuit3"
                ? GameState.FpsFactor / 4f
                : GameState.FpsFactor / (2f + (temp == "finevest" ? 0.5f : 0f));

            WearItem.State = Math.Min(WearItem.State + rate, 100f);
            if (WearItem.State < 100f) return;

            var item = WearItem;
            WearItem = null;
            item.State = 0f;

            if (temp is "hazmatsuit" or "hazmatsuit2" or "hazmatsuit3")
                CompleteHazmatWear(item);
            else if (temp is "vest" or "finevest")
                CompleteVestWear(item);
        }

        public static bool UseInventorySlot(int slot)
        {
            if (slot < 0 || slot >= ItemSystem.MaxItemAmount) return false;
            var item = ItemSystem.Inventory[slot];
            if (item == null) return false;
            ItemSystem.SelectedItem = item;
            ItemSystem.InvSelect = slot;
            return UseSelectedItem();
        }

        public static bool UseSelectedItem()
        {
            var item = ItemSystem.SelectedItem;
            if (item?.Template == null) return false;

            string temp = item.Template.TempName;
            string name = item.Template.Name;

            switch (temp)
            {
                case "gasmask":
                case "supergasmask":
                case "gasmask3":
                    return ToggleGasMask(item);
                case "vest":
                case "finevest":
                    return ToggleVest(item);
                case "hazmatsuit":
                case "hazmatsuit2":
                case "hazmatsuit3":
                    return ToggleHazmat(item);
                case "nvgoggles":
                    return ToggleNightVision(item, 1);
                case "supernv":
                    return ToggleNightVision(item, 2);
                case "finenvgoggles":
                    return ToggleNightVision(item, 3);
                case "scp714":
                    return Toggle714(item);
                case "scp500":
                    return UseScp500(item);
                case "veryfinefirstaid":
                    return UseVeryFineFirstAid(item);
                case "firstaid":
                case "finefirstaid":
                case "firstaid2":
                    return UseFirstAid(item);
                case "eyedrops":
                case "fineeyedrops":
                case "supereyedrops":
                    return UseEyedrops(item, temp);
                case "paper":
                case "oldpaper":
                case "ticket":
                    GameState.Msg = "You read the document.";
                    GameState.MsgTimer = 70f * 5f;
                    ItemSystem.SelectedItem = null;
                    return true;
                case "420":
                case "cigarette":
                    return Use420(item);
                case "420s":
                    return Use420s(item);
                case "key1":
                case "key2":
                case "key3":
                case "key4":
                case "key5":
                case "key6":
                case "scp860":
                case "hand":
                case "hand2":
                case "25ct":
                    GameState.Msg = "Select a door and press A to use this item.";
                    GameState.MsgTimer = 70f * 4f;
                    return true;
                default:
                    GameState.Msg = "You can't use that item here.";
                    GameState.MsgTimer = 70f * 3f;
                    return false;
            }
        }

        public static void Use914(Item item, string setting, float x, float y, float z)
        {
            if (item?.Template == null) return;

            GameState.RefinedItems++;
            string name = item.Template.Name;
            setting = Normalize914Setting(setting);

            switch (name)
            {
                case "Gas Mask":
                case "Heavy Gas Mask":
                    RefineGasMask(item, setting, x, y, z);
                    break;
                case "SCP-1499":
                    RefineScp1499(item, setting, x, y, z);
                    break;
                case "Ballistic Vest":
                    RefineVest(item, setting, x, y, z);
                    break;
                case "First Aid Kit":
                case "Blue First Aid Kit":
                    RefineFirstAid(item, setting, x, y, z);
                    break;
                case "Level 1 Key Card":
                case "Level 2 Key Card":
                case "Level 3 Key Card":
                case "Level 4 Key Card":
                case "Level 5 Key Card":
                case "Key Card":
                    RefineKeyCard(item, setting, x, y, z);
                    break;
                case "Key Card Omni":
                    RefineKeyOmni(item, setting, x, y, z);
                    break;
                case "Playing Card":
                case "Coin":
                case "Quarter":
                    RefineToKey1(item, setting, x, y, z);
                    break;
                case "Mastercard":
                    RefineMastercard(item, setting, x, y, z);
                    break;
                case "9V Battery":
                case "18V Battery":
                case "Strange Battery":
                    RefineBattery(item, setting, x, y, z);
                    break;
                case "Metal Panel":
                case "SCP-148 Ingot":
                    RefineScp148(item, setting, x, y, z);
                    break;
                case "Severed Hand":
                case "Black Severed Hand":
                    RefineHand(item, setting, x, y, z);
                    break;
                case "Night Vision Goggles":
                    RefineNvg(item, setting, x, y, z);
                    break;
                case "Hazmat Suit":
                    RefineHazmat(item, setting, x, y, z);
                    break;
                case "Some SCP-420-J":
                case "Cigarette":
                    Refine420(item, setting, x, y, z);
                    break;
                case "S-NAV 300 Navigator":
                case "S-NAV 310 Navigator":
                case "S-NAV Navigator":
                case "S-NAV Navigator Ultimate":
                    RefineNav(item, setting, x, y, z);
                    break;
                case "Radio Transceiver":
                    RefineRadio(item, setting, x, y, z);
                    break;
                case "SCP-513":
                    RefineScp513(item, setting, x, y, z);
                    break;
                default:
                    RefineDefault(item, setting, x, y, z);
                    break;
            }
        }

        private static string Normalize914Setting(string setting)
        {
            if (string.IsNullOrWhiteSpace(setting)) return "1:1";
            setting = setting.Trim().ToLowerInvariant();
            if (setting == "veryfine" || setting == "very_fine") return "very fine";
            return setting;
        }

        private static void RefineDestroy(Item item, float x, float z, float size = 0.12f)
        {
            var d = DecalSystem.Create(0, x, 8f * GameState.RoomScale + 0.005f, z, 90f, Rng.Next(360), 0f);
            d.Size = size;
            ItemSystem.RemoveItem(item);
        }

        private static void RefineReposition(Item item, float x, float y, float z)
        {
            PositionEntity(item.Collider, x, y, z);
            ResetEntity(item.Collider);
        }

        private static void RefineReplace(Item item, string newName, string newTemp, float x, float y, float z)
        {
            ItemSystem.CreateItem(newName, newTemp, x, y, z);
            ItemSystem.RemoveItem(item);
        }

        private static void RefineDefault(Item item, string setting, float x, float y, float z)
        {
            if (setting is "rough" or "coarse")
                RefineDestroy(item, x, z);
            else
                RefineReposition(item, x, y, z);
        }

        private static void RefineGasMask(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z);
                    break;
                case "1:1":
                    RefineReposition(item, x, y, z);
                    break;
                case "fine":
                case "very fine":
                    RefineReplace(item, "Gas Mask", "supergasmask", x, y, z);
                    break;
            }
        }

        private static void RefineScp1499(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z);
                    break;
                case "1:1":
                    RefineReplace(item, "Gas Mask", "gasmask", x, y, z);
                    break;
                case "fine":
                    RefineReplace(item, "SCP-1499", "super1499", x, y, z);
                    break;
                case "very fine":
                    ItemSystem.RemoveItem(item);
                    break;
            }
        }

        private static void RefineVest(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z);
                    break;
                case "1:1":
                    RefineReposition(item, x, y, z);
                    break;
                case "fine":
                    RefineReplace(item, "Heavy Ballistic Vest", "finevest", x, y, z);
                    break;
                case "very fine":
                    RefineReplace(item, "Bulky Ballistic Vest", "veryfinevest", x, y, z);
                    break;
            }
        }

        private static void RefineFirstAid(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z);
                    break;
                case "1:1":
                    if (Rng.Next(2) == 1)
                        RefineReplace(item, "Blue First Aid Kit", "firstaid2", x, y, z);
                    else
                        RefineReplace(item, "First Aid Kit", "firstaid", x, y, z);
                    break;
                case "fine":
                    RefineReplace(item, "Small First Aid Kit", "finefirstaid", x, y, z);
                    break;
                case "very fine":
                    RefineReplace(item, "Strange Bottle", "veryfinefirstaid", x, y, z);
                    break;
            }
        }

        private static void RefineKeyCard(Item item, string setting, float x, float y, float z)
        {
            var diff = GameState.SelectedDifficulty?.OtherFactors ?? OtherFactors.Normal;

            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z, 0.07f);
                    break;
                case "1:1":
                    RefineReplace(item, "Playing Card", "misc", x, y, z);
                    break;
                case "fine":
                    RefineKeyCardFine(item, diff, x, y, z);
                    break;
                case "very fine":
                    RefineKeyCardVeryFine(diff, x, y, z);
                    ItemSystem.RemoveItem(item);
                    break;
            }
        }

        private static void RefineKeyCardFine(Item item, OtherFactors diff, float x, float y, float z)
        {
            string name = item.Template.Name;
            string resultName = null;
            string resultTemp = "key2";

            if (name == "Level 1 Key Card")
            {
                resultName = diff == OtherFactors.Easy ? "Level 2 Key Card" :
                    Rng.Next(diff == OtherFactors.Normal ? 5 : 4) == 0 ? "Mastercard" : "Level 2 Key Card";
                resultTemp = resultName == "Mastercard" ? "misc" : "key2";
            }
            else if (name == "Level 2 Key Card")
            {
                resultName = diff == OtherFactors.Easy ? "Level 3 Key Card" :
                    Rng.Next(diff == OtherFactors.Normal ? 4 : 3) == 0 ? "Mastercard" : "Level 3 Key Card";
                resultTemp = resultName == "Mastercard" ? "misc" : "key3";
            }
            else if (name == "Level 3 Key Card")
            {
                int chance = diff == OtherFactors.Easy ? 10 : diff == OtherFactors.Normal ? 15 : 20;
                resultName = Rng.Next(chance) == 0 ? "Level 4 Key Card" : "Playing Card";
                resultTemp = resultName == "Playing Card" ? "misc" : "key4";
            }
            else if (name == "Level 4 Key Card")
            {
                resultName = diff == OtherFactors.Easy ? "Level 5 Key Card" :
                    Rng.Next(diff == OtherFactors.Normal ? 4 : 3) == 0 ? "Mastercard" : "Level 5 Key Card";
                resultTemp = resultName == "Mastercard" ? "misc" : "key5";
            }
            else if (name == "Level 5 Key Card")
            {
                RefineKeyCardVeryFine(diff, x, y, z);
                ItemSystem.RemoveItem(item);
                return;
            }

            if (resultName != null)
                RefineReplace(item, resultName, resultTemp, x, y, z);
            else
                ItemSystem.RemoveItem(item);
        }

        private static void RefineKeyCardVeryFine(OtherFactors diff, float x, float y, float z)
        {
            int achvCount = 0;
            foreach (var a in AchievementSystem.All)
                if (a.Unlocked) achvCount++;

            int max = Math.Max(1, (AchievementSystem.All.Count - 1) * (diff == OtherFactors.Easy ? 3 : diff == OtherFactors.Normal ? 4 : 5)
                - (achvCount - 1) * 3);

            if (Rng.Next(0, max) == 0)
                ItemSystem.CreateItem("Key Card Omni", "key6", x, y, z);
            else
                ItemSystem.CreateItem("Mastercard", "misc", x, y, z);
        }

        private static void RefineKeyOmni(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z, 0.07f);
                    break;
                case "1:1":
                    RefineReplace(item, Rng.Next(2) == 0 ? "Mastercard" : "Playing Card", "misc", x, y, z);
                    break;
                case "fine":
                case "very fine":
                    RefineReplace(item, "Key Card Omni", "key6", x, y, z);
                    break;
            }
        }

        private static void RefineToKey1(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z, 0.07f);
                    break;
                case "1:1":
                    RefineReplace(item, "Level 1 Key Card", "key1", x, y, z);
                    break;
                case "fine":
                case "very fine":
                    RefineReplace(item, "Level 2 Key Card", "key2", x, y, z);
                    break;
            }
        }

        private static void RefineMastercard(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                    RefineDestroy(item, x, z, 0.07f);
                    break;
                case "coarse":
                    RefineReplace(item, "Quarter", "25ct", x, y, z);
                    for (int i = 0; i < 4; i++)
                        ItemSystem.CreateItem("Quarter", "25ct", x, y, z);
                    break;
                case "1:1":
                    RefineReplace(item, "Level 1 Key Card", "key1", x, y, z);
                    break;
                case "fine":
                case "very fine":
                    RefineReplace(item, "Level 2 Key Card", "key2", x, y, z);
                    break;
            }
        }

        private static void RefineBattery(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z, 0.2f);
                    break;
                case "1:1":
                    RefineReplace(item, "18V Battery", "18vbat", x, y, z);
                    break;
                case "fine":
                case "very fine":
                    RefineReplace(item, "Strange Battery", "killbat", x, y, z);
                    break;
            }
        }

        private static void RefineScp148(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineReplace(item, "SCP-148 Ingot", "scp148ingot", x, y, z);
                    break;
                case "1:1":
                case "fine":
                case "very fine":
                    if (item.Template.Name == "SCP-148 Ingot")
                        RefineReplace(item, "Metal Panel", "scp148", x, y, z);
                    else
                        RefineReposition(item, x, y, z);
                    break;
            }
        }

        private static void RefineHand(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    DecalSystem.Create(3, x, 8f * GameState.RoomScale + 0.005f, z, 90f, Rng.Next(360), 0f);
                    break;
                case "1:1":
                case "fine":
                case "very fine":
                    if (item.Template.Name == "Severed Hand")
                        ItemSystem.CreateItem("Black Severed Hand", "hand2", x, y, z);
                    else
                        ItemSystem.CreateItem("Severed Hand", "hand", x, y, z);
                    break;
            }
            ItemSystem.RemoveItem(item);
        }

        private static void RefineNvg(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z);
                    break;
                case "1:1":
                    RefineReposition(item, x, y, z);
                    break;
                case "fine":
                    RefineReplace(item, "Night Vision Goggles", "finenvgoggles", x, y, z);
                    break;
                case "very fine":
                    var it = ItemSystem.CreateItem("Night Vision Goggles", "supernv", x, y, z);
                    if (it != null) it.State = 1000f;
                    ItemSystem.RemoveItem(item);
                    break;
            }
        }

        private static void RefineHazmat(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z, 0.2f);
                    break;
                case "1:1":
                    RefineReplace(item, "Hazmat Suit", "hazmatsuit", x, y, z);
                    break;
                case "fine":
                case "very fine":
                    RefineReplace(item, "Hazmat Suit", "hazmatsuit2", x, y, z);
                    break;
            }
        }

        private static void Refine420(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z, 0.2f);
                    break;
                case "1:1":
                    RefineReplace(item, "Cigarette", "cigarette", x + 1.5f, y + 0.5f, z + 1f);
                    break;
                case "fine":
                    RefineReplace(item, "Joint", "420s", x + 1.5f, y + 0.5f, z + 1f);
                    break;
                case "very fine":
                    RefineReplace(item, "Smelly Joint", "420s", x + 1.5f, y + 0.5f, z + 1f);
                    break;
            }
        }

        private static void RefineNav(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineReplace(item, "Electronical components", "misc", x, y, z);
                    break;
                case "1:1":
                    var nav = ItemSystem.CreateItem("S-NAV Navigator", "nav", x, y, z);
                    if (nav != null) nav.State = 100f;
                    ItemSystem.RemoveItem(item);
                    break;
                case "fine":
                    nav = ItemSystem.CreateItem("S-NAV 310 Navigator", "nav", x, y, z);
                    if (nav != null) nav.State = 100f;
                    ItemSystem.RemoveItem(item);
                    break;
                case "very fine":
                    nav = ItemSystem.CreateItem("S-NAV Navigator Ultimate", "nav", x, y, z);
                    if (nav != null) nav.State = 101f;
                    ItemSystem.RemoveItem(item);
                    break;
            }
        }

        private static void RefineRadio(Item item, string setting, float x, float y, float z)
        {
            Item created = null;
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineReplace(item, "Electronical components", "misc", x, y, z);
                    return;
                case "1:1":
                    created = ItemSystem.CreateItem("Radio Transceiver", "18vradio", x, y, z);
                    if (created != null) created.State = 100f;
                    break;
                case "fine":
                    created = ItemSystem.CreateItem("Radio Transceiver", "fineradio", x, y, z);
                    if (created != null) created.State = 101f;
                    break;
                case "very fine":
                    created = ItemSystem.CreateItem("Radio Transceiver", "veryfineradio", x, y, z);
                    if (created != null) created.State = 101f;
                    break;
            }
            ItemSystem.RemoveItem(item);
        }

        private static void RefineScp513(Item item, string setting, float x, float y, float z)
        {
            switch (setting)
            {
                case "rough":
                case "coarse":
                    RefineDestroy(item, x, z, 0.2f);
                    break;
                case "1:1":
                case "fine":
                case "very fine":
                    RefineReplace(item, "SCP-513", "scp513", x, y, z);
                    break;
            }
        }

        private static bool ToggleGasMask(Item item)
        {
            if (GameState.Wearing1499 || GameState.WearingHazmat > 0)
            {
                GameState.Msg = GameState.WearingHazmat > 0
                    ? "You need to take off the hazmat suit in order to put on the gas mask."
                    : "You need to take off SCP-1499 in order to put on the gas mask.";
                GameState.MsgTimer = 70f * 5f;
                return false;
            }

            if (GameState.WearingGasMask)
            {
                GameState.WearingGasMask = false;
                GameState.Msg = "You removed the gas mask.";
            }
            else
            {
                GameState.WearingGasMask = true;
                if (GameState.WearingNightVision > 0)
                    GameState.CameraFogFar = GameState.StoredCameraFogFar;
                GameState.WearingNightVision = 0;
                GameState.Msg = item.Template.TempName == "supergasmask"
                    ? "You put on the gas mask and you can breathe easier."
                    : "You put on the gas mask.";
            }

            GameState.MsgTimer = 70f * 5f;
            ItemSystem.SelectedItem = null;
            return true;
        }

        private static bool ToggleVest(Item item)
        {
            if (GameState.WearingVest > 0)
            {
                GameState.WearingVest = 0;
                GameState.Msg = "You removed the vest.";
                ItemSystem.DropItem(item);
                GameState.MsgTimer = 70f * 5f;
                item.State = 0f;
                ItemSystem.SelectedItem = null;
                WearItem = null;
                return true;
            }

            WearItem = item;
            item.State = 0f;
            ItemSystem.SelectedItem = item;
            return true;
        }

        private static void CompleteVestWear(Item item)
        {
            GameState.WearingVest = item.Template.TempName == "finevest" ? 2 : 1;
            GameState.Msg = item.Template.TempName == "finevest"
                ? "You put on the vest and feel heavily encumbered."
                : "You put on the vest and feel slightly encumbered.";
            if (item.Template.Sound != 66)
                AudioSystem.PlayPickSound(item.Template.Sound);
            GameState.MsgTimer = 70f * 5f;
            ItemSystem.SelectedItem = null;
        }

        private static bool ToggleHazmat(Item item)
        {
            if (GameState.WearingVest > 0) return false;

            if (GameState.WearingHazmat > 0)
            {
                GameState.WearingHazmat = 0;
                GameState.Msg = "You removed the hazmat suit.";
                ItemSystem.DropItem(item);
                GameState.MsgTimer = 70f * 5f;
                item.State = 0f;
                ItemSystem.SelectedItem = null;
                WearItem = null;
                return true;
            }

            WearItem = item;
            item.State = 0f;
            ItemSystem.SelectedItem = item;
            return true;
        }

        private static void CompleteHazmatWear(Item item)
        {
            GameState.WearingHazmat = item.Template.TempName switch
            {
                "hazmatsuit" => 1,
                "hazmatsuit2" => 2,
                _ => 3,
            };
            if (GameState.WearingNightVision > 0)
                GameState.CameraFogFar = GameState.StoredCameraFogFar;
            GameState.WearingGasMask = false;
            GameState.WearingNightVision = 0;
            GameState.Msg = "You put on the hazmat suit.";
            if (item.Template.Sound != 66)
                AudioSystem.PlayPickSound(item.Template.Sound);
            GameState.MsgTimer = 70f * 5f;
            ItemSystem.SelectedItem = null;
        }

        private static bool ToggleNightVision(Item item, int level)
        {
            if (GameState.Wearing1499 || GameState.WearingHazmat > 0)
            {
                GameState.Msg = GameState.Wearing1499
                    ? "You need to take off SCP-1499 in order to put on the goggles."
                    : "You need to take off the hazmat suit in order to put on the goggles.";
                GameState.MsgTimer = 70f * 5f;
                return false;
            }

            if (GameState.WearingNightVision == level)
            {
                GameState.WearingNightVision = 0;
                GameState.CameraFogFar = GameState.StoredCameraFogFar;
                GameState.Msg = "You removed the goggles.";
            }
            else
            {
                GameState.WearingGasMask = false;
                GameState.StoredCameraFogFar = GameState.CameraFogFar;
                GameState.CameraFogFar = 30f;
                GameState.WearingNightVision = level;
                GameState.Msg = "You put on the goggles.";
            }

            GameState.MsgTimer = 70f * 5f;
            ItemSystem.SelectedItem = null;
            return true;
        }

        private static bool Toggle714(Item item)
        {
            if (GameState.Wearing714 == 1)
            {
                GameState.Wearing714 = 0;
                GameState.Msg = "You removed the ring.";
            }
            else
            {
                AchievementSystem.Unlock("714");
                GameState.Wearing714 = 1;
                GameState.Msg = "You put on the ring.";
            }

            GameState.MsgTimer = 70f * 5f;
            ItemSystem.SelectedItem = null;
            return true;
        }

        private static bool UseScp500(Item item)
        {
            if (!ItemSystem.CanUseItem(false, false, true)) return false;

            AchievementSystem.Unlock("500");
            GameState.Msg = GameState.Infect > 0f
                ? "You swallowed the pill. Your nausea is fading."
                : "You swallowed the pill.";
            GameState.MsgTimer = 70f * 7f;
            GameState.DeathTimer = 0;
            GameState.Infect = 0f;
            GameState.Stamina = 100f;
            for (int i = 0; i < GameState.Scp1025State.Length; i++)
                GameState.Scp1025State[i] = 0f;
            if (GameState.StaminaEffect > 1f)
            {
                GameState.StaminaEffect = 1f;
                GameState.StaminaEffectTimer = 0f;
            }

            ItemSystem.RemoveItem(item);
            ItemSystem.SelectedItem = null;
            return true;
        }

        private static bool UseVeryFineFirstAid(Item item)
        {
            if (!ItemSystem.CanUseItem(false, false, true)) return false;

            switch (Rng.Next(5))
            {
                case 1:
                    GameState.Injuries = 3.5f;
                    GameState.Msg = "You started bleeding heavily.";
                    break;
                case 2:
                    GameState.Injuries = 0f;
                    GameState.Bloodloss = 0f;
                    GameState.Msg = "Your wounds are healing up rapidly.";
                    break;
                case 3:
                    GameState.Injuries = Math.Max(0f, GameState.Injuries - Rng.Next(1, 4));
                    GameState.Bloodloss = Math.Max(0f, GameState.Bloodloss - Rng.Next(10, 100));
                    GameState.Msg = "You feel much better.";
                    break;
                case 4:
                    GameState.BlurTimer = 10000;
                    GameState.Bloodloss = 0f;
                    GameState.Msg = "You feel nauseated.";
                    break;
                default:
                    GameState.BlinkTimer = -10f;
                    break;
            }

            GameState.MsgTimer = 70f * 7f;
            ItemSystem.RemoveItem(item);
            ItemSystem.SelectedItem = null;
            return true;
        }

        private static bool UseFirstAid(Item item)
        {
            if (!ItemSystem.CanUseItem(false, false, true)) return false;

            GameState.Injuries = Math.Max(0f, GameState.Injuries - 1f);
            GameState.Bloodloss = Math.Max(0f, GameState.Bloodloss - 20f);
            GameState.Msg = "You bandaged your wounds.";
            GameState.MsgTimer = 70f * 5f;
            ItemSystem.RemoveItem(item);
            ItemSystem.SelectedItem = null;
            return true;
        }

        private static bool UseEyedrops(Item item, string temp)
        {
            if (!ItemSystem.CanUseItem(false, false, false)) return false;
            if (GameState.Wearing714 == 1)
            {
                ItemSystem.RemoveItem(item);
                ItemSystem.SelectedItem = null;
                return true;
            }

            switch (temp)
            {
                case "eyedrops":
                    GameState.BlinkEffect = 0.6f;
                    GameState.BlinkEffectTimer = Rng.Next(20, 31);
                    GameState.BlurTimer = 200;
                    break;
                case "fineeyedrops":
                    GameState.BlinkEffect = 0.4f;
                    GameState.BlinkEffectTimer = Rng.Next(30, 41);
                    GameState.Bloodloss = Math.Max(GameState.Bloodloss - 1f, 0f);
                    GameState.BlurTimer = 200;
                    break;
                case "supereyedrops":
                    GameState.BlinkEffect = 0f;
                    GameState.BlinkEffectTimer = 60f;
                    GameState.EyeStuck = 10000f;
                    GameState.BlurTimer = 1000;
                    break;
            }

            ItemSystem.RemoveItem(item);
            ItemSystem.SelectedItem = null;
            return true;
        }

        private static bool Use420(Item item)
        {
            if (!ItemSystem.CanUseItem(false, false, true)) return false;

            if (GameState.Wearing714 == 1)
                GameState.Msg = "\"DUDE WTF THIS SHIT DOESN'T EVEN WORK\"";
            else
            {
                GameState.Msg = "\"MAN DATS SUM GOOD ASS SHIT\"";
                GameState.Injuries = Math.Max(0f, GameState.Injuries - 0.5f);
                GameState.BlurTimer = 500;
                AchievementSystem.Unlock("420");
            }

            GameState.MsgTimer = 70f * 5f;
            ItemSystem.RemoveItem(item);
            ItemSystem.SelectedItem = null;
            return true;
        }

        private static bool Use420s(Item item)
        {
            if (!ItemSystem.CanUseItem(false, false, true)) return false;

            if (GameState.Wearing714 == 1)
                GameState.Msg = "\"DUDE WTF THIS SHIT DOESN'T EVEN WORK\"";
            else
            {
                GameState.DeathMsg = "Subject D-9341 found in a comatose state.";
                GameState.Msg = "\"UH WHERE... WHAT WAS I DOING AGAIN... MAN I NEED TO TAKE A NAP...\"";
                GameState.KillTimer = -1f;
            }

            GameState.MsgTimer = 70f * 6f;
            ItemSystem.RemoveItem(item);
            ItemSystem.SelectedItem = null;
            return true;
        }
    }
}