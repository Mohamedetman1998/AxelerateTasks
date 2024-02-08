using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task2.Application;

namespace Task2.Utilities
{
    public class RevitUtils
    {
        public static List<Room> GetRoomsNextToSelectedWall(Wall selectedWall, IList<Room> rooms)
        {
            List<Room> roomsFound = new List<Room>();

            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
            };

            for (int i = 0; i < rooms.Count; i++)
            {
                foreach (IList<Autodesk.Revit.DB.BoundarySegment> boundSegList in rooms[i].GetBoundarySegments(options))
                {
                    foreach (Autodesk.Revit.DB.BoundarySegment boundSeg in boundSegList)
                    {
                        Element e = ExtCmd.CommandDoc.GetElement(boundSeg.ElementId);
                        Wall wall = e as Wall;
                        if (wall.Id == selectedWall.Id)
                        {
                            roomsFound.Add(rooms[i]);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            return roomsFound;
        }   
        public static XYZ GetFamilyOrientation(Room room, XYZ point, Wall wall)
        {
            XYZ roomCentroid = (room.Location as LocationPoint).Point;
            Curve wallCurve = GetWallCurve(room, wall);
            bool isVertical = IsVerticalWall(wallCurve);
            XYZ directionVector = roomCentroid - point;

            if (isVertical)
            {
                directionVector = new XYZ(Math.Sign(directionVector.X), 0, 0);
            }
            else
            {
                directionVector = new XYZ(0, Math.Sign(directionVector.Y), 0);
            }

            return directionVector;
        }
        public static bool IsVerticalWall(Curve wallCurve)
        {
            // You can customize this based on your criteria for determining if the wall is vertical
            // For simplicity, here we assume it's vertical if the difference in X coordinates is negligible
            return Math.Abs(wallCurve.GetEndPoint(0).X - wallCurve.GetEndPoint(1).X) < 0.001;
        }
        public static XYZ FarestPointToDoor(List<XYZ> doorsLocPoints, Curve wallCurve)
        {
            XYZ doorEndPoint = doorsLocPoints.First();
            IList<XYZ> wallEndPoints = wallCurve.Tessellate();

            double maxDistance = double.MinValue;
            XYZ farthestPoint = null;

            foreach (XYZ wallPoint in wallEndPoints)
            {
                double distance = wallPoint.DistanceTo(doorEndPoint);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPoint = wallPoint;
                }
            }

            return farthestPoint;

        }
        public static Curve GetWallCurve(Room room, Wall wallElement)
        {
            Curve selectedWallCurve = null;
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;

            foreach (IList<Autodesk.Revit.DB.BoundarySegment> boundSegList in room.GetBoundarySegments(options))
            {
                foreach (Autodesk.Revit.DB.BoundarySegment boundSeg in boundSegList)
                {
                    if (boundSeg.ElementId == wallElement.Id)
                    {
                        selectedWallCurve = boundSeg.GetCurve();
                        break;
                    }
                }
            }
            return selectedWallCurve;
        }
        public static List<XYZ> GetDoorsLocationInRoom(Room room)
        {
            List<FamilyInstance> doorsList = new List<FamilyInstance>();
            List<XYZ> doorsLocationPoint = new List<XYZ>();

            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;

            foreach (IList<Autodesk.Revit.DB.BoundarySegment> boundSegList in room.GetBoundarySegments(options))
            {
                foreach (Autodesk.Revit.DB.BoundarySegment boundSeg in boundSegList)
                {
                    var wallInRoom = ExtCmd.CommandDoc.GetElement(boundSeg.ElementId) as Wall;
                    var wallHostObj = wallInRoom as HostObject;
                    var hostedElementsOnWall = wallHostObj.FindInserts(true, true, true, true);
                    if (hostedElementsOnWall.Count > 0)
                    {

                        var famInstanceCollector = new FilteredElementCollector(ExtCmd.CommandDoc, hostedElementsOnWall)
                                             .OfCategory(BuiltInCategory.OST_Doors)
                                             .WhereElementIsNotElementType()
                                             .Cast<FamilyInstance>()
                                             .Where(A => A.ToRoom.Name == room.Name || A.FromRoom.Name == room.Name)
                                             .ToList();

                        doorsList.AddRange(famInstanceCollector);
                    }
                }
            }
            foreach (var famInstance in doorsList)
            {
                doorsLocationPoint.Add((famInstance.Location as LocationPoint).Point);
            }
            return doorsLocationPoint;
        }
    }
}
