using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace HelpBot
{
    public class GeoLocator
    {

        public static dynamic getCity()
        {
            //2da7d59b916ec038bdb243d2adf389f4958d5f8e9fe8cf6fb838d72cef829bbf

            string s = new WebClient().DownloadString("http://api.ipinfodb.com/v3/ip-city/?key=2da7d59b916ec038bdb243d2adf389f4958d5f8e9fe8cf6fb838d72cef829bbf&format=json");
            dynamic location = JObject.Parse(s);
            return location;
        }
        public  static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

    }
}