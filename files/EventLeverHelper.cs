// EventLeverHelper.cs — ports UpdateLever from MapSystem.bb

using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class EventLeverHelper
    {
        public static bool UpdateLever(int obj, bool locked = false)
        {
            if (obj == -1 || GameState.Camera == -1) return EntityPitch(obj) > 0f;

            float dist = EntityDistance(GameState.Camera, obj);
            if (dist >= 8f) return EntityPitch(obj) > 0f;

            float prevPitch = EntityPitch(obj);

            if (dist < 0.8f && !locked)
            {
                if (EntityVisible(obj, GameState.Camera))
                {
                    GameState.DrawHandIcon = true;
                    if (GameState.GrabbedEntity == obj)
                    {
                        float pitch = EntityPitch(obj) + 15f * GameState.FpsFactor;
                        pitch = MathUtil.Max(-80f, MathUtil.Min(80f, pitch));
                        RotateEntity(obj, pitch, EntityYaw(obj), 0f);
                        GameState.DrawArrowIcon[0] = true;
                        GameState.DrawArrowIcon[2] = true;
                    }
                }
            }

            if (EntityPitch(obj) > 75f && prevPitch <= 75f)
                AudioSystem.PlaySound2(AudioSystem.Load("SFX/Door/Lever.ogg"), GameState.Camera, obj);
            else if (EntityPitch(obj) < -75f && prevPitch >= -75f)
                AudioSystem.PlaySound2(AudioSystem.Load("SFX/Door/Lever.ogg"), GameState.Camera, obj);

            if (EntityPitch(obj) > 0f)
                RotateEntity(obj, MathUtil.CurveValue(80f, EntityPitch(obj), 10f), EntityYaw(obj), 0f);
            else
                RotateEntity(obj, MathUtil.CurveValue(-80f, EntityPitch(obj), 10f), EntityYaw(obj), 0f);

            return EntityPitch(obj) > 0f;
        }
    }
}