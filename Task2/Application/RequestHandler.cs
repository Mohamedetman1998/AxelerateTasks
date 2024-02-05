using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Autodesk.Revit.UI;

namespace Task2.Application
{
    public class RequestHandler : IExternalEventHandler
    {
        private Request m_request = new Request();
        public Request Request
        {
            get { return m_request; }
        }
        public void Execute(UIApplication app)
        {

            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;
                        }


                    default:
                        {
                            break;
                        }
                }
            }
            finally
            {
                //ExternalApplication.thisApp.WakeFormUp();
            }

            return;
        }

        public string GetName()
        {
            return "None";
        }
    }
}
