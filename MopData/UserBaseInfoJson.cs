using System.Collections.Generic;
using RestSharp.Deserializers;

namespace MopData
{
    public class UserBaseInfoJson
    {
        public class Basicinfo
        {
            public string title { get; set; }
            public string context { get; set; }
        }

        public class UserBaseInfo
        {
            public string home_city { get; set; }
            public List<Basicinfo> basicinfo { get; set; }
            public string cnt_user_time { get; set; }
            public string user_id { get; set; }
        }

        public class RootObject
        {
            public UserBaseInfo userBaseInfo { get; set; }
            public bool success { get; set; }
        }
    }
}