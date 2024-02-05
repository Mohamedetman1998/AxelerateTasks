using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Task3.Application
{
    public class App : IExternalApplication
    {
        public static App thisApp;
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


        public Result OnStartup(UIControlledApplication application)
        {
            #region Ribbon

            string RibbonTabName = "Example";

            application.CreateRibbonTab(RibbonTabName);

            string path = Assembly.GetExecutingAssembly().Location;

            #endregion

            #region Ribbon Panel
            RibbonPanel RibbonPanel = application.CreateRibbonPanel(RibbonTabName, "Panel Name");
            #endregion

            #region Button

            PushButtonData Button = new PushButtonData("Button", "Text", path, "RevitPluginPart.Application.RevitCommands.Entry");

            BitmapImage bti = new BitmapImage(new Uri("pack://application:,,,/RevitPluginPart;component/Resources/navisworksimg.png"));

            Button.Image = bti;
            #endregion

            PushButton btn = RibbonPanel.AddItem(Button) as PushButton;
            btn.LargeImage = bti;

            return Result.Succeeded;

        }
        #region Show a Window
        public void ShowWindow() //AppendWindow
        {
            //if (Win == null)
            //{
            //    RequestHandler _Handler = new RequestHandler();
            //    AppendFilesViewModel.m_Handler = _Handler;
            //    ExternalEvent exe = ExternalEvent.Create(_Handler);
            //    AppendFilesViewModel.m_ExEvent = exe;
            //    Win = new AppendFiles();
            //    Win.Show();

            //}
        }
        #endregion

    }
}
