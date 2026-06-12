// XInputRouter.cs
// Strips out all of CB's Win32 mouse-locking and DirectInput keyboard calls.
// Routes character look/move directly to Xbox 360 controller thumbsticks and buttons.
//
// Original CB input pattern (Win32):
//   mx = MouseXSpeed() : my = MouseYSpeed()
//   MoveMouse(GraphicsWidth()/2, GraphicsHeight()/2)
//   If KeyDown(17) : MoveEntity camPivot, 0, 0, movespd
//
// XInput replacement:
//   var look = XInputRouter.GetLookDelta()
//   var move = XInputRouter.GetMoveVector()

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SCPCB360.Input
{
    /// <summary>
    /// Maps Xbox 360 controller state to the input values CB's game logic expects.
    /// All public methods return the same semantic values the original Win32 calls returned,
    /// so ported game code needs minimal modification.
    /// </summary>
    public static class XInputRouter
    {
        // ── Tuning constants ──────────────────────────────────────────────────────

        // How fast looking turns the camera (degrees per frame at full stick deflection)
        public const float LookSensitivity  = 3.5f;

        // Dead zone — sticks inside this radius are treated as zero (XInput raw range 0–1)
        public const float StickDeadZone    = 0.18f;

        // Trigger threshold — below this, triggers are treated as unpressed
        public const float TriggerThreshold = 0.12f;

        // Walking speed multiplier when left trigger is held (CB's sprint mechanic maps well here)
        public const float SprintMultiplier = 2.0f;

        // ── Button binding map ────────────────────────────────────────────────────
        // Maps CB's logical actions to Xbox 360 buttons.
        // Modify freely — this is the single place to remap the whole controller.

        public static readonly Dictionary<CBAction, Buttons> ButtonMap = new()
        {
            { CBAction.Interact,      Buttons.A },
            { CBAction.Crouch,        Buttons.B },
            { CBAction.Inventory,     Buttons.Y },
            { CBAction.Flashlight,    Buttons.X },
            { CBAction.PauseMenu,     Buttons.Start },
            { CBAction.Sprint,        Buttons.LeftStick },    // click left stick = sprint toggle (alt)
            { CBAction.Blink,         Buttons.RightShoulder },// CB blink mechanic
            { CBAction.DropItem,      Buttons.LeftShoulder },
        };

        // ── State ─────────────────────────────────────────────────────────────────

        private static GamePadState _current;
        private static GamePadState _previous;
        private static int _mouseCenterX;
        private static int _mouseCenterY;
        private static bool _mouseLookInitialized;
        private static bool _mouseLookReady;

        // Accumulated look angles this frame (fed to camera pivot entity)
        private static Vector2 _lookDelta;

        // ─────────────────────────────────────────────────────────────────────────
        // Per-frame update (call at the top of Game.Update)
        // ─────────────────────────────────────────────────────────────────────────

        public static void Update()
        {
            _previous = _current;
            _current  = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None);

            _keyboard = Keyboard.GetState();

            if (_mouseLookInitialized && !_mouseLookReady)
            {
                Mouse.SetPosition(_mouseCenterX, _mouseCenterY);
                _mouseCurrent = Mouse.GetState();
                _mousePrevious = _mouseCurrent;
                _mouseLookReady = true;
            }
            else
            {
                _mousePrevious = _mouseCurrent;
                _mouseCurrent = Mouse.GetState();
            }

            // Compute look delta from right thumbstick
            var rx = ApplyDeadZone(_current.ThumbSticks.Right.X);
            var ry = ApplyDeadZone(_current.ThumbSticks.Right.Y);

            // Y axis is inverted on stick (up = +1) but we want up = negative pitch
            _lookDelta = new Vector2(rx * LookSensitivity, ry * LookSensitivity);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Camera look — replaces MouseXSpeed() / MouseYSpeed()
        // Returns (yaw delta, pitch delta) in degrees this frame.
        // ─────────────────────────────────────────────────────────────────────────

        public static Vector2 GetLookDelta()
        {
            var result = _lookDelta;

            int dx = _mouseLookInitialized ? _mouseCurrent.X - _mouseCenterX : _mouseCurrent.X - _mousePrevious.X;
            int dy = _mouseLookInitialized ? _mouseCurrent.Y - _mouseCenterY : _mouseCurrent.Y - _mousePrevious.Y;

            result.X -= dx * 0.15f;
            result.Y += dy * 0.15f;

            if (_mouseLookInitialized && (dx != 0 || dy != 0))
                Mouse.SetPosition(_mouseCenterX, _mouseCenterY);

            return result;
        }

        public static void InitializeMouseLook(int centerX, int centerY)
        {
            _mouseCenterX = centerX;
            _mouseCenterY = centerY;
            _mouseLookInitialized = true;
            _mouseLookReady = false;
        }

        public static float MouseXSpeed() => _lookDelta.X;
        public static float MouseYSpeed() => _lookDelta.Y;

        // ─────────────────────────────────────────────────────────────────────────
        // Movement — replaces KeyDown(W/A/S/D) checks
        // Returns a normalized (or zero) movement vector in entity-local XZ space.
        // ─────────────────────────────────────────────────────────────────────────

        public static Vector2 GetMoveVector()
        {
            var lx = ApplyDeadZone(_current.ThumbSticks.Left.X);
            var ly = ApplyDeadZone(_current.ThumbSticks.Left.Y);
            var vec = new Vector2(lx, ly);

            // Clamp to unit circle — prevents diagonal movement being faster
            if (vec.LengthSquared() > 1f) vec.Normalize();
            return vec;
        }

        /// <summary>
        /// Returns the forward/back component of left stick (replaces KeyDown(17) = W, KeyDown(31) = S)
        /// Positive = forward, negative = backward.
        /// </summary>
        /// 
        private static MouseState _mouseCurrent;
        private static MouseState _mousePrevious;
        private static KeyboardState _keyboard;


        public static float GetForwardAxis()
        {
            float pad = ApplyDeadZone(_current.ThumbSticks.Left.Y);

            if (_keyboard.IsKeyDown(Keys.W)) pad += 1f;
            if (_keyboard.IsKeyDown(Keys.S)) pad -= 1f;

            return MathHelper.Clamp(pad, -1f, 1f);
        }

        /// <summary>Returns the strafe component (replaces Q/E or A/D strafing in CB)</summary>
        public static float GetStrafeAxis()
        {
            float pad = ApplyDeadZone(_current.ThumbSticks.Left.X);

            if (_keyboard.IsKeyDown(Keys.D)) pad += 1f;
            if (_keyboard.IsKeyDown(Keys.A)) pad -= 1f;

            return MathHelper.Clamp(pad, -1f, 1f);
        }
        /// <summary>
        /// True while left trigger is held past threshold.
        /// Maps to CB's sprint / run mechanic.
        /// </summary>
        public static bool IsSprinting() => _current.Triggers.Left > TriggerThreshold;

        /// <summary>
        /// Speed multiplier to apply to MoveEntity calls.
        /// </summary>
        public static float GetMoveSpeed(float baseSpeed)
            => IsSprinting() ? baseSpeed * SprintMultiplier : baseSpeed;

        // ─────────────────────────────────────────────────────────────────────────
        // Button state queries — replaces KeyDown / KeyHit
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>IsHeld() = held this frame (replaces KeyDown)</summary>
        public static bool IsHeld(CBAction action)
            => ButtonMap.TryGetValue(action, out var btn) && _current.IsButtonDown(btn);

        /// <summary>IsPressed() = pressed this frame only (replaces KeyHit)</summary>
        public static bool IsPressed(CBAction action)
            => ButtonMap.TryGetValue(action, out var btn)
            && _current.IsButtonDown(btn)
            && _previous.IsButtonUp(btn);

        /// <summary>IsReleased() = released this frame (useful for blink mechanic duration)</summary>
        public static bool IsReleased(CBAction action)
            => ButtonMap.TryGetValue(action, out var btn)
            && _current.IsButtonUp(btn)
            && _previous.IsButtonDown(btn);

        // ─────────────────────────────────────────────────────────────────────────
        // Rumble (replaces Win32 XInputSetState calls CB doesn't have but we can add)
        // ─────────────────────────────────────────────────────────────────────────

        private static float _rumbleTimer;
        private static float _leftMotor, _rightMotor;

        /// <summary>
        /// Trigger a rumble pulse. duration in seconds.
        /// left = low-freq motor (impacts), right = high-freq motor (ambient hum).
        /// </summary>
        public static void Rumble(float left, float right, float duration)
        {
            _leftMotor  = MathHelper.Clamp(left,  0f, 1f);
            _rightMotor = MathHelper.Clamp(right, 0f, 1f);
            _rumbleTimer = duration;
            GamePad.SetVibration(PlayerIndex.One, _leftMotor, _rightMotor);
        }

        public static void UpdateRumble(float deltaSeconds)
        {
            if (_rumbleTimer <= 0f) return;
            _rumbleTimer -= deltaSeconds;
            if (_rumbleTimer <= 0f)
            {
                _rumbleTimer = 0f;
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Utility
        // ─────────────────────────────────────────────────────────────────────────

        private static float ApplyDeadZone(float v)
        {
            if (System.Math.Abs(v) < StickDeadZone) return 0f;
            // Rescale so edge of dead zone = 0, stick edge = full range
            return (v - System.Math.Sign(v) * StickDeadZone) / (1f - StickDeadZone);
        }

        public static bool IsConnected() => _current.IsConnected;
    }

    // ── Action enum (CB logical actions, controller-agnostic) ─────────────────────
    public enum CBAction
    {
        Interact,
        Crouch,
        Inventory,
        Flashlight,
        PauseMenu,
        Sprint,
        Blink,
        DropItem,
    }
}
