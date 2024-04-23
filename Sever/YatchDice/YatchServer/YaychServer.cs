using ServerCore;
using System.Diagnostics;
using System.Net;

namespace YatchServer
{

    class YaychServer
    {
        static Listner listner = new Listner();
        static ServerSession clientSession = new ServerSession();

        static readonly HttpClient httpClient = new HttpClient();
        static string host = string.Empty;
        static void Main(string[] args)
        {
         
            //string host = new WebClient().DownloadString("https://ipinfo.io/ip");
            // string host = Dns.GetHostName();

            test("https://ipinfo.io/ip").GetAwaiter().GetResult();

            if (!String.IsNullOrEmpty(host))
            {
                //IPHostEntry ipHost = Dns.GetHostEntry(host);
                IPAddress ipAddr = IPAddress.Parse(host);
                //for(int i =0;i<ipHost.AddressList.Length;++i)
                //{
                //    Console.WriteLine(ipHost.AddressList[i].ToString());
                //}
                //IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 7777);
                listner.Init(endPoint, () => { return new ServerSession(); });

                Console.WriteLine("This is Server ");

                Console.WriteLine(IPAddress.Any.ToString());


                while (true)
                {


                    ;
                }
            }

        }
        
        static async Task test (string url)
        {
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync("https://ipinfo.io/ip");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

               

                host = responseBody;
                Console.WriteLine($"Host : {host}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

    }
}
