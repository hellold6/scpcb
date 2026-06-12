// ButtonSystem.cs — stub for CreateButton from Main.bb

using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class ButtonSystem
    {
        public static int Create(float x, float y, float z, float pitch, float yaw, float roll = 0f)
        {
            int obj = CopyEntity(MapAssets.ButtonObj);
            ScaleEntity(obj, 0.03f, 0.03f, 0.03f);
            PositionEntity(obj, x, y, z, true);
            RotateEntity(obj, pitch, yaw, roll);
            return obj;
        }
    }
}