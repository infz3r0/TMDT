using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace TMDT.Common
{
    public class RSSActionResult : ActionResult
    {
        public SyndicationFeed Feed { get; set; }
        public RSSActionResult() { }
        public RSSActionResult(SyndicationFeed feed)
        {
            this.Feed = feed;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";
            Rss20FeedFormatter formatter = new Rss20FeedFormatter(this.Feed);
            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }
}