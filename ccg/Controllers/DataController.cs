using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using ccg.Models;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using HtmlAgilityPack;


namespace ccg.Controllers
{
    public class DataController : Controller
    {

        HtmlWeb web = new HtmlWeb();

        [OutputCache(Duration = 86400, Location = OutputCacheLocation.Server, VaryByParam = "none")]
        public JsonResult GetLocations()
        {
            try
            {
                string myUrl = "http://home.corpcomm.com/locations/";
                List<Locations> LocationList = new List<Locations>();

                HtmlAgilityPack.HtmlDocument doc = web.Load(myUrl);

                var locationsNode = doc.DocumentNode.SelectSingleNode("//div[@id='locationsWrapper']").ChildNodes.ToList();
                
                for (var i = 0; i < locationsNode.Count; i++)
                {
                    
                    Locations l = new Locations();

                    l.Name = locationsNode[i].SelectNodes("//div[@itemprop='name']")[i].InnerText;
                    l.StreetAddress = locationsNode[i].SelectNodes("//div[@itemprop='streetAddress']")[i].InnerText;
                    l.City = locationsNode[i].SelectNodes("//span[@itemprop='addressLocality']")[i].InnerText;
                    l.State = locationsNode[i].SelectNodes("//span[@itemprop='addressRegion']")[i].InnerText;
                    l.Zip = locationsNode[i].SelectNodes("//span[@itemprop='postalCode']")[i].InnerText;
                    string img = locationsNode[i].SelectNodes("//div[@class='image']")[i].Attributes["style"].Value;
                    
                    // strip out all css but url from style attribute of div
                    l.Image = img.Replace("background-image: url('", "").Replace("')", "");

                    //geocode the location to get lat and long

                    string loc = $"{l.StreetAddress} {l.City}, {l.State} {l.Zip}";
                    var Geocoded = GeocodeLocation(loc);
                    if (Geocoded != null)
                    {
                        List<decimal> c = new List<decimal>();
                        c.Add(Geocoded.Item1);
                        c.Add(Geocoded.Item2);
                        l.Coordinates = c;
                    }

                    LocationList.Add(l);
                }

                return Json(new
                {
                    status = "ok",
                    locations = LocationList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [OutputCache(Duration = 86400, Location = OutputCacheLocation.Server, VaryByParam = "none")]
        public JsonResult GetNews()
        {
            try
            {
                string myUrl = "http://home.corpcomm.com/news-events-blog/";
                List<News> NewsList = new List<News>();
                HtmlAgilityPack.HtmlDocument doc = web.Load(myUrl);

                // since no dates are provided in markup and we obviously don't have database access
                // loop through archived sections to determine which articles belong to which month/year
                // this approach is expensive as it requires multiple http requests but seems like a valid workaround given data limitations

                var datesNode = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[4]/div[1]/div[2]/ul[1]");

                var dates = datesNode.ChildNodes.Where(x => x.Name == "li").ToList();
                for (var i = 0; i < dates.Count; i++)
                {
                    var NewsDate = dates[i].ChildNodes[0].InnerText;
                    var link = dates[i].ChildNodes[0].Attributes["href"].Value;
                    
                    // go to each archived page and loop through results there in order associate date

                    HtmlAgilityPack.HtmlDocument innerDoc = web.Load(link);
                    var news = innerDoc.DocumentNode.SelectNodes("//div[@class='post excerpt case-study']").ToList();
                    for (var ii = 0; ii < news.Count; ii++)
                    {
                        // fill out each item in our list with data from html

                        var data = news[ii].ChildNodes.Where(x => x.Name == "div").ToList();

                        News n = new News();
                        n.Date = NewsDate;
                        n.Image = data[0].Attributes.FirstOrDefault(x => x.Name == "style").Value.Replace("background-image: url('", "").Replace("');","");

                        var titleNode = data[1].ChildNodes.FirstOrDefault(x => x.Name == "h2");
                        n.Title = WebUtility.HtmlDecode(titleNode.InnerText);
                        n.Link = titleNode.ChildNodes[0].Attributes["href"].Value;
                        n.Description = data[1].ChildNodes.FirstOrDefault(x => x.Name == "div").InnerText.Replace("\t","").Replace("\n","");

                        NewsList.Add(n);
                        
                    }

                }

                return Json(new
                {
                    status = "ok",
                    news = NewsList
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = "error",
                    message = e.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(Duration = 86400, Location = OutputCacheLocation.Server, VaryByParam = "none")]
        public JsonResult GetCaseStudies()
        {
            try
            {
                List<CaseStudies> CaseStudyList = new List<CaseStudies>();
                string myUrl = String.Empty;

                // case studies are paginated on source website - loop through
                // we could keep looping until caseStudies returns null but to limit network traffic 2 is hardcoded

                for (var page = 1; page <= 2; page++)
                {
                    myUrl = $"http://home.corpcomm.com/case-studies/page/{page}";
                    HtmlAgilityPack.HtmlDocument doc = web.Load(myUrl);

                    var caseStudies = doc.DocumentNode.SelectSingleNode("//div[@id='caseStudies']").ChildNodes.Where(x => x.Name == "div").ToList();

                    // loop through found case studies on current page
                    foreach (var item in caseStudies)
                    {
                        CaseStudies cs = new CaseStudies();

                        cs.Image = item.ChildNodes[1].Attributes["style"].Value.Replace("background-image: url('", "").Replace("');", ""); ;
                        var title = item.ChildNodes[3].ChildNodes.FirstOrDefault(x => x.Name == "h2");
                        cs.Title = WebUtility.HtmlDecode(title.InnerText);
                        cs.Link = title.FirstChild.Attributes["href"].Value;
                        var y = item.ChildNodes[3].ChildNodes.Where(x => x.Name == "div").ToList();
                        cs.Category = y[0].ChildNodes.FirstOrDefault(x => x.Name == "a").InnerText;
                        var thisDescription = item.SelectSingleNode("//div[@class='text']").InnerHtml.Replace("\t", "").Replace("\n", "");
                        cs.Description = WebUtility.HtmlDecode(thisDescription);
                        CaseStudyList.Add(cs);
                    }
                }
                
                return Json(new
                {
                    status = "ok",
                    casestudies = CaseStudyList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        private static Tuple<decimal, decimal> GeocodeLocation(string loc)
        {
            try
            {
                string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyBPsDaLY2SWLxNqHNbZzjkMBgGVisLqFx8&address={0}&sensor=false", Uri.EscapeDataString(loc));

                WebRequest request = WebRequest.Create(requestUri);
                WebResponse response = request.GetResponse();
                XDocument xdoc = XDocument.Load(response.GetResponseStream());

                XElement result = xdoc.Element("GeocodeResponse").Element("result");
                XElement locationElement = result.Element("geometry").Element("location");
                XElement lat = locationElement.Element("lat");
                XElement lng = locationElement.Element("lng");

                return Tuple.Create(Convert.ToDecimal(lat.Value), Convert.ToDecimal(lng.Value));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}