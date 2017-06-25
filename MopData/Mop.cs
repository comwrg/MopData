using System;
using RestSharp;
using System.Net;
using DotNet4.Utilities;

namespace MopData
{
    public class Mop
    {
        public const string HomeCity = "592";

        public Mop(string mobile)
        {
            Mobile = mobile;
            if (Http == null)
            {
                Http = new Http
                {
                    Proxy = null,
                    Url =
                        new Uri(
                            "http://112.5.185.82:8881/MBossWeb/mbop/index_hidden.jsp?vc=805691021746849026205712&remurl=http%3A%2F%2F112.5.185.82%3A8881%2FMBossWeb&localpre=file%3A%2Fdata%2Fdata%2Fcom.newland.mbop%2Ffiles%2Fwebcache%2F&pid=128110&pmid=18859235646&ptid=770489400020&hc=592&sm=1&sw=480&sh=800&enc=utf-8&fastmode=0&fc=89100123&url=%5Bhttp%5Dpage-fj%2Fcrm%2F4Assamble%2F4_assamble.jsp&theme=&randCode=1A2B3C4D5E6F7G8H&isVirtualXML=false&menuName=%E6%99%BA%E8%83%BD%E8%90%A5%E9%94%80&portal_id=101704&op_home_country=206&opType=0&exturl=mode%3D0%26cancel_query%3Dfalse"),
                    CookieContainer = new CookieContainer()
                };
                Http.RequestContentType = "application/x-www-form-urlencoded";
                Http.Post();
                
            }
        }

        public Http Http { get; set; }
        public HttpWebRequest Request { get; set; }
        public IRestResponse Response { get; set; }
        public string UserId { get; } = string.Empty;
        public string Mobile { get; }

        public HttpResponse GetBaseInfo()
        {
            Http.Url = new Uri("http://112.5.185.82:8881/MBossWeb/bmaccept/4assambleQueryMgr.do?method=queryUserInfo");
            Http.RequestBody = $"msisdn={Mobile}";
//            Http.RequestContentType = "application/x-www-form-urlencoded";
            return Http.Post();
        }

        public HttpResponse GetBusinessInfo()
        {
            Http.Url = new Uri("http://112.5.185.82:8881/MBossWeb/bmaccept/4assambleQueryMgr.do?method=queryBusinessInfo");
            Http.RequestBody = $"msisdn={Mobile}&home_city={HomeCity}&user_id={UserId}";
            return Http.Post();
        }

        public HttpResponse GetConsumeInfo()
        {
            Http.Url = new Uri("http://112.5.185.82:8881/MBossWeb/bmaccept/4assambleQueryMgr.do?method=queryConsumeInfo");
            Http.RequestBody = $"msisdn={Mobile}&home_city={HomeCity}&user_id={UserId}";
            return Http.Post();
        }

        public HttpResponse GetRecomendInfo()
        {
            Http.Url = new Uri("http://112.5.185.82:8881/MBossWeb/bmaccept/4assambleQueryMgr.do?method=QueryUserBaseInfo");
            Http.RequestBody = $"msisdn={Mobile}&home_city={HomeCity}";
            return Http.Post();
        }
    }
}