// FillRoomSystem.cs — ports FillRoom() from MapSystem.bb

using System;
using SCPCB360;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class FillRoomSystem
    {
        public static void Fill(RoomInstance r)
        {
            if (r == null) return;
            switch ((r.RoomName ?? "").ToLowerInvariant())
            {
                case "room860":
                    Fill_Room860(r);
                    break;
                case "lockroom":
                    Fill_Lockroom(r);
                    break;
                case "lockroom2":
                    Fill_Lockroom2(r);
                    break;
                case "gatea":
                    Fill_Gatea(r);
                    break;
                case "gateaentrance":
                    Fill_Gateaentrance(r);
                    break;
                case "exit1":
                    Fill_Exit1(r);
                    break;
                case "roompj":
                    Fill_Roompj(r);
                    break;
                case "room079":
                    Fill_Room079(r);
                    break;
                case "checkpoint1":
                    Fill_Checkpoint1(r);
                    break;
                case "checkpoint2":
                    Fill_Checkpoint2(r);
                    break;
                case "room2pit":
                    Fill_Room2pit(r);
                    break;
                case "room2testroom2":
                    Fill_Room2testroom2(r);
                    break;
                case "room3tunnel":
                    Fill_Room3tunnel(r);
                    break;
                case "room2toilets":
                    Fill_Room2toilets(r);
                    break;
                case "room2storage":
                    Fill_Room2storage(r);
                    break;
                case "room2sroom":
                    Fill_Room2sroom(r);
                    break;
                case "room2shaft":
                    Fill_Room2shaft(r);
                    break;
                case "room2poffices":
                    Fill_Room2poffices(r);
                    break;
                case "room2poffices2":
                    Fill_Room2poffices2(r);
                    break;
                case "room2elevator":
                    Fill_Room2elevator(r);
                    break;
                case "room2cafeteria":
                    Fill_Room2cafeteria(r);
                    break;
                case "room2nuke":
                    Fill_Room2nuke(r);
                    break;
                case "room2tunnel":
                    Fill_Room2tunnel(r);
                    break;
                case "008":
                    Fill_Room008(r);
                    break;
                case "room035":
                    Fill_Room035(r);
                    break;
                case "room513":
                    Fill_Room513(r);
                    break;
                case "room966":
                    Fill_Room966(r);
                    break;
                case "room3storage":
                    Fill_Room3storage(r);
                    break;
                case "room049":
                    Fill_Room049(r);
                    break;
                case "room2_2":
                    Fill_Room22(r);
                    break;
                case "room012":
                    Fill_Room012(r);
                    break;
                case "tunnel2":
                    Fill_Tunnel2(r);
                    break;
                case "room2pipes":
                    Fill_Room2pipes(r);
                    break;
                case "room3pit":
                    Fill_Room3pit(r);
                    break;
                case "room2servers":
                    Fill_Room2servers(r);
                    break;
                case "room3servers":
                    Fill_Room3servers(r);
                    break;
                case "room3servers2":
                    Fill_Room3servers2(r);
                    break;
                case "testroom":
                    Fill_Testroom(r);
                    break;
                case "room2closets":
                    Fill_Room2closets(r);
                    break;
                case "room2offices":
                    Fill_Room2offices(r);
                    break;
                case "room2offices2":
                    Fill_Room2offices2(r);
                    break;
                case "room2offices3":
                    Fill_Room2offices3(r);
                    break;
                case "start":
                    Fill_Start(r);
                    break;
                case "room2scps":
                    Fill_Room2scps(r);
                    break;
                case "room205":
                    Fill_Room205(r);
                    break;
                case "endroom":
                    Fill_Endroom(r);
                    break;
                case "endroomc":
                    Fill_Endroomc(r);
                    break;
                case "coffin":
                    Fill_Coffin(r);
                    break;
                case "room2tesla":
                case "room2tesla_lcz":
                case "room2tesla_hcz":
                    Fill_Room2tesla(r);
                    break;
                case "room2doors":
                    Fill_Room2doors(r);
                    break;
                case "914":
                    Fill_Room914(r);
                    break;
                case "173":
                    Fill_Room173(r);
                    break;
                case "room2ccont":
                    Fill_Room2ccont(r);
                    break;
                case "room106":
                    Fill_Room106(r);
                    break;
                case "room1archive":
                    Fill_Room1archive(r);
                    break;
                case "room2test1074":
                    Fill_Room2test1074(r);
                    break;
                case "room1123":
                    Fill_Room1123(r);
                    break;
                case "pocketdimension":
                    Fill_Pocketdimension(r);
                    break;
                case "room3z3":
                    Fill_Room3z3(r);
                    break;
                case "room2_3":
                case "room3_3":
                    Fill_Room23(r);
                    break;
                case "room1lifts":
                    Fill_Room1lifts(r);
                    break;
                case "room2servers2":
                    Fill_Room2servers2(r);
                    break;
                case "room2gw":
                case "room2gw_b":
                    Fill_Room2gw(r);
                    break;
                case "room3gw":
                    Fill_Room3gw(r);
                    break;
                case "room1162":
                    Fill_Room1162(r);
                    break;
                case "room2scps2":
                    Fill_Room2scps2(r);
                    break;
                case "room3offices":
                    Fill_Room3offices(r);
                    break;
                case "room2offices4":
                    Fill_Room2offices4(r);
                    break;
                case "room2sl":
                    Fill_Room2sl(r);
                    break;
                case "room2_4":
                    Fill_Room24(r);
                    break;
                case "room3z2":
                    Fill_Room3z2(r);
                    break;
                case "lockroom3":
                    Fill_Lockroom3(r);
                    break;
                case "medibay":
                    Fill_Medibay(r);
                    break;
                case "room2cpit":
                    Fill_Room2cpit(r);
                    break;
                case "dimension1499":
                    Fill_Dimension1499(r);
                    break;
                default:
                    break;
            }
        }
        private static void Fill_Room860(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            //the wooden door
            r.Objects[2] = LoadMesh("GFX.Map.Forest.Door_frame.b3d");
            PositionEntity(r.Objects[2],r.x + 184.0f * rs,0,r.z,true);
            ScaleEntity(r.Objects[2],45.0f*rs,45.0f*rs,80.0f*rs,true);
            EntityParent(r.Objects[2],r.obj);
            r.Objects[3] =  LoadMesh("GFX.Map.Forest.Door.b3d");
            PositionEntity(r.Objects[3],r.x + 112.0f * rs,0,r.z+0.05f,true);
            EntityType(r.Objects[3], 1);
            ScaleEntity(r.Objects[3],46.0f*rs,45.0f*rs,46.0f*rs,true);
            EntityParent(r.Objects[3],r.obj);
            r.Objects[4] = CopyEntity(r.Objects[3]);
            PositionEntity(r.Objects[4],r.x + 256.0f * rs,0,r.z-0.05f,true);
            RotateEntity(r.Objects[4], 0,180,0);
            ScaleEntity(r.Objects[4],46.0f*rs,45.0f*rs,46.0f*rs,true);
            EntityParent(r.Objects[4],r.obj);
            //			;DrawPortal stuff
            //			Local dp.DrawPortal = CreateDrawPortal(r\x + 184.0f * RoomScale,164.25f*RoomScale,r\z,0.0f,0.0f,0.0f,328.5f*RoomScale,328.5f*RoomScale);,r\x,r\y+5.2f,r\z,0.0f,0.0f,0.0f)
            //			r\dp=dp
            //			EntityParent dp\portal,r\obj
            //
            //			CameraClsColor dp\cam,98,133,162
            //			CameraRange dp\cam,RoomScale,8.0f
            //			CameraFogRange dp\cam,0.5f,8.0f
            //			CameraFogColor dp\cam,98,133,162
            //			CameraFogMode dp\cam,1
            //doors to observation booth
            d = DoorSystem.CreateDoor(r.zone, r.x + 928.0f * rs, 0, r.z + 640.0f * rs, 0, r, false, 0, 0, "ABCD");
            d = DoorSystem.CreateDoor(r.zone, r.x + 928.0f * rs, 0, r.z - 640.0f * rs, 0, r, true, 0, 0, "ABCD");
            d.AutoClose = false;
            //doors to the room itself
            d = DoorSystem.CreateDoor(r.zone, r.x+416.0f*rs, 0, r.z - 640.0f * rs, 0, r, false, 0, 1);
            d = DoorSystem.CreateDoor(r.zone, r.x+416.0f*rs, 0, r.z + 640.0f * rs, 0, r, false, 0, 1);
            //the forest
            if (!ZoneInfo.HasCustomForest) { ForestSystem.PlaceForest(r); }
            //EntityParent fr\Forest_Pivot,r\obj
            //			PositionEntity dp\cam,EntityX(fr\Door[0],True),r\y+31.0f,EntityZ(fr\Door[0],True),True
            //			dp\camyaw=EntityYaw(fr\Door[0],True)
            //			RotateEntity dp\cam, 0, dp\camyaw, 0, True
            //			MoveEntity dp\cam, 0,0,0.5f
            //
            //			;place the camera at the door
            //			For xtemp=0 To -1;gridsize-1
            //				if fr\grid[xtemp+((gridsize-1)*gridsize)]=3 
            //					PositionEntity dp\cam,r\x+(xtemp*8.0f),r\y+30.5f,r\z+((gridsize-2)*8.0f)+0.2f,True
            //					;make the camera point the right way
            //					ytemp=CreatePivot(r\obj)
            //					ztemp=CreatePivot()
            //					PositionEntity ytemp,EntityX(dp\cam,True),EntityY(dp\cam,True),EntityZ(dp\cam,True),True
            //					PositionEntity ztemp,EntityX(dp\cam,True),EntityY(dp\cam,True),EntityZ(dp\cam,True),True
            //					TranslateEntity ztemp,0.0f,0.0f,-10.0f,True
            //					PointEntity ytemp,ztemp
            //					dp\campitch=EntityPitch(ytemp)
            //					dp\camyaw=EntityYaw(ytemp)
            //					r\Objects[4]=ytemp; ytemp = 0
            //					FreeEntity ztemp; ztemp = 0
            //				EndIf
            //			Next
            //
            //			EntityParent dp\cam,fr\Forest_Pivot
            it = ItemSystem.CreateItem("Document SCP-860-1", "paper", r.x + 672.0f * rs, r.y + 176.0f * rs, r.z + 335.0f * rs);
            RotateEntity(it.Collider, 0, r.Angle+10, 0);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Document SCP-860", "paper", r.x + 1152.0f * rs, r.y + 176.0f * rs, r.z - 384.0f * rs);
            RotateEntity(it.Collider, 0, r.Angle+170, 0);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Lockroom(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x - 736.0f * rs, 0, r.z - 104.0f * rs, 0, r, true);
            d.Timer = 70 * 5; d.AutoClose = false; d.Open = false;
            EntityParent(d.Buttons[0], -1);
            PositionEntity(d.Buttons[0], r.x - 288.0f * rs, 0.7f, r.z - 640.0f * rs);
            EntityParent(d.Buttons[0], r.obj);
            FreeEntity(d.Buttons[1]); d.Buttons[1] = -1;
            d2 = DoorSystem.CreateDoor(r.zone, r.x + 104.0f * rs, 0, r.z + 736.0f * rs, 270, r, true);
            d2.Timer = 70 * 5; d2.AutoClose = false; d2.Open = false;
            EntityParent(d2.Buttons[0], -1);
            PositionEntity(d2.Buttons[0], r.x + 640.0f * rs, 0.7f, r.z + 288.0f * rs);
            RotateEntity(d2.Buttons[0], 0, 90, 0);
            EntityParent(d2.Buttons[0], r.obj);
            FreeEntity(d2.Buttons[1]); d2.Buttons[1] = -1;
            d.LinkedDoor = d2;
            d2.LinkedDoor = d;
            sc = SecurityCamSystem.Create(r.x - 688.0f * rs, r.y + 384 * rs, r.z + 688.0f * rs, r, true);
            sc.Angle = 45 + 180;
            sc.Turn = 45;
            sc.ScrTexture = 1;
            EntityTexture(sc.ScrObj, SecurityCamSystem.ScreenTexs[sc.ScrTexture]);
            TurnEntity(sc.CameraObj, 40, 0, 0);
            EntityParent(sc.Obj, r.obj);
            PositionEntity(sc.ScrObj, r.x + 668 * rs, 1.1f, r.z - 96.0f * rs);
            TurnEntity(sc.ScrObj, 0, 90, 0);
            EntityParent(sc.ScrObj, r.obj);
            sc = SecurityCamSystem.Create(r.x - 112.0f * rs, r.y + 384 * rs, r.z + 112.0f * rs, r, true);
            sc.Angle = 45;
            sc.Turn = 45;
            sc.ScrTexture = 1;
            EntityTexture(sc.ScrObj, SecurityCamSystem.ScreenTexs[sc.ScrTexture]);
            TurnEntity(sc.CameraObj, 40, 0, 0);
            EntityParent(sc.Obj, r.obj);
            PositionEntity(sc.ScrObj, r.x + 96.0f * rs, 1.1f, r.z - 668.0f * rs);
            EntityParent(sc.ScrObj, r.obj);
            em = ParticleSystem.CreateEmitter(r.x - 175.0f * rs, 370.0f * rs, r.z + 656.0f * rs, 0);
            TurnEntity(em.Obj, 90, 0, 0, true);
            EntityParent(em.Obj, r.obj);
            em.RandAngle = 20;
            em.Speed = 0.05f;
            em.SizeChange = 0.007f;
            em.AChange = -0.006f;
            em.Gravity = -0.24f;
            em = ParticleSystem.CreateEmitter(r.x - 655.0f * rs, 370.0f * rs, r.z + 240.0f * rs, 0);
            TurnEntity(em.Obj, 90, 0, 0, true);
            EntityParent(em.Obj, r.obj);
            em.RandAngle = 20;
            em.Speed = 0.05f;
            em.SizeChange = 0.007f;
            em.AChange = -0.006f;
            em.Gravity = -0.24f;
            //This needs more work
            //dem.DevilEmitters = CreateDevilEmitter(r\x-175.0f*RoomScale,r\y+370.0f*RoomScale,r\z+656.0f*RoomScale,r,2)
            //dem\isDeconGas = True
            //dem.DevilEmitters = CreateDevilEmitter(r\x-655.0f*RoomScale,r\y+370.0f*RoomScale,r\z+240.0f*RoomScale,r,2)
            //dem\isDeconGas = True
            //[End Block]
        }

        private static void Fill_Lockroom2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            for (i = 0; i <= 5; i++)
            {
                de = DecalSystem.Create(Rand(2,3), r.x+Rnd(-392,520)*rs, 3.0f*rs+Rnd(0,0.001f), r.z+Rnd(-392,520)*rs,90,Rnd(360),0);
                de.Size = Rnd(0.3f,0.6f);
                ScaleSprite(de.Obj, de.Size,de.Size);
                DecalSystem.Create(Rand(15,16), r.x+Rnd(-392,520)*rs, 3.0f*rs+Rnd(0,0.001f), r.z+Rnd(-392,520)*rs,90,Rnd(360),0);
                de.Size = Rnd(0.1f,0.6f);
                ScaleSprite(de.Obj, de.Size,de.Size);
                DecalSystem.Create(Rand(15,16), r.x+Rnd(-0.5f,0.5f), 3.0f*rs+Rnd(0,0.001f), r.z+Rnd(-0.5f,0.5f),90,Rnd(360),0);
                de.Size = Rnd(0.1f,0.6f);
                ScaleSprite(de.Obj, de.Size,de.Size);
            }
            sc = SecurityCamSystem.Create(r.x + 512.0f * rs, r.y + 384 * rs, r.z + 384.0f * rs, r, true);
            sc.Angle = 45 + 90;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 40, 0, 0);
            EntityParent(sc.Obj, r.obj);
            PositionEntity(sc.ScrObj, r.x + 668 * rs, 1.1f, r.z - 96.0f * rs);
            TurnEntity(sc.ScrObj, 0, 90, 0);
            EntityParent(sc.ScrObj, r.obj);
            sc = SecurityCamSystem.Create(r.x - 384.0f * rs, r.y + 384 * rs, r.z - 512.0f * rs, r, true);
            sc.Angle = 45 + 90 + 180;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 40, 0, 0);
            EntityParent(sc.Obj, r.obj);
            PositionEntity(sc.ScrObj, r.x + 96.0f * rs, 1.1f, r.z - 668.0f * rs);
            EntityParent(sc.ScrObj, r.obj);
            //[End Block]
        }

        private static void Fill_Gatea(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x - 4064.0f * rs, r.y-1280.0f*rs, r.z + 3952.0f * rs, 0, r, false);
            r.RoomDoors[2].AutoClose = false; r.RoomDoors[2].Open = false;
            d2 = DoorSystem.CreateDoor(r.zone, r.x, r.y, r.z - 1024.0f * rs, 0, r, false);
            d2.AutoClose = false; d2.Open = false; d2.Locked = true;
            d2 = DoorSystem.CreateDoor(r.zone, r.x-1440*rs, r.y-480.0f*rs, r.z + 2328.0f * rs, 0, r, false, 0, 2);
            if (EventSystem.SelectedEnding  == "A2")
            {
                d2.AutoClose = false; d2.Open = true; d2.Locked = true;
            }
            else
            {
                d2.AutoClose = false; d2.Open = false; d2.Locked = false;
            }
            PositionEntity(d2.Buttons[0], r.x-1320.0f*rs, EntityY(d2.Buttons[0],true), r.z + 2288.0f*rs, true);
            PositionEntity(d2.Buttons[1], r.x-1584*rs, EntityY(d2.Buttons[0],true), r.z + 2488.0f*rs, true);
            RotateEntity(d2.Buttons[1], 0, 90, 0, true);
            d2 = DoorSystem.CreateDoor(r.zone, r.x-1440*rs, r.y-480.0f*rs, r.z + 4352.0f * rs, 0, r, false, 0, 2);
            if (EventSystem.SelectedEnding  == "A2")
            {
                d2.AutoClose = false; d2.Open = true; d2.Locked = true;
            }
            else
            {
                d2.AutoClose = false; d2.Open = false; d2.Locked = false;
            }
            PositionEntity(d2.Buttons[0], r.x-1320.0f*rs, EntityY(d2.Buttons[0],true), r.z + 4384.0f*rs, true);
            RotateEntity(d2.Buttons[0], 0, 180, 0, true);
            PositionEntity(d2.Buttons[1], r.x-1584.0f*rs, EntityY(d2.Buttons[0],true), r.z + 4232.0f*rs, true);
            RotateEntity(d2.Buttons[1], 0, 90, 0, true);
            foreach (var otherRoom in MapSystem.All)
            {
                if (otherRoom.RoomName  == "exit1")
                {
                    r.Objects[1]=r2.Objects[1];
                    r.Objects[2]=r2.Objects[2];
                }
                else if (r2.RoomName  == "gateaentrance")
                {
                    //ylempi hissi
                    r.RoomDoors[1] = DoorSystem.CreateDoor(0, r.x+1544.0f*rs, r.y, r.z-64.0f*rs, 90, r, false, 3);
                    r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = false;
                    PositionEntity(r.RoomDoors[1].Buttons[0],r.x+1584*rs, EntityY(r.RoomDoors[1].Buttons[0],true), r.z+80*rs, true);
                    PositionEntity(r.RoomDoors[1].Buttons[1],r.x+1456*rs, EntityY(r.RoomDoors[1].Buttons[1],true), r.z-208*rs, true);
                    r2.Objects[1] = CreatePivot();
                    PositionEntity(r2.Objects[1], r.x+1848.0f*rs, r.y+240.0f*rs, r.z-64.0f*rs, true);
                    EntityParent(r2.Objects[1], r.obj);
                }
            }
            //106; n spawnpoint
            r.Objects[3]=CreatePivot();
            PositionEntity(r.Objects[3], r.x+1216.0f*rs, r.y, r.z+2112.0f*rs, true);
            EntityParent(r.Objects[3], r.obj);
            //sillan loppup��
            r.Objects[4]=CreatePivot();
            PositionEntity(r.Objects[4], r.x, r.y+96.0f*rs, r.z+6400.0f*rs, true);
            EntityParent(r.Objects[4], r.obj);
            //vartiotorni 1
            r.Objects[5]=CreatePivot();
            PositionEntity(r.Objects[5], r.x+1784.0f*rs, r.y+2124.0f*rs, r.z+4512.0f*rs, true);
            EntityParent(r.Objects[5], r.obj);
            //vartiotorni 2
            r.Objects[6]=CreatePivot();
            PositionEntity(r.Objects[6], r.x-5048.0f*rs, r.y+1912.0f*rs, r.z+4656.0f*rs, true);
            EntityParent(r.Objects[6], r.obj);
            //sillan takareuna
            r.Objects[7]=CreatePivot();
            PositionEntity(r.Objects[7], r.x+1824.0f*rs, r.y+224.0f*rs, r.z+7056.0f*rs, true);
            EntityParent(r.Objects[7], r.obj);
            //sillan takareuna2
            r.Objects[8]=CreatePivot();
            PositionEntity(r.Objects[8], r.x-1824.0f*rs, r.y+224.0f*rs, r.z+7056.0f*rs, true);
            EntityParent(r.Objects[8], r.obj);
            //"valopyssy"
            r.Objects[9]=CreatePivot();
            PositionEntity(r.Objects[9], r.x+2624.0f*rs, r.y+992.0f*rs, r.z+6157.0f*rs, true);
            EntityParent(r.Objects[9], r.obj);
            //objects[10] = valopyssyn yl�osa
            //tunnelin loppu
            r.Objects[11]=CreatePivot();
            PositionEntity(r.Objects[11], r.x-4064.0f*rs, r.y-1248.0f*rs, r.z-1696.0f*rs, true);
            EntityParent(r.Objects[11], r.obj);
            r.Objects[13]=LoadMesh("GFX.Map.Gateawall1.b3d",r.obj);
            PositionEntity(r.Objects[13], r.x-4308.0f*rs, r.y-1045.0f*rs, r.z+544.0f*rs, true);
            EntityColor(r.Objects[13], 25,25,25);
            EntityType(r.Objects[13],1);
            //EntityFX(r\Objects[13],1)
            r.Objects[14]=LoadMesh("GFX.Map.Gateawall2.b3d",r.obj);
            PositionEntity(r.Objects[14], r.x-3820.0f*rs, r.y-1045.0f*rs, r.z+544.0f*rs, true);
            EntityColor(r.Objects[14], 25,25,25);
            EntityType(r.Objects[14],1);
            //EntityFX(r\Objects[14],1)
            r.Objects[15]=CreatePivot(r.obj);
            PositionEntity(r.Objects[15], r.x-3568.0f*rs, r.y-1089.0f*rs, r.z+4944.0f*rs, true);
            r.Objects[16] = LoadMesh("GFX.Map.Gatea_hitbox1.b3d",r.obj);
            EntityPickMode(r.Objects[16],2);
            EntityType(r.Objects[16],1);
            EntityAlpha(r.Objects[16],0.0f);
            //[End Block]
        }

        private static void Fill_Gateaentrance(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            //alempi hissi
            r.RoomDoors[0] = DoorSystem.CreateDoor(0, r.x+744.0f*rs, 0, r.z+512.0f*rs, 90, r, true, 3);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true;
            PositionEntity(r.RoomDoors[0].Buttons[1],r.x+688*rs, EntityY(r.RoomDoors[0].Buttons[1],true), r.z+368*rs, true);
            PositionEntity(r.RoomDoors[0].Buttons[0],r.x+784*rs, EntityY(r.RoomDoors[0].Buttons[0],true), r.z+656*rs, true);
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], r.x+1048.0f*rs, 0, r.z+512.0f*rs, true);
            EntityParent(r.Objects[0], r.obj);
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z - 360.0f * rs, 0, r, false, 1, 5);
            r.RoomDoors[1].Dir = 1; r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = false;
            PositionEntity(r.RoomDoors[1].Buttons[1], r.x+416*rs, EntityY(r.RoomDoors[0].Buttons[1],true), r.z-576*rs, true);
            RotateEntity(r.RoomDoors[1].Buttons[1],0,r.Angle-90,0,true);
            PositionEntity(r.RoomDoors[1].Buttons[0], r.x, 20.0f, r.z, true);
            //[End Block]
        }

        private static void Fill_Exit1(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x+4356.0f*rs, 9767.0f*rs, r.z+2588.0f*rs, true);
            r.RoomDoors[4] = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z - 320.0f * rs, 0, r, false, 1, 5);
            r.RoomDoors[4].Dir = 1; r.RoomDoors[4].AutoClose = false; r.RoomDoors[4].Open = false;
            PositionEntity(r.RoomDoors[4].Buttons[1], r.x+352*rs, 0.7f, r.z-528*rs, true);
            RotateEntity(r.RoomDoors[4].Buttons[1],0,r.Angle-90,0,true);
            PositionEntity(r.RoomDoors[4].Buttons[0], r.x, 7.0f, r.z, true);
            //k�yt�v�n takaosa
            r.Objects[3] = CreatePivot();
            PositionEntity(r.Objects[3], r.x-7680.0f*rs, 10992.0f*rs, r.z-27048.0f*rs, true);
            EntityParent(r.Objects[3], r.obj);
            //oikean puolen watchpoint 1
            r.Objects[4] = CreatePivot();
            PositionEntity(r.Objects[4], r.x+5203.36f*rs, 12128.0f*rs, r.z-1739.19f*rs, true);
            EntityParent(r.Objects[4], r.obj);
            //oikean puolen watchpoint 2
            r.Objects[5] = CreatePivot();
            PositionEntity(r.Objects[5], r.x+4363.02f*rs, 10536.0f*rs, r.z+2766.16f*rs, true);
            EntityParent(r.Objects[5], r.obj);
            //vasemman puolen watchpoint 1
            r.Objects[6] = CreatePivot();
            PositionEntity(r.Objects[6], r.x+5192.0f*rs, 12192.0f*rs, r.z-1760.0f*rs, true);
            EntityParent(r.Objects[6], r.obj);
            //vasemman puolen watchpoint 2
            r.Objects[7] = CreatePivot();
            PositionEntity(r.Objects[7], r.x+5192.0f*rs, 12192.0f*rs, r.z-4352.0f*rs, true);
            EntityParent(r.Objects[7], r.obj);
            //alempi hissi
            r.RoomDoors[0] = DoorSystem.CreateDoor(0, r.x+720.0f*rs, 0, r.z+1432.0f*rs, 0, r, true, 3);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true;
            MoveEntity(r.RoomDoors[0].Buttons[0],0,0,22.0f*rs);
            MoveEntity(r.RoomDoors[0].Buttons[1],0,0,22.0f*rs);
            r.Objects[8] = CreatePivot();
            PositionEntity(r.Objects[8], r.x+720.0f*rs, 0, r.z+1744.0f*rs, true);
            EntityParent(r.Objects[8], r.obj);
            //ylempi hissi
            r.RoomDoors[1] = DoorSystem.CreateDoor(0, r.x-5424.0f*rs, 10784.0f*rs, r.z-1380.0f*rs, 0, r, false, 3);
            r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = false;
            MoveEntity(r.RoomDoors[1].Buttons[0],0,0,22.0f*rs);
            MoveEntity(r.RoomDoors[1].Buttons[1],0,0,22.0f*rs);
            r.Objects[9] = CreatePivot();
            PositionEntity(r.Objects[9], r.x-5424.0f*rs, 10784.0f*rs, r.z-1068.0f*rs, true);
            EntityParent(r.Objects[9], r.obj);
            r.RoomDoors[2] = DoorSystem.CreateDoor(0, r.x+4352.0f*rs, 10784.0f*rs, r.z-492.0f*rs, 0, r, false);
            r.RoomDoors[2].AutoClose = false; r.RoomDoors[2].Open = false;
            r.RoomDoors[3] = DoorSystem.CreateDoor(0, r.x+4352.0f*rs, 10784.0f*rs, r.z+500.0f*rs, 0, r, false);
            r.RoomDoors[3].AutoClose = false; r.RoomDoors[3].Open = false;
            //walkway
            r.Objects[10] = CreatePivot();
            PositionEntity(r.Objects[10], r.x+4352.0f*rs, 10778.0f*rs, r.z+1344.0f*rs, true);
            EntityParent(r.Objects[10], r.obj);
            //"682"
            r.Objects[11] = CreatePivot();
            PositionEntity(r.Objects[11], r.x+2816.0f*rs, 11024.0f*rs, r.z-2816.0f*rs, true);
            EntityParent(r.Objects[11], r.obj);
            //r\Objects[12] = 682; n k�si
            //"valvomon" takaovi
            r.RoomDoors[5] = DoorSystem.CreateDoor(0, r.x+3248.0f*rs, 9856.0f*rs, r.z+6400.0f*rs, 0, r, false, 0, 0, "ABCD");
            r.RoomDoors[5].AutoClose = false; r.RoomDoors[5].Open = false;
            //"valvomon" etuovi
            d = DoorSystem.CreateDoor(0, r.x+3072.0f*rs, 9856.0f*rs, r.z+5800.0f*rs, 90, r, false, 0, 3);
            d.AutoClose = false; d.Open = false;
            r.Objects[14] = CreatePivot();
            PositionEntity(r.Objects[14], r.x+3536.0f*rs, 10256.0f*rs, r.z+5512.0f*rs, true);
            EntityParent(r.Objects[14], r.obj);
            r.Objects[15] = CreatePivot();
            PositionEntity(r.Objects[15], r.x+3536.0f*rs, 10256.0f*rs, r.z+5824.0f*rs, true);
            EntityParent(r.Objects[15], r.obj);
            r.Objects[16] = CreatePivot();
            PositionEntity(r.Objects[16], r.x+3856.0f*rs, 10256.0f*rs, r.z+5512.0f*rs, true);
            EntityParent(r.Objects[16], r.obj);
            r.Objects[17] = CreatePivot();
            PositionEntity(r.Objects[17], r.x+3856.0f*rs, 10256.0f*rs, r.z+5824.0f*rs, true);
            EntityParent(r.Objects[17], r.obj);
            //MTF; n spawnpoint
            r.Objects[18] = CreatePivot();
            //PositionEntity(r\Objects[18], r\x+3727.0f*RoomScale, 10066.0f*RoomScale, r\z+6623.0f*RoomScale, True)
            PositionEntity(r.Objects[18], r.x+3250.0f*rs, 9896.0f*rs, r.z+6623.0f*rs, true);
            EntityParent(r.Objects[18], r.obj);
            //piste johon helikopterit pakenee nukea
            r.Objects[19] = CreatePivot();
            PositionEntity(r.Objects[19], r.x+3808.0f*rs, 12320.0f*rs, r.z-13568.0f*rs, true);
            EntityParent(r.Objects[19], r.obj);
            //[End Block]
        }

        private static void Fill_Roompj(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            it = ItemSystem.CreateItem("Document SCP-372", "paper", r.x + 800.0f * rs, r.y + 176.0f * rs, r.z + 1108.0f * rs);
            RotateEntity(it.Collider, 0, r.Angle, 0);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Radio Transceiver", "radio", r.x + 800.0f * rs, r.y + 112.0f * rs, r.z + 944.0f * rs);
            it.State = 80.0f;
            EntityParent(it.Collider, r.obj);
            r.Objects[3] = LoadMesh("GFX.Map.372_hb.b3d",r.obj);
            EntityPickMode(r.Objects[3],2);
            EntityType(r.Objects[3],1);
            EntityAlpha(r.Objects[3],0.0f);
            d = DoorSystem.CreateDoor(r.zone, r.x, r.y, r.z-368.0f*rs, 0, r, true, 1, 2);
            d.AutoClose = false;
            PositionEntity(d.Buttons[0], r.x - 496.0f * rs, 0.7f, r.z - 272.0f * rs, true);
            TurnEntity(d.Buttons[0], 0, 90, 0);
            //[End Block]
        }

        private static void Fill_Room079(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x, -448.0f*rs, r.z + 1136.0f * rs, 0, r, false, 1, 4);
            d.Dir = 1; d.AutoClose = false; d.Open = false;
            PositionEntity(d.Buttons[1], r.x + 224.0f * rs, -250*rs, r.z + 918.0f * rs, true);
            //TurnEntity(d\buttons[0],0,-90,0,True)
            PositionEntity(d.Buttons[0], r.x - 240.0f * rs, -250*rs, r.z + 1366.0f * rs, true);
            //TurnEntity(d\buttons[1],0, 90,0,True)
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x + 1456.0f*rs, -448.0f*rs, r.z + 976.0f * rs, 0, r, false, 1, 3);
            r.RoomDoors[0].Dir = 1; r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = false;
            PositionEntity(r.RoomDoors[0].Buttons[1], r.x + 1760.0f * rs, -250*rs, r.z + 1236.0f * rs, true);
            TurnEntity(r.RoomDoors[0].Buttons[0],0,-90-90,0,true);
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 1760.0f * rs, -240*rs, r.z + 740.0f * rs, true);
            TurnEntity(r.RoomDoors[0].Buttons[1],0, 90-90,0,true);
            DoorSystem.CreateDoor(0, r.x + 1144.0f*rs, -448.0f*rs, r.z + 704.0f * rs, 90, r, false, 0, -1);
            r.Objects[0] = LoadAnimMesh("GFX.Map.079.b3d");
            ScaleEntity(r.Objects[0], 1.3f, 1.3f, 1.3f, true);
            PositionEntity(r.Objects[0], r.x + 1856.0f*rs, -560.0f*rs, r.z-672.0f*rs, true);
            EntityParent(r.Objects[0], r.obj);
            TurnEntity(r.Objects[0],0,180,0,true);
            r.Objects[1] = CreateSprite(r.Objects[0]);
            SpriteViewMode(r.Objects[1],2);
            PositionEntity(r.Objects[1], 0.082f, 0.119f, 0.010f);
            ScaleSprite(r.Objects[1],0.18f*0.5f,0.145f*0.5f);
            TurnEntity(r.Objects[1],0,13.0f,0);
            MoveEntity(r.Objects[1], 0,0,-0.022f);
            EntityTexture(r.Objects[1],SecurityCamSystem.OldAiPics(0));
            HideEntity(r.Objects[1]);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], r.x + 1184.0f*rs, -448.0f*rs, r.z+1792.0f*rs, true);
            de = DecalSystem.Create(3,  r.x + 1184.0f*rs, -448.0f*rs+0.01f, r.z+1792.0f*rs,90,Rnd(360),0);
            de.Size = 0.5f;
            ScaleSprite(de.Obj, de.Size,de.Size);
            EntityParent(de.Obj, r.obj);
            //[End Block]
        }

        private static void Fill_Checkpoint1(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.RoomDoors[0] = DoorSystem.CreateDoor(0, r.x + 48.0f*rs, 0, r.z - 128.0f * rs, 0, r, false, 0, 3);
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x - 152.0f * rs, EntityY(r.RoomDoors[0].Buttons[0],true), r.z - 352.0f * rs, true);
            PositionEntity(r.RoomDoors[0].Buttons[1], r.x - 152.0f * rs, EntityY(r.RoomDoors[0].Buttons[1],true), r.z + 96.0f * rs, true);
            r.RoomDoors[1] = DoorSystem.CreateDoor(0, r.x - 352.0f*rs, 0, r.z - 128.0f * rs, 0, r, false, 0, 3);
            //FreeEntity r\RoomDoors[1]\buttons[0]
            //FreeEntity r\RoomDoors[1]\buttons[1]
            r.RoomDoors[1].LinkedDoor = r.RoomDoors[0];
            r.RoomDoors[0].LinkedDoor = r.RoomDoors[1];
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x + 720.0f*rs, 120.0f*rs, r.z+333.0f*rs, true);
            r.RoomDoors[0].Timer = 70 * 5;
            r.RoomDoors[1].Timer = 70 * 5;
            sc = SecurityCamSystem.Create(r.x+192.0f*rs, r.y+704.0f*rs, r.z-960.0f*rs, r);
            sc.Angle = 45;
            sc.Turn = 0;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            r.Objects[2] = CopyEntity(MapAssets.Monitor2,r.obj);
            ScaleEntity(r.Objects[2], 2.0f, 2.0f, 2.0f);
            PositionEntity(r.Objects[2], r.x - 152.0f*rs, 384.0f*rs, r.z+124.0f*rs, true);
            RotateEntity(r.Objects[2],0,180,0);
            // EntityFX r.Objects[2],1
            r.Objects[3] = CopyEntity(MapAssets.Monitor2,r.obj);
            ScaleEntity(r.Objects[3], 2.0f, 2.0f, 2.0f);
            PositionEntity(r.Objects[3], r.x - 152.0f*rs, 384.0f*rs, r.z-380.0f*rs, true);
            RotateEntity(r.Objects[3],0,0,0);
            // EntityFX r.Objects[3],1
            if (MapSystem.MapTemp[(int)Math.Floor(r.x / 8.0f), (int)Math.Floor(r.z / 8.0f) - 1] == 0)
            {
                DoorSystem.CreateDoor(r.zone, r.x, 0, r.z - 4.0f, 0, r, false, 0, 0, "GEAR");
            }
            //[End Block]
        }

        private static void Fill_Checkpoint2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.RoomDoors[0]= DoorSystem.CreateDoor(0, r.x - 48.0f*rs, 0, r.z + 128.0f * rs, 0, r, false, 0, 5);
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 152.0f * rs, EntityY(r.RoomDoors[0].Buttons[0],true), r.z - 96.0f * rs, true);
            PositionEntity(r.RoomDoors[0].Buttons[1], r.x + 152.0f * rs, EntityY(r.RoomDoors[0].Buttons[1],true), r.z + 352.0f * rs, true);
            r.RoomDoors[1] = DoorSystem.CreateDoor(0, r.x + 352.0f*rs, 0, r.z + 128.0f * rs, 0, r, false, 0, 5);
            //FreeEntity r\RoomDoors[1]\buttons[0]
            //FreeEntity r\RoomDoors[1]\buttons[1]
            r.RoomDoors[1].LinkedDoor = r.RoomDoors[0];
            r.RoomDoors[0].LinkedDoor = r.RoomDoors[1];
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x - 720.0f*rs, 120.0f*rs, r.z+464.0f*rs, true);
            r.Objects[2] = CopyEntity(MapAssets.Monitor3,r.obj);
            ScaleEntity(r.Objects[2], 2.0f, 2.0f, 2.0f);
            PositionEntity(r.Objects[2], r.x + 152.0f*rs, 384.0f*rs, r.z+380.0f*rs, true);
            RotateEntity(r.Objects[2],0,180,0);
            // EntityFX r.Objects[2],1
            r.Objects[3] = CopyEntity(MapAssets.Monitor3,r.obj);
            ScaleEntity(r.Objects[3], 2.0f, 2.0f, 2.0f);
            PositionEntity(r.Objects[3], r.x + 152.0f*rs, 384.0f*rs, r.z-124.0f*rs, true);
            RotateEntity(r.Objects[3],0,0,0);
            // EntityFX r.Objects[3],1
            r.RoomDoors[0].Timer = 70 * 5;
            r.RoomDoors[1].Timer = 70 * 5;
            if (MapSystem.MapTemp[(int)Math.Floor(r.x / 8.0f), (int)Math.Floor(r.z / 8.0f) - 1] == 0)
            {
                DoorSystem.CreateDoor(r.zone, r.x, 0, r.z - 4.0f, 0, r, false, 0, 0, "GEAR");
            }
            //[End Block]
        }

        private static void Fill_Room2pit(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            i = 0;
            for (int xtemp = -1; xtemp <= 1; xtemp += 2)
            {
                for (int ztemp = -1; ztemp <= 1; ztemp++)
                {
                    em = ParticleSystem.CreateEmitter(r.x + 202.0f * rs * xtemp, 8.0f * rs, r.z + 256.0f * rs * ztemp, 0);
                    em.RandAngle = 30;
                    em.Speed = 0.0045f;
                    em.SizeChange = 0.007f;
                    em.AChange = -0.016f;
                    r.Objects[i] = em.Obj;
                    if (i < 3)
                    {
                        TurnEntity(em.Obj, 0, -90, 0, true);
                    }
                    else
                    {
                        TurnEntity(em.Obj, 0, 90, 0, true);
                    }
                    TurnEntity(em.Obj, -45, 0, 0, true);
                    EntityParent(em.Obj, r.obj);
                    i=i+1;
                }
            }
            r.Objects[6] = CreatePivot();
            PositionEntity(r.Objects[6], r.x + 640.0f * rs, 8.0f * rs, r.z - 896.0f * rs);
            EntityParent(r.Objects[6], r.obj);
            r.Objects[7] = CreatePivot();
            PositionEntity(r.Objects[7], r.x - 864.0f * rs, -400.0f * rs, r.z - 632.0f * rs);
            EntityParent(r.Objects[7],r.obj);
            //[End Block]
        }

        private static void Fill_Room2testroom2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], r.x - 640.0f * rs, 0.5f, r.z - 912.0f * rs);
            EntityParent(r.Objects[0], r.obj);
            r.Objects[1] = CreatePivot();
            PositionEntity(r.Objects[1], r.x - 669.0f * rs, 0.5f, r.z - 16.0f * rs); //r.x - 632;
            EntityParent(r.Objects[1], r.obj);
            tex = LoadTextureHandle("GFX.Map.Glass.png",1+2);
            r.Objects[2] = CreateSprite();
            EntityTexture(r.Objects[2], tex);
            SpriteViewMode(r.Objects[2],2);
            ScaleSprite(r.Objects[2],182.0f*rs*0.5f, 192.0f*rs*0.5f);
            PositionEntity(r.Objects[2], r.x - 632.0f * rs, 224.0f*rs, r.z - 208.0f * rs);
            TurnEntity(r.Objects[2],0,180,0);
            EntityParent(r.Objects[2], r.obj);
            HideEntity(r.Objects[2]);
            FreeTexture(tex);
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x - 240.0f * rs, 0.0f, r.z + 640.0f * rs, 90, r, false, 0, 1);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = false;
            d = DoorSystem.CreateDoor(r.zone, r.x - 512.0f * rs, 0.0f, r.z + 384.0f * rs, 0, r, false, 0);
            d.AutoClose = false; d.Open = false;
            //d = CreateDoor(r\zone, r\x - 816.0f * RoomScale, 0.0f, r\z - 208.0f * RoomScale, 0, r, False, False)
            //d\AutoClose = False; d\open = False
            //FreeEntity(d\buttons[0]) : d\buttons[0]=0
            //FreeEntity(d\buttons[1]) : d\buttons[1]=0
            it = ItemSystem.CreateItem("Level 2 Key Card", "key2", r.x - 914.0f * rs, r.y + 137.0f * rs, r.z + 61.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("S-NAV 300 Navigator", "nav", r.x - 312.0f * rs, r.y + 264.0f * rs, r.z + 176.0f * rs);
            it.State = 20; EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room3tunnel(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x - 190.0f*rs, 4.0f*rs, r.z+190.0f*rs, true);
            //[End Block]
        }

        private static void Fill_Room2toilets(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], r.x + 1040.0f * rs, 192.0f * rs, r.z);
            EntityParent(r.Objects[0], r.obj);
            r.Objects[1] = CreatePivot();
            //PositionEntity(r\Objects[1], r\x + 1270.0f*RoomScale, 0.5f, r\z+570.0f*RoomScale)
            PositionEntity(r.Objects[1], r.x + 1530.0f*rs, 0.5f, r.z+512.0f*rs);
            EntityParent(r.Objects[1], r.obj);
            r.Objects[2] = CreatePivot();
            PositionEntity(r.Objects[2], r.x + 1535.0f*rs, r.y+150.0f*rs, r.z+512.0f*rs);
            EntityParent(r.Objects[2], r.obj);
            //[End Block]
        }

        private static void Fill_Room2storage(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x - 1288.0f * rs, 0, r.z, 270, r);
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x - 760.0f * rs, 0, r.z, 270, r);
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x - 264.0f * rs, 0, r.z, 270, r);
            r.RoomDoors[3] = DoorSystem.CreateDoor(r.zone, r.x + 264.0f * rs, 0, r.z, 270, r);
            r.RoomDoors[4] = DoorSystem.CreateDoor(r.zone, r.x + 760.0f * rs, 0, r.z, 270, r);
            r.RoomDoors[5] = DoorSystem.CreateDoor(r.zone, r.x + 1288.0f * rs, 0, r.z, 270, r);
            for (i = 0; i <= 5; i++)
            {
                MoveEntity(r.RoomDoors[i].Buttons[0], 0,0,-8.0f);
                MoveEntity(r.RoomDoors[i].Buttons[1], 0,0,-8.0f);
                r.RoomDoors[i].AutoClose = false; r.RoomDoors[i].Open = false;
            }
            it = ItemSystem.CreateItem("Document SCP-939", "paper", r.x + 352.0f * rs, r.y + 176.0f * rs, r.z + 256.0f * rs);
            RotateEntity(it.Collider, 0, r.Angle+4, 0);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("9V Battery", "bat", r.x + 352.0f * rs, r.y + 112.0f * rs, r.z + 448.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Empty Cup", "emptycup", r.x-672*rs, 240*rs, r.z+288.0f*rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Level 1 Key Card", "key1", r.x - 672.0f * rs, r.y + 240.0f * rs, r.z + 224.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room2sroom(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 1440.0f * rs, 224.0f * rs, r.z + 32.0f * rs, 90, r, false, 0, 4);
            d.AutoClose = false; d.Open = false;
            it = ItemSystem.CreateItem("Some SCP-420-J", "420", r.x + 1776.0f * rs, r.y + 400.0f * rs, r.z + 427.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Some SCP-420-J", "420", r.x + 1808.0f * rs, r.y + 400.0f * rs, r.z + 435.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Level 5 Key Card", "key5", r.x + 2232.0f * rs, r.y + 392.0f * rs, r.z + 387.0f * rs);
            RotateEntity(it.Collider, 0, r.Angle, 0, true);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Nuclear Device Document", "paper", r.x + 2248.0f * rs, r.y + 440.0f * rs, r.z + 372.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Radio Transceiver", "radio", r.x + 2240.0f * rs, r.y + 320.0f * rs, r.z + 128.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room2shaft(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 1552.0f * rs, r.y, r.z + 552.0f * rs, 0, r, false, 0);
            PositionEntity(d.Buttons[0], EntityX(d.Buttons[0],true), EntityY(d.Buttons[0],true), r.z + 518.0f * rs, true);
            PositionEntity(d.Buttons[1], EntityX(d.Buttons[1],true), EntityY(d.Buttons[1],true), r.z + 575.0f * rs, true);
            d.AutoClose = false; d.Open = false;
            d = DoorSystem.CreateDoor(r.zone, r.x + 256.0f * rs, r.y, r.z + 744.0f * rs, 90, r, false, 0, 2);
            d.AutoClose = false; d.Open = false;
            it = ItemSystem.CreateItem("Level 3 Key Card", "key3", r.x + 1119.0f * rs, r.y + 233.0f * rs, r.z + 494.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("First Aid Kit", "firstaid", r.x + 1035.0f * rs, r.y + 145.0f * rs, r.z + 56.0f * rs);
            EntityParent(it.Collider, r.obj); RotateEntity(it.Collider, 0, 90, 0);
            it = ItemSystem.CreateItem("9V Battery", "bat", r.x + 1930.0f * rs, r.y + 97.0f * rs, r.z + 256.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("9V Battery", "bat", r.x + 1061.0f * rs, r.y + 161.0f * rs, r.z + 494.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("ReVision Eyedrops", "eyedrops", r.x + 1930.0f*rs, r.y + 225.0f * rs, r.z + 128.0f*rs);
            EntityParent(it.Collider, r.obj);
            //Player's position after leaving the pocket dimension
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0],r.x+1560.0f*rs,r.y,r.z+250.0f*rs,true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1],r.x + 1344.0f * rs, -752.0f * rs,r.z - 384.0f * rs,true);
            de = DecalSystem.Create(3,  r.x + 1334.0f*rs, -796.0f*rs+0.01f, r.z-220.0f*rs,90,Rnd(360),0);
            de.Size = 0.25f;
            ScaleSprite(de.Obj, de.Size,de.Size);
            EntityParent(de.Obj, r.obj);
            r.Objects[2] = ButtonSystem.Create(r.x + 1181.0f *rs, r.y + 180.0f * rs, r.z - 512.0f * rs, 0, 270);
            EntityParent(r.Objects[2],r.obj);
            //[End Block]
        }

        private static void Fill_Room2poffices(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 240.0f * rs, 0.0f, r.z + 448.0f * rs, 90, r, false, 0, 0, GameState.AccessCode.ToString());
            PositionEntity(d.Buttons[0], r.x + 248.0f * rs, EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true),true);
            PositionEntity(d.Buttons[1], r.x + 232.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true),true);
            d.AutoClose = false; d.Open = false;
            d = DoorSystem.CreateDoor(r.zone, r.x - 496.0f * rs, 0.0f, r.z, 90, r, false, 0, 0, "ABCD");
            PositionEntity(d.Buttons[0], r.x - 488.0f * rs, EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true),true);
            PositionEntity(d.Buttons[1], r.x - 504.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true),true);
            d.AutoClose = false; d.Open = false; d.Locked = true;
            d = DoorSystem.CreateDoor(r.zone, r.x + 240.0f * rs, 0.0f, r.z - 576.0f * rs, 90, r, false, 0, 0, "7816");
            PositionEntity(d.Buttons[0], r.x + 248.0f * rs, EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true),true);
            PositionEntity(d.Buttons[1], r.x + 232.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true),true);
            d.AutoClose = false; d.Open = false;
            it = ItemSystem.CreateItem("Mysterious Note", "paper", r.x + 736.0f * rs, r.y + 224.0f * rs, r.z + 544.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Ballistic Vest", "vest", r.x + 608.0f * rs, r.y + 112.0f * rs, r.z + 32.0f * rs);
            EntityParent(it.Collider, r.obj); RotateEntity(it.Collider, 0, 90, 0);
            it = ItemSystem.CreateItem("Incident Report SCP-106-0204", "paper", r.x + 704.0f * rs, r.y + 183.0f * rs, r.z - 576.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Journal Page", "paper", r.x + 912 * rs, r.y + 176.0f * rs, r.z - 160.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("First Aid Kit", "firstaid", r.x + 912.0f * rs, r.y + 112.0f * rs, r.z - 336.0f * rs);
            EntityParent(it.Collider, r.obj); RotateEntity(it.Collider, 0, 90, 0);
            //[End Block]
        }

        private static void Fill_Room2poffices2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 240.0f * rs, 0.0f, r.z + 48.0f * rs, 270, r, false, 0, 3);
            PositionEntity(d.Buttons[0], r.x + 224.0f * rs, EntityY(d.Buttons[0],true), r.z + 176.0f * rs,true);
            PositionEntity(d.Buttons[1], r.x + 256.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true),true);
            d.AutoClose = false; d.Open = false;
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x - 432.0f * rs, 0.0f, r.z, 90, r, false, 0, 0, "1234");
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x - 416.0f * rs, EntityY(r.RoomDoors[0].Buttons[0],true), r.z + 176.0f * rs,true);
            FreeEntity(r.RoomDoors[0].Buttons[1]); r.RoomDoors[0].Buttons[1] = -1;
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = false; r.RoomDoors[0].Locked = true;
            de = DecalSystem.Create(0, r.x - 808.0f * rs, 0.005f, r.z - 72.0f * rs, 90, Rand(360), 0);
            EntityParent(de.Obj, r.obj);
            de = DecalSystem.Create(2, r.x - 808.0f * rs, 0.01f, r.z - 72.0f * rs, 90, Rand(360), 0);
            de.Size = 0.3f; ScaleSprite(de.Obj, de.Size, de.Size); EntityParent(de.Obj, r.obj);
            de = DecalSystem.Create(0, r.x - 432.0f * rs, 0.01f, r.z, 90, Rand(360), 0);
            EntityParent(de.Obj, r.obj);
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x - 808.0f * rs, 1.0f, r.z - 72.0f * rs, true);
            it = ItemSystem.CreateItem("Dr. L's Burnt Note", "paper", r.x - 688.0f * rs, 1.0f, r.z - 16.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Dr L's Burnt Note", "paper", r.x - 808.0f * rs, 1.0f, r.z - 72.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("The Modular Site Project", "paper", r.x + 622.0f*rs, r.y + 125.0f*rs, r.z - 73.0f*rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room2elevator(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x+888.0f*rs, 240.0f*rs, r.z, true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x+1024.0f*rs-0.01f, 120.0f*rs, r.z, true);
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x + 448.0f * rs, 0.0f, r.z, 90, r, false, 3);
            PositionEntity(r.RoomDoors[0].Buttons[1], r.x + 416.0f * rs, EntityY(r.RoomDoors[0].Buttons[1],true), r.z - 208.0f * rs,true);
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 480.0f * rs, EntityY(r.RoomDoors[0].Buttons[0],true), r.z + 184.0f * rs,true);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true; r.RoomDoors[0].Locked = true;
            //[End Block]
        }

        private static void Fill_Room2cafeteria(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            //scp-294
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x+1847.0f*rs, -240.0f*rs, r.z-321*rs, true);
            //"spawnpoint" for the cups
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x+1780.0f*rs, -248.0f*rs, r.z-276*rs, true);
            it = ItemSystem.CreateItem("cup", "cup", r.x-508.0f*rs, -187*rs, r.z+284.0f*rs, 240,175,70);
            EntityParent(it.Collider, r.obj); it.CustomName = "Cup of Orange Juice";
            it = ItemSystem.CreateItem("cup", "cup", r.x+1412 * rs, -187*rs, r.z-716.0f * rs, 87,62,45);
            EntityParent(it.Collider, r.obj); it.CustomName = "Cup of Coffee";
            it = ItemSystem.CreateItem("Empty Cup", "emptycup", r.x-540*rs, -187*rs, r.z+124.0f*rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Quarter", "25ct", r.x-447.0f*rs, r.y-334.0f*rs, r.z+36.0f*rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Quarter", "25ct", r.x+1409.0f*rs, r.y-334.0f*rs, r.z-732.0f*rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room2nuke(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            //"tuulikaapin" ovi
            d = DoorSystem.CreateDoor(r.zone, r.x + 576.0f * rs, 0.0f, r.z + 152.0f * rs, 90, r, false, 0, 5);
            d.AutoClose = false; d.Open = false;
            PositionEntity(d.Buttons[0], r.x + 602.0f * rs, EntityY(d.Buttons[0],true), r.z + 20.0f * rs,true);
            PositionEntity(d.Buttons[1], r.x + 550.0f * rs, EntityY(d.Buttons[1],true), r.z + 20.0f * rs,true);
            FreeEntity(d.Obj2); d.Obj2 = -1;
            d = DoorSystem.CreateDoor(r.zone, r.x - 544.0f * rs, 1504.0f*rs, r.z + 738.0f * rs, 90, r, false, 0, 5);
            d.AutoClose = false; d.Open = false;
            PositionEntity(d.Buttons[0], EntityX(d.Buttons[0],true), EntityY(d.Buttons[0],true), r.z + 608.0f * rs,true);
            PositionEntity(d.Buttons[1], EntityX(d.Buttons[1],true), EntityY(d.Buttons[1],true), r.z + 608.0f * rs,true);
            //yl�kerran hissin ovi
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x + 1192.0f * rs, 0.0f, r.z, 90, r, true, 3);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true;
            //yl�kerran hissi
            r.Objects[4] = CreatePivot();
            PositionEntity(r.Objects[4], r.x + 1496.0f * rs, 240.0f * rs, r.z);
            EntityParent(r.Objects[4], r.obj);
            //alakerran hissin ovi
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 680.0f * rs, 1504.0f * rs, r.z, 90, r, false, 3);
            r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = false;
            //alakerran hissi
            r.Objects[5] = CreatePivot();
            PositionEntity(r.Objects[5], r.x + 984.0f * rs, 1744.0f * rs, r.z);
            EntityParent(r.Objects[5], r.obj);
            for (n = 0; n <= 1; n++)
            {
                r.Objects[n * 2] = CopyEntity(MapAssets.LeverBaseObj);
                r.Objects[n * 2 + 1] = CopyEntity(MapAssets.LeverObj);
                r.Levers[n] = r.Objects[n * 2 + 1];
                for (i = 0; i <= 1; i++)
                {
                    ScaleEntity(r.Objects[n * 2 + i], 0.04f, 0.04f, 0.04f);
                    PositionEntity(r.Objects[n * 2 + i], r.x - 975.0f * rs, r.y + 1712.0f * rs, r.z - (502.0f-132.0f*n) * rs, true);
                    EntityParent(r.Objects[n * 2 + i], r.obj);
                }
                RotateEntity(r.Objects[n * 2], 0, -90-180, 0);
                RotateEntity(r.Objects[n * 2 + 1], 10, -90 - 180-180, 0);
                //EntityPickMode(r\Objects[n * 2 + 1], 2)
                EntityPickMode(r.Objects[n * 2 + 1], 1, false);
                EntityRadius(r.Objects[n * 2 + 1], 0.1f);
                //makecollbox(r\Objects[n * 2 + 1])
            }
            it = ItemSystem.CreateItem("Nuclear Device Document", "paper", r.x - 768.0f * rs, r.y + 1684.0f * rs, r.z - 768.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Ballistic Vest", "vest", r.x - 944.0f * rs, r.y + 1652.0f * rs, r.z - 656.0f * rs);
            EntityParent(it.Collider, r.obj); RotateEntity(it.Collider, 0, -90, 0);
            sc = SecurityCamSystem.Create(r.x+624.0f*rs, r.y+1888.0f*rs, r.z-312.0f*rs, r);
            sc.Angle = 90;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            r.Objects[6] = CreatePivot();
            PositionEntity(r.Objects[6],r.x+1110.0f*rs,r.y+36.0f*rs,r.z-208.0f*rs);
            EntityParent(r.Objects[6],r.obj);
            //[End Block]
        }

        private static void Fill_Room2tunnel(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], r.x + 2640.0f * rs, -2496.0f * rs, r.z + 400.0f * rs);
            EntityParent(r.Objects[0], r.obj);
            r.Objects[1] = CreatePivot();
            PositionEntity(r.Objects[1], r.x - 4336.0f * rs, -2496.0f * rs, r.z - 2512.0f * rs);
            EntityParent(r.Objects[1], r.obj);
            r.Objects[2] = CreatePivot();
            RotateEntity(r.Objects[2],0.0f,180.0f,0.0f,true);
            PositionEntity(r.Objects[2], r.x + 552.0f * rs, 240.0f * rs, r.z + 656.0f * rs);
            EntityParent(r.Objects[2], r.obj);
            //
            r.Objects[4] = CreatePivot();
            PositionEntity(r.Objects[4], r.x - 552.0f * rs, 240.0f * rs, r.z - 656.0f * rs);
            EntityParent(r.Objects[4], r.obj);
            //
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x + 264.0f * rs, 0.0f, r.z + 656.0f * rs, 90, r, true, 3);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true;
            PositionEntity(r.RoomDoors[0].Buttons[1], r.x + 224.0f * rs, 0.7f, r.z + 480.0f * rs, true);
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 304.0f * rs, 0.7f, r.z + 832.0f * rs, true);
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x - 264.0f * rs, 0.0f, r.z - 656.0f * rs, 90, r, true, 3);
            r.RoomDoors[2].AutoClose = false; r.RoomDoors[2].Open = true;
            PositionEntity(r.RoomDoors[2].Buttons[0], r.x - 224.0f * rs, 0.7f, r.z - 480.0f * rs, true);
            PositionEntity(r.RoomDoors[2].Buttons[1], r.x - 304.0f * rs, 0.7f, r.z - 832.0f * rs, true);
            //
            temp = (((int)(GameState.AccessCode)*3) % 10000);
            if (temp < 1000) { temp = temp+1000; }
            d = DoorSystem.CreateDoor(0, r.x, r.y, r.z, 0, r, false, 1, 0, temp.ToString());
            PositionEntity(d.Buttons[0], r.x + 224.0f * rs, r.y + 0.7f, r.z - 384.0f * rs, true);
            RotateEntity(d.Buttons[0], 0,-90,0,true);
            PositionEntity(d.Buttons[1], r.x - 224.0f * rs, r.y + 0.7f, r.z + 384.0f * rs, true);
            RotateEntity(d.Buttons[1], 0,90,0,true);
            de = DecalSystem.Create(0, r.x + 64.0f * rs, 0.005f, r.z + 144.0f * rs, 90, Rand(360), 0);
            EntityParent(de.Obj, r.obj);
            it = ItemSystem.CreateItem("Scorched Note", "paper", r.x + 64.0f * rs, r.y +144.0f * rs, r.z - 384.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room008(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            //the container
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x + 292.0f * rs, 130.0f*rs, r.z + 516.0f * rs, true);
            //the lid of the container
            r.Objects[1] = LoadMesh("GFX.Map.008_2.b3d");
            ScaleEntity(r.Objects[1], rs, rs, rs);
            PositionEntity(r.Objects[1], r.x + 292 * rs, 151 * rs, r.z + 576.0f * rs, true);
            EntityParent(r.Objects[1], r.obj);
            RotateEntity(r.Objects[1],89,0,0,true);
            r.Levers[0] = r.Objects[1];
            tex = LoadTextureHandle("GFX.Map.Glass.png",1+2);
            r.Objects[2] = CreateSprite();
            EntityTexture(r.Objects[2], tex);
            SpriteViewMode(r.Objects[2],2);
            ScaleSprite(r.Objects[2],256.0f*rs*0.5f, 194.0f*rs*0.5f);
            PositionEntity(r.Objects[2], r.x - 176.0f * rs, 224.0f*rs, r.z + 448.0f * rs);
            TurnEntity(r.Objects[2],0,90,0);
            EntityParent(r.Objects[2], r.obj);
            FreeTexture(tex);
            //scp-173 spawnpoint
            r.Objects[3] = CreatePivot(r.obj);
            PositionEntity(r.Objects[3], r.x - 445.0f * rs, 120.0f*rs, r.z + 544.0f * rs, true);
            //scp-173 attack point
            r.Objects[4] = CreatePivot(r.obj);
            PositionEntity(r.Objects[4], r.x + 67.0f * rs, 120.0f*rs, r.z + 464.0f * rs, true);
            r.Objects[5] = CreateSprite();
            PositionEntity(r.Objects[5], r.x - 158 * rs, 368 * rs, r.z + 298.0f * rs);
            ScaleSprite(r.Objects[5], 0.02f, 0.02f);
            EntityTexture(r.Objects[5], MapAssets.LightSpriteTex(1));
            EntityBlend(r.Objects[5], 3);
            EntityParent(r.Objects[5], r.obj);
            HideEntity(r.Objects[5]);
            d = DoorSystem.CreateDoor(r.zone, r.x + 296.0f * rs, 0, r.z - 672.0f * rs, 180, r, true, 0, 4);
            d.AutoClose = false;
            PositionEntity(d.Buttons[1], r.x + 164.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true), true);
            FreeEntity(d.Buttons[0]); d.Buttons[0] = -1;
            FreeEntity(d.Obj2); d.Obj2 = -1;
            r.RoomDoors[0] = d;
            d2 = DoorSystem.CreateDoor(r.zone, r.x + 296.0f * rs, 0, r.z - 144.0f * rs, 0, r, false);
            d2.AutoClose = false;
            PositionEntity(d2.Buttons[0], r.x + 432.0f * rs, EntityY(d2.Buttons[0],true), r.z - 480.0f * rs, true);
            RotateEntity(d2.Buttons[0], 0, -90, 0, true);
            PositionEntity(d2.Buttons[1], r.x + 164.0f * rs, EntityY(d2.Buttons[0],true), r.z - 128.0f * rs, true);
            FreeEntity(d2.Obj2); d2.Obj2 = -1;
            r.RoomDoors[1] = d2;
            d.LinkedDoor = d2;
            d2.LinkedDoor = d;
            d = DoorSystem.CreateDoor(r.zone, r.x - 384.0f * rs, 0, r.z - 672.0f * rs, 0, r, false, 0, 4);
            d.AutoClose = false; d.Locked = true; r.RoomDoors[2]=d;
            it = ItemSystem.CreateItem("Hazmat Suit", "hazmatsuit", r.x - 76.0f * rs, 0.5f, r.z - 396.0f * rs);
            EntityParent(it.Collider, r.obj); RotateEntity(it.Collider, 0, 90, 0);
            it = ItemSystem.CreateItem("Document SCP-008", "paper", r.x - 245.0f * rs, r.y + 192.0f * rs, r.z + 368.0f * rs);
            EntityParent(it.Collider, r.obj);
            //spawnpoint for the scientist used in the "008 zombie scene"
            r.Objects[6] = CreatePivot(r.obj);
            PositionEntity(r.Objects[6], r.x + 160 * rs, 672 * rs, r.z - 384.0f * rs, true);
            //spawnpoint for the player
            r.Objects[7] = CreatePivot(r.obj);
            PositionEntity(r.Objects[7], r.x, 672 * rs, r.z + 352.0f * rs, true);
            sc = SecurityCamSystem.Create(r.x+578.956f*rs, r.y+444.956f*rs, r.z+772.0f*rs, r);
            sc.Angle = 135;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            //[End Block]
        }

        private static void Fill_Room035(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x - 296.0f * rs, 0, r.z - 672.0f * rs, 180, r, true, 0, 5);
            d.AutoClose = false; d.Locked = true; r.RoomDoors[0]=d;
            PositionEntity(d.Buttons[1], r.x - 164.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true), true);
            FreeEntity(d.Buttons[0]); d.Buttons[0] = -1;
            FreeEntity(d.Obj2); d.Obj2 = -1;
            d2 = DoorSystem.CreateDoor(r.zone, r.x - 296.0f * rs, 0, r.z - 144.0f * rs, 0, r, false);
            d2.AutoClose = false; d2.Locked = true; r.RoomDoors[1]=d2;
            PositionEntity(d2.Buttons[0], r.x - 432.0f * rs, EntityY(d2.Buttons[0],true), r.z - 480.0f * rs, true);
            RotateEntity(d2.Buttons[0], 0, 90, 0, true);
            FreeEntity(d2.Buttons[1]); d2.Buttons[1] = -1;
            FreeEntity(d2.Obj2); d2.Obj2 = -1;
            //door to the control room
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x + 384.0f * rs, 0, r.z - 672.0f * rs, 180, r, false, 0, 5);
            r.RoomDoors[2].AutoClose = false;
            //door to the storage room
            r.RoomDoors[3] = DoorSystem.CreateDoor(0, r.x + 768.0f * rs, 0, r.z +512.0f * rs, 90, r, false, 0, 0, "5731");
            r.RoomDoors[3].AutoClose = false;
            d.LinkedDoor = d2; d2.LinkedDoor = d;
            for (i = 0; i <= 1; i++)
            {
                r.Objects[i*2] = CopyEntity(MapAssets.LeverBaseObj);
                r.Objects[i*2+1] = CopyEntity(MapAssets.LeverObj);
                r.Levers[i] = r.Objects[i*2+1];
                for (n = 0; n <= 1; n++)
                {
                    ScaleEntity(r.Objects[i*2+n], 0.04f, 0.04f, 0.04f);
                    PositionEntity(r.Objects[i*2+n], r.x + 210.0f * rs, r.y + 224.0f * rs, r.z - (208-i*76) * rs, true);
                    EntityParent(r.Objects[i*2+n], r.obj);
                }
                RotateEntity(r.Objects[i*2], 0, -90-180, 0);
                RotateEntity(r.Objects[i*2+1], -80, -90, 0);
                EntityPickMode(r.Objects[i*2+1], 1, false);
                EntityRadius(r.Objects[i*2+1], 0.1f);
            }
            //the control room
            r.Objects[3] = CreatePivot(r.obj);
            PositionEntity(r.Objects[3], r.x + 456 * rs, 0.5f, r.z + 400.0f * rs, true);
            r.Objects[4] = CreatePivot(r.obj);
            PositionEntity(r.Objects[4], r.x - 576 * rs, 0.5f, r.z + 640.0f * rs, true);
            for (i = 0; i <= 1; i++)
            {
                em = ParticleSystem.CreateEmitter(r.x - 272.0f * rs, 10, r.z + (624.0f-i*512) * rs, 0);
                TurnEntity(em.Obj, 90, 0, 0, true);
                EntityParent(em.Obj, r.obj);
                em.RandAngle = 15;
                em.Speed = 0.05f;
                em.SizeChange = 0.007f;
                em.AChange = -0.006f;
                em.Gravity = -0.24f;
                r.Objects[5+i]=em.Obj;
            }
            //the corners of the cont chamber (needed to calculate whether the player is inside the chamber)
            r.Objects[7] = CreatePivot(r.obj);
            PositionEntity(r.Objects[7], r.x - 720 * rs, 0.5f, r.z + 880.0f * rs, true);
            r.Objects[8] = CreatePivot(r.obj);
            PositionEntity(r.Objects[8], r.x + 176 * rs, 0.5f, r.z - 144.0f * rs, true);
            it = ItemSystem.CreateItem("SCP-035 Addendum", "paper", r.x + 248.0f * rs, r.y + 220.0f * rs, r.z + 576.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Radio Transceiver", "radio", r.x - 544.0f * rs, 0.5f, r.z + 704.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("SCP-500-01", "scp500", r.x + 1168*rs, 224*rs, r.z+576*rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Metal Panel", "scp148", r.x - 360 * rs, 0.5f, r.z + 644 * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Document SCP-035", "paper", r.x + 1168.0f * rs, 104.0f * rs, r.z + 608.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room513(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x - 704.0f * rs, 0, r.z + 304.0f * rs, 0, r, false, 0, 2);
            d.AutoClose = false; FreeEntity(d.Buttons[0]); d.Buttons[0] = -1;
            PositionEntity(d.Buttons[0], EntityX(d.Buttons[0],true), EntityY(d.Buttons[0],true), r.z + 288.0f * rs, true);
            PositionEntity(d.Buttons[1], EntityX(d.Buttons[1],true), EntityY(d.Buttons[1],true), r.z + 320.0f * rs, true);
            sc = SecurityCamSystem.Create(r.x-312.0f * rs, r.y + 414*rs, r.z + 656*rs, r);
            sc.FollowPlayer = true;
            it = ItemSystem.CreateItem("SCP-513", "scp513", r.x - 32.0f * rs, r.y + 196.0f * rs, r.z + 688.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Blood-stained Note", "paper", r.x + 736.0f * rs,1.0f, r.z + 48.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Document SCP-513", "paper", r.x - 480.0f * rs, 104.0f*rs, r.z - 176.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room966(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x - 400.0f * rs, 0, r.z, -90, r, false, 0, 3);
            d = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z - 480.0f * rs, 180, r, false, 0, 3);
            //: d\buttons[0] = False
            //PositionEntity (d\buttons[0], EntityX(d\buttons[0],True), EntityY(d\buttons[0],True), r\z + 288.0f * RoomScale, True)
            //PositionEntity (d\buttons[1], EntityX(d\buttons[1],True), EntityY(d\buttons[1],True), r\z + 320.0f * RoomScale, True)
            sc = SecurityCamSystem.Create(r.x-312.0f * rs, r.y + 414*rs, r.z + 656*rs, r);
            sc.Angle = 225;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            //sc\FollowPlayer = True
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x, 0.5f, r.z + 512.0f * rs, true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x + 64.0f * rs, 0.5f, r.z - 640.0f * rs, true);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], r.x, 0.5f, r.z, true);
            r.Objects[3] = CreatePivot(r.obj);
            PositionEntity(r.Objects[3], r.x + 320.0f * rs, 0.5f, r.z + 704.0f * rs, true);
            it = ItemSystem.CreateItem("Night Vision Goggles", "nvgoggles", r.x + 320.0f * rs, 0.5f, r.z + 704.0f * rs);
            EntityParent(it.Collider, r.obj);
            it.State = 300;
            //[End Block]
        }

        private static void Fill_Room3storage(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x, 240.0f * rs, r.z + 752.0f * rs, true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x + 5840.0f * rs, -5392.0f * rs, r.z + 1360.0f * rs, true);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], r.x + 608.0f * rs, 240.0f * rs, r.z - 624.0f * rs, true);
            r.Objects[3] = CreatePivot(r.obj);
            //PositionEntity(r\Objects[3], r\x + 720.0f * RoomScale, -5392.0f * RoomScale, r\z + 752.0f * RoomScale, True)
            PositionEntity(r.Objects[3], r.x - 456.0f * rs, -5392.0f * rs, r.z - 1136 * rs, true);
            //"waypoints" # 1
            r.Objects[4] = CreatePivot(r.obj);
            PositionEntity(r.Objects[4], r.x + 2128.0f * rs, -5550.0f * rs, r.z + 2048.0f * rs, true);
            r.Objects[5] = CreatePivot(r.obj);
            PositionEntity(r.Objects[5], r.x + 2128.0f * rs, -5550.0f * rs, r.z - 1136.0f * rs, true);
            r.Objects[6] = CreatePivot(r.obj);
            PositionEntity(r.Objects[6], r.x + 3824.0f * rs, -5550.0f * rs, r.z - 1168.0f * rs, true);
            r.Objects[7] = CreatePivot(r.obj);
            PositionEntity(r.Objects[7], r.x + 3760.0f * rs, -5550.0f * rs, r.z + 2048.0f * rs, true);
            r.Objects[8] = CreatePivot(r.obj);
            PositionEntity(r.Objects[8], r.x + 4848.0f * rs, -5550.0f * rs, r.z + 112.0f * rs, true);
            //"waypoints" # 2
            r.Objects[9] = CreatePivot(r.obj);
            PositionEntity(r.Objects[9], r.x + 592.0f * rs, -5550.0f * rs, r.z + 6352.0f * rs, true);
            r.Objects[10] = CreatePivot(r.obj);
            PositionEntity(r.Objects[10], r.x + 2928.0f * rs, -5550.0f * rs, r.z + 6352.0f * rs, true);
            r.Objects[11] = CreatePivot(r.obj);
            PositionEntity(r.Objects[11], r.x + 2928.0f * rs, -5550.0f * rs, r.z + 5200.0f * rs, true);
            r.Objects[12] = CreatePivot(r.obj);
            PositionEntity(r.Objects[12], r.x + 592.0f * rs, -5550.0f * rs, r.z + 5200.0f * rs, true);
            //"waypoints" # 3
            r.Objects[13] = CreatePivot(r.obj);
            PositionEntity(r.Objects[13], r.x + 1136.0f * rs, -5550.0f * rs, r.z + 2944.0f * rs, true);
            r.Objects[14] = CreatePivot(r.obj);
            PositionEntity(r.Objects[14], r.x + 1104.0f * rs, -5550.0f * rs, r.z + 1184.0f * rs, true);
            r.Objects[15] = CreatePivot(r.obj);
            PositionEntity(r.Objects[15], r.x - 464.0f * rs,  -5550.0f * rs, r.z + 1216.0f * rs, true);
            r.Objects[16] = CreatePivot(r.obj);
            PositionEntity(r.Objects[16], r.x - 432.0f * rs, -5550.0f * rs, r.z + 2976.0f * rs, true);
            r.Objects[20] = LoadMesh("GFX.Map.Room3storage_hb.b3d",r.obj);
            EntityPickMode(r.Objects[20],2);
            EntityType(r.Objects[20],1);
            EntityAlpha(r.Objects[20],0.0f);
            //Doors
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x, 0.0f, r.z + 448.0f * rs, 0, r, true, 3);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true;
            PositionEntity(r.RoomDoors[0].Buttons[1], r.x - 160.0f * rs, 0.7f, r.z + 480.0f * rs, true);
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 160.0f * rs, 0.7f, r.z + 416.0f * rs, true);
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 5840.0f * rs, -5632.0f * rs, r.z + 1048.0f * rs, 0, r, false, 3);
            r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = false;
            PositionEntity(r.RoomDoors[1].Buttons[0], r.x + 6000.0f * rs, EntityY(r.RoomDoors[1].Buttons[0],true), r.z + 1008.0f * rs, true);
            PositionEntity(r.RoomDoors[1].Buttons[1], r.x + 5680.0f * rs, EntityY(r.RoomDoors[1].Buttons[1],true), r.z + 1088.0f * rs, true);
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x + 608.0f * rs, 0.0f, r.z - 312.0f * rs, 0, r, true, 3);
            r.RoomDoors[2].AutoClose = false; r.RoomDoors[2].Open = true;
            PositionEntity(r.RoomDoors[2].Buttons[1], r.x + 448.0f * rs, 0.7f, r.z - 272.0f * rs, true);
            PositionEntity(r.RoomDoors[2].Buttons[0], r.x + 768.0f * rs, 0.7f, r.z - 352.0f * rs, true);
            //r\RoomDoors[3] = CreateDoor(r\zone, r\x + 720.0f * RoomScale,  -5632.0f * RoomScale, r\z + 1064.0f * RoomScale, 0, r, False)
            //PositionEntity(r\RoomDoors[3]\buttons[0], r\x + 896.0f * RoomScale, EntityY(r\RoomDoors[3]\buttons[0],True), r\z + 1024.0f * RoomScale, True)
            //PositionEntity(r\RoomDoors[3]\buttons[1], r\x + 544.0f * RoomScale, EntityY(r\RoomDoors[3]\buttons[1],True), r\z + 1104.0f * RoomScale, True)
            r.RoomDoors[3] = DoorSystem.CreateDoor(r.zone, r.x - 456.0f * rs, -5632.0f * rs, r.z - 824.0f * rs, 0, r, false, 3);
            r.RoomDoors[3].AutoClose = false; r.RoomDoors[3].Open = false;
            //X=+176 | Z=-40
            PositionEntity(r.RoomDoors[3].Buttons[0], r.x - 280.0f*rs, EntityY(r.RoomDoors[3].Buttons[0],true), r.z - 864.0f * rs, true);
            //X=-176 | Z=+40
            PositionEntity(r.RoomDoors[3].Buttons[1], r.x - 632.0f*rs, EntityY(r.RoomDoors[3].Buttons[1],true), r.z - 784.0f * rs, true);
            em = ParticleSystem.CreateEmitter(r.x + 5218.0f * rs, -5584.0f*rs, r.z - 600* rs, 0);
            TurnEntity(em.Obj, 20, -100, 0, true);
            EntityParent(em.Obj, r.obj); em.Room = r;
            em.RandAngle = 15; em.Speed = 0.03f;
            em.SizeChange = 0.01f; em.AChange = -0.006f;
            em.Gravity = -0.2f;
            
            
            x = 2312; z = -952;
            switch (Rand(3))
            {
                case 2: x = 3032; z = 1288; break;
                case 3: x = 2824; z = 2808; break;
            }
            it = ItemSystem.CreateItem("Black Severed Hand", "hand2", r.x + x * rs, -5596.0f * rs + 1.0f, r.z + z * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Night Vision Goggles", "nvgoggles", r.x + 1936.0f * rs, r.y - 5496.0f * rs, r.z - 944.0f * rs);
            EntityParent(it.Collider, r.obj);
            it.State = 450;
            de = DecalSystem.Create(3, r.x + x * rs, -5632.0f * rs + 0.01f, r.z + z * rs, 90, Rnd(360), 0);
            de.Size = 0.5f;
            ScaleSprite(de.Obj, de.Size, de.Size);
            EntityParent(de.Obj, r.obj);
            for (n = 10; n <= 11; n++)
            {
                r.Objects[n * 2] = CopyEntity(MapAssets.LeverBaseObj);
                r.Objects[n * 2 + 1] = CopyEntity(MapAssets.LeverObj);
                r.Levers[n - 10] = r.Objects[n * 2 + 1];
                for (i = 0; i <= 1; i++)
                {
                    ScaleEntity(r.Objects[n * 2 + i], 0.04f, 0.04f, 0.04f);
                    if (n == 10)
                        PositionEntity(r.Objects[n * 2 + i], r.x + 3101 * rs, r.y - 5461 * rs, r.z + 6568 * rs, true);
                    else
                        PositionEntity(r.Objects[n * 2 + i], r.x + 1209 * rs, r.y - 5461 * rs, r.z + 3164 * rs, true);
                    EntityParent(r.Objects[n * 2 + i], r.obj);
                }
                RotateEntity(r.Objects[n * 2], 0, 0, 0);
                RotateEntity(r.Objects[n * 2 + 1], -10, -180, 0);
                EntityPickMode(r.Objects[n * 2 + 1], 1, false);
                EntityRadius(r.Objects[n * 2 + 1], 0.1f);
            }
            r.RoomDoors[4] = DoorSystem.CreateDoor(r.zone, r.x+56*rs, r.y-5632*rs, r.z+6344*rs, 90, r, false, 2);
            r.RoomDoors[4].AutoClose = false; r.RoomDoors[4].Open = false;
            for (i = 0; i <= 1; i++)
            {
                FreeEntity(r.RoomDoors[4].Buttons[i]); r.RoomDoors[4].Buttons[i] = -1;
            }
            d = DoorSystem.CreateDoor(r.zone, r.x+1157.0f*rs, r.y-5632.0f*rs, r.z+660.0f*rs, 0, r, false, 2);
            d.Locked = true; d.Open = false; d.AutoClose = false;
            for (i = 0; i <= 1; i++)
            {
                FreeEntity(d.Buttons[i]); d.Buttons[i] = -1;
            }
            d = DoorSystem.CreateDoor(r.zone, r.x+234.0f*rs, r.y-5632.0f*rs, r.z+5239.0f*rs, 90, r, false, 2);
            d.Locked = true; d.Open = false; d.AutoClose = false;
            for (i = 0; i <= 1; i++)
            {
                FreeEntity(d.Buttons[i]); d.Buttons[i] = -1;
            }
            d = DoorSystem.CreateDoor(r.zone, r.x+3446.0f*rs, r.y-5632.0f*rs, r.z+6369.0f*rs, 90, r, false, 2);
            d.Locked = true; d.Open = false; d.AutoClose = false;
            for (i = 0; i <= 1; i++)
            {
                FreeEntity(d.Buttons[i]); d.Buttons[i] = -1;
            }
            //[End Block]
        }

        private static void Fill_Room049(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x + 640.0f * rs, 240.0f * rs, r.z + 656.0f * rs, true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x + 3211.0f * rs, -3280.0f * rs, r.z + 1824.0f * rs, true);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], r.x - 672.0f * rs, 240.0f * rs, r.z - 93.0f * rs, true);
            r.Objects[3] = CreatePivot(r.obj);
            PositionEntity(r.Objects[3], r.x - 2766.0f * rs, -3280.0f * rs, r.z - 1277.0f * rs, true);
            //zombie 1
            r.Objects[4] = CreatePivot(r.obj);
            PositionEntity(r.Objects[4], r.x + 528.0f * rs, -3440.0f * rs, r.z + 96.0f * rs, true);
            //zombie 2
            r.Objects[5] = CreatePivot(r.obj);
            PositionEntity(r.Objects[5], r.x  + 64.0f * rs, -3440.0f * rs, r.z - 1000.0f * rs, true);
            for (n = 0; n <= 1; n++)
            {
                r.Objects[n * 2 + 6] = CopyEntity(MapAssets.LeverBaseObj);
                r.Objects[n * 2 + 7] = CopyEntity(MapAssets.LeverObj);
                r.Levers[n] = r.Objects[n * 2 + 7];
                for (i = 0; i <= 1; i++)
                {
                    ScaleEntity(r.Objects[n * 2 + 6 + i], 0.03f, 0.03f, 0.03f);
                    switch (n)
                    {
                        case 0:
                            PositionEntity(r.Objects[n * 2 + 6 + i], r.x + 852.0f * rs, r.y - 3374.0f * rs, r.z - 854.0f * rs, true);
                            break;
                        case 1:
                            PositionEntity(r.Objects[n * 2 + 6 + i], r.x - 834.0f * rs, r.y - 3400.0f * rs, r.z + 1093.0f * rs, true);
                            break;
                    }
                    EntityParent(r.Objects[n * 2 + 6 + i], r.obj);
                    RotateEntity(r.Objects[n * 2 + 6], 0, 180 + 90 * (n == 0 ? 1 : 0), 0);
                    RotateEntity(r.Objects[n * 2 + 7], 81 - 92 * n, 90 * (n == 0 ? 1 : 0), 0);
                    EntityPickMode(r.Objects[n * 2 + 7], 1, false);
                    EntityRadius(r.Objects[n * 2 + 7], 0.1f);
                }
                r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x + 330.0f * rs, 0.0f, r.z + 656.0f * rs, 90, r, true, 3);
                r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true;
                PositionEntity(r.RoomDoors[0].Buttons[1], r.x + 288.0f * rs, 0.7f, r.z + 512.0f * rs, true);
                PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 368.0f * rs, 0.7f, r.z + 840.0f * rs, true);
                r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 2898.0f * rs, -3520.0f * rs, r.z + 1824.0f * rs, 90, r, false, 3);
                r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = false;
                PositionEntity(r.RoomDoors[1].Buttons[1], r.x + 2881.0f * rs, EntityY(r.RoomDoors[1].Buttons[1],true), r.z + 1663.0f * rs, true);
                PositionEntity(r.RoomDoors[1].Buttons[0], r.x + 2936.0f * rs, EntityY(r.RoomDoors[1].Buttons[0],true), r.z + 2009.0f * rs, true);
                r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x - 672.0f * rs, 0.0f, r.z - 408.0f * rs, 0, r, true, 3);
                r.RoomDoors[2].AutoClose = false; r.RoomDoors[2].Open = true;
                PositionEntity(r.RoomDoors[2].Buttons[0], r.x - 487.0f * rs, 0.7f, r.z - 447.0f * rs, true);
                PositionEntity(r.RoomDoors[2].Buttons[1], r.x - 857.0f * rs, 0.7f, r.z - 369.0f * rs, true);
                r.RoomDoors[3] = DoorSystem.CreateDoor(r.zone, r.x - 2766.0f * rs, -3520.0f * rs, r.z - 1592.0f * rs, 0, r, false, 3);
                r.RoomDoors[3].AutoClose = false; r.RoomDoors[3].Open = false;
                PositionEntity(r.RoomDoors[3].Buttons[0], r.x - 2581.0f * rs, EntityY(r.RoomDoors[3].Buttons[0],true), r.z - 1631.0f * rs, true);
                PositionEntity(r.RoomDoors[3].Buttons[1], r.x - 2951.0f * rs, EntityY(r.RoomDoors[3].Buttons[1],true), r.z - 1553.0f * rs, true);
                //For i = 0 To 3
                //	if (i Mod 2) = 1
                //		AssignElevatorObj(r\Objects[i],r\RoomDoors[i],2)
                //	Else
                //		AssignElevatorObj(r\Objects[i],r\RoomDoors[i],True)
                //	EndIf
                //Next
                //storage room doors
                r.RoomDoors[4] = DoorSystem.CreateDoor(r.zone, r.x + 272.0f * rs, -3552.0f * rs, r.z + 104.0f * rs, 90, r, false);
                r.RoomDoors[4].AutoClose = false; r.RoomDoors[4].Open = true; r.RoomDoors[4].Locked = true;
                r.RoomDoors[5] = DoorSystem.CreateDoor(r.zone, r.x + 264.0f * rs, -3520.0f * rs, r.z - 1824.0f * rs, 90, r, false);
                r.RoomDoors[5].AutoClose = false; r.RoomDoors[5].Open = true; r.RoomDoors[5].Locked = true;
                r.RoomDoors[6] = DoorSystem.CreateDoor(r.zone, r.x - 264.0f * rs, -3520.0f * rs, r.z + 1824.0f * rs, 90, r, false);
                r.RoomDoors[6].AutoClose = false; r.RoomDoors[6].Open = true; r.RoomDoors[6].Locked = true;
                d = DoorSystem.CreateDoor(0, r.x, 0, r.z, 0, r, false, 2, -2);
                it = ItemSystem.CreateItem("Document SCP-049", "paper", r.x - 608.0f * rs, r.y - 3332.0f * rs, r.z + 876.0f * rs);
                EntityParent(it.Collider, r.obj);
                it = ItemSystem.CreateItem("Level 4 Key Card", "key4", r.x - 512.0f * rs, r.y - 3412.0f * rs, r.z + 864.0f * rs);
                EntityParent(it.Collider, r.obj);
                it = ItemSystem.CreateItem("First Aid Kit", "firstaid", r.x +385.0f * rs, r.y - 3412.0f * rs, r.z + 271.0f * rs);
                EntityParent(it.Collider, r.obj);
                d = DoorSystem.CreateDoor(r.zone, r.x-272.0f*rs, r.y-3552.0f*rs, r.z+98.0f*rs, 90, r, true, 1);
                d.AutoClose = false; d.Open = true; d.MTFClose = false; d.Locked = true;
                for (i = 0; i <= 1; i++)
                {
                    FreeEntity(d.Buttons[i]); d.Buttons[i] = -1;
                }
                d = DoorSystem.CreateDoor(r.zone, r.x-2990.0f*rs, r.y-3520.0f*rs, r.z-1824.0f*rs, 90, r, false, 2);
                d.Locked = true; d.DisableWaypoint = true;
                d = DoorSystem.CreateDoor(r.zone, r.x-896.0f*rs, r.y, r.z-640*rs, 90, r, false, 2);
                d.Locked = true; d.DisableWaypoint = true;
                r.Objects[10] = CreatePivot(r.obj);
                PositionEntity(r.Objects[10],r.x-832.0f*rs,r.y-3484.0f*rs,r.z+1572.0f*rs,true);
                //Spawnpoint for the map layout document
                r.Objects[11] = CreatePivot(r.obj);
                PositionEntity(r.Objects[11],r.x+2642.0f*rs,r.y-3516.0f*rs,r.z+1822.0f*rs,true);
                r.Objects[12] = CreatePivot(r.obj);
                PositionEntity(r.Objects[12],r.x-2666.0f*rs,r.y-3516.0f*rs,r.z-1792.0f*rs,true);
                //[End Block]
            }
        }

        private static void Fill_Room22(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            foreach (var otherRoom in MapSystem.All)
            {
                if (otherRoom!=r)
                {
                    if (r2.RoomName  == "room2_2")
                    {
                        r.Objects[0] = CopyEntity(r2.Objects[0]); //don't load the mesh again;
                        break;
                    }
                }
            }
            if (r.Objects[0]==-1) { r.Objects[0] = LoadMesh("GFX.Map.Fan.b3d"); }
            ScaleEntity(r.Objects[0], rs, rs, rs);
            PositionEntity(r.Objects[0], r.x - 248 * rs, 528 * rs, r.z, true);
            EntityParent(r.Objects[0], r.obj);
            //[End Block]
        }

        private static void Fill_Room012(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 264.0f * rs, 0.0f, r.z + 672.0f * rs, 270, r, false, 0, 3);
            PositionEntity(d.Buttons[0], r.x + 224.0f * rs, EntityY(d.Buttons[0],true), r.z + 540.0f * rs, true);
            PositionEntity(d.Buttons[1], r.x + 304.0f * rs, EntityY(d.Buttons[1],true), r.z + 840.0f * rs, true);
            TurnEntity(d.Buttons[1],0,0,0,true);
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x -512.0f * rs, -768.0f*rs, r.z -336.0f * rs, 0, r, false, 0);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = false; r.RoomDoors[0].Locked = true;
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 176.0f * rs, -512.0f*rs, r.z - 364.0f * rs, true);
            FreeEntity(r.RoomDoors[0].Buttons[1]); r.RoomDoors[0].Buttons[1] = -1;
            r.Objects[0] = CopyEntity(MapAssets.LeverBaseObj);
            r.Objects[1] = CopyEntity(MapAssets.LeverObj);
            r.Levers[0] = r.Objects[1];
            for (i = 0; i <= 1; i++)
            {
                ScaleEntity(r.Objects[i], 0.04f, 0.04f, 0.04f);
                PositionEntity(r.Objects[i], r.x + 240.0f * rs, r.y - 512.0f * rs, r.z - 364 * rs, true);
                EntityParent(r.Objects[i], r.obj);
            }
            //RotateEntity(r\Objects[0], 0, 0, 0)
            RotateEntity(r.Objects[1], 10, -180, 0);
            EntityPickMode(r.Objects[1], 1, false);
            EntityRadius(r.Objects[1], 0.1f);
            r.Objects[2] = LoadMesh("GFX.Map.Room012_2.b3d");
            ScaleEntity(r.Objects[2], rs, rs, rs);
            PositionEntity(r.Objects[2], r.x - 360 * rs, -130 * rs, r.z + 456.0f * rs, true);
            EntityParent(r.Objects[2], r.obj);
            r.Objects[3] = CreateSprite();
            PositionEntity(r.Objects[3], r.x - 43.5f * rs, - 574 * rs, r.z - 362.0f * rs);
            ScaleSprite(r.Objects[3], 0.015f, 0.015f);
            EntityTexture(r.Objects[3], MapAssets.LightSpriteTex(1));
            EntityBlend(r.Objects[3], 3);
            EntityParent(r.Objects[3], r.obj);
            HideEntity(r.Objects[3]);
            r.Objects[4] = LoadMesh("GFX.Map.Room012_3.b3d");
            tex=LoadTextureHandle("GFX.Map.Scp-012_0.jpg");
            EntityTexture(r.Objects[4], tex);
            ScaleEntity(r.Objects[4], rs, rs, rs);
            PositionEntity(r.Objects[4], r.x - 360 * rs, -130 * rs, r.z + 456.0f * rs, true);
            EntityParent(r.Objects[4], r.Objects[2]);
            it = ItemSystem.CreateItem("Document SCP-012", "paper", r.x - 56.0f * rs, r.y - 576.0f * rs, r.z - 408.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Severed Hand", "hand", r.x - 784*rs, -576*rs+0.3f, r.z+640*rs);
            EntityParent(it.Collider, r.obj);
            de = DecalSystem.Create(3,  r.x - 784*rs, -768*rs+0.01f, r.z+640*rs,90,Rnd(360),0);
            de.Size = 0.5f;
            ScaleSprite(de.Obj, de.Size,de.Size);
            EntityParent(de.Obj, r.obj);
            //[End Block]
        }

        private static void Fill_Tunnel2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x, 544.0f * rs, r.z + 512.0f * rs, true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x, 544.0f * rs, r.z - 512.0f * rs, true);
            //[End Block]
        }

        private static void Fill_Room2pipes(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0]= CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x + 368.0f * rs, 0.0f, r.z, true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x - 368.0f * rs, 0.0f, r.z, true);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], r.x + 224.0f * rs - 0.005f, 192.0f * rs, r.z, true);
            r.Objects[3] = CreatePivot(r.obj);
            PositionEntity(r.Objects[3], r.x - 224.0f * rs + 0.005f, 192.0f * rs, r.z, true);
            //[End Block]
        }

        private static void Fill_Room3pit(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            em = ParticleSystem.CreateEmitter(r.x + 512.0f * rs, -76 * rs, r.z - 688 * rs, 0);
            TurnEntity(em.Obj, -90, 0, 0);
            EntityParent(em.Obj, r.obj);
            em.RandAngle = 55;
            em.Speed = 0.0005f;
            em.AChange = -0.015f;
            em.SizeChange = 0.007f;
            em = ParticleSystem.CreateEmitter(r.x - 512.0f * rs, -76 * rs, r.z - 688 * rs, 0);
            TurnEntity(em.Obj, -90, 0, 0);
            EntityParent(em.Obj, r.obj);
            em.RandAngle = 55;
            em.Speed = 0.0005f;
            em.AChange = -0.015f;
            em.SizeChange = 0.007f;
            r.Objects[0]= CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x + 704.0f * rs, 112.0f*rs, r.z-416.0f*rs, true);
            //[End Block]
        }

        private static void Fill_Room2servers(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(0, r.x, 0, r.z, 0, r, false, 2, 0);
            d.Locked = true;
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x - 208.0f * rs, 0.0f, r.z - 736.0f * rs, 90, r, true, 0, 0, "", true);
            r.RoomDoors[0].AutoClose=false;
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x - 208.0f * rs, 0.0f, r.z + 736.0f * rs, 90, r, true, 0, 0, "", true);
            r.RoomDoors[1].AutoClose=false;
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x - 672.0f * rs, 0.0f, r.z - 1024.0f * rs, 0, r, false, 0, 0, "GEAR");
            r.RoomDoors[2].AutoClose=false; r.RoomDoors[2].DisableWaypoint = true;
            FreeEntity(r.RoomDoors[2].Buttons[0]); r.RoomDoors[2].Buttons[0] = -1;
            FreeEntity(r.RoomDoors[2].Buttons[1]); r.RoomDoors[2].Buttons[1] = -1;
            for (n = 0; n <= 2; n++)
            {
                r.Objects[n * 2] = CopyEntity(MapAssets.LeverBaseObj);
                r.Objects[n * 2 + 1] = CopyEntity(MapAssets.LeverObj);
                r.Levers[n] = r.Objects[n * 2 + 1];
                for (i = 0; i <= 1; i++)
                {
                    ScaleEntity(r.Objects[n * 2 + i], 0.03f, 0.03f, 0.03f);
                    switch (n)
                    {
                        case 0:
                            PositionEntity(r.Objects[n * 2 + i], r.x - 1260.0f * rs, r.y + 234.0f * rs, r.z + 750 * rs, true);
                            break;
                        case 1:
                            PositionEntity(r.Objects[n * 2 + i], r.x - 920.0f * rs, r.y + 164.0f * rs, r.z + 898 * rs, true);
                            break;
                        case 2:
                            PositionEntity(r.Objects[n * 2 + i], r.x - 837.0f * rs, r.y + 152.0f * rs, r.z + 886 * rs, true);
                            break;
                    }
                    EntityParent(r.Objects[n * 2 + i], r.obj);
                    //RotateEntity(r\Objects[n * 2], 0, -90, 0)
                    RotateEntity(r.Objects[n*2+1], 81, -180, 0);
                    //EntityPickMode(r\Objects[n * 2 + 1], 2)
                    EntityPickMode(r.Objects[n * 2 + 1], 1, false);
                    EntityRadius(r.Objects[n * 2 + 1], 0.1f);
                    //makecollbox(r\Objects[n * 2 + 1])
                }
                RotateEntity(r.Objects[2+1], -81, -180, 0);
                RotateEntity(r.Objects[4+1], -81, -180, 0);
                //096 spawnpoint
                //			r\Objects[6]=CreatePivot(r\obj)
                //			PositionEntity(r\Objects[6], r\x - 848*RoomScale, 0.5f, r\z-576*RoomScale, True)
                r.Objects[6]=CreatePivot(r.obj);
                PositionEntity(r.Objects[6],r.x-320*rs,0.5f,r.z,true);
                //guard spawnpoint
                r.Objects[7]=CreatePivot(r.obj);
                PositionEntity(r.Objects[7], r.x - 1328.0f * rs, 0.5f, r.z + 528*rs, true);
                //the point where the guard walks to
                r.Objects[8]=CreatePivot(r.obj);
                PositionEntity(r.Objects[8], r.x - 1376.0f * rs, 0.5f, r.z + 32*rs, true);
                r.Objects[9]=CreatePivot(r.obj);
                PositionEntity(r.Objects[9], r.x - 848*rs, 0.5f, r.z+576*rs, true);
                //[End Block]
            }
        }

        private static void Fill_Room3servers(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            it = ItemSystem.CreateItem("9V Battery", "bat", r.x - 132.0f * rs, r.y - 368.0f * rs, r.z - 648.0f * rs);
            EntityParent(it.Collider, r.obj);
            if (Rand(2) == 1)
            {
                it = ItemSystem.CreateItem("9V Battery", "bat", r.x - 76.0f * rs, r.y - 368.0f * rs, r.z - 648.0f * rs);
                EntityParent(it.Collider, r.obj);
            }
            if (Rand(2) == 1)
            {
                it = ItemSystem.CreateItem("9V Battery", "bat", r.x - 196.0f * rs, r.y - 368.0f * rs, r.z - 648.0f * rs);
                EntityParent(it.Collider, r.obj);
            }
            it = ItemSystem.CreateItem("S-NAV 300 Navigator", "nav", r.x + 124.0f * rs, r.y - 368.0f * rs, r.z - 648.0f * rs);
            it.State = 20; EntityParent(it.Collider, r.obj);
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x + 736.0f * rs, -512.0f * rs, r.z - 400.0f * rs, true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x - 552.0f * rs, -512.0f * rs, r.z - 528.0f * rs, true);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], r.x + 736.0f * rs, -512.0f * rs, r.z + 272.0f * rs, true);
            r.Objects[3] = LoadMesh("GFX.Npcs.Duck_low_res.b3d");
            ScaleEntity(r.Objects[3], 0.07f, 0.07f, 0.07f);
            tex = LoadTextureHandle("GFX.Npcs.Duck2.png");
            EntityTexture(r.Objects[3], tex);
            PositionEntity(r.Objects[3], r.x + 928.0f * rs, -640*rs, r.z + 704.0f * rs);
            EntityParent(r.Objects[3], r.obj);
            //[End Block]
        }

        private static void Fill_Room3servers2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x - 504.0f * rs, -512.0f * rs, r.z + 271.0f * rs, true);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x + 628.0f * rs, -512.0f * rs, r.z + 271.0f * rs, true);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], r.x - 532.0f * rs, -512.0f * rs, r.z - 877.0f * rs, true);
            it = ItemSystem.CreateItem("Document SCP-970", "paper", r.x + 960.0f * rs, r.y - 448.0f * rs, r.z + 251.0f * rs);
            RotateEntity(it.Collider, 0, r.Angle, 0);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Gas Mask", "gasmask", r.x + 954.0f * rs, r.y - 504.0f * rs, r.z + 235.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Testroom(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            for (int xtemp = 0; xtemp <= 1; xtemp++)
            {
                for (int ztemp = -1; ztemp <= 1; ztemp++)
                {
                    r.Objects[xtemp * 3 + (ztemp + 1)] = CreatePivot();
                    PositionEntity(r.Objects[xtemp * 3 + (ztemp + 1)], r.x + (-236.0f + 280.0f * xtemp) * rs, -700.0f * rs, r.z + 384.0f * ztemp * rs);
                    EntityParent(r.Objects[xtemp * 3 + (ztemp + 1)], r.obj);
                }
            }
            r.Objects[6] = CreatePivot();
            PositionEntity(r.Objects[6], r.x + 754.0f * rs, r.y - 1248.0f * rs, r.z);
            EntityParent(r.Objects[6], r.obj);
            sc = SecurityCamSystem.Create(r.x + 744.0f * rs, r.y - 856.0f * rs, r.z + 236.0f * rs, r);
            sc.FollowPlayer = true;
            DoorSystem.CreateDoor(0, r.x + 720.0f * rs, 0, r.z, 0, r, false, 2, -1);
            DoorSystem.CreateDoor(0, r.x - 624.0f * rs, -1280.0f * rs, r.z, 90, r, true);
            it = ItemSystem.CreateItem("Document SCP-682", "paper", r.x + 656.0f * rs, r.y - 1200.0f * rs, r.z - 16.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room2closets(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            it = ItemSystem.CreateItem("Document SCP-1048", "paper", r.x + 736.0f * rs, r.y + 176.0f * rs, r.z + 736.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Gas Mask", "gasmask", r.x + 736.0f * rs, r.y + 176.0f * rs, r.z + 544.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("9V Battery", "bat", r.x + 736.0f * rs, r.y + 176.0f * rs, r.z - 448.0f * rs);
            EntityParent(it.Collider, r.obj);
            if (Rand(2) == 1)
            {
                it = ItemSystem.CreateItem("9V Battery", "bat", r.x + 730.0f * rs, r.y + 176.0f * rs, r.z - 496.0f * rs);
                EntityParent(it.Collider, r.obj);
            }
            if (Rand(2) == 1)
            {
                it = ItemSystem.CreateItem("9V Battery", "bat", r.x + 740.0f * rs, r.y + 176.0f * rs, r.z - 560.0f * rs);
                EntityParent(it.Collider, r.obj);
            }
            it = ItemSystem.CreateItem("Level 1 Key Card", "key1", r.x + 736.0f * rs, r.y + 240.0f * rs, r.z + 752.0f * rs);
            EntityParent(it.Collider, r.obj);
            clipboard = ItemSystem.CreateItem("Clipboard","clipboard",r.x + 736.0f * rs, r.y + 224.0f * rs, r.z -480.0f * rs);
            EntityParent(clipboard.Collider, r.obj);
            it = ItemSystem.CreateItem("Incident Report SCP-1048-A", "paper",r.x + 736.0f * rs, r.y + 224.0f * rs, r.z -480.0f * rs);
            //clipboard\SecondInv[0] = it
            HideEntity(it.Collider);
            r.Objects[0]=CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x-1120*rs, -256*rs, r.z+896*rs, true);
            r.Objects[1]=CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x-1232*rs, -256*rs, r.z-160*rs, true);
            d = DoorSystem.CreateDoor(0, r.x - 240.0f * rs, 0.0f, r.z, 90, r, false);
            PositionEntity(d.Buttons[0], r.x - 230.0f * rs, EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true), true);
            PositionEntity(d.Buttons[1], r.x - 250.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true), true);
            d.Open = false; d.AutoClose = false;
            sc = SecurityCamSystem.Create(r.x, r.y + 704*rs, r.z + 863*rs, r);
            sc.Angle = 180;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            //sc\FollowPlayer = True
            //[End Block]
        }

        private static void Fill_Room2offices(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            it = ItemSystem.CreateItem("Document SCP-106", "paper", r.x + 404.0f * rs, r.y + 145.0f * rs, r.z + 559.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Level 2 Key Card", "key2", r.x - 156.0f * rs, r.y + 151.0f * rs, r.z + 72.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("S-NAV 300 Navigator", "nav", r.x + 305.0f * rs, r.y + 153.0f * rs, r.z + 944.0f * rs);
            it.State = 20; EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Notification", "paper", r.x -137.0f * rs, r.y + 153.0f * rs, r.z + 464.0f * rs);
            EntityParent(it.Collider, r.obj);
            w = WaypointSystem.Create(r.x - 32.0f * rs, r.y + 66.0f * rs, r.z + 288.0f * rs, null, r);
            w2 = WaypointSystem.Create(r.x, r.y + 66.0f * rs, r.z - 448.0f * rs, null, r);
            w.Connected[0] = w2; w.Dist[0] = EntityDistance(w.Obj, w2.Obj);
            w2.Connected[0] = w; w2.Dist[0] = w.Dist[0];
            //[End Block]
        }

        private static void Fill_Room2offices2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            it = ItemSystem.CreateItem("Level 1 Key Card", "key1", r.x - 368.0f * rs, r.y - 48.0f * rs, r.z + 80.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Document SCP-895", "paper", r.x - 800.0f * rs, r.y - 48.0f * rs, r.z + 368.0f * rs);
            EntityParent(it.Collider, r.obj);
            if (Rand(2) == 1)
            {
                it = ItemSystem.CreateItem("Document SCP-860", "paper", r.x - 800.0f * rs, r.y - 48.0f * rs, r.z - 464.0f * rs);
            }
            else
            {
                it = ItemSystem.CreateItem("SCP-093 Recovered Materials", "paper", r.x - 800.0f * rs, r.y - 48.0f * rs, r.z - 464.0f * rs);
            }
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("S-NAV 300 Navigator", "nav", r.x - 336.0f * rs, r.y - 48.0f * rs, r.z - 480.0f * rs);
            it.State = 28; EntityParent(it.Collider, r.obj);
            r.Objects[0] = LoadMesh("GFX.Npcs.Duck_low_res.b3d");
            ScaleEntity(r.Objects[0], 0.07f, 0.07f, 0.07f);
            EntityParent(r.Objects[0], r.obj);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x-808.0f * rs, -72.0f * rs, r.z - 40.0f * rs, true);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], r.x-488.0f * rs, 160.0f * rs, r.z + 700.0f * rs, true);
            r.Objects[3] = CreatePivot(r.obj);
            PositionEntity(r.Objects[3], r.x-488.0f * rs, 160.0f * rs, r.z - 668.0f * rs, true);
            r.Objects[4] = CreatePivot(r.obj);
            PositionEntity(r.Objects[4], r.x-572.0f * rs, 350.0f * rs, r.z - 4.0f * rs, true);
            temp = Rand(1,4);
            PositionEntity(r.Objects[0], EntityX(r.Objects[temp],true),EntityY(r.Objects[temp],true),EntityZ(r.Objects[temp],true),true);
            //[End Block]
        }

        private static void Fill_Room2offices3(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            if (Rand(2)==1)
            {
                it = ItemSystem.CreateItem("Mobile Task Forces", "paper", r.x + 744.0f * rs, r.y +240.0f * rs, r.z + 944.0f * rs);
                EntityParent(it.Collider, r.obj);
            }
            else
            {
                it = ItemSystem.CreateItem("Security Clearance Levels", "paper", r.x + 680.0f * rs, r.y +240.0f * rs, r.z + 944.0f * rs);
                EntityParent(it.Collider, r.obj);
            }
            it = ItemSystem.CreateItem("Object Classes", "paper", r.x + 160.0f * rs, r.y +240.0f * rs, r.z + 568.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Document", "paper", r.x -1440.0f * rs, r.y +624.0f * rs, r.z + 152.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Radio Transceiver", "radio", r.x - 1184.0f * rs, r.y + 480.0f * rs, r.z - 800.0f * rs);
            EntityParent(it.Collider, r.obj);
            for (i = 0; i <= Rand(0,1); i++)
            {
                it = ItemSystem.CreateItem("ReVision Eyedrops", "eyedrops", r.x - 1529.0f*rs, r.y + 563.0f * rs, r.z - 572.0f*rs + i*0.05f);
                EntityParent(it.Collider, r.obj);
            }
            it = ItemSystem.CreateItem("9V Battery", "bat", r.x - 1545.0f * rs, r.y + 603.0f * rs, r.z - 372.0f * rs);
            EntityParent(it.Collider, r.obj);
            if (Rand(2) == 1)
            {
                it = ItemSystem.CreateItem("9V Battery", "bat", r.x - 1540.0f * rs, r.y + 603.0f * rs, r.z - 340.0f * rs);
                EntityParent(it.Collider, r.obj);
            }
            if (Rand(2) == 1)
            {
                it = ItemSystem.CreateItem("9V Battery", "bat", r.x - 1529.0f * rs, r.y + 603.0f * rs, r.z - 308.0f * rs);
                EntityParent(it.Collider, r.obj);
            }
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x - 1056.0f * rs, 384.0f*rs, r.z + 290.0f * rs, 90, r, true);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true;
            PositionEntity(r.RoomDoors[0].Buttons[0], EntityX(r.RoomDoors[0].Buttons[0],true),EntityY(r.RoomDoors[0].Buttons[0],true),r.z + 161.0f * rs,true);
            PositionEntity(r.RoomDoors[0].Buttons[1], EntityX(r.RoomDoors[0].Buttons[1],true),EntityY(r.RoomDoors[0].Buttons[1],true),r.z + 161.0f * rs,true);
            //[End Block]
        }

        private static void Fill_Start(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            //the containment doors
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 4000.0f * rs, 384.0f*rs, r.z + 1696.0f * rs, 90, r, true, 1);
            r.RoomDoors[1].Locked = false; r.RoomDoors[1].AutoClose = false;
            r.RoomDoors[1].Dir = 1; r.RoomDoors[1].Open = true;
            FreeEntity(r.RoomDoors[1].Buttons[0]); r.RoomDoors[1].Buttons[0] = -1;
            FreeEntity(r.RoomDoors[1].Buttons[1]); r.RoomDoors[1].Buttons[1] = -1;
            r.RoomDoors[1].MTFClose = false;
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x + 2704.0f * rs, 384.0f*rs, r.z + 624.0f * rs, 90, r, false);
            r.RoomDoors[2].AutoClose = false; r.RoomDoors[2].Open = false;
            FreeEntity(r.RoomDoors[2].Buttons[0]); r.RoomDoors[2].Buttons[0] = -1;
            FreeEntity(r.RoomDoors[2].Buttons[1]); r.RoomDoors[2].Buttons[1] = -1;
            r.RoomDoors[2].MTFClose = false;
            d = DoorSystem.CreateDoor(r.zone, r.x + 1392.0f * rs, 384.0f*rs, r.z + 64.0f * rs, 90, r, true);
            d.AutoClose = false;
            d.MTFClose = false;
            d.Locked = true;
            d = DoorSystem.CreateDoor(r.zone, r.x - 640.0f * rs, 384.0f*rs, r.z + 64.0f * rs, 90, r, false);
            d.Locked = true; d.AutoClose = false;
            d = DoorSystem.CreateDoor(r.zone, r.x + 1280.0f * rs, 384.0f*rs, r.z + 312.0f * rs, 180, r, true);
            d.Locked = true; d.AutoClose = false;
            PositionEntity(d.Buttons[0], r.x + 1120.0f * rs, EntityY(d.Buttons[0],true), r.z + 328.0f * rs, true);
            PositionEntity(d.Buttons[1], r.x + 1120.0f * rs, EntityY(d.Buttons[1],true), r.z + 296.0f * rs, true);
            FreeEntity(d.Obj2); d.Obj2=0;
            d.MTFClose = false;
            d = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z + 1184.0f * rs, 0, r, false);
            d.Locked = true;
            r.Objects[0] = LoadMesh("GFX.Map.IntroDesk.b3d");
            ScaleEntity(r.Objects[0], rs, rs ,rs);
            PositionEntity(r.Objects[0], r.x + 272.0f * rs, 0, r.z + 400.0f * rs);
            EntityParent(r.Objects[0], r.obj);
            de = DecalSystem.Create(0, r.x + 272.0f * rs, 0.005f, r.z + 262.0f * rs, 90, Rand(360), 0);
            EntityParent(de.Obj, r.obj);
            r.Objects[1] = LoadMesh("GFX.Map.IntroDrawer.b3d");
            ScaleEntity(r.Objects[1], rs, rs ,rs);
            PositionEntity(r.Objects[1], r.x + 448.0f * rs, 0, r.z + 192.0f * rs);
            EntityParent(r.Objects[1], r.obj);
            de = DecalSystem.Create(0, r.x + 456.0f * rs, 0.005f, r.z + 135.0f * rs, 90, Rand(360), 0);
            EntityParent(de.Obj, r.obj);
            sc = SecurityCamSystem.Create(r.x - 336.0f * rs, r.y + 352 * rs, r.z + 48.0f * rs, r, true);
            sc.Angle = 270;
            sc.Turn = 45;
            sc.Room = r;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            EntityParent(sc.Obj, r.obj);
            PositionEntity(sc.ScrObj, r.x + 1456 * rs, 608 * rs, r.z +352.0f * rs);
            TurnEntity(sc.ScrObj, 0, 90, 0);
            EntityParent(sc.ScrObj, r.obj);
            r.Objects[2] = CreatePivot();
            PositionEntity(r.Objects[2], EntityX(r.obj) + 40.0f * rs, 460.0f * rs, EntityZ(r.obj) + 1072.0f * rs);
            r.Objects[3] = CreatePivot();
            PositionEntity(r.Objects[3], EntityX(r.obj) - 80.0f * rs, 100.0f * rs, EntityZ(r.obj) + 526.0f * rs);
            r.Objects[4] = CreatePivot();
            PositionEntity(r.Objects[4], EntityX(r.obj) - 128.0f * rs, 100.0f * rs, EntityZ(r.obj) + 320.0f * rs);
            r.Objects[5] = CreatePivot();
            PositionEntity(r.Objects[5], EntityX(r.obj) + 660.0f * rs, 100.0f * rs, EntityZ(r.obj) + 526.0f * rs);
            r.Objects[6] = CreatePivot();
            PositionEntity(r.Objects[6], EntityX(r.obj) + 700 * rs, 100.0f * rs, EntityZ(r.obj) + 320.0f * rs);
            r.Objects[7] = CreatePivot();
            PositionEntity(r.Objects[7], EntityX(r.obj) + 1472.0f * rs, 100.0f * rs, EntityZ(r.obj) + 912.0f * rs);
            for (i = 2; i <= 7; i++)
            {
                EntityParent(r.Objects[i], r.obj);
            }
            //3384,510,2400
            DevilParticleSystem.CreateDevilEmitter(r.x+3384.0f*rs,r.y+510.0f*rs,r.z+2400.0f*rs,r,1,4);
            //[End Block]
        }

        private static void Fill_Room2scps(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 264.0f * rs, 0, r.z, 90, r, true, 0, 3);
            d.AutoClose = false; d.Open = false;
            PositionEntity(d.Buttons[0], r.x + 320.0f * rs, EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true), true);
            PositionEntity(d.Buttons[1], r.x + 224.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true), true);
            d = DoorSystem.CreateDoor(r.zone, r.x - 264.0f * rs, 0, r.z, 270, r, true, 0, 3);
            d.AutoClose = false; d.Open = false;
            PositionEntity(d.Buttons[0], r.x - 320.0f * rs, EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true), true);
            PositionEntity(d.Buttons[1], r.x - 224.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true), true);
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x-560.0f * rs, 0, r.z - 272.0f * rs, 0, r, true, 0, 3);
            r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = false;
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x + 560.0f * rs, 0, r.z - 272.0f * rs, 180, r, true, 0, 3);
            r.RoomDoors[2].AutoClose = false; r.RoomDoors[2].Open = false;
            r.RoomDoors[3] = DoorSystem.CreateDoor(r.zone, r.x + 560.0f * rs, 0, r.z + 272.0f * rs, 180, r, true, 0, 3);
            r.RoomDoors[3].AutoClose = false; r.RoomDoors[3].Open = false;
            r.RoomDoors[4] = DoorSystem.CreateDoor(r.zone, r.x-560.0f * rs, 0, r.z + 272.0f * rs, 0, r, true, 0, 3);
            r.RoomDoors[4].AutoClose = false; r.RoomDoors[4].Open = false;
            it = ItemSystem.CreateItem("SCP-714", "scp714", r.x - 552.0f * rs, r.y + 220.0f * rs, r.z - 760.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("SCP-1025", "scp1025", r.x + 552.0f * rs, r.y + 224.0f * rs, r.z - 758.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("SCP-860", "scp860", r.x + 568.0f * rs, r.y + 178.0f * rs, r.z + 760.0f * rs);
            EntityParent(it.Collider, r.obj);
            sc = SecurityCamSystem.Create(r.x + 560.0f * rs, r.y + 386 * rs, r.z - 416.0f * rs, r);
            sc.Angle = 180; sc.Turn = 30;
            TurnEntity(sc.CameraObj, 30, 0, 0);
            EntityParent(sc.Obj, r.obj);
            sc = SecurityCamSystem.Create(r.x - 560.0f * rs, r.y + 386 * rs, r.z - 416.0f * rs, r);
            sc.Angle = 180; sc.Turn = 30;
            TurnEntity(sc.CameraObj, 30, 0, 0);
            EntityParent(sc.Obj, r.obj);
            sc = SecurityCamSystem.Create(r.x + 560.0f * rs, r.y + 386 * rs, r.z + 480.0f * rs, r);
            sc.Angle = 0; sc.Turn = 30;
            TurnEntity(sc.CameraObj, 30, 0, 0);
            EntityParent(sc.Obj, r.obj);
            sc = SecurityCamSystem.Create(r.x - 560.0f * rs, r.y + 386 * rs, r.z + 480.0f * rs, r);
            sc.Angle = 0; sc.Turn = 30;
            TurnEntity(sc.CameraObj, 30, 0, 0);
            EntityParent(sc.Obj, r.obj);
            it = ItemSystem.CreateItem("Document SCP-714", "paper", r.x - 728.0f * rs, r.y + 288.0f * rs, r.z - 360.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Document SCP-427", "paper", r.x - 608.0f * rs, r.y + 66.0f * rs, r.z + 636.0f * rs);
            EntityParent(it.Collider, r.obj);

            for (i = 0; i <= 14; i++)
            {
                switch (i)
                {
                    case 0: dx = -64.0f; dz = -516.0f; break;
                    case 1: dx = -96.0f; dz = -388.0f; break;
                    case 2: dx = -128.0f; dz = -292.0f; break;
                    case 3: dx = -128.0f; dz = -132.0f; break;
                    case 4: dx = -160.0f; dz = -36.0f; break;
                    case 5: dx = -192.0f; dz = 28.0f; break;
                    case 6: dx = -384.0f; dz = 28.0f; break;
                    case 7: dx = -448.0f; dz = 92.0f; break;
                    case 8: dx = -480.0f; dz = 124.0f; break;
                    case 9: dx = -512.0f; dz = 156.0f; break;
                    case 10: dx = -544.0f; dz = 220.0f; break;
                    case 11: dx = -544.0f; dz = 380.0f; break;
                    case 12: dx = -544.0f; dz = 476.0f; break;
                    case 13: dx = -544.0f; dz = 572.0f; break;
                    case 14: dx = -544.0f; dz = 636.0f; break;
                    default: dx = 0; dz = 0; break;
                }
                de = DecalSystem.Create(Rand(15, 16), r.x + dx * rs, 0.005f, r.z + dz * rs, 90, Rand(360), 0);
                if (i > 10)
                    de.Size = Rnd(0.2f, 0.25f);
                else
                    de.Size = Rnd(0.1f, 0.17f);
                EntityAlpha(de.Obj, 1.0f);
                ScaleSprite(de.Obj, de.Size, de.Size);
                EntityParent(de.Obj, r.obj);
            }
            //[End Block]
        }

        private static void Fill_Room205(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            //d.Doors = CreateDoor(r\zone, r\x + 128.0f * RoomScale, 0, r\z + 640.0f *RoomScale, 90, r, True, False, 3)
            //d\AutoClose = False; d\open = False
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 128.0f * rs, 0, r.z + 640.0f *rs, 90, r, true, 0, 3);
            r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = false;
            //PositionEntity(d\buttons[0], r\x + 320.0f * RoomScale, EntityY(d\buttons[0],True), EntityZ(d\buttons[0],True), True)
            //PositionEntity(d\buttons[1], r\x + 224.0f * RoomScale, EntityY(d\buttons[1],True), EntityZ(d\buttons[1],True), True)
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x - 1392.0f * rs, -128.0f * rs, r.z - 384*rs, 0, r, true, 0, 3, "", true);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = false;
            FreeEntity(r.RoomDoors[0].Buttons[0]); r.RoomDoors[0].Buttons[0] = -1;
            FreeEntity(r.RoomDoors[0].Buttons[1]); r.RoomDoors[0].Buttons[1] = -1;
            sc = SecurityCamSystem.Create(r.x - 1152.0f * rs, r.y + 900.0f * rs, r.z + 176.0f * rs, r, true);
            sc.Angle = 90; sc.Turn = 0;
            EntityParent(sc.Obj, r.obj);
            sc.AllowSaving = false;
            sc.RenderInterval = 0;
            EntityParent(sc.ScrObj, -1);
            PositionEntity(sc.ScrObj, r.x - 1716.0f * rs, r.y + 160.0f * rs, r.z + 176.0f * rs, true);
            TurnEntity(sc.ScrObj, 0, 90, 0);
            ScaleSprite(sc.ScrObj, 896.0f*0.5f*rs, 896.0f*0.5f*rs);
            EntityParent(sc.ScrObj, r.obj);
            //EntityBlend(sc\ScrObj, 2)
            // CameraZoom (sc.Cam, 1.5f)
            HideEntity(sc.ScrOverlay);
            HideEntity(sc.MonitorObj);
            r.Objects[0] = CreatePivot(r.obj);
            PositionEntity(r.Objects[0], r.x - 1536.0f * rs, r.y + 730.0f * rs, r.z + 192.0f * rs, true);
            RotateEntity(r.Objects[0], 0,-90,0,true);
            r.Objects[1] = sc.ScrObj;
            //[End Block]
        }

        private static void Fill_Endroom(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z + 1136 * rs, 0, r, false, 1, 6);
            r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = false;
            FreeEntity(r.RoomDoors[0].Buttons[0]); r.RoomDoors[0].Buttons[0] = -1;
            FreeEntity(r.RoomDoors[0].Buttons[1]); r.RoomDoors[0].Buttons[1] = -1;
            //[End Block]
        }

        private static void Fill_Endroomc(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x+1024*rs, 0, r.z, 0, r, false, 2, 0, "");
            d.Open = false; d.AutoClose = false; d.Locked = true;
            //[End Block]
        }

        private static void Fill_Coffin(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z - 448.0f * rs, 0, r, false, 1, 2);
            d.AutoClose = false; d.Open = false;
            PositionEntity(d.Buttons[0], r.x - 384.0f * rs, 0.7f, r.z - 280.0f * rs, true);
            sc = SecurityCamSystem.Create(r.x - 320.0f * rs, r.y + 704 * rs, r.z + 288.0f * rs, r, true);
            sc.Angle = 45 + 180;
            sc.Turn = 45;
            sc.CoffinEffect = 1;
            TurnEntity(sc.CameraObj, 120, 0, 0);
            EntityParent(sc.Obj, r.obj);
            SecurityCamSystem.CoffinCam = sc;
            PositionEntity(sc.ScrObj, r.x - 800 * rs, 288.0f * rs, r.z - 340.0f * rs);
            EntityParent(sc.ScrObj, r.obj);
            TurnEntity(sc.ScrObj, 0, 180, 0);
            r.Objects[2] = CopyEntity(MapAssets.LeverBaseObj);
            r.Objects[3] = CopyEntity(MapAssets.LeverObj);
            r.Levers[0] = r.Objects[3];
            for (i = 0; i <= 1; i++)
            {
                ScaleEntity(r.Objects[2 + i], 0.04f, 0.04f, 0.04f);
                PositionEntity(r.Objects[2 + i], r.x - 800.0f * rs, r.y + 180.0f * rs, r.z - 336 * rs, true);
                EntityParent(r.Objects[2 + i], r.obj);
            }
            RotateEntity(r.Objects[2], 0, 180, 0);
            RotateEntity(r.Objects[3], 10, 0, 0);
            EntityPickMode(r.Objects[3], 1, false);
            EntityRadius(r.Objects[3], 0.1f);
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], r.x, -1320.0f * rs, r.z + 2304.0f * rs);
            EntityParent(r.Objects[0], r.obj);
            it = ItemSystem.CreateItem("Document SCP-895", "paper", r.x - 688.0f * rs, r.y + 133.0f * rs, r.z - 304.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Level 3 Key Card", "key3", r.x + 240.0f * rs, r.y -1456.0f * rs, r.z + 2064.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Night Vision Goggles", "nvgoggles", r.x + 280.0f * rs, r.y -1456.0f * rs, r.z + 2164.0f * rs);
            EntityParent(it.Collider, r.obj);
            it.State = 400;
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x + 96.0f*rs, -1532.0f * rs, r.z + 2016.0f * rs,true);
            //de.Decals = CreateDecal(0, r\x + 96.0f*RoomScale, -1535.0f * RoomScale, r\z + 32.0f * RoomScale, 90, Rand(360), 0)
            //EntityParent de\obj, r\obj
            //[End Block]
        }

        private static void Fill_Room2tesla(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], r.x - 114.0f * rs, 0.0f, r.z);
            EntityParent(r.Objects[0], r.obj);
            r.Objects[1] = CreatePivot();
            PositionEntity(r.Objects[1], r.x + 114.0f * rs, 0.0f, r.z);
            EntityParent(r.Objects[1], r.obj);
            r.Objects[2] = CreatePivot();
            PositionEntity(r.Objects[2], r.x, 0.0f, r.z);
            EntityParent(r.Objects[2], r.obj);
            r.Objects[3] = CreateSprite();
            EntityTexture(r.Objects[3], MapAssets.TeslaTexture);
            SpriteViewMode(r.Objects[3],2);
            //ScaleSprite (r\Objects[3],((512.0f * RoomScale)/2.0f),((512.0f * RoomScale)/2.0f))
            EntityBlend(r.Objects[3], 3);
            // EntityFX(r.Objects[3], 1 + 8 + 16)
            PositionEntity(r.Objects[3], r.x, 0.8f, r.z);
            HideEntity(r.Objects[3]);
            EntityParent(r.Objects[3], r.obj);
            w = WaypointSystem.Create(r.x, r.y + 66.0f * rs, r.z + 292.0f * rs, null, r);
            w2 = WaypointSystem.Create(r.x, r.y + 66.0f * rs, r.z - 284.0f * rs, null, r);
            w.Connected[0] = w2; w.Dist[0] = EntityDistance(w.Obj, w2.Obj);
            w2.Connected[0] = w; w2.Dist[0] = w.Dist[0];
            r.Objects[4] = CreateSprite();
            PositionEntity(r.Objects[4], r.x - 32 * rs, 568 * rs, r.z);
            ScaleSprite(r.Objects[4], 0.03f, 0.03f);
            EntityTexture(r.Objects[4], MapAssets.LightSpriteTex(1));
            EntityBlend(r.Objects[4], 3);
            EntityParent(r.Objects[4], r.obj);
            HideEntity(r.Objects[4]);
            r.Objects[5] = CreatePivot();
            PositionEntity(r.Objects[5],r.x,0,r.z-800*rs);
            EntityParent(r.Objects[5],r.obj);
            r.Objects[6] = CreatePivot();
            PositionEntity(r.Objects[6],r.x,0,r.z+800*rs);
            EntityParent(r.Objects[6],r.obj);
            foreach (var otherRoom in MapSystem.All)
            {
                if (otherRoom!=r)
                {
                    if (r2.RoomName  == "room2tesla" || r2.RoomName == "room2tesla_lcz" || r2.RoomName == "room2tesla_hcz")
                    {
                        r.Objects[7] = CopyEntity(r2.Objects[7],r.obj); //don't load the mesh again;
                        break;
                    }
                }
            }
            if (r.Objects[7]==-1) { r.Objects[7] = LoadMesh("GFX.Map.Room2tesla_caution.b3d",r.obj); }
            //[End Block]
        }

        private static void Fill_Room2doors(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z + 528.0f * rs, 0, r, true);
            d.AutoClose = false; FreeEntity(d.Buttons[0]); d.Buttons[0] = -1;
            PositionEntity(d.Buttons[0], r.x - 832.0f * rs, 0.7f, r.z + 160.0f * rs, true);
            PositionEntity(d.Buttons[1], r.x + 160.0f * rs, 0.7f, r.z + 536.0f * rs, true);
            //RotateEntity(d\buttons[1], 0, 90, 0, True)
            d2 = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z - 528.0f * rs, 180, r, true);
            d2.AutoClose = false; FreeEntity (d2.Buttons[0]); d2.Buttons[0] = -1;
            PositionEntity(d2.Buttons[1], r.x +160.0f * rs, 0.7f, r.z - 536.0f * rs, true);
            //RotateEntity(d2\buttons[1], 0, 90, 0, True)
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], r.x - 832.0f * rs, 0.5f, r.z);
            EntityParent(r.Objects[0], r.obj);
            d2.LinkedDoor = d; d.LinkedDoor = d2;
            d.Open = false; d2.Open = true;
            //[End Block]
        }

        private static void Fill_Room914(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            //d = CreateDoor(r\zone, r\x, 0, r\z - 368.0f * RoomScale, 0, r, False, True, 2)
            //d\dir = 1; d\AutoClose = False; d\open = False
            //PositionEntity (d\buttons[0], r\x - 496.0f * RoomScale, 0.7f, r\z - 272.0f * RoomScale, True)
            //TurnEntity(d\buttons[0], 0, 90, 0)
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z-368.0f*rs, 0, r, false, 1, 2);
            r.RoomDoors[2].Dir=1; r.RoomDoors[2].AutoClose=false; r.RoomDoors[2].Open=false;
            PositionEntity(r.RoomDoors[2].Buttons[0], r.x - 496.0f * rs, 0.7f, r.z - 272.0f * rs, true);
            TurnEntity(r.RoomDoors[2].Buttons[0], 0, 90, 0);
            r.Objects[0] = LoadMesh("GFX.Map.914key.x");
            r.Objects[1] = LoadMesh("GFX.Map.914knob.x");
            for (i = 0; i <= 1; i++)
            {
                ScaleEntity(r.Objects[i], rs, rs, rs);
                EntityPickMode(r.Objects[i], 2);
            }
            PositionEntity(r.Objects[0], r.x, r.y + 190.0f * rs, r.z + 374.0f * rs);
            PositionEntity(r.Objects[1], r.x, r.y + 230.0f * rs, r.z + 374.0f * rs);
            EntityParent(r.Objects[0], r.obj);
            EntityParent(r.Objects[1], r.obj);
            d = DoorSystem.CreateDoor(r.zone, r.x - 624.0f * rs, 0.0f, r.z + 528.0f * rs, 180, r, true);
            FreeEntity(d.Obj2); d.Obj2 = -1;
            FreeEntity(d.Buttons[0]); d.Buttons[0] = -1;
            FreeEntity(d.Buttons[1]); d.Buttons[1] = -1;
            d.Dir = 4;
            r.RoomDoors[0] = d; d.AutoClose = false;
            d = DoorSystem.CreateDoor(r.zone, r.x + 816.0f * rs, 0.0f, r.z + 528.0f * rs, 180, r, true);
            FreeEntity(d.Obj2); d.Obj2 = -1;
            FreeEntity(d.Buttons[0]); d.Buttons[0] = -1;
            FreeEntity(d.Buttons[1]); d.Buttons[1] = -1;
            d.Dir = 4;
            r.RoomDoors[1] = d; d.AutoClose = false;
            r.Objects[2] = CreatePivot();
            r.Objects[3] = CreatePivot();
            PositionEntity(r.Objects[2], r.x - 712.0f * rs, 0.5f, r.z + 640.0f * rs);
            PositionEntity(r.Objects[3], r.x + 728.0f * rs, 0.5f, r.z + 640.0f * rs);
            EntityParent(r.Objects[2], r.obj);
            EntityParent(r.Objects[3], r.obj);
            it = ItemSystem.CreateItem("Addendum: 5/14 Test Log", "paper", r.x +954.0f * rs, r.y +228.0f * rs, r.z + 127.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("First Aid Kit", "firstaid", r.x + 960.0f * rs, r.y + 112.0f * rs, r.z - 40.0f * rs);
            EntityParent(it.Collider, r.obj); RotateEntity(it.Collider, 0, 90, 0);
            it = ItemSystem.CreateItem("Dr. L's Note", "paper", r.x - 928.0f * rs, 160.0f * rs, r.z - 160.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room173(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], EntityX(r.obj) + 40.0f * rs, 460.0f * rs, EntityZ(r.obj) + 1072.0f * rs);
            r.Objects[1] = CreatePivot();
            PositionEntity(r.Objects[1], EntityX(r.obj) - 80.0f * rs, 100.0f * rs, EntityZ(r.obj) + 526.0f * rs);
            r.Objects[2] = CreatePivot();
            PositionEntity(r.Objects[2], EntityX(r.obj) - 128.0f * rs, 100.0f * rs, EntityZ(r.obj) + 320.0f * rs);
            r.Objects[3] = CreatePivot();
            PositionEntity(r.Objects[3], EntityX(r.obj) + 660.0f * rs, 100.0f * rs, EntityZ(r.obj) + 526.0f * rs);
            r.Objects[4] = CreatePivot();
            PositionEntity(r.Objects[4], EntityX(r.obj) + 700 * rs, 100.0f * rs, EntityZ(r.obj) + 320.0f * rs);
            r.Objects[5] = CreatePivot();
            PositionEntity(r.Objects[5], EntityX(r.obj) + 1472.0f * rs, 100.0f * rs, EntityZ(r.obj) + 912.0f * rs);
            for (i = 0; i <= 5; i++)
            {
                EntityParent(r.Objects[i], r.obj);
            }
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, EntityX(r.obj) + 288.0f * rs, 0, EntityZ(r.obj) + 384.0f * rs, 90, r, false, 1);
            r.RoomDoors[1].AutoClose = false ;; r.RoomDoors[1].Locked = true;
            r.RoomDoors[1].Dir = 1; r.RoomDoors[1].Open = false;
            FreeEntity(r.RoomDoors[1].Buttons[0]); r.RoomDoors[1].Buttons[0] = -1;
            FreeEntity(r.RoomDoors[1].Buttons[1]); r.RoomDoors[1].Buttons[1] = -1;
            de = DecalSystem.Create(Rand(4, 5), EntityX(r.Objects[5], true), 0.002f, EntityZ(r.Objects[5], true), 90, Rnd(360), 0);
            de.Size = 1.2f;
            ScaleSprite(de.Obj, de.Size, de.Size);
            for (int xtemp = 0; xtemp <= 1; xtemp++)
            {
                for (int ztemp = 0; ztemp <= 1; ztemp++)
                {
                    de = DecalSystem.Create(Rand(4, 6), r.x + 700.0f * rs + xtemp * 700.0f * rs + Rnd(-0.5f, 0.5f), Rnd(0.001f, 0.0018f), r.z + 600 * ztemp * rs + Rnd(-0.5f, 0.5f), 90, Rnd(360), 0);
                    de.Size = Rnd(0.5f, 0.8f);
                    de.Alpha = Rnd(0.8f, 1.0f);
                    ScaleSprite(de.Obj, de.Size, de.Size);
                }
            }
            //AddLight(r, r\x-224.0f*RoomScale, r\y+640.0f*RoomScale, r\z+128.0f*RoomScale,2,2,200,200,200)
            //AddLight(r, r\x-1056.0f*RoomScale, r\y+608.0f*RoomScale, r\z+416.0f*RoomScale,2,2,200,200,200)
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x - 1008.0f * rs, 0, r.z - 688.0f * rs, 90, r, true, 0, 0, "", true);
            r.RoomDoors[2].AutoClose = false; r.RoomDoors[2].Open = false; r.RoomDoors[2].Locked = true;
            FreeEntity(r.RoomDoors[2].Buttons[0]); r.RoomDoors[2].Buttons[0] = -1;
            FreeEntity(r.RoomDoors[2].Buttons[1]); r.RoomDoors[2].Buttons[1] = -1;
            r.RoomDoors[3] = DoorSystem.CreateDoor(r.zone, r.x - 2320.0f * rs, 0, r.z - 1248.0f * rs, 90, r, true);
            r.RoomDoors[3].AutoClose = false; r.RoomDoors[3].Open = true; r.RoomDoors[3].Locked = true;
            r.RoomDoors[4] = DoorSystem.CreateDoor(r.zone, r.x - 4352.0f * rs, 0, r.z - 1248.0f * rs, 90, r, true);
            r.RoomDoors[4].AutoClose = false; r.RoomDoors[4].Open = true; r.RoomDoors[4].Locked = true;
            //the door in the office below the walkway
            r.RoomDoors[7] = DoorSystem.CreateDoor(r.zone, r.x - 3712.0f * rs, -385*rs, r.z - 128.0f * rs, 0, r, true);
            r.RoomDoors[7].AutoClose = false; r.RoomDoors[7].Open = true;
            d = DoorSystem.CreateDoor(r.zone, r.x - 3712 * rs, -385*rs, r.z - 2336 * rs, 0, r, false);
            d.Locked = true; d.DisableWaypoint = true;
            //the door from the concrete tunnel to the large hall
            d = DoorSystem.CreateDoor(r.zone, r.x - 6864 * rs, 0, r.z - 1248 * rs, 90, r, true);
            d.AutoClose = false;
            d.Locked = true;
            //the locked door to the lower level of the hall
            d = DoorSystem.CreateDoor(r.zone, r.x - 5856 * rs, 0, r.z - 1504 * rs, 0, r, false);
            d.Locked = true; d.DisableWaypoint = true;
            //the door to the staircase in the office room
            d = DoorSystem.CreateDoor(r.zone, r.x - 2432 * rs, 0, r.z - 1000 * rs, 0, r, false);
            PositionEntity(d.Buttons[0], r.x - 2592 * rs, EntityY(d.Buttons[0],true), r.z - 1016 * rs, true);
            PositionEntity(d.Buttons[1], r.x - 2592 * rs, EntityY(d.Buttons[0],true), r.z - 984 * rs, true);
            d.Locked = true; d.DisableWaypoint = true;
            tex = LoadTextureHandle("GFX.Map.Door02.jpg");
            for (int ztemp = 0; ztemp <= 1; ztemp++)
            {
                d = DoorSystem.CreateDoor(r.zone, r.x - 5760 * rs, 0, r.z + (320+896*ztemp) * rs, 0, r, false);
                d.Locked = true;
                d.DisableWaypoint = true;
                d = DoorSystem.CreateDoor(r.zone, r.x - 8288 * rs, 0, r.z + (320+896*ztemp) * rs, 0, r, false);
                d.Locked = true;
                if (ztemp == 0) { d.Open = true; } else { d.DisableWaypoint = true; }
                for (int xtemp = 0; xtemp <= 2; xtemp++)
                {
                    d = DoorSystem.CreateDoor(r.zone, r.x - (7424.0f-512.0f*xtemp) * rs, 0, r.z + (1008.0f-480.0f*ztemp) * rs, 180 * (ztemp == 0 ? 1 : 0), r, false);
                    EntityTexture(d.Obj, tex);
                    d.Locked = true;
                    FreeEntity(d.Obj2); d.Obj2 = -1;
                    FreeEntity(d.Buttons[0]); d.Buttons[0] = -1;
                    FreeEntity(d.Buttons[1]); d.Buttons[1] = -1;
                    d.DisableWaypoint = true;
                }
                for (int xtemp = 0; xtemp <= 4; xtemp++)
                {
                    d = DoorSystem.CreateDoor(r.zone, r.x - (5120.0f-512.0f*xtemp) * rs, 0, r.z + (1008.0f-480.0f*ztemp) * rs, 180 * (ztemp == 0 ? 1 : 0), r, false);
                    EntityTexture(d.Obj, tex);
                    d.Locked = true;
                    FreeEntity(d.Obj2); d.Obj2 = -1;
                    FreeEntity(d.Buttons[0]); d.Buttons[0] = -1;
                    FreeEntity(d.Buttons[1]); d.Buttons[1] = -1;
                    d.DisableWaypoint = true;
                    if (xtemp == 2 && ztemp == 1) { r.RoomDoors[6] = d; }
                }
            }
            ItemSystem.CreateItem("Class D Orientation Leaflet", "paper", r.x-(2914+1024)*rs, 170.0f*rs, r.z+40*rs);
            sc = SecurityCamSystem.Create(r.x - 4048.0f * rs, r.y - 32.0f * rs, r.z - 1232.0f * rs, r, true);
            sc.Angle = 270;
            sc.Turn = 45;
            sc.Room = r;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            EntityParent(sc.Obj, r.obj);
            PositionEntity(sc.ScrObj, r.x - 2256 * rs, 224.0f * rs, r.z - 928.0f * rs);
            TurnEntity(sc.ScrObj, 0, 90, 0);
            EntityParent(sc.ScrObj, r.obj);
            r.Objects[9] = LoadMesh("GFX.Map.173_2.b3d",r.obj);
            EntityType(r.Objects[9],1);
            EntityPickMode(r.Objects[9],2);
            r.Objects[10] = LoadMesh("GFX.Map.Intro_labels.b3d",r.obj);
            //[End Block]
        }

        private static void Fill_Room2ccont(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 64.0f * rs, 0.0f, r.z + 368.0f * rs, 180, r, false, 0, 2);
            d.AutoClose = false; d.Open = false;
            it = ItemSystem.CreateItem("Note from Daniel", "paper", r.x-400.0f*rs,1040.0f*rs,r.z+115.0f*rs);
            EntityParent(it.Collider, r.obj);
            for (n = 0; n <= 2; n++)
            {
                r.Objects[n * 2] = CopyEntity(MapAssets.LeverBaseObj);
                r.Objects[n * 2 + 1] = CopyEntity(MapAssets.LeverObj);
                r.Levers[n] = r.Objects[n * 2 + 1];
                for (i = 0; i <= 1; i++)
                {
                    ScaleEntity(r.Objects[n * 2 + i], 0.04f, 0.04f, 0.04f);
                    PositionEntity(r.Objects[n * 2 + i], r.x - 240.0f * rs, r.y + 1104.0f * rs, r.z + (632.0f - 64.0f * n) * rs, true);
                    EntityParent(r.Objects[n * 2 + i], r.obj);
                }
                RotateEntity(r.Objects[n * 2], 0, -90, 0);
                RotateEntity(r.Objects[n * 2 + 1], 10, -90 - 180, 0);
                EntityPickMode(r.Objects[n * 2 + 1], 1, false);
                EntityRadius(r.Objects[n * 2 + 1], 0.1f);
            }
            sc = SecurityCamSystem.Create(r.x-265.0f*rs, r.y+1280.0f*rs, r.z+105.0f*rs, r);
            sc.Angle = 45;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            //[End Block]
        }

        private static void Fill_Room106(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            it = ItemSystem.CreateItem("Level 5 Key Card", "key5", r.x - 752.0f * rs, r.y - 592 * rs, r.z + 3026.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Dr. Allok's Note", "paper", r.x - 416.0f * rs, r.y - 576 * rs, r.z + 2492.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Recall Protocol RP-106-N", "paper", r.x + 268.0f * rs, r.y - 576 * rs, r.z + 2593.0f * rs);
            EntityParent(it.Collider, r.obj);
            d = DoorSystem.CreateDoor(r.zone, r.x - 968.0f * rs, -764.0f * rs, r.z + 1392.0f * rs, 0, r, false, 0, 4);
            d.AutoClose = false; d.Open = false;
            d = DoorSystem.CreateDoor(r.zone, r.x, 0, r.z - 464.0f * rs, 0, r, false, 0, 4);
            d.AutoClose = false; d.Open = false;
            d = DoorSystem.CreateDoor(r.zone, r.x - 624.0f * rs, -1280.0f * rs, r.z, 90, r, false, 0, 4);
            d.AutoClose = false; d.Open = false;
            r.Objects[6] = LoadMesh("GFX.Map.Room1062.b3d");
            ScaleEntity(r.Objects[6],rs,rs,rs);
            EntityType(r.Objects[6], 1);
            EntityPickMode(r.Objects[6], 3);
            PositionEntity(r.Objects[6],r.x+784.0f*rs,-980.0f*rs,r.z+720.0f*rs,true);
            //if BumpEnabled 
            //
            //	For i = 1 To CountSurfaces(r\Objects[6])
            //		sf = GetSurface(r\Objects[6],i)
            //		b = GetSurfaceBrush( sf )
            //		t = GetBrushTexture(b,1)
            //		texname$ =  StripPath(TextureName(t))
            //
            //		mat.Materials=GetCache(texname)
            //		if mat!=Null 
            //			if mat\Bump!=0 
            //				t1 = GetBrushTexture(b,0)
            //
            //				BrushTexture b, t1, 0, 0
            //				BrushTexture b, mat\Bump, 0, 1
            //				BrushTexture b, t, 0, 2
            //
            //				PaintSurface sf,b
            //
            //				if t1!=0  FreeTexture(t1); t1=0
            //			EndIf
            //		EndIf
            //
            //		if t!=0  FreeTexture(t); t=0
            //		if b!=0  FreeBrush b; b=0
            //	Next
            //
            //EndIf
            EntityParent(r.Objects[6], r.obj);
            for (n = 0; n <= 2; n += 2)
            {
                r.Objects[n] = CopyEntity(MapAssets.LeverBaseObj);
                r.Objects[n+1] = CopyEntity(MapAssets.LeverObj);
                r.Levers[n/2] = r.Objects[n+1];
                for (i = 0; i <= 1; i++)
                {
                    ScaleEntity(r.Objects[n+i], 0.04f, 0.04f, 0.04f);
                    PositionEntity(r.Objects[n+i], r.x - (555.0f - 81.0f * (n/2)) * rs, r.y - 576.0f * rs, r.z + 3040.0f * rs, true);
                    EntityParent(r.Objects[n+i], r.obj);
                }
                RotateEntity(r.Objects[n], 0, 0, 0);
                RotateEntity(r.Objects[n+1], 10, -180, 0);
                //EntityPickMode(r\Objects[n * 2 + 1], 2)
                EntityPickMode(r.Objects[n+1], 1, false);
                EntityRadius(r.Objects[n+1], 0.1f);
                //makecollbox(r\Objects[n * 2 + 1])
            }
            RotateEntity(r.Objects[1], 81,-180,0);
            RotateEntity(r.Objects[3], -81,-180,0);
            r.Objects[4] = ButtonSystem.Create(r.x - 146.0f*rs, r.y - 576.0f * rs, r.z + 3045.0f * rs, 0,0,0);
            EntityParent(r.Objects[4],r.obj);
            sc = SecurityCamSystem.Create(r.x + 768.0f * rs, r.y + 1392.0f * rs, r.z + 1696.0f * rs, r, true);
            sc.Angle = 45 + 90 + 180;
            sc.Turn = 20;
            TurnEntity(sc.CameraObj, 45, 0, 0);
            EntityParent(sc.Obj, r.obj);
            r.Objects[7] = sc.CameraObj;
            r.Objects[8] = sc.Obj;
            PositionEntity(sc.ScrObj, r.x - 272.0f * rs, -544.0f * rs, r.z + 3020.0f * rs);
            TurnEntity(sc.ScrObj, 0, -10, 0);
            EntityParent(sc.ScrObj, r.obj);
            sc.CoffinEffect = 0;
            //r\NPC[0] = CreateNPC(NPCtypeD, r\x + 1088.0f * RoomScale, 1096.0f * RoomScale, r\z + 1728.0f * RoomScale)
            r.Objects[5] = CreatePivot();
            TurnEntity(r.Objects[5], 0,180,0);
            PositionEntity(r.Objects[5], r.x + 1088.0f * rs, 1104.0f * rs, r.z + 1888.0f * rs);
            EntityParent(r.Objects[5], r.obj);
            //HideEntity r\NPC[0]\obj
            r.Objects[9] = CreatePivot(r.obj);
            PositionEntity(r.Objects[9], r.x - 272 * rs, r.y - 672.0f * rs, r.z + 2736.0f * rs, true);
            r.Objects[10] = CreatePivot(r.obj);
            PositionEntity(r.Objects[10], r.x, r.y, r.z - 720.0f * rs, true);
            //[End Block]
        }

        private static void Fill_Room1archive(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";

            for (int xtemp = 0; xtemp <= 1; xtemp++)
            for (int ytemp = 0; ytemp <= 2; ytemp++)
            for (int ztemp = 0; ztemp <= 2; ztemp++)
            {
                tempstr = "9V Battery";
                tempstr2 = "bat";
                chance = Rand(-10, 100);
                if (chance < 0) continue;
                if (chance < 40)
                {
                    tempstr = "Document SCP-";
                    switch (Rand(1, 6))
                    {
                        case 1: tempstr += "1123"; break;
                        case 2: tempstr += "1048"; break;
                        case 3: tempstr += "939"; break;
                        case 4: tempstr += "682"; break;
                        case 5: tempstr += "079"; break;
                        case 6: tempstr += (Rand(0, 1) == 0) ? "096" : "966"; break;
                    }
                    tempstr2 = "paper";
                }
                else if (chance >= 40 && chance < 45)
                {
                    temp3 = Rand(1, 2);
                    tempstr = "Level " + temp3 + " Key Card";
                    tempstr2 = "key" + temp3;
                }
                else if (chance >= 45 && chance < 50)
                {
                    tempstr = "First Aid Kit";
                    tempstr2 = "firstaid";
                }
                else if (chance >= 50 && chance < 60)
                {
                    tempstr = "9V Battery";
                    tempstr2 = "bat";
                }
                else if (chance >= 60 && chance < 70)
                {
                    tempstr = "S-NAV 300 Navigator";
                    tempstr2 = "nav";
                }
                else if (chance >= 70 && chance < 85)
                {
                    tempstr = "Radio Transceiver";
                    tempstr2 = "radio";
                }
                else if (chance >= 85 && chance < 95)
                {
                    tempstr = "Clipboard";
                    tempstr2 = "clipboard";
                }
                else if (chance >= 95 && chance <= 100)
                {
                    temp3 = Rand(1, 3);
                    switch (temp3)
                    {
                        case 1: tempstr = "Playing Card"; break;
                        case 2: tempstr = "Mastercard"; break;
                        case 3: tempstr = "Origami"; break;
                    }
                    tempstr2 = "misc";
                }
                x = (-672.0f + 864.0f * xtemp) * rs;
                y = (96.0f + 96.0f * ytemp) * rs;
                z = (480.0f - 352.0f * ztemp + Rnd(-96.0f, 96.0f)) * rs;
                it = ItemSystem.CreateItem(tempstr, tempstr2, r.x + x, y, r.z + z);
                EntityParent(it.Collider, r.obj);
            }
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x, r.y, r.z - 528.0f * rs, 0, r, false, 0, 6);
            sc = SecurityCamSystem.Create(r.x - 256.0f * rs, r.y + 384.0f * rs, r.z + 640.0f * rs, r);
            sc.Angle = 180;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 20, 0, 0);

        }

        private static void Fill_Room2test1074(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";

            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x, r.y, r.z, 0, r, false, 0, 0, "");
            r.RoomDoors[0].Locked = true;
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 336.0f * rs, r.y, r.z + 671.0f * rs, 90, r, true, 0, 3);
            r.RoomDoors[1].AutoClose = false;
            r.RoomDoors[2] = DoorSystem.CreateDoor(r.zone, r.x + 336.0f * rs, r.y, r.z - 800.0f * rs, 90, r, true, 0, 3);
            r.RoomDoors[2].AutoClose = false;
            r.RoomDoors[3] = DoorSystem.CreateDoor(r.zone, r.x + 672.0f * rs, r.y, r.z, 0, r, false, 0);
            r.Textures[0] = LoadTextureHandle("GFX.Map.1074tex0.jpg");
            r.Textures[1] = LoadTextureHandle("GFX.Map.1074tex1.jpg");
            it = ItemSystem.CreateItem("Document SCP-1074", "paper", r.x + 300.0f * rs, r.y + 20.0f * rs, r.z + 671.0f * rs);
            EntityParent(it.Collider, r.obj);
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0], r.x + 835.0f * rs, r.y + 165.0f * rs, r.z + 540.0f * rs, true);
            EntityParent(r.Objects[0], r.obj);
            r.Objects[1] = CreatePivot();
            PositionEntity(r.Objects[1], r.x + 835.0f * rs, r.y + 10.0f * rs, r.z + 300.0f * rs, true);
            EntityParent(r.Objects[1], r.obj);

        }

        private static void Fill_Room1123(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            it = ItemSystem.CreateItem("Document SCP-1123", "paper", r.x + 511.0f * rs, r.y + 125.0f * rs, r.z - 936.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("SCP-1123", "1123", r.x + 832.0f * rs, r.y + 166.0f * rs, r.z + 784.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Leaflet", "paper", r.x - 816.0f * rs, r.y + 704.0f * rs, r.z+ 888.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Gas Mask", "gasmask", r.x + 457.0f * rs, r.y + 150.0f * rs, r.z + 960.0f * rs);
            EntityParent(it.Collider, r.obj);
            d = DoorSystem.CreateDoor(r.zone, r.x + 832.0f * rs, 0.0f, r.z + 367.0f * rs, 0, r, false, 0, 3);
            PositionEntity(d.Buttons[0], r.x + 956.0f * rs, EntityY(d.Buttons[0],true), r.z + 352.0f * rs, true);
            PositionEntity(d.Buttons[1], r.x + 713.0f * rs, EntityY(d.Buttons[1],true), r.z + 384.0f * rs, true);
            FreeEntity(d.Obj2); d.Obj2 = -1;
            d = DoorSystem.CreateDoor(r.zone, r.x + 280.0f * rs, 0.0f, r.z - 607.0f * rs, 90, r, false, 0);
            PositionEntity(d.Buttons[0], EntityX(d.Buttons[0],true), EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true), true);
            PositionEntity(d.Buttons[1], EntityX(d.Buttons[1],true), EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true), true);
            d = DoorSystem.CreateDoor(r.zone, r.x + 280.0f * rs, 512.0f * rs, r.z - 607.0f * rs, 90, r, false, 0);
            PositionEntity(d.Buttons[0], EntityX(d.Buttons[0],true), EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true), true);
            FreeEntity(d.Buttons[1]); d.Buttons[1] = -1;
            r.RoomDoors[0] = d;
            //PositionEntity(d\buttons[1], EntityX(d\buttons[1],True), EntityY(d\buttons[1],True), EntityZ(d\buttons[1],True), True)
            r.Objects[3] = CreatePivot(r.obj);
            PositionEntity(r.Objects[3], r.x + 832.0f * rs, r.y + 166.0f * rs, r.z + 784.0f * rs, true);
            r.Objects[4] = CreatePivot(r.obj);
            PositionEntity(r.Objects[4], r.x -648.0f * rs, r.y + 592.0f * rs, r.z + 692.0f * rs, true);
            r.Objects[5] = CreatePivot(r.obj);
            PositionEntity(r.Objects[5], r.x + 828.0f * rs, r.y + 592.0f * rs, r.z + 592.0f * rs, true);
            r.Objects[6] = CreatePivot(r.obj);
            PositionEntity(r.Objects[6], r.x - 76.0f * rs, r.y + 620.0f * rs, r.z + 744.0f * rs, true);
            r.Objects[7] = CreatePivot(r.obj);
            PositionEntity(r.Objects[7], r.x - 640.0f * rs, r.y + 620.0f * rs, r.z - 864.0f * rs, true);
            r.Objects[8] = LoadMesh("GFX.Map.Forest.Door_frame.b3d");
            PositionEntity(r.Objects[8], r.x - 272.0f * rs, 512.0f * rs, r.z + 288.0f * rs,true);
            RotateEntity(r.Objects[8],0,90,0,true);
            ScaleEntity(r.Objects[8],45.0f*rs,45.0f*rs,80.0f*rs,true);
            EntityParent(r.Objects[8],r.obj);
            r.Objects[9] =  LoadMesh("GFX.Map.Forest.Door.b3d");
            PositionEntity(r.Objects[9],r.x - 272.0f * rs, 512.0f * rs, r.z + (288.0f-70) * rs,true);
            RotateEntity(r.Objects[9],0,10,0,true);
            EntityType(r.Objects[9], 1);
            ScaleEntity(r.Objects[9],46.0f*rs,45.0f*rs,46.0f*rs,true);
            EntityParent(r.Objects[9],r.obj);
            r.Objects[10] = CopyEntity(r.Objects[8]);
            PositionEntity(r.Objects[10], r.x - 272.0f * rs, 512.0f * rs, r.z + 736.0f * rs,true);
            RotateEntity(r.Objects[10],0,90,0,true);
            ScaleEntity(r.Objects[10],45.0f*rs,45.0f*rs,80.0f*rs,true);
            EntityParent(r.Objects[10],r.obj);
            r.Objects[11] =  CopyEntity(r.Objects[9]);
            PositionEntity(r.Objects[11],r.x - 272.0f * rs, 512.0f * rs, r.z + (736.0f-70) * rs,true);
            RotateEntity(r.Objects[11],0,90,0,true);
            EntityType(r.Objects[11], 1);
            ScaleEntity(r.Objects[11],46.0f*rs,45.0f*rs,46.0f*rs,true);
            EntityParent(r.Objects[11],r.obj);
            r.Objects[12] = CopyEntity(r.Objects[8]);
            PositionEntity(r.Objects[12], r.x - 592.0f * rs, 512.0f * rs, r.z - 704.0f * rs,true);
            RotateEntity(r.Objects[12],0,0,0,true);
            ScaleEntity(r.Objects[12],45.0f*rs,45.0f*rs,80.0f*rs,true);
            EntityParent(r.Objects[12],r.obj);
            r.Objects[13] =  CopyEntity(r.Objects[9]);
            PositionEntity(r.Objects[13],r.x - (592.0f+70.0f) * rs, 512.0f * rs, r.z - 704.0f * rs,true);
            RotateEntity(r.Objects[13],0,0,0,true);
            EntityType(r.Objects[13], 1);
            ScaleEntity(r.Objects[13],46.0f*rs,45.0f*rs,46.0f*rs,true);
            EntityParent(r.Objects[13],r.obj);
            r.Objects[14] = LoadMesh("GFX.Map.1123_hb.b3d",r.obj);
            EntityPickMode(r.Objects[14],2);
            EntityType(r.Objects[14],1);
            EntityAlpha(r.Objects[14],0.0f);
            //[End Block]
        }

        private static void Fill_Pocketdimension(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";

            hallway = LoadMesh("GFX.Map.pocketdimension2.b3d");
            r.Objects[8] = LoadMesh("GFX.Map.pocketdimension3.b3d");
            r.Objects[9] = LoadMesh("GFX.Map.pocketdimension4.b3d");
            r.Objects[10] = CopyEntity(r.Objects[9]);
            r.Objects[11] = LoadMesh("GFX.Map.pocketdimension5.b3d");
            terrain = LoadMesh("GFX.Map.pocketdimensionterrain.b3d");
            ScaleEntity(terrain, rs, rs, rs, true);
            PositionEntity(terrain, 0, 2944, 0, true);
            ItemSystem.CreateItem("Burnt Note", "paper", EntityX(r.obj), 0.5f, EntityZ(r.obj) + 3.5f);
            for (i = 8; i <= 11; i++)
            {
                ScaleEntity(r.Objects[i], rs, rs, rs);
                EntityType(r.Objects[i], 1);
                EntityPickMode(r.Objects[i], 2);
                PositionEntity(r.Objects[i], r.x, r.y, r.z + 32.0f, true);
            }
            ScaleEntity(terrain, rs, rs, rs);
            EntityType(terrain, 1);
            EntityPickMode(terrain, 3);
            PositionEntity(terrain, r.x, r.y + 2944.0f * rs, r.z + 32.0f, true);
            r.RoomDoors[0] = DoorSystem.CreateDoor(0, r.x, 2048 * rs, r.z + 32.0f - 1024 * rs, 0, r, false);
            r.RoomDoors[1] = DoorSystem.CreateDoor(0, r.x, 2048 * rs, r.z + 32.0f + 1024 * rs, 180, r, false);
            de = DecalSystem.Create(18, r.x - (1536 * rs), 0.02f, r.z + 608 * rs + 32.0f, 90, 0, 0);
            EntityParent(de.Obj, r.obj);
            de.Size = Rnd(0.8f, 0.8f);
            de.BlendMode = 2;
            de.Fx = 1 + 8;
            ScaleSprite(de.Obj, de.Size, de.Size);
            EntityBlend(de.Obj, 2);
            ScaleEntity(r.Objects[10], rs * 1.5f, rs * 2.0f, rs * 1.5f, true);
            PositionEntity(r.Objects[11], r.x, r.y, r.z + 64.0f, true);
            for (i = 1; i <= 8; i++)
            {
                r.Objects[i - 1] = CopyEntity(hallway);
                ScaleEntity(r.Objects[i - 1], rs, rs, rs);
                angle = (i - 1) * (360.0f / 8.0f);
                EntityType(r.Objects[i - 1], 1);
                EntityPickMode(r.Objects[i - 1], 2);
                RotateEntity(r.Objects[i - 1], 0, angle - 90, 0);
                PositionEntity(r.Objects[i - 1], r.x + Cos(angle) * (512.0f * rs), 0.0f, r.z + Sin(angle) * (512.0f * rs));
                EntityParent(r.Objects[i - 1], r.obj);
                if (i < 6)
                {
                    de = DecalSystem.Create(i + 7, r.x + Cos(angle) * (512.0f * rs) * 3.0f, 0.02f, r.z + Sin(angle) * (512.0f * rs) * 3.0f, 90, angle - 90, 0);
                    de.Size = Rnd(0.5f, 0.5f);
                    de.BlendMode = 2;
                    de.Fx = 1 + 8;
                    ScaleSprite(de.Obj, de.Size, de.Size);
                    EntityBlend(de.Obj, 2);
                }
            }
            for (i = 12; i <= 16; i++)
            {
                r.Objects[i] = CreatePivot(r.Objects[11]);
                switch (i)
                {
                    case 12: PositionEntity(r.Objects[i], r.x, r.y + 200 * rs, r.z + 64.0f, true); break;
                    case 13: PositionEntity(r.Objects[i], r.x + 390 * rs, r.y + 200 * rs, r.z + 64.0f + 272 * rs, true); break;
                    case 14: PositionEntity(r.Objects[i], r.x + 838 * rs, r.y + 200 * rs, r.z + 64.0f - 551 * rs, true); break;
                    case 15: PositionEntity(r.Objects[i], r.x - 139 * rs, r.y + 200 * rs, r.z + 64.0f + 1201 * rs, true); break;
                    case 16: PositionEntity(r.Objects[i], r.x - 1238 * rs, r.y - 1664 * rs, r.z + 64.0f + 381 * rs, true); break;
                }
            }
            OldManEyes = LoadTextureHandle("GFX.Npcs.oldmaneyes.jpg");
            r.Objects[17] = CreateSprite();
            ScaleSprite(r.Objects[17], 0.03f, 0.03f);
            EntityTexture(r.Objects[17], OldManEyes);
            EntityBlend(r.Objects[17], 3);
            SpriteViewMode(r.Objects[17], 2);
            r.Objects[18] = LoadTextureHandle("GFX.Npcs.pdplane.png", 1 + 2);
            r.Objects[19] = LoadTextureHandle("GFX.Npcs.pdplaneeye.png", 1 + 2);
            r.Objects[20] = CreateSprite();
            ScaleSprite(r.Objects[20], 8.0f, 8.0f);
            EntityTexture(r.Objects[20], r.Objects[18]);
            EntityBlend(r.Objects[20], 2);
            SpriteViewMode(r.Objects[20], 2);
            FreeEntity(hallway);

        }

        private static void Fill_Room3z3(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            sc = SecurityCamSystem.Create(r.x-320.0f*rs, r.y+384.0f*rs, r.z+512.25f*rs, r);
            sc.Angle = 225;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            //sc\FollowPlayer = True
            //[End Block]
        }

        private static void Fill_Room23(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            w = WaypointSystem.Create(r.x, r.y + 66.0f * rs, r.z, null, r);
            //[End Block]
            //New rooms (in SCP; CB 1.3f) - ENDSHN
        }

        private static void Fill_Room1lifts(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = ButtonSystem.Create(r.x + 96.0f*rs, r.y + 160.0f * rs, r.z + 64.0f * rs, 0,0,0);
            EntityParent(r.Objects[0],r.obj);
            r.Objects[1] = ButtonSystem.Create(r.x - 96.0f*rs, r.y + 160.0f * rs, r.z + 64.0f * rs, 0,0,0);
            EntityParent(r.Objects[1],r.obj);
            sc = SecurityCamSystem.Create(r.x+384.0f*rs, r.y+(448-64)*rs, r.z-960.0f*rs, r, true);
            sc.Angle = 45;
            sc.Turn = 45;
            sc.Room = r;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            EntityParent(sc.Obj, r.obj);
            w = WaypointSystem.Create(r.x, r.y + 66.0f * rs, r.z, null, r);
            //[End Block]
        }

        private static void Fill_Room2servers2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 264.0f * rs, 0.0f, r.z + 672.0f * rs, 270, r, false, 0, 3);
            PositionEntity(d.Buttons[0], r.x + 224.0f * rs, EntityY(d.Buttons[0],true), r.z + 510.0f * rs, true);
            PositionEntity(d.Buttons[1], r.x + 304.0f * rs, EntityY(d.Buttons[1],true), r.z + 840.0f * rs, true);
            TurnEntity(d.Buttons[1],0,0,0,true);
            d = DoorSystem.CreateDoor(r.zone, r.x -512.0f * rs, -768.0f*rs, r.z -336.0f * rs, 0, r, false, 0, 3);
            d = DoorSystem.CreateDoor(r.zone, r.x -509.0f * rs, -768.0f*rs, r.z -1037.0f * rs, 0, r, false, 0, 3);
            d.Locked = true;
            d.DisableWaypoint = true;
            it = ItemSystem.CreateItem("Night Vision Goggles", "nvgoggles", r.x + 56.0154f * rs, r.y - 648.0f * rs, r.z + 749.638f * rs);
            it.State = 200;
            RotateEntity(it.Collider, 0, r.Angle+Rand(245), 0);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room2gw(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";

            if (r.RoomName == "room2gw_b")
            {
                r.Objects[2] = CreatePivot(r.obj);
                PositionEntity(r.Objects[2], r.x - 156.825f * rs, -37.3458f * rs, r.z + 121.364f * rs, true);
                de = DecalSystem.Create(3, r.x - 156.825f * rs, -37.3458f * rs, r.z + 121.364f * rs, 90, Rnd(360), 0);
                de.Size = 0.5f;
                ScaleSprite(de.Obj, de.Size, de.Size);
                EntityParent(de.Obj, r.obj);
                r.Objects[0] = CreatePivot();
                PositionEntity(r.Objects[0], r.x + 280.0f * rs, r.y + 345.0f * rs, r.z - 340.0f * rs, true);
                EntityParent(r.Objects[0], r.obj);
            }

            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x + 336.0f * rs, 0.0f, r.z - 382.0f * rs, 0, r, false, 0);
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 580.822f * rs, EntityY(r.RoomDoors[0].Buttons[0], true), r.z - 606.679f * rs, true);
            PositionEntity(r.RoomDoors[0].Buttons[1], r.x + 580.822f * rs, EntityY(r.RoomDoors[0].Buttons[1], true), r.z - 606.679f * rs, true);
            r.RoomDoors[0].Dir = 0;
            r.RoomDoors[0].AutoClose = false;
            r.RoomDoors[0].Open = true;
            r.RoomDoors[0].Locked = true;
            r.RoomDoors[0].MTFClose = false;

            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 336.0f * rs, 0.0f, r.z + 462.0f * rs, 180, r, false, 0);
            PositionEntity(r.RoomDoors[1].Buttons[0], r.x + 580.822f * rs, EntityY(r.RoomDoors[1].Buttons[0], true), r.z - 606.679f * rs, true);
            PositionEntity(r.RoomDoors[1].Buttons[1], r.x + 580.822f * rs, EntityY(r.RoomDoors[1].Buttons[1], true), r.z - 606.679f * rs, true);
            r.RoomDoors[1].Dir = 0;
            r.RoomDoors[1].AutoClose = false;
            r.RoomDoors[1].Open = true;
            r.RoomDoors[1].Locked = true;
            r.RoomDoors[1].MTFClose = false;

            foreach (var other in MapSystem.All)
            {
                if (other != r && (other.RoomName == "room2gw" || other.RoomName == "room2gw_b"))
                {
                    r.Objects[3] = CopyEntity(other.Objects[3], r.obj);
                    break;
                }
            }
            if (r.Objects[3] == -1)
                r.Objects[3] = LoadMesh("GFX.Map.Room2gw_pipes.b3d", r.obj);
            EntityPickMode(r.Objects[3], 2);

            if (r.RoomName == "room2gw")
            {
                r.Objects[0] = CreatePivot();
                PositionEntity(r.Objects[0], r.x + 344.0f * rs, 128.0f * rs, r.z);
                EntityParent(r.Objects[0], r.obj);

                bool bd_temp = false;
                if (MapGlobals.Room2GwBrokenDoor)
                {
                    if (MapGlobals.Room2GwX == r.x && MapGlobals.Room2GwZ == r.z)
                        bd_temp = true;
                }

                if ((!MapGlobals.Room2GwBrokenDoor && Rand(1, 2) == 1) || bd_temp)
                {
                    r.Objects[1] = CopyEntity(MapAssets.DoorObj);
                    ScaleEntity(r.Objects[1],
                        (204.0f * rs) / MapAssets.MeshWidth(r.Objects[1]),
                        312.0f * rs / MapAssets.MeshHeight(r.Objects[1]),
                        16.0f * rs / MapAssets.MeshDepth(r.Objects[1]));
                    EntityType(r.Objects[1], 1);
                    PositionEntity(r.Objects[1], r.x + 336.0f * rs, 0.0f, r.z + 462.0f * rs);
                    RotateEntity(r.Objects[1], 0, 360, 0);
                    EntityParent(r.Objects[1], r.obj);
                    MoveEntity(r.Objects[1], 120.0f, 0, 5.0f);
                    MapGlobals.Room2GwBrokenDoor = true;
                    MapGlobals.Room2GwX = r.x;
                    MapGlobals.Room2GwZ = r.z;
                    FreeEntity(r.RoomDoors[1].Obj2);
                    r.RoomDoors[1].Obj2 = -1;
                }
            }
        }

        private static void Fill_Room3gw(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x - 728.0f * rs, 0.0f, r.z - 458.0f * rs, 0, r, false, 0, 3);
            d.AutoClose = false; d.Open = false; d.Locked = false;
            d = DoorSystem.CreateDoor(r.zone, r.x - 223.0f * rs, 0.0f, r.z - 736.0f * rs, -90, r, false, 0, 3);
            d.AutoClose = false; d.Open = false; d.Locked = false;
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x - 459.0f * rs, 0.0f, r.z + 339.0f * rs, 90, r, false, 0);
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 580.822f * rs, EntityY(r.RoomDoors[0].Buttons[0],true), r.z - 606.679f * rs, true);
            PositionEntity(r.RoomDoors[0].Buttons[1], r.x + 580.822f * rs, EntityY(r.RoomDoors[0].Buttons[1],true), r.z - 606.679f * rs, true);
            r.RoomDoors[0].Dir = 0; r.RoomDoors[0].AutoClose = false; r.RoomDoors[0].Open = true; r.RoomDoors[0].Locked = true;
            r.RoomDoors[0].MTFClose = false;
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 385.0f * rs, 0.0f, r.z + 339.0f * rs, 270, r, false, 0);
            PositionEntity(r.RoomDoors[1].Buttons[0], r.x + 580.822f * rs, EntityY(r.RoomDoors[1].Buttons[0],true), r.z - 606.679f * rs, true);
            PositionEntity(r.RoomDoors[1].Buttons[1], r.x + 580.822f * rs, EntityY(r.RoomDoors[1].Buttons[1],true), r.z - 606.679f * rs, true);
            r.RoomDoors[1].Dir = 0; r.RoomDoors[1].AutoClose = false; r.RoomDoors[1].Open = true; r.RoomDoors[1].Locked = true;
            r.RoomDoors[1].MTFClose = false;
            FreeEntity(r.RoomDoors[1].Obj2); r.RoomDoors[1].Obj2 = -1;
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0],r.x-48.0f*rs,128.0f*rs,r.z+320.0f*rs);
            EntityParent(r.Objects[0],r.obj);
            foreach (var otherRoom in MapSystem.All)
            {
                if (otherRoom!=r)
                {
                    if (otherRoom.RoomName == "room3gw")
                    {
                        r.Objects[3] = CopyEntity(otherRoom.Objects[3], r.obj); //don't load the mesh again;
                        break;
                    }
                }
            }
            if (r.Objects[3]==-1) { r.Objects[3] = LoadMesh("GFX.Map.Room3gw_pipes.b3d",r.obj); }
            EntityPickMode(r.Objects[3],2);
            //[End Block]
        }

        private static void Fill_Room1162(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 248.0f*rs, 0.0f, r.z - 736.0f*rs, 90, r, false, 0, 2);
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0],r.x+1012.0f*rs,r.y+128.0f*rs,r.z-640.0f*rs);
            EntityParent(r.Objects[0],r.obj);
            EntityPickMode(r.Objects[0],1);
            it = ItemSystem.CreateItem("Document SCP-1162", "paper", r.x + 863.227f * rs, r.y + 152.0f * rs, r.z - 953.231f * rs);
            EntityParent(it.Collider, r.obj);
            sc = SecurityCamSystem.Create(r.x-192.0f*rs, r.y+704.0f*rs, r.z+192.0f*rs, r);
            sc.Angle = 225;
            sc.Turn = 45;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            //[End Block]
        }

        private static void Fill_Room2scps2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x + 288.0f*rs, r.y, r.z + 576.0f*rs, 90, r, false, 0, 3);
            r.RoomDoors[0].Open = false; r.RoomDoors[0].Locked = true;
            d = DoorSystem.CreateDoor(r.zone, r.x + 777.0f*rs, r.y, r.z + 671.0f*rs, 90, r, false, 0, 4);
            d = DoorSystem.CreateDoor(r.zone, r.x + 556.0f*rs, r.y, r.z + 296.0f*rs, 0, r, false, 0, 3);
            r.Objects[0] = CreatePivot();
            PositionEntity(r.Objects[0],r.x + 576.0f*rs,r.y+160.0f*rs,r.z+632.0f*rs);
            EntityParent(r.Objects[0],r.obj);
            it = ItemSystem.CreateItem("SCP-1499", "scp1499", r.x + 600.0f * rs, r.y + 176.0f * rs, r.z - 228.0f * rs);
            RotateEntity(it.Collider, 0, r.Angle, 0);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Document SCP-1499", "paper", r.x + 840.0f * rs, r.y + 260.0f * rs, r.z + 224.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Document SCP-500", "paper", r.x + 1152.0f * rs, r.y + 224.0f * rs, r.z + 336.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Emily Ross' Badge", "badge", r.x + 364.0f * rs, r.y + 5.0f * rs, r.z + 716.0f * rs);
            EntityParent(it.Collider, r.obj);
            sc = SecurityCamSystem.Create(r.x + 850.0f * rs, r.y + 350.0f * rs, r.z + 876.0f * rs, r);
            sc.Angle = 220; sc.Turn = 30;
            TurnEntity(sc.CameraObj, 30, 0, 0);
            EntityParent(sc.Obj, r.obj);
            sc = SecurityCamSystem.Create(r.x + 600.0f * rs, r.y + 514.0f * rs, r.z + 150.0f * rs, r);
            sc.Angle = 180; sc.Turn = 30;
            TurnEntity(sc.CameraObj, 30, 0, 0);
            EntityParent(sc.Obj, r.obj);
            //[End Block]
        }

        private static void Fill_Room3offices(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x + 736.0f * rs, 0.0f, r.z + 240.0f * rs, 0, r, false, 0, 3);
            PositionEntity(d.Buttons[0], r.x + 892.0f * rs, EntityY(d.Buttons[0],true), r.z + 224.0f * rs, true);
            PositionEntity(d.Buttons[1], r.x + 892.0f * rs, EntityY(d.Buttons[1],true), r.z + 255.0f * rs, true);
            FreeEntity(d.Obj2); d.Obj2 = -1;
            r.Objects[0] = LoadMesh("GFX.Map.Room3offices_hb.b3d",r.obj);
            EntityPickMode(r.Objects[0],2);
            EntityType(r.Objects[0],1);
            EntityAlpha(r.Objects[0],0.0f);
            //[End Block]
        }

        private static void Fill_Room2offices4(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(0, r.x - 240.0f * rs, 0.0f, r.z, 90, r, false);
            PositionEntity(d.Buttons[0], r.x - 230.0f * rs, EntityY(d.Buttons[0],true), EntityZ(d.Buttons[0],true), true);
            PositionEntity(d.Buttons[1], r.x - 250.0f * rs, EntityY(d.Buttons[1],true), EntityZ(d.Buttons[1],true), true);
            d.Open = false; d.AutoClose = false;
            it = ItemSystem.CreateItem("Sticky Note", "paper", r.x - 991.0f*rs, r.y - 242.0f*rs, r.z + 904.0f*rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Room2sl(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            float scale = rs * 4.5f * 0.4f;

            r.Textures[0] = LoadTextureHandle("GFX.SL_monitors_checkpoint.jpg", 1);
            r.Textures[1] = LoadTextureHandle("GFX.Sl_monitors.jpg", 1);

            for (i = 0; i <= 14; i++)
            {
                if (i != 7)
                {
                    r.Objects[i] = CopyEntity(MapAssets.Monitor);
                    ScaleEntity(r.Objects[i], scale, scale, scale);
                    if (i != 4 && i != 13)
                    {
                        screen = CreateSprite();
                        SpriteViewMode(screen, 2);
                        ScaleSprite(screen, MapAssets.MeshWidth(MapAssets.Monitor) * scale * 0.95f * 0.5f, MapAssets.MeshHeight(MapAssets.Monitor) * scale * 0.95f * 0.5f);
                        switch (i)
                        {
                            case 0: EntityTexture(screen, r.Textures[1]); break;
                            case 2: EntityTexture(screen, r.Textures[1]); break;
                            case 3: EntityTexture(screen, r.Textures[1]); break;
                            case 8: EntityTexture(screen, r.Textures[1]); break;
                            case 9: EntityTexture(screen, r.Textures[1]); break;
                            case 10: EntityTexture(screen, r.Textures[1]); break;
                            case 11: EntityTexture(screen, r.Textures[1]); break;
                            default: EntityTexture(screen, r.Textures[0]); break;
                        }
                        EntityParent(screen, r.Objects[i]);
                    }
                    else if (i == 4)
                    {
                        r.Objects[20] = CreateSprite();
                        SpriteViewMode(r.Objects[20], 2);
                        ScaleSprite(r.Objects[20], MapAssets.MeshWidth(MapAssets.Monitor) * scale * 0.95f * 0.5f, MapAssets.MeshHeight(MapAssets.Monitor) * scale * 0.95f * 0.5f);
                        EntityTexture(r.Objects[20], r.Textures[0]);
                        EntityParent(r.Objects[20], r.Objects[i]);
                    }
                    else
                    {
                        r.Objects[21] = CreateSprite();
                        SpriteViewMode(r.Objects[21], 2);
                        ScaleSprite(r.Objects[21], MapAssets.MeshWidth(MapAssets.Monitor) * scale * 0.95f * 0.5f, MapAssets.MeshHeight(MapAssets.Monitor) * scale * 0.95f * 0.5f);
                        EntityTexture(r.Objects[21], r.Textures[1]);
                        EntityParent(r.Objects[21], r.Objects[i]);
                    }
                }
            }

            for (i = 0; i <= 2; i++)
            {
                PositionEntity(r.Objects[i], r.x - 207.94f * rs, r.y + (648.0f + (112 * i)) * rs, r.z - 60.0686f * rs);
                RotateEntity(r.Objects[i], 0, 105 + r.Angle, 0);
                EntityParent(r.Objects[i], r.obj);
            }
            for (i = 3; i <= 5; i++)
            {
                PositionEntity(r.Objects[i], r.x - 231.489f * rs, r.y + (648.0f + (112 * (i - 3))) * rs, r.z + 95.7443f * rs);
                RotateEntity(r.Objects[i], 0, 90 + r.Angle, 0);
                EntityParent(r.Objects[i], r.obj);
            }
            for (i = 6; i <= 8; i += 2)
            {
                PositionEntity(r.Objects[i], r.x - 231.489f * rs, r.y + (648.0f + (112 * (i - 6))) * rs, r.z + 255.744f * rs);
                RotateEntity(r.Objects[i], 0, 90 + r.Angle, 0);
                EntityParent(r.Objects[i], r.obj);
            }
            for (i = 9; i <= 11; i++)
            {
                PositionEntity(r.Objects[i], r.x - 231.489f * rs, r.y + (648.0f + (112 * (i - 9))) * rs, r.z + 415.744f * rs);
                RotateEntity(r.Objects[i], 0, 90 + r.Angle, 0);
                EntityParent(r.Objects[i], r.obj);
            }
            for (i = 12; i <= 14; i++)
            {
                PositionEntity(r.Objects[i], r.x - 208.138f * rs, r.y + (648.0f + (112 * (i - 12))) * rs, r.z + 571.583f * rs);
                RotateEntity(r.Objects[i], 0, 75 + r.Angle, 0);
                EntityParent(r.Objects[i], r.obj);
            }

            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x + 480.0f * rs, r.y, r.z - 640.0f * rs, 90, r, false, 0, 3);
            r.RoomDoors[0].AutoClose = false;
            PositionEntity(r.RoomDoors[0].Buttons[0], r.x + 576.0f * rs, EntityY(r.RoomDoors[0].Buttons[0], true), r.z - 480 * rs, true);
            RotateEntity(r.RoomDoors[0].Buttons[0], 0, 270, 0);
            r.RoomDoors[1] = DoorSystem.CreateDoor(r.zone, r.x + 544.0f * rs, r.y + 480.0f * rs, r.z + 256.0f * rs, 270, r, false, 0, 3);
            r.RoomDoors[1].AutoClose = false;
            FreeEntity(r.RoomDoors[1].Obj2);
            r.RoomDoors[1].Obj2 = -1;
            d = DoorSystem.CreateDoor(r.zone, r.x + 1504.0f * rs, r.y + 480.0f * rs, r.z + 960.0f * rs, 0, r);
            d.AutoClose = false;
            d.Locked = true;

            r.Objects[7] = CreatePivot();
            PositionEntity(r.Objects[7], r.x, r.y + 100.0f * rs, r.z - 800.0f * rs, true);
            EntityParent(r.Objects[7], r.obj);
            r.Objects[15] = CreatePivot();
            PositionEntity(r.Objects[15], r.x + 700.0f * rs, r.y + 700.0f * rs, r.z + 256.0f * rs, true);
            EntityParent(r.Objects[15], r.obj);
            r.Objects[16] = CreatePivot();
            PositionEntity(r.Objects[16], r.x - 60.0f * rs, r.y + 700.0f * rs, r.z + 200.0f * rs, true);
            EntityParent(r.Objects[16], r.obj);
            r.Objects[17] = CreatePivot();
            PositionEntity(r.Objects[17], r.x - 48.0f * rs, r.y + 540.0f * rs, r.z + 656.0f * rs, true);
            EntityParent(r.Objects[17], r.obj);

            r.Objects[18] = CopyEntity(MapAssets.LeverBaseObj);
            r.Objects[19] = CopyEntity(MapAssets.LeverObj);
            r.Levers[0] = r.Objects[19];
            for (i = 0; i <= 1; i++)
            {
                ScaleEntity(r.Objects[18 + i], 0.04f, 0.04f, 0.04f);
                PositionEntity(r.Objects[18 + i], r.x - 49 * rs, r.y + 689 * rs, r.z + 912 * rs, true);
                EntityParent(r.Objects[18 + i], r.obj);
            }
            RotateEntity(r.Objects[18], 0, 0, 0);
            RotateEntity(r.Objects[19], 10, -180, 0);
            EntityPickMode(r.Objects[19], 1, false);
            EntityRadius(r.Objects[19], 0.1f);

            sc = SecurityCamSystem.Create(r.x - 159.0f * rs, r.y + 384.0f * rs, r.z - 929.0f * rs, r, true);
            sc.Angle = 315;
            sc.Room = r;
            TurnEntity(sc.CameraObj, 20, 0, 0);
            EntityParent(sc.Obj, r.obj);
            PositionEntity(sc.ScrObj, r.x - 231.489f * rs, r.y + 760.0f * rs, r.z + 255.744f * rs);
            TurnEntity(sc.ScrObj, 0, 90, 0);
            EntityParent(sc.ScrObj, r.obj);
        }
        private static void Fill_Room24(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[6] = CreatePivot();
            PositionEntity(r.Objects[6], r.x + 640.0f * rs, 8.0f * rs, r.z - 896.0f * rs);
            EntityParent(r.Objects[6], r.obj);
            //[End Block]
        }

        private static void Fill_Room3z2(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";

            foreach (var other in MapSystem.All)
            {
                if (other.RoomName == r.RoomName && other != r)
                {
                    r.Objects[0] = CopyEntity(other.Objects[0], r.obj);
                    break;
                }
            }
            if (r.Objects[0] == -1)
                r.Objects[0] = LoadMesh("GFX.Map.Room3z2_hb.b3d", r.obj);
            EntityPickMode(r.Objects[0], 2);
            EntityType(r.Objects[0], 1);
            EntityAlpha(r.Objects[0], 0.0f);
        }
        private static void Fill_Lockroom3(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            d = DoorSystem.CreateDoor(r.zone, r.x - 736.0f * rs, 0, r.z - 104.0f * rs, 0, r, true);
            d.Timer = 70 * 5; d.AutoClose = false; d.Open = false; d.Locked = true;
            EntityParent(d.Buttons[0], -1);
            PositionEntity(d.Buttons[0], r.x - 288.0f * rs, 0.7f, r.z - 640.0f * rs);
            EntityParent(d.Buttons[0], r.obj);
            FreeEntity(d.Buttons[1]); d.Buttons[1] = -1;
            d2 = DoorSystem.CreateDoor(r.zone, r.x + 104.0f * rs, 0, r.z + 736.0f * rs, 270, r, true);
            d2.Timer = 70 * 5; d2.AutoClose = false; d2.Open = false; d2.Locked = true;
            EntityParent(d2.Buttons[0], -1);
            PositionEntity(d2.Buttons[0], r.x + 640.0f * rs, 0.7f, r.z + 288.0f * rs);
            RotateEntity(d2.Buttons[0], 0, 90, 0);
            EntityParent(d2.Buttons[0], r.obj);
            FreeEntity(d2.Buttons[1]); d2.Buttons[1] = -1;
            d.LinkedDoor = d2;
            d2.LinkedDoor = d;
            float scale = rs * 4.5f * 0.4f;
            r.Objects[0] = CopyEntity(MapAssets.Monitor);
            ScaleEntity(r.Objects[0],scale,scale,scale);
            PositionEntity(r.Objects[0],r.x+668*rs,1.1f,r.z-96.0f*rs,true);
            RotateEntity(r.Objects[0],0,90,0);
            EntityParent(r.Objects[0],r.obj);
            r.Objects[1] = CopyEntity(MapAssets.Monitor);
            ScaleEntity(r.Objects[1],scale,scale,scale);
            PositionEntity(r.Objects[1],r.x+96.0f*rs,1.1f,r.z-668.0f*rs,true);
            EntityParent(r.Objects[1],r.obj);
            //[End Block]
        }

        private static void Fill_Medibay(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Objects[0] = LoadMesh("GFX.Map.Medibay_props.b3d",r.obj);
            EntityType(r.Objects[0],1);
            EntityPickMode(r.Objects[0],2);
            r.Objects[1] = CreatePivot(r.obj);
            PositionEntity(r.Objects[1], r.x - 762.0f * rs, r.y + 0.0f * rs, r.z - 346.0f * rs, true);
            r.Objects[2] = CreatePivot(r.obj);
            PositionEntity(r.Objects[2], (EntityX(r.Objects[1],true)+(126.0f * rs)), EntityY(r.Objects[1],true), EntityZ(r.Objects[1],true), true);
            it = ItemSystem.CreateItem("First Aid Kit", "firstaid", r.x - 506.0f * rs, r.y + 192.0f * rs, r.z - 322.0f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Syringe", "syringe", r.x - 333.0f * rs, r.y + 100.0f * rs, r.z + 97.3f * rs);
            EntityParent(it.Collider, r.obj);
            it = ItemSystem.CreateItem("Syringe", "syringe", r.x - 340.0f * rs, r.y + 100.0f * rs, r.z + 52.3f * rs);
            EntityParent(it.Collider, r.obj);
            r.RoomDoors[0] = DoorSystem.CreateDoor(r.zone, r.x - 264.0f * rs, r.y - 0.0f * rs, r.z + 640.0f * rs, 90, r, false, 0, 3);
            r.Objects[3] = CreatePivot(r.obj);
            //PositionEntity r\Objects[3],r\x-926.891f*RoomScale,r\y,r\z-318.399f*RoomScale,True
            PositionEntity(r.Objects[3],r.x-820.0f*rs,r.y,r.z-318.399f*rs,true);
            //[End Block]
        }

        private static void Fill_Room2cpit(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            em = ParticleSystem.CreateEmitter(r.x + 512.0f * rs, -76 * rs, r.z - 688 * rs, 0);
            TurnEntity(em.Obj, -90, 0, 0);
            EntityParent(em.Obj, r.obj);
            em.RandAngle = 55;
            em.Speed = 0.0005f;
            em.AChange = -0.015f;
            em.SizeChange = 0.007f;
            d = DoorSystem.CreateDoor(r.zone, r.x-256.0f*rs, 0.0f, r.z-752.0f*rs, 90, r, false, 2, 3);
            d.Locked = true; d.Open = false; d.AutoClose = false; d.MTFClose = false; d.DisableWaypoint = true;
            PositionEntity(d.Buttons[0],r.x-240.0f*rs,EntityY(d.Buttons[0],true),EntityZ(d.Buttons[0],true),true);
            it = ItemSystem.CreateItem("Dr L's Note", "paper", r.x - 160.0f * rs, 32.0f * rs, r.z - 353.0f * rs);
            EntityParent(it.Collider, r.obj);
            //[End Block]
        }

        private static void Fill_Dimension1499(RoomInstance r)
        {
            float rs = GameState.RoomScale;
            Door d = null, d2 = null;
            Item it = null;
            Emitter em = null;
            Decal de = null;
            SecurityCam sc = null, sc2 = null;
            WaypointNode w = null, w2 = null;
            RoomInstance r2 = null;
            int hallway = -1, terrain = -1, entity = -1, OldManEyes = -1;
            float x, y, z, dx, dy, dz, angle;
            int temp, temp2, temp3, chance, i, n, tex = -1, screen = -1;
            Item clipboard = null;
            string tempstr = "", tempstr2 = "";
            //[Block]
            r.Levers[1] = LoadMesh("GFX.Map.Dimension1499.1499object0_cull.b3d",r.obj);
            EntityType(r.Levers[1],1);
            EntityAlpha(r.Levers[1],0);
            r.Levers[0] = CreatePivot();
            PositionEntity(r.Levers[0],r.x+205.0f*rs,r.y+200.0f*rs,r.z+2287.0f*rs);
            EntityParent(r.Levers[0],r.obj);
            //[End Block]
        }
    }
}
