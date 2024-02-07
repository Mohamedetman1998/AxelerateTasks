using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.Utilities;

namespace Task1.Application
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class EntryPoint : IExternalCommand
    {

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

                Level level = CommandDoc.ActiveView.GenLevel;

                List<Line> linesGiven = UserInput.GetLines();

                IList<CurveLoop> curveLoop= CurveLoopLogic.ProcessCurveLoop(linesGiven);

                if(curveLoop != null)
                    using (Transaction transaction = new Transaction(CommandDoc, "Create Floor"))
                    {
                        transaction.Start();


                        if (level == null)
                        {
                            message = "No level found in the active view.";
                            transaction.RollBack();
                            return Result.Failed;
                        }

                        Floor floor = Floor.Create(CommandDoc, curveLoop, floorType.Id, level.Id);

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
