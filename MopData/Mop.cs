using System;
using System.Net;
using System.Text;
using RestSharp;
using RestSharp.Serializers;

namespace MopData
{
    public class Mop
    {
        public const string HomeCity = "592";

        public Mop(string mobile)
        {
            Mobile = mobile;
            if (Client == null)
            {
                Client = new RestClient("http://112.5.185.82:8881") {CookieContainer = new CookieContainer()};
//                Client.Proxy = new WebProxy("127.0.0.1:8888");
                var vc = "337911944302901013043303";
                Request =
                    new RestRequest(
                        $"MBossWeb/mbop/index_hidden.jsp?vc={vc}&remurl=http%3A%2F%2F112.5.185.82%3A8881%2FMBossWeb&localpre=file%3A%2Fdata%2Fdata%2Fcom.newland.mbop%2Ffiles%2Fwebcache%2F&pid=200679&pmid=15960809888&ptid=770489400020&hc=592&sm=1&sw=480&sh=800&enc=utf-8&fastmode=0&fc=89100123&url=%5Bhttp%5Dpage-fj%2Fcrm%2F4Assamble%2F4_assamble.jsp&theme=&randCode=1A2B3C4D5E6F7G8H&isVirtualXML=false&menuName=%E6%99%BA%E8%83%BD%E8%90%A5%E9%94%80&portal_id=101704&op_home_country=206&opType=0&exturl=mode%3D0%26cancel_query%3Dfalse",
                        Method.POST);
                Client.Execute(Request);
            }
        }

        public static RestClient Client { get; set; }
        public RestRequest Request { get; set; }
        public RestRequestAsyncHandle Response { get; set; }
        public string UserId { get; } = string.Empty;
        public string Mobile { get; }

        public RestRequestAsyncHandle GetBaseInfo(Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            Request = new RestRequest("MBossWeb/bmaccept/4assambleQueryMgr.do?method=queryUserInfo");
            Request.AddParameter("msisdn", Mobile);
            Response = Client.ExecuteAsync(Request, callback);
            return Response;
        }

        public RestRequestAsyncHandle GetBusinessInfo(Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            Request = new RestRequest("MBossWeb/bmaccept/4assambleQueryMgr.do?method=queryBusinessInfo", Method.POST);
            Request.AddParameter("msisdn", Mobile);
            Request.AddParameter("home_city", HomeCity);
            Request.AddParameter("user_id", UserId);
            Response = Client.ExecuteAsync(Request, callback);
            return Response;
        }

        public RestRequestAsyncHandle GetConsumeInfo(Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            Request = new RestRequest("MBossWeb/bmaccept/4assambleQueryMgr.do?method=queryConsumeInfo", Method.POST);
            Request.AddParameter("msisdn", Mobile);
            Request.AddParameter("home_city", HomeCity);
            Request.AddParameter("user_id", UserId);
            Response = Client.ExecuteAsync(Request, callback);
            return Response;
        }

        public RestRequestAsyncHandle GetRecomendInfo(Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            Request = new RestRequest("MBossWeb/bmaccept/4assambleQueryMgr.do?method=QueryUserBaseInfo");
            Request.AddParameter("msisdn", Mobile);
            Request.AddParameter("home_city", HomeCity);
            Response = Client.ExecuteAsync(Request, callback);
            return Response;
        }
    }
}