using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Task2.Utilities;

namespace Task2.Application
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class ExtCmd : IExternalCommand
    {
        public static UIDocument CommandUIdoc { get; set; }
        public static Document CommandDoc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application CommandApp { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CommandApp = commandData.Application.Application;
            CommandUIdoc = commandData.Application.ActiveUIDocument;
            CommandDoc = CommandUIdoc.Document;

            var wallReference = CommandUIdoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            var wallElement = CommandDoc.GetElement(wallReference) as Wall;

            var roomsCollector = new FilteredElementCollector(CommandDoc)
                                    .OfCategory(BuiltInCategory.OST_Rooms)
                                    .WhereElementIsNotElementType()
                                    .Where(a => a.Name.Contains("Bathroom"))
                                    .Cast<Room>()
                                    .ToList();

            var wcFamilySymbol = new FilteredElementCollector(CommandDoc)
                                 .OfCategory(BuiltInCategory.OST_GenericModel)
                                 .WhereElementIsElementType()
                                 .Where(a => a.Name == "ADA")
                                 .Cast<FamilySymbol>()
                                 .FirstOrDefault();


            var roomsFound = RevitUtils.GetRoomsNextToSelectedWall(wallElement, roomsCollector);

            var selectedWallCurve = RevitUtils.GetWallCurve(roomsFound.First(), wallElement);

            var doorsLocPoints= RevitUtils.GetDoorsLocationInRoom(roomsFound.First());

            var familyLocationPoint = RevitUtils.FarestPointToDoor(doorsLocPoints, selectedWallCurve);

            var familyOrientation = RevitUtils.GetFamilyOrientation(roomsFound.First(), familyLocationPoint,wallElement);

            using (Transaction tr = new Transaction(CommandDoc, "Place Family"))
            {
                tr.Start();

                if (!wcFamilySymbol.IsActive)
                {
                    wcFamilySymbol.Activate();
                }


               var familyCreated= CommandDoc.Create.NewFamilyInstance(familyLocationPoint,wcFamilySymbol,wallElement,Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                if(!familyCreated.FacingOrientation.IsAlmostEqualTo(familyOrientation))
                {
                    familyCreated.flipFacing();
                }
                tr.Commit();

            }
            return Result.Succeeded;
        }

    }
}
