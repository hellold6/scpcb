// GuiSystem.cs — ports DrawGUI() from Main.bb

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SCPCB360.Engine;
using SCPCB360.Input;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class GuiSystem
    {
        private static GraphicsDevice _gfx;
        private static Texture2D _whitePixel;
        private static Texture2D _blinkMeter;
        private static Texture2D _staminaMeter;
        private static Texture2D _blinkIcon;
        private static Texture2D _sprintIcon;
        private static Texture2D _crouchIcon;
        private static Texture2D _handIcon;
        private static Texture2D _handIcon2;
        private static Texture2D[] _arrowIcons = new Texture2D[4];
        private static Texture2D _keypadHud;

        private const int MeterWidth = 204;
        private const int MeterHeight = 20;
        private const int MeterX = 80;

        public static void Initialize(GraphicsDevice gfx)
        {
            _gfx = gfx;
            _whitePixel = new Texture2D(gfx, 1, 1);
            _whitePixel.SetData(new[] { Color.White });

            _blinkMeter = LoadGuiTexture("GFX/blinkmeter");
            _staminaMeter = LoadGuiTexture("GFX/staminameter");
            _blinkIcon = LoadGuiTexture("GFX/blinkicon");
            _sprintIcon = LoadGuiTexture("GFX/sprinticon");
            _crouchIcon = LoadGuiTexture("GFX/crouchicon");
            _handIcon = LoadGuiTexture("GFX/hand");
            _handIcon2 = LoadGuiTexture("GFX/hand2");
            _keypadHud = LoadGuiTexture("GFX/hud");

            for (int i = 0; i < 4; i++)
                _arrowIcons[i] = LoadGuiTexture("GFX/arrow" + i);
        }

        private static Texture2D LoadGuiTexture(string path)
        {
            var tex = B3D.LoadTexture(path);
            return tex;
        }

        public static void Draw(SpriteBatch sb)
        {
            if (sb == null || _whitePixel == null) return;

            int gw = _gfx.Viewport.Width;
            int gh = _gfx.Viewport.Height;

            DrawPocketDimension(sb, gw, gh);
            DrawInteractIcons(sb, gw, gh);

            if (GameState.HudEnabled)
                DrawHudMeters(sb, gw, gh);

            if (GameState.DebugHud)
                DrawDebugHud(sb, gw, gh);

            DrawHudMessage(sb, gw, gh);
            DrawWearProgress(sb, gw, gh);
            DrawInventoryOverlay(sb, gw, gh);
            DrawKeypadOverlay(sb, gw, gh);
            DrawDeathOverlay(sb, gw, gh);
            DrawBlinkVignette(sb, gw, gh);
        }

        private static void DrawPocketDimension(SpriteBatch sb, int gw, int gh)
        {
            if (GameState.PlayerRoom?.def?.Name != "pocketdimension") return;

            foreach (var ev in EventSystem.All)
            {
                if (ev.Room != GameState.PlayerRoom) continue;
                if (ev.EventState <= 600) continue;
                if (GameState.BlinkTimer >= -3 || GameState.BlinkTimer <= -10) continue;

                var img = ev.OverlayImage;
                if (img == null) continue;

                int x = gw / 2 - Random.Shared.Next(310, 390);
                int y = gh / 2 - Random.Shared.Next(290, 310);
                sb.Draw(img, new Rectangle(x, y, img.Width, img.Height), Color.White);
                break;
            }
        }

        private static void DrawInteractIcons(SpriteBatch sb, int gw, int gh)
        {
            if (GameState.MenuOpen || GameState.InvOpen || GameState.SelectedDoor != null) return;

            if (GameState.ClosestButton != -1 && GameState.ClosestDoor != null)
            {
                float yaw = ClampLookAngle(MathUtil.WrapAngle(EntityYaw(GameState.Camera) - GetLookYaw(GameState.Camera, GameState.ClosestButton)));
                float pitch = ClampLookAngle(MathUtil.WrapAngle(EntityPitch(GameState.Camera) - GetLookPitch(GameState.Camera, GameState.ClosestButton)));
                DrawHandAt(sb, _handIcon, gw, gh, yaw, pitch);
            }

            if (ItemSystem.ClosestItem != null && !ItemSystem.ClosestItem.Picked)
            {
                float yaw = ClampLookAngle(-MathUtil.DeltaYaw(GameState.Camera, ItemSystem.ClosestItem.Collider));
                float pitch = ClampLookAngle(-MathUtil.DeltaPitch(GameState.Camera, ItemSystem.ClosestItem.Collider));
                DrawHandAt(sb, _handIcon2, gw, gh, yaw, pitch);
            }

            if (GameState.DrawHandIcon)
                DrawTextureCentered(sb, _handIcon, gw / 2, gh / 2);

            for (int i = 0; i < 4; i++)
            {
                if (!GameState.DrawArrowIcon[i]) continue;
                int x = gw / 2;
                int y = gh / 2;
                switch (i)
                {
                    case 0: y -= 64 + 5; break;
                    case 1: x += 64 + 5; break;
                    case 2: y += 64 + 5; break;
                    case 3: x -= 64 + 5; break;
                }
                DrawTextureCentered(sb, _handIcon, x, y);
                DrawRect(sb, x - 28, y - 28, 56, 56, Color.Black);
                if (_arrowIcons[i] != null)
                    sb.Draw(_arrowIcons[i], new Rectangle(x - 11, y - 11, 22, 22), Color.White);
                GameState.DrawArrowIcon[i] = false;
            }
        }

        private static void DrawHudMeters(SpriteBatch sb, int gw, int gh)
        {
            int x = MeterX;
            int y = gh - 95;

            DrawRectOutline(sb, x, y, MeterWidth, MeterHeight, Color.White);
            DrawBlinkMeter(sb, x, y);
            DrawIconBox(sb, x - 50, y, GameState.EyeIrritation > 0);
            DrawTexture(sb, _blinkIcon, x - 50, y, 30, 30);

            y = gh - 55;
            DrawRectOutline(sb, x, y, MeterWidth, MeterHeight, Color.White);
            DrawStaminaMeter(sb, x, y);
            DrawIconBox(sb, x - 50, y, false);
            DrawTexture(sb, GameState.Crouch ? _crouchIcon : _sprintIcon, x - 50, y, 30, 30);
        }

        private static void DrawBlinkMeter(SpriteBatch sb, int x, int y)
        {
            float blinkFreq = PlayerSystem.BlinkFreq * 70f;
            int segments = (int)(((MeterWidth - 2) * (GameState.BlinkTimer / blinkFreq)) / 10);
            for (int i = 1; i <= segments; i++)
            {
                if (_blinkMeter != null)
                    sb.Draw(_blinkMeter, new Rectangle(x + 3 + 10 * (i - 1), y + 3, 8, 14), Color.White);
                else
                    DrawRect(sb, x + 3 + 10 * (i - 1), y + 3, 8, 14, new Color(180, 220, 255));
            }
        }

        private static void DrawStaminaMeter(SpriteBatch sb, int x, int y)
        {
            int segments = (int)(((MeterWidth - 2) * (GameState.Stamina / 100f)) / 10);
            for (int i = 1; i <= segments; i++)
            {
                if (_staminaMeter != null)
                    sb.Draw(_staminaMeter, new Rectangle(x + 3 + 10 * (i - 1), y + 3, 8, 14), Color.White);
                else
                    DrawRect(sb, x + 3 + 10 * (i - 1), y + 3, 8, 14, new Color(80, 200, 80));
            }
        }

        private static void DrawHudMessage(SpriteBatch sb, int gw, int gh)
        {
            if (GameState.MsgTimer <= 0f || string.IsNullOrEmpty(GameState.Msg)) return;

            float alpha = Math.Min(GameState.MsgTimer / 2f, 255f) / 255f;
            bool centerScreen = !GameState.InvOpen;
            int y = centerScreen ? gh / 2 + 200 : (int)(gh * 0.94f);
            var color = GameState.Msg.StartsWith("Blitz3D Error!", StringComparison.OrdinalIgnoreCase)
                ? Color.Red
                : Color.White;

            TextRenderer.SetColor(Color.Black);
            TextRenderer.AAText(sb, gw / 2 + 1, y + 1, GameState.Msg, true, false, alpha);
            TextRenderer.SetColor(color);
            TextRenderer.AAText(sb, gw / 2, y, GameState.Msg, true, false, alpha);
        }

        private static void DrawWearProgress(SpriteBatch sb, int gw, int gh)
        {
            var item = ItemUseSystem.WearItem ?? ItemSystem.SelectedItem;
            if (item?.Template == null) return;

            string temp = item.Template.TempName;
            if (temp is not ("vest" or "finevest" or "hazmatsuit" or "hazmatsuit2" or "hazmatsuit3"))
                return;
            if (item.State <= 0f && ItemUseSystem.WearItem == null) return;

            int iconX = gw / 2 - 16;
            int iconY = gh / 2 - 16;
            DrawItemIcon(sb, item, iconX, iconY);

            const int width = 300;
            const int height = 20;
            int x = gw / 2 - width / 2;
            int y = gh / 2 + 80;
            DrawRectOutline(sb, x, y, width + 4, height, Color.White);

            int filled = (int)((width - 2) * (item.State / 100f) / 10f);
            for (int i = 0; i < filled; i++)
            {
                if (_blinkMeter != null)
                    sb.Draw(_blinkMeter, new Rectangle(x + 3 + 10 * i, y + 3, 8, 14), Color.White);
                else
                    DrawRectOutline(sb, x + 3 + 10 * i, y + 3, 8, 14, Color.Cyan);
            }
        }

        private static void DrawInventoryOverlay(SpriteBatch sb, int gw, int gh)
        {
            if (!GameState.InvOpen) return;

            const int slotW = 70;
            const int slotH = 70;
            const int spacing = 35;
            int x = gw / 2 - (slotW * ItemSystem.MaxItemAmount / 2 + spacing * (ItemSystem.MaxItemAmount / 2 - 1)) / 2;
            int y = gh / 2 - slotH;

            for (int n = 0; n < ItemSystem.MaxItemAmount; n++)
            {
                bool hover = n == GameState.InvHoverSlot;
                if (hover)
                    DrawRectOutline(sb, x - 1, y - 1, slotW + 2, slotH + 2, Color.Red);

                DrawInventoryFrame(sb, x, y, slotW, slotH);

                var item = ItemSystem.Inventory[n];
                if (item != null && item != ItemSystem.SelectedItem || hover)
                {
                    DrawItemIcon(sb, item, x + slotW / 2 - 16, y + slotH / 2 - 16);
                    if (hover && ItemSystem.SelectedItem == null)
                    {
                        TextRenderer.SetColor(Color.Black);
                        TextRenderer.AAText(sb, x + slotW / 2 + 1, y + slotH + spacing - 14, item.Template?.Name ?? "", true);
                        TextRenderer.SetColor(Color.White);
                        TextRenderer.AAText(sb, x + slotW / 2, y + slotH + spacing - 15, item.Template?.Name ?? "", true);
                    }
                }

                x += slotW + spacing;
                if (n == 4)
                {
                    y += slotH * 2;
                    x = gw / 2 - (slotW * ItemSystem.MaxItemAmount / 2 + spacing * (ItemSystem.MaxItemAmount / 2 - 1)) / 2;
                }
            }

            if (ItemSystem.ClosestItem != null)
                DrawTextPrompt(sb, "PRESS A TO PICK UP", gw, gh - 50, Color.Yellow);
        }

        private static void DrawKeypadOverlay(SpriteBatch sb, int gw, int gh)
        {
            if (GameState.SelectedDoor == null) return;

            float scale = 0.75f;
            int kw = (int)(256 * scale);
            int kh = (int)(320 * scale);
            int kx = gw / 2 - kw / 2;
            int ky = gh / 2 - kh / 2;

            if (_keypadHud != null)
                sb.Draw(_keypadHud, new Rectangle(kx, ky, kw, kh), Color.White * 0.9f);
            else
                DrawRectOutline(sb, kx, ky, kw, kh, Color.White);

            TextRenderer.AASetFont(3);
            if (!string.IsNullOrEmpty(GameState.KeypadMsg))
            {
                if (((int)GameState.KeypadTimer % 70) < 35)
                {
                    TextRenderer.SetColor(Color.Red);
                    TextRenderer.AAText(sb, gw / 2, ky + (int)(124 * scale), GameState.KeypadMsg, true, true);
                }
            }
            else
            {
                TextRenderer.SetColor(Color.White);
                TextRenderer.AAText(sb, gw / 2, ky + (int)(70 * scale), "ACCESS CODE:", true, true);
                TextRenderer.AASetFont(4);
                TextRenderer.AAText(sb, gw / 2, ky + (int)(124 * scale), GameState.KeypadInput, true, true);
            }
            TextRenderer.AASetFont(1);
        }

        private static void DrawDeathOverlay(SpriteBatch sb, int gw, int gh)
        {
            if (GameState.KillTimer >= 0) return;

            float darkness = Math.Min(Math.Abs(GameState.KillTimer) / 400f, 1f);
            sb.Draw(_whitePixel, new Rectangle(0, 0, gw, gh), Color.Black * darkness);

            if (GameState.KillTimer < -120 && !string.IsNullOrEmpty(GameState.DeathMsg))
            {
                TextRenderer.SetColor(Color.Red);
                TextRenderer.AAText(sb, gw / 2, gh / 2 - 40, "SUBJECT TERMINATED", true, true);
                TextRenderer.SetColor(Color.White);
                TextRenderer.AAText(sb, gw / 2, gh / 2 + 10, GameState.DeathMsg, true, false, 0.85f);
            }
        }

        private static void DrawBlinkVignette(SpriteBatch sb, int gw, int gh)
        {
            if (GameState.BlinkTimer >= 0) return;

            float darkA;
            if (GameState.BlinkTimer > -5)
                darkA = Math.Max(0f, (float)Math.Sin(Math.Abs(GameState.BlinkTimer * 18f)));
            else if (GameState.BlinkTimer > -15)
                darkA = 1f;
            else
                darkA = Math.Max(0f, (float)Math.Abs(Math.Sin(GameState.BlinkTimer * 18f)));

            if (darkA > 0.01f)
                sb.Draw(_whitePixel, new Rectangle(0, 0, gw, gh), Color.Black * Math.Min(darkA, 1f));
        }

        private static void DrawDebugHud(SpriteBatch sb, int gw, int gh)
        {
            TextRenderer.AASetFont(5);
            TextRenderer.SetColor(Color.White);
            int x = 30;
            int line = 50;
            int step = 20;

            TextRenderer.AAText(sb, x, line, "Player: (" + MathUtil.F2S(EntityX(GameState.Collider, true), 3) + ", " +
                MathUtil.F2S(EntityY(GameState.Collider, true), 3) + ", " + MathUtil.F2S(EntityZ(GameState.Collider, true), 3) + ")");
            line += step;
            TextRenderer.AAText(sb, x, line, "Room: " + (GameState.PlayerRoom?.def?.Name ?? "NULL"));
            line += step;
            TextRenderer.AAText(sb, x, line, "Stamina: " + MathUtil.F2S(GameState.Stamina, 3));
            line += step;
            TextRenderer.AAText(sb, x, line, "Blink: " + MathUtil.F2S(GameState.BlinkTimer, 3));
            line += step;
            TextRenderer.AAText(sb, x, line, "KillTimer: " + MathUtil.F2S(GameState.KillTimer, 3));

            if (NPCSystem.Curr173 != null)
            {
                line += step;
                TextRenderer.AAText(sb, x, line, "173 State: " + NPCSystem.Curr173.State);
            }
            if (NPCSystem.Curr106 != null)
            {
                line += step;
                TextRenderer.AAText(sb, x, line, "106 State: " + NPCSystem.Curr106.State);
            }

            TextRenderer.AASetFont(1);
        }

        private static void DrawTextPrompt(SpriteBatch sb, string text, int gw, int y, Color color)
        {
            TextRenderer.SetColor(color);
            TextRenderer.AAText(sb, gw / 2, y, text, true);
        }

        private static void DrawHandAt(SpriteBatch sb, Texture2D icon, int gw, int gh, float yaw, float pitch)
        {
            if (icon == null) return;
            float radYaw = MathHelper.ToRadians(yaw);
            float radPitch = MathHelper.ToRadians(pitch);
            int x = (int)(gw / 2 + Math.Sin(radYaw) * (gw / 3f) - 32);
            int y = (int)(gh / 2 - Math.Sin(radPitch) * (gh / 3f) - 32);
            sb.Draw(icon, new Rectangle(x, y, 64, 64), Color.White);
        }

        private static float ClampLookAngle(float angle)
        {
            if (angle > 90 && angle <= 180) return 90;
            if (angle > 180 && angle < 270) return 270;
            return angle;
        }

        private static float GetLookYaw(int from, int to)
        {
            int pivot = CreatePivot();
            PositionEntity(pivot, EntityX(from), EntityY(from), EntityZ(from));
            PointEntity(pivot, to);
            float yaw = EntityYaw(pivot);
            FreeEntity(pivot);
            return yaw;
        }

        private static float GetLookPitch(int from, int to)
        {
            int pivot = CreatePivot();
            PositionEntity(pivot, EntityX(from), EntityY(from), EntityZ(from));
            PointEntity(pivot, to);
            float pitch = EntityPitch(pivot);
            FreeEntity(pivot);
            return pitch;
        }

        private static void DrawTexture(SpriteBatch sb, Texture2D tex, int x, int y, int w, int h)
        {
            if (tex != null)
                sb.Draw(tex, new Rectangle(x, y, w, h), Color.White);
        }

        private static void DrawTextureCentered(SpriteBatch sb, Texture2D tex, int cx, int cy)
        {
            if (tex != null)
                sb.Draw(tex, new Rectangle(cx - 32, cy - 32, 64, 64), Color.White);
        }

        private static void DrawItemIcon(SpriteBatch sb, Item item, int x, int y)
        {
            if (item?.Template == null) return;
            var tex = B3D.LoadTexture(item.Template.InvImgPath);
            if (tex != null)
                sb.Draw(tex, new Rectangle(x, y, 32, 32), Color.White);
        }

        private static void DrawInventoryFrame(SpriteBatch sb, int x, int y, int w, int h)
        {
            DrawRectOutline(sb, x, y, w, h, Color.White);
            int offset = x % 64;
            for (int i = 0; i < w; i += 8)
                DrawRect(sb, x + i, y, 1, h, new Color(30, 30, 30, 40 + ((offset + i) % 32)));
        }

        private static void DrawIconBox(SpriteBatch sb, int x, int y, bool irritated)
        {
            if (irritated)
                DrawRect(sb, x - 3, y - 3, 36, 36, new Color(200, 0, 0));
            DrawRect(sb, x, y, 30, 30, Color.Black);
            DrawRectOutline(sb, x - 1, y - 1, 32, 32, Color.White);
        }

        private static void DrawRect(SpriteBatch sb, int x, int y, int w, int h, Color color)
            => sb.Draw(_whitePixel, new Rectangle(x, y, w, h), color);

        private static void DrawRectOutline(SpriteBatch sb, int x, int y, int w, int h, Color color)
        {
            DrawRect(sb, x, y, w, 1, color);
            DrawRect(sb, x, y + h - 1, w, 1, color);
            DrawRect(sb, x, y, 1, h, color);
            DrawRect(sb, x + w - 1, y, 1, h, color);
        }
    }
}