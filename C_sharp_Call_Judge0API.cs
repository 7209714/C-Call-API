using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;
using System.Web.Script.Serialization;
using System.Web;
using System.Net;
using Newtonsoft.Json;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // 加密
            string inputString = "#include<iostream>\nusing namespace std;\nint main()\n{\n\tcout<<\"Hello,world!\";\n}";
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(inputString);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            Console.WriteLine(returnValue);


            //API 網址
            string url = "https://ce.judge0.com/submissions/?base64_encoded=true&wait=true&fields=stdout";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); //建立 HTTP request 物件
            request.Timeout = 100000000; //傳送逾時時間
            request.Method = "POST";    //設定傳送方式 (POST)
            request.ContentType = "application/json; charset=utf-8"; //設定內容類型
            using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new  //製作BODY 的 JSON 語法
                {
                    //BODY裡面要求的參數(鍵值對) souce_code:用base64加密的程式碼 language_id:語言代碼
                    source_code = returnValue,
                    // source_code = returnValue,
                    language_id = "54"
                });
                //command line 顯示 json 語法
                Console.WriteLine(json);
                sw.Write(json);     //寫入BODY的JSON 檔案 到 request 物件內
                sw.Flush();         //執行FLUSH 將所有資料存入資料流中
            }
            HttpWebResponse httpResponse = null;
            try
            {
                httpResponse = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)   //抓取錯誤訊息
            {
                Console.WriteLine("This program is expected to throw WebException on successful run." +
                        "\n\nException Message :" + e.Status.ToString());
                Console.ReadKey(); //暫停程式 以便讀取錯誤訊息
                System.Environment.Exit(0); //結束程式
            }

            //取出API回傳訊息
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                /*
                // 處理json
                JObject jsondata = JsonConvert.DeserializeObject<JObject>(result); //將資料轉為json物件
                JArray jsondataArray = (JArray)jsondata["records"]["location"]; //json陣列
                foreach (JObject data in jsondataArray)
                {
                    string resultWord = (string)data["stdout"];
                    Console.WriteLine(resultWord);
                }
                Console.ReadLine();
                */

                Console.WriteLine(result.ToString());   //顯示回傳的JSON檔案

                var NameObject = JsonConvert.DeserializeObject<test>(result);
                Console.WriteLine(NameObject.stdout);

                string c = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(NameObject.stdout));
                
                Console.WriteLine(c);

                Console.ReadKey();//暫停程式 以便讀取訊息

                /*            
                // 解密
                byte[] bytes = Convert.FromBase64String(resultWord);
                string originalText = Encoding.ASCII.GetString(bytes);
                */
            }
        }
        public class test
        {
            public string stdout
            {
                get;
                set;
            }


        }
    }
}
