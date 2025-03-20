using System;
using System.Text;
using System.Linq;
using System.Web.UI.WebControls;

namespace SalesTool.Web.UserControls
{
    public partial class Announcement : HomeUserControl
    {
        int _sectionId = 0;

        protected override void InnerInit(bool bFirstTime)
        {
        }

        protected override void InnerLoad(bool bFirstTime)
        {
            if (bFirstTime)
            {
                DisplayAnnouncements(_sectionId);
            }
        }

        public int SectionId
        {
            get
            {
                return _sectionId;
            }
            set
            {
                _sectionId = value;
            }
        }

        void DisplayAnnouncements(int sectionId)
        {
            //SZ [Jul 18, 2013] get the section details
            var x = Engine.DashboadActions.Get(sectionId);
            x = x ?? new DataAccess.Models.Section();
            spnHeader.InnerText = x.Title;

            var list = Engine.AnnouncementActions.GetAnnouncementsBySection(sectionId);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<ul>");
            foreach (var item in list)
                sb.AppendFormat("<li>{0}</li>", item.Body);
            sb.AppendFormat("</ul>");
            litBody.Text = sb.ToString();
        }
    };
}