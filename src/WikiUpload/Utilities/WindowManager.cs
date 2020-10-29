using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WikiUpload
{
    public class WindowManager : IWindowManager
    {
        public void ShowNewVersionWindow(CheckForUpdatesEventArgs checkUpdateEventArrgs)
        {
            var newVersionWindow = new NewVersionWindow(checkUpdateEventArrgs);
            newVersionWindow.Show();
        }
    }
}
