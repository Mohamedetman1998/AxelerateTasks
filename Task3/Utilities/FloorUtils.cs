using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace Task3.Utilities
{
    internal class FloorUtils
    {
        public static IList<CurveLoop> RoomsCurveLoops(Document doc, List<Room> rooms)
        {
            IList<CurveLoop> processedCurves = new List<CurveLoop>();
            List<CurveLoop> curveLoops = new List<CurveLoop>();

            foreach (var room in rooms)
            {
                GeometryElement roomShell = room.ClosedShell;

                foreach (Solid solid in roomShell)
                {
                    EdgeArray edges = solid.Edges;

                    CurveLoop curveLoop = new CurveLoop();

                    foreach (Edge edge in edges)
                    {
                        Curve edgeCurve = edge.AsCurve();

                        if (IsEdgeOnGround(edgeCurve))
                        {
                            curveLoop.Append(edgeCurve);
                        }
                    }
                    List<Curve> doorCurves = GetDoorStepCurvesInRoom(doc, room);
                 
                    if (curveLoop.Any())
                    {
                        List<Curve> allCurvesFound = new List<Curve>();
                        allCurvesFound.AddRange(doorCurves);
                        allCurvesFound.AddRange(curveLoop);

                        List<Line> allLinesFound = new List<Line>();

                        foreach (var curve in allCurvesFound)
                        {
                            allLinesFound.Add(curve as Line);
                        }
                        processedCurves = Task1.Utilities.CurveLoopLogic.ProcessCurveLoop(allLinesFound);
                    }
                }
            }
            return processedCurves;
        }
        private static bool IsEdgeOnGround(Curve edgeCurve)
        {
            // Assuming edgeCurve is a Line here, modify as needed for other curve types
            if (edgeCurve is Line line)
            {
                XYZ startPoint = line.GetEndPoint(0);
                XYZ endPoint = line.GetEndPoint(1);

                return startPoint.Z == 0 && endPoint.Z == 0;
            }

            return false;
        }
        private static List<Curve> GetDoorStepCurvesInRoom(Document doc, Room room)
        {
            List<Curve> doorCurves = new List<Curve>();
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
            };

            foreach (IList<Autodesk.Revit.DB.BoundarySegment> boundSegList in room.GetBoundarySegments(options))
            {
                foreach (Autodesk.Revit.DB.BoundarySegment boundSeg in boundSegList)
                {
                    List<Curve> doorCurve = new List<Curve>();

                    var wallInRoom = doc.GetElement(boundSeg.ElementId) as Wall;

                    if (WallContainDoor(doc,wallInRoom,room,out doorCurve))
                    {
                        doorCurves.AddRange(doorCurve);
                    }
                }
            }
            return doorCurves;
        }

        private static bool WallContainDoor(Document doc, Wall wall, Room room, out List<Curve> doorCurves)
        {
            doorCurves = new List<Curve>();

            var wallHostObj = wall as HostObject;
            var hostedElementsOnWall = wallHostObj.FindInserts(true, true, true, true);

            if (hostedElementsOnWall != null && hostedElementsOnWall.Any())
            {
                var famInstanceCollector = new FilteredElementCollector(doc, hostedElementsOnWall)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(A => A != null && (A.ToRoom?.Name == room.Name || A.FromRoom?.Name == room.Name))
                    .ToList();

                foreach (var famInstance in famInstanceCollector)
                {
                    var geometryElement = famInstance.get_Geometry(new Options());
                    foreach (var geometryInstance in geometryElement)
                    {
                        var instanceGeometry = geometryInstance as GeometryInstance;
                        if (instanceGeometry != null)
                        {
                            foreach (var geometryObject in instanceGeometry.GetInstanceGeometry())
                            {
                                var curve = geometryObject as Curve;
                                if (curve != null)
                                {
                                    doorCurves.Add(curve);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
