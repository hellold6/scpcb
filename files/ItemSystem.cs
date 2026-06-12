// ItemSystem.cs — ports Items.bb (templates, inventory, pickup, drop, remove)

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework.Input;
using SCPCB360.Engine;
using SCPCB360.Input;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class ItemTemplate
    {
        public string Name;
        public string TempName;
        public int Sound = 1;
        public bool Found;
        public int Obj = -1;
        public string ObjPath;
        public string ParentObjPath;
        public string InvImgPath;
        public string ImgPath;
        public bool IsAnim;
        public float Scale = 1f;
        public int Tex = -1;
        public string TexPath;
    }

    public class Item
    {
        public int Collider = -1;
        public ItemTemplate Template;
        public bool Picked;
        public int Dropped = -1;
        public int Id;
        public float Dist;
        public float State;
        public string CustomName;
        public string Name => CustomName ?? Template?.Name ?? "";
        private static int _nextId;
        public Item() { Id = _nextId++; }
    }

    public static class ItemSystem
    {
        public const int MaxItemAmount = 10;

        private static readonly List<ItemTemplate> _templates = new();
        private static readonly List<Item> _items = new();
        private static readonly Item[] _inventory = new Item[MaxItemAmount + 1];

        public static int ItemAmount;
        public static int InvSelect;
        public static Item SelectedItem;
        public static Item ClosestItem;

        public static IReadOnlyList<Item> All => _items;
        public static IReadOnlyList<ItemTemplate> AllTemplates => _templates;
        public static Item[] Inventory => _inventory;

        public static void InitTemplates()
        {
            _templates.Clear();
            foreach (var def in ItemTemplateRegistry.All)
            {
                float scale = def.ScaleExpr == "GameState.RoomScale"
                    ? GameState.RoomScale
                    : float.Parse(def.ScaleExpr.TrimEnd('f'), CultureInfo.InvariantCulture);

                var it = RegisterTemplate(def.Name, def.TempName, def.ObjPath, def.InvImgPath,
                    def.ImgPath, scale, def.TexPath);
                it.Sound = def.Sound;
            }
        }

        private static ItemTemplate RegisterTemplate(string name, string tempName, string objPath,
            string invImgPath, string imgPath, float scale, string texturePath = "")
        {
            var existing = _templates.Find(t => t.ObjPath == objPath && t.Obj != -1);
            var it = new ItemTemplate
            {
                Name = name,
                TempName = tempName,
                ObjPath = objPath,
                InvImgPath = invImgPath,
                ImgPath = imgPath,
                Scale = scale,
                TexPath = texturePath,
            };

            if (existing != null)
            {
                it.Obj = CopyEntity(existing.Obj);
                it.ParentObjPath = existing.ObjPath;
            }
            else
            {
                it.Obj = LoadMesh(objPath);
                ScaleEntity(it.Obj, scale, scale, scale);
            }

            HideEntity(it.Obj);
            _templates.Add(it);
            return it;
        }

        public static Item CreateItem(string name, string tempName, float x, float y, float z,
            int r = 0, int g = 0, int b = 0)
        {
            var template = _templates.Find(t => t.TempName == tempName && t.Name == name)
                ?? _templates.Find(t => t.TempName == tempName);

            if (template == null) return null;

            var it = new Item { Template = template };
            it.Collider = CreatePivot();
            PositionEntity(it.Collider, x, y, z);
            EntityType(it.Collider, 3);
            EntityRadius(it.Collider, 0.15f);

            int visual = CopyEntity(template.Obj);
            EntityParent(visual, it.Collider);
            ShowEntity(visual);

            if (r != 0 || g != 0 || b != 0)
                EntityColor(visual, r, g, b);

            _items.Add(it);
            return it;
        }

        public static void Update(int playerEnt)
        {
            ClosestItem = null;
            float closestDist = 2f;

            foreach (var it in _items)
            {
                if (it.Picked) continue;

                float dist = EntityDistance(it.Collider, playerEnt);
                it.Dist = dist;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    ClosestItem = it;
                }
            }

            if (GameState.InvOpen)
                UpdateInventoryNavigation();
        }

        public static void UpdateInventoryNavigation()
        {
            int slot = GameState.InvHoverSlot;
            if (slot < 0 || slot >= MaxItemAmount)
                slot = 0;

            if (XInputRouter.IsDpadPressed(Buttons.DPadLeft))
            {
                if (slot % 5 > 0) slot--;
                else if (slot >= 5) slot = slot - 1;
            }
            else if (XInputRouter.IsDpadPressed(Buttons.DPadRight))
            {
                if (slot % 5 < 4 && slot < MaxItemAmount - 1) slot++;
                else if (slot < 4) slot++;
            }
            else if (XInputRouter.IsDpadPressed(Buttons.DPadUp) && slot >= 5)
                slot -= 5;
            else if (XInputRouter.IsDpadPressed(Buttons.DPadDown) && slot < 5)
                slot += 5;

            GameState.InvHoverSlot = Math.Clamp(slot, 0, MaxItemAmount - 1);
            InvSelect = GameState.InvHoverSlot;
        }

        public static bool CanUseItem(bool canUseWithHazmat, bool canUseWithGasMask, bool canUseWithEyewear)
        {
            if (!canUseWithHazmat && GameState.WearingHazmat > 0)
            {
                GameState.Msg = "You can't use that item while wearing a hazmat suit.";
                GameState.MsgTimer = 70f * 5f;
                return false;
            }

            if (!canUseWithGasMask && (GameState.WearingGasMask || GameState.Wearing1499))
            {
                GameState.Msg = "You can't use that item while wearing a gas mask.";
                GameState.MsgTimer = 70f * 5f;
                return false;
            }

            if (!canUseWithEyewear && GameState.WearingNightVision > 0)
            {
                GameState.Msg = "You can't use that item while wearing headgear.";
                GameState.MsgTimer = 70f * 5f;
                return false;
            }

            return true;
        }

        public static bool PickItem(Item item)
        {
            if (item == null) return false;

            if (GameState.WearingHazmat > 0)
            {
                GameState.Msg = "You cannot pick up any items while wearing a hazmat suit.";
                GameState.MsgTimer = 70f * 5f;
                return false;
            }

            bool fullInv = true;
            for (int n = 0; n < MaxItemAmount; n++)
            {
                if (_inventory[n] == null)
                {
                    fullInv = false;
                    break;
                }
            }

            if (fullInv)
            {
                GameState.Msg = "You cannot carry any more items.";
                GameState.MsgTimer = 70f * 5f;
                return false;
            }

            string temp = item.Template.TempName;
            switch (temp)
            {
                case "1123":
                    Handle1123Pickup();
                    return false;
                case "killbat":
                    GameState.DeathMsg = "Subject D-9341 found dead inside SCP-914's output booth next to what appears to be an ordinary nine-volt battery.";
                    GameState.KillTimer = 0f;
                    return false;
                case "scp148":
                    AchievementSystem.Unlock("148");
                    break;
                case "scp513":
                    AchievementSystem.Unlock("513");
                    break;
                case "scp860":
                    AchievementSystem.Unlock("860");
                    break;
                case "key6":
                    AchievementSystem.Unlock("omni");
                    break;
                case "veryfinevest":
                    GameState.Msg = "The vest is too heavy to pick up.";
                    GameState.MsgTimer = 70f * 6f;
                    return false;
                case "firstaid":
                case "finefirstaid":
                case "veryfinefirstaid":
                case "firstaid2":
                    item.State = 0f;
                    break;
                case "nav":
                    if (item.Template.Name == "S-NAV Navigator Ultimate")
                        AchievementSystem.Unlock("snav");
                    break;
                case "hazmatsuit":
                case "hazmatsuit2":
                case "hazmatsuit3":
                    if (!CanPickWearable(temp, true))
                        return false;
                    SelectedItem = item;
                    break;
                case "vest":
                case "finevest":
                    if (!CanPickWearable(temp, false))
                        return false;
                    SelectedItem = item;
                    break;
            }

            for (int n = 0; n < MaxItemAmount; n++)
            {
                if (_inventory[n] != null) continue;

                if (item.Template.Sound != 66)
                    AudioSystem.PlayPickSound(item.Template.Sound);

                item.Picked = true;
                item.Dropped = -1;
                item.Template.Found = true;
                ItemAmount++;
                _inventory[n] = item;
                HideEntity(item.Collider);
                return true;
            }

            return false;
        }

        private static bool CanPickWearable(string tempName, bool isHazmat)
        {
            for (int z = 0; z < MaxItemAmount; z++)
            {
                var inv = _inventory[z];
                if (inv == null) continue;
                string t = inv.Template.TempName;

                if (isHazmat)
                {
                    if (t is "hazmatsuit" or "hazmatsuit2" or "hazmatsuit3")
                    {
                        GameState.Msg = "You are not able to wear two hazmat suits at the same time.";
                        GameState.MsgTimer = 70f * 5f;
                        return false;
                    }
                    if (t is "vest" or "finevest")
                    {
                        GameState.Msg = "You are not able to wear a vest and a hazmat suit at the same time.";
                        GameState.MsgTimer = 70f * 5f;
                        return false;
                    }
                }
                else
                {
                    if (t is "vest" or "finevest")
                    {
                        GameState.Msg = "You are not able to wear two vests at the same time.";
                        GameState.MsgTimer = 70f * 5f;
                        return false;
                    }
                    if (t is "hazmatsuit" or "hazmatsuit2" or "hazmatsuit3")
                    {
                        GameState.Msg = "You are not able to wear a vest and a hazmat suit at the same time.";
                        GameState.MsgTimer = 70f * 5f;
                        return false;
                    }
                }
            }

            return true;
        }

        private static void Handle1123Pickup()
        {
            if (GameState.Wearing714 == 1) return;

            if (GameState.PlayerRoom?.Template?.Name != "room1123")
            {
                GameState.DeathMsg = "Subject D-9341 was shot dead after attempting to attack a member of Nine-Tailed Fox.";
                GameState.KillTimer = 0f;
                return;
            }

            EventSystem.Trigger1123Touch();
        }

        public static bool PickupClosest() => PickItem(ClosestItem);

        public static void DropItem(Item item, bool playDropSound = true)
        {
            if (item == null || !item.Picked) return;

            if (GameState.WearingHazmat > 0)
            {
                GameState.Msg = "You cannot drop any items while wearing a hazmat suit.";
                GameState.MsgTimer = 70f * 5f;
                return;
            }

            if (playDropSound && item.Template.Sound != 66)
                AudioSystem.PlayPickSound(item.Template.Sound);

            item.Dropped = 1;
            ShowEntity(item.Collider);

            int cam = GameState.Camera;
            PositionEntity(item.Collider, EntityX(cam), EntityY(cam), EntityZ(cam));
            RotateEntity(item.Collider, EntityPitch(cam), EntityYaw(cam) + Random.Shared.Next(-20, 21), 0f);
            MoveEntity(item.Collider, 0f, -0.1f, 0.1f);
            RotateEntity(item.Collider, 0f, EntityYaw(cam) + Random.Shared.Next(-110, 111), 0f);
            ResetEntity(item.Collider);

            item.Picked = false;
            for (int z = 0; z < MaxItemAmount; z++)
            {
                if (_inventory[z] == item)
                    _inventory[z] = null;
            }

            ItemAmount = Math.Max(0, ItemAmount - 1);
            ClearWearFlagsForDrop(item.Template.TempName);

            if (SelectedItem == item)
                SelectedItem = null;
        }

        private static void ClearWearFlagsForDrop(string temp)
        {
            switch (temp)
            {
                case "gasmask":
                case "supergasmask":
                case "gasmask3":
                    GameState.WearingGasMask = false;
                    break;
                case "hazmatsuit":
                case "hazmatsuit2":
                case "hazmatsuit3":
                    GameState.WearingHazmat = 0;
                    break;
                case "vest":
                case "finevest":
                    GameState.WearingVest = 0;
                    break;
                case "nvgoggles":
                    if (GameState.WearingNightVision == 1)
                    {
                        GameState.CameraFogFar = GameState.StoredCameraFogFar;
                        GameState.WearingNightVision = 0;
                    }
                    break;
                case "supernv":
                    if (GameState.WearingNightVision == 2)
                    {
                        GameState.CameraFogFar = GameState.StoredCameraFogFar;
                        GameState.WearingNightVision = 0;
                    }
                    break;
                case "finenvgoggles":
                    if (GameState.WearingNightVision == 3)
                    {
                        GameState.CameraFogFar = GameState.StoredCameraFogFar;
                        GameState.WearingNightVision = 0;
                    }
                    break;
                case "scp714":
                    GameState.Wearing714 = 0;
                    break;
                case "scp1499":
                case "super1499":
                    GameState.Wearing1499 = false;
                    break;
            }
        }

        public static void RemoveItem(Item item)
        {
            if (item == null) return;

            for (int n = 0; n < MaxItemAmount; n++)
            {
                if (_inventory[n] == item)
                {
                    _inventory[n] = null;
                    ItemAmount = Math.Max(0, ItemAmount - 1);
                    break;
                }
            }

            if (SelectedItem == item)
            {
                ClearWearFlagsForDrop(item.Template.TempName);
                SelectedItem = null;
            }

            if (item.Collider != -1)
                FreeEntity(item.Collider);
            _items.Remove(item);
        }

        public static Item GetHoveredInventoryItem()
        {
            int slot = GameState.InvHoverSlot;
            if (slot < 0 || slot >= MaxItemAmount) return null;
            return _inventory[slot];
        }

        public static void ForceSetItemId(Item it, int id)
        {
            if (it == null) return;
            it.Id = id;
        }

        public static void FreeAll()
        {
            foreach (var it in _items)
            {
                if (it.Collider != -1)
                    FreeEntity(it.Collider);
            }
            _items.Clear();
            for (int i = 0; i <= MaxItemAmount; i++)
                _inventory[i] = null;
            ItemAmount = 0;
            SelectedItem = null;
            InvSelect = 0;
            ClosestItem = null;
        }
    }
}