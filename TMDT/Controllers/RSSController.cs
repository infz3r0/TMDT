using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using TMDT.Common;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace TMDT.Controllers
{
    public class RSSController : Controller
    {
        private TMDTDBDataContext db = new TMDTDBDataContext();
        // GET: Blog
        public ActionResult Index()
        {
            List<NSX> nsx = db.NSXes.ToList();
            return View(nsx);
        }

        public ActionResult PostFeed(string nsxAlias)
        {
            string urlweb = "http://localhost:41221/";
            NSX nsx = db.NSXes.FirstOrDefault(x => x.NSXAlias.Contains(nsxAlias));

            if (nsx == null)
            {
                return HttpNotFound();
            }
            IEnumerable<SanPham> sanPhams = db.SanPhams.Where(x => x.IDNSX == nsx.IDNSX).ToList();

            var feed = new SyndicationFeed(nsx.TenNSX, "RSS Feed", new Uri(urlweb + "RSS"), Guid.NewGuid().ToString(), DateTime.Now);

            var items = new List<SyndicationItem>();
            foreach (SanPham a in sanPhams)
            {
                string postUrl = string.Format(urlweb + "r/{0}/{1}/{2}", a.NSX.Loai.LoaiAlias, a.NSX.NSXAlias, a.SanPhamAlias);
                //var item = new SyndicationItem(Helper.RemoveIllegalCharacters(a.A_Title),
                //    Helper.RemoveIllegalCharacters(a.A_Description),
                //    new Uri(postUrl),
                //    a.A_ID.ToString(),
                //    a.A_DatePublished.Value);
                SyndicationItem syndicationItem = new SyndicationItem();
                syndicationItem.Title = new TextSyndicationContent(Helper.RemoveIllegalCharacters(a.TenSanPham));
                syndicationItem.Content = new TextSyndicationContent(Helper.RemoveIllegalCharacters(a.MoTaSanPham));
                syndicationItem.AddPermalink(new Uri(postUrl));
                syndicationItem.Id = a.IDSanPham.ToString();
                syndicationItem.PublishDate = a.NgayCapNhat.Value;

                items.Add(syndicationItem);
            }
            feed.Items = items;
            return new RSSActionResult { Feed = feed };


        }
        





    }
}