using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3.Utilities
{
    internal class FloorUtils
    {
        public static IList<CurveLoop> RoomsCurveLoops(List<Room> rooms)
        {
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

                    if (curveLoop.Any())
                    {
                        curveLoops.Add(curveLoop);
                    }
                }
            }

            return curveLoops;
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
    }
}
