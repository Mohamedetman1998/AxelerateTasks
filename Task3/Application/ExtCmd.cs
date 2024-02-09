using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.Utilities;
using Task3.Utilities;

namespace Task3.Application
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class ExtCmd : IExternalCommand
    {
        public static UIDocument CommandUIdoc { get; set; }
        public static Document CommandDoc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application CommandApp { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var CommandUIdoc = commandData.Application.ActiveUIDocument;
            var CommandDoc = CommandUIdoc.Document;

            try
            {
                var floorType = new FilteredElementCollector(CommandDoc)
                                 .OfClass(typeof(FloorType))
                                 .WhereElementIsElementType()
                                 .FirstOrDefault();

                var roomsInProject = new FilteredElementCollector(CommandDoc)
                 .OfCategory(BuiltInCategory.OST_Rooms)
                 .WhereElementIsNotElementType()
                 .Cast<Room>()
                 .ToList();

                Level level = CommandDoc.ActiveView.GenLevel;
                IList<CurveLoop> curveLoops = FloorUtils.RoomsCurveLoops(roomsInProject);

                if (curveLoops != null)
                    using (Transaction transaction = new Transaction(CommandDoc, "Create Floor"))
                    {
                        transaction.Start();


                        if (level == null)
                        {
                            message = "No level found in the active view.";
                            transaction.RollBack();
                            return Result.Failed;
                        }

                        foreach (var curveLoop in curveLoops)
                        {
                            List<CurveLoop> singleCurveLoopList = new List<CurveLoop> { curveLoop };
                            Floor.Create(CommandDoc, singleCurveLoopList, floorType.Id, level.Id);
                        }
                        transaction.Commit();
                    }

                else
                {
                    TaskDialog.Show("Error", "Curve Loop Isn't Correct");
                    return Result.Failed;
                }

                return Result.Succeeded;
            }

            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
