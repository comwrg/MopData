using System.Collections.Generic;

namespace MopData
{
    public class BusinessInfoJson
    {
        public class Firstlevel
        {
            public List<object> secondlevel { get; set; }
            public int secondlevelsize { get; set; }
            public string firstvalue { get; set; }
        }

        public class BusinessList
        {
            public int firstlevelsize { get; set; }
            public List<Firstlevel> firstlevel { get; set; }
        }

        public class RootObject
        {
            public BusinessList businessList { get; set; }
            public bool success { get; set; }
        }
    }
}