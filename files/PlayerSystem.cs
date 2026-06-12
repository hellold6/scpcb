// PlayerSystem.cs — ports MovePlayer, MouseLook, Kill from Main.bb

using System;
using Microsoft.Xna.Framework;
using SCPCB360.Engine;
using SCPCB360.Input;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class PlayerSystem
    {
        public const float BlinkFreq = 10f;
        public const float PitchLimit = 75f;

        private static float _playerPitch;
        private static float _playerYaw;
        private static float _shake;
        private static float _cameraShake;

        public static void SetCameraShake(float amount) => _cameraShake = Math.Max(_cameraShake, amount);

        public static void Initialize(int collider, int head, int camPivot, int camera)
        {
            GameState.Collider = collider;
            GameState.Head = head;
            GameState.CamPivot = camPivot;
            GameState.Camera = camera;
        }

        public static void MouseLook()
        {
            if (GameState.UnableToMove) return;

            var look = XInputRouter.GetLookDelta();
            _playerYaw += look.X;
            _playerPitch = MathHelper.Clamp(_playerPitch - look.Y, -PitchLimit, PitchLimit);

            RotateEntity(GameState.Collider, 0, _playerYaw, 0);
            RotateEntity(GameState.Camera, _playerPitch, 0, 0);
        }

        public static void MovePlayer()
        {
            if (!GameState.Playable) return;

            float sprint = 1f;
            float speed = 0.018f * GameState.FpsFactor;

            if (GameState.DeathTimer > 0)
            {
                GameState.DeathTimer -= (int)GameState.FpsFactor;
                if (GameState.DeathTimer < 1) GameState.DeathTimer = -1;
            }
            else if (GameState.DeathTimer < 0 && GameState.KillTimer < 0)
            {
                Kill();
                return;
            }

            if (GameState.CurrSpeed > 0)
                GameState.Stamina = Math.Min(GameState.Stamina + 0.15f * GameState.FpsFactor / 1.25f, 100f);
            else
                GameState.Stamina = Math.Min(GameState.Stamina + 0.15f * GameState.FpsFactor * 1.25f, 100f);

            if (GameState.StaminaEffectTimer > 0)
                GameState.StaminaEffectTimer -= GameState.FpsFactor / 70f;
            else if (GameState.StaminaEffect != 1f)
                GameState.StaminaEffect = 1f;

            if (GameState.Wearing714 > 0)
            {
                GameState.Stamina = Math.Min(GameState.Stamina, 10f);
                GameState.Sanity = Math.Max(-850f, GameState.Sanity);
            }

            if (GameState.IsZombie) GameState.Crouch = false;

            if (Math.Abs(GameState.CrouchState - (GameState.Crouch ? 1f : 0f)) < 0.001f)
                GameState.CrouchState = GameState.Crouch ? 1f : 0f;
            else
                GameState.CrouchState = MathUtil.CurveValue(GameState.Crouch ? 1f : 0f, GameState.CrouchState, 10f);

            float fwd = XInputRouter.GetForwardAxis();
            float strafe = XInputRouter.GetStrafeAxis();

            if (GameState.UnableToMove)
            {
                GameState.CurrSpeed = 0f;
            }
            else if (!GameState.NoClip && (fwd != 0f || strafe != 0f))
            {
                if (!GameState.Crouch && XInputRouter.IsSprinting() && GameState.Stamina > 0f && !GameState.IsZombie)
                {
                    sprint = 2.5f;
                    GameState.Stamina -= GameState.FpsFactor * 0.4f * GameState.StaminaEffect;
                    if (GameState.Stamina <= 0f) GameState.Stamina = -20f;
                }

                speed *= sprint * XInputRouter.GetMoveSpeed(1f);

                var playerWorld = Get(GameState.Collider).GetWorldMatrix();
                var forward = Vector3.TransformNormal(Vector3.Forward, playerWorld);
                var right = Vector3.TransformNormal(Vector3.Right, playerWorld);
                forward.Y = 0f;
                right.Y = 0f;
                if (forward.LengthSquared() > 0.0001f) forward.Normalize();
                if (right.LengthSquared() > 0.0001f) right.Normalize();

                var move = forward * fwd + right * strafe;
                if (move.LengthSquared() > 1f) move.Normalize();
                move *= speed;

                _shake = (_shake + GameState.FpsFactor * Math.Min(sprint, 1.5f) * 7f) % 720f;

                var pos = Get(GameState.Collider).GetWorldPosition();
                PositionEntity(GameState.Collider, pos.X + move.X, pos.Y + move.Y, pos.Z + move.Z, true);
                GameState.CurrSpeed = move.Length();
            }
            else
            {
                GameState.CurrSpeed = 0f;
            }

            float camHeight = MathHelper.Lerp(1.7f, 1.0f, GameState.CrouchState);
            PositionEntity(GameState.CamPivot, 0f, camHeight, 0f);

            UpdateBlink();
            PhysicsSystem.Update();
        }

        private static void UpdateBlink()
        {
            if (XInputRouter.IsPressed(CBAction.Blink))
            {
                GameState.BlinkTimer = -16f;
                GameState.BlinkEffect = 1f;
            }

            GameState.BlinkTimer += GameState.FpsFactor;

            if (GameState.BlinkTimer >= BlinkFreq)
            {
                GameState.BlinkTimer = 0f;
                GameState.BlinkEffect = 1f;
            }

            if (GameState.BlinkEffect > 0f)
            {
                GameState.BlinkEffect -= GameState.FpsFactor * 0.15f;
                if (GameState.BlinkEffect < 0f) GameState.BlinkEffect = 0f;
            }
        }

        public static void Kill()
        {
            GameState.KillTimer = 200;
            GameState.Playable = false;
            GameState.Screen = GameScreen.Dead;
            AudioSystem.KillSounds();
        }

        public static void ResetEntity()
        {
            _playerPitch = 0f;
            _playerYaw = EntityYaw(GameState.Collider);
            GameState.DropSpeed = 0f;
        }
    }
}