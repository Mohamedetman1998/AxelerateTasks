using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4.Application
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



            return Result.Succeeded;
        }
    }
}
