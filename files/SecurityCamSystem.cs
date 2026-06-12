// SecurityCamSystem.cs — ports CreateSecurityCam + UpdateSecurityCams from MapSystem.bb

using System.Collections.Generic;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class SecurityCam
    {
        public int Obj = -1;
        public int CameraObj = -1;
        public int ScrObj = -1;
        public int ScrOverlay = -1;
        public int MonitorObj = -1;
        public int Cam = -1;
        public float Angle;
        public float Turn;
        public float CurrAngle;
        public int Dir;
        public float State;
        public int ScrTexture;
        public bool FollowPlayer;
        public bool Screen;
        public RoomInstance Room;
        public int CoffinEffect;
        public bool AllowSaving = true;
        public int RenderInterval = 1;
        public bool SpecialCam;
    }

    public static class SecurityCamSystem
    {
        public static SecurityCam CoffinCam;
        public static readonly List<int> ScreenTexs = new();
        private static readonly List<SecurityCam> _cams = new();
        private static int _camBase = -1;

        public static IReadOnlyList<SecurityCam> All => _cams;

        public static void Initialize()
        {
            _camBase = LoadMesh("GFX/map/cam_base");
            ScreenTexs.Clear();
            for (int i = 0; i < 4; i++)
                ScreenTexs.Add(-1);
        }

        public static int OldAiPics(int index) => -1;

        public static SecurityCam Create(float x, float y, float z, RoomInstance room, bool screen = false)
        {
            var sc = new SecurityCam { Room = room, Screen = screen };
            sc.Obj = CopyEntity(_camBase);
            sc.CameraObj = CreatePivot(sc.Obj);
            PositionEntity(sc.Obj, x, y, z, true);
            if (screen)
            {
                sc.ScrObj = CreatePivot();
                sc.ScrOverlay = CreatePivot();
                sc.MonitorObj = CreatePivot();
            }
            if (room != null)
                EntityParent(sc.Obj, room.obj);
            _cams.Add(sc);
            return sc;
        }

        public static void Update()
        {
            if (GameState.PlayerRoom?.RoomName == "dimension1499") return;

            foreach (var sc in _cams)
            {
                if (sc.Room == null)
                {
                    if (sc.Cam != -1) HideEntity(sc.Cam);
                    continue;
                }

                float dist = EventSystem.GetContext(sc.Room).Dist;
                bool close = dist < 6f || GameState.PlayerRoom == sc.Room;

                if (sc.Room.RoomName == "room2sl")
                    sc.CoffinEffect = 0;

                if (!close && sc != CoffinCam)
                {
                    if (sc.Cam != -1) HideEntity(sc.Cam);
                    continue;
                }

                if (sc.FollowPlayer)
                {
                    PointEntity(sc.CameraObj, GameState.Camera);
                    float temp = EntityPitch(sc.CameraObj);
                    RotateEntity(sc.Obj, 0,
                        MathUtil.CurveValue(EntityYaw(sc.CameraObj), EntityYaw(sc.Obj), 75f), 0);
                    temp = System.Math.Clamp(temp, 40f, 70f);
                    RotateEntity(sc.CameraObj,
                        MathUtil.CurveValue(temp, EntityPitch(sc.CameraObj), 75f),
                        EntityYaw(sc.Obj), 0);
                    PositionEntity(sc.CameraObj,
                        EntityX(sc.Obj, true), EntityY(sc.Obj, true) - 0.083f, EntityZ(sc.Obj, true));
                    RotateEntity(sc.CameraObj, EntityPitch(sc.CameraObj), EntityYaw(sc.Obj), 0);
                }
                else
                {
                    if (sc.Turn > 0f)
                    {
                        if (sc.Dir == 0)
                        {
                            sc.CurrAngle += 0.2f * GameState.FpsFactor;
                            if (sc.CurrAngle > sc.Turn * 1.3f) sc.Dir = 1;
                        }
                        else
                        {
                            sc.CurrAngle -= 0.2f * GameState.FpsFactor;
                            if (sc.CurrAngle < -sc.Turn * 1.3f) sc.Dir = 0;
                        }
                    }

                    float clamped = System.Math.Max(System.Math.Min(sc.CurrAngle, sc.Turn), -sc.Turn);
                    RotateEntity(sc.Obj, 0, sc.Room.Angle + sc.Angle + clamped, 0);
                    PositionEntity(sc.CameraObj,
                        EntityX(sc.Obj, true), EntityY(sc.Obj, true) - 0.083f, EntityZ(sc.Obj, true));
                    RotateEntity(sc.CameraObj, EntityPitch(sc.CameraObj), EntityYaw(sc.Obj), 0);

                    if (sc.Cam != -1)
                    {
                        PositionEntity(sc.Cam,
                            EntityX(sc.CameraObj, true), EntityY(sc.CameraObj, true), EntityZ(sc.CameraObj, true));
                        RotateEntity(sc.Cam, EntityPitch(sc.CameraObj), EntityYaw(sc.CameraObj), 0);
                        MoveEntity(sc.Cam, 0f, 0f, 0.1f);
                    }
                }

                if (!sc.Screen || !close) continue;

                sc.State += GameState.FpsFactor;

                if (GameState.BlinkTimer > -5f && sc.ScrObj != -1 &&
                    EntityVisible(sc.ScrObj, GameState.Camera))
                {
                    if ((sc.CoffinEffect == 1 || sc.CoffinEffect == 3) &&
                        GameState.Wearing714 != 1 && GameState.WearingHazmat < 3 && !GameState.WearingGasMask)
                    {
                        if (GameState.BlinkTimer > -5f)
                            GameState.Sanity -= GameState.FpsFactor;
                    }
                }

                if (GameState.Sanity < -1000f)
                {
                    GameState.DeathMsg = "Subject D-9341 died of cardiac arrest, likely caused by SCP-895.";
                    if (GameState.VomitTimer < -10f)
                        GameState.KillTimer = 0f;
                }
            }
        }

        public static void FreeAll()
        {
            foreach (var sc in _cams)
            {
                if (sc.Obj != -1) FreeEntity(sc.Obj);
                if (sc.Cam != -1) FreeEntity(sc.Cam);
            }
            _cams.Clear();
            CoffinCam = null;
        }
    }
}