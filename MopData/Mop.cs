using System;
using RestSharp;
using System.Net;
using DotNet4.Utilities;

namespace MopData
{
    public class Mop
    {
        public const string HomeCity = "592";

        public Mop(string mobile, string url)
        {
            Mobile = mobile;
            if (Http == null)
            {
                Http = new Http
                {
                    Proxy = null,
                    Url =
                        new Uri(url),
                    CookieContainer = new CookieContainer()
                };
                Http.RequestContentType = "application/x-www-form-urlencoded";
                Response = Http.Post();
                if (Response.Content == string.Empty)
                    IsEffective = true;
            }
        }

        public Http Http { get; set; }
        public HttpWebRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public string UserId { get; } = string.Empty;
        public string Mobile { get; }
        public bool IsEffective { get; private set; }

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