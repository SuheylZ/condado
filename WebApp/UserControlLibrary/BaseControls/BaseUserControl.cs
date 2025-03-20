using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace UserControlLibrary.BaseControls
{
    public abstract class UserControlBase : System.Web.UI.UserControl
    {
        protected override void FrameworkInitialize()
        {
            base.FrameworkInitialize();
            string content;

            var resourceName = GetType().FullName + ".ascx";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException(string.Format("Loading resource '{0}' failed", resourceName));
            }

            using (var reader = new StreamReader(stream))
                content = reader.ReadToEnd();

            var userControl = Page.ParseControl(content);
            if (userControl == null)
            { 
                throw new InvalidOperationException(string.Format("Parsing user control in resource '{0}' failed", resourceName));
            }
            Controls.Add(userControl);
               
            WireControls(userControl);
        }

        protected abstract void WireControls(System.Web.UI.Control userControl);
    }
}