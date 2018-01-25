using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StockExchange
{
    public static class AVApiWrapper
    {
        //reformats the stock value string to two decimals and adds a currency sign
        public static string ReformatStockValueString(string StockValue)
        {
            string newFormat = StockValue;

            //check if there are unnecessary 0s
            if (newFormat.EndsWith("0"))
            {
                newFormat = newFormat.TrimEnd('0');
            }

            //check if the value is a whole number
            if (newFormat.EndsWith("."))
            {
               newFormat = newFormat.TrimEnd('.');
            }

            //add currency
            return newFormat + "$";
        }

        //gets the latest date for which data is available
        public static DateTime GetDate()
        {
            DateTime Date = new DateTime();
            //check if monday and stock exchange opened if not use fridays data
            if (((int)DateTime.Now.DayOfWeek == 1) && DateTime.Now.Hour < 17)
            {
                Date = DateTime.Today.AddDays(-3);
            }
            //check if weekend->use fridays data
            if((int)DateTime.Now.DayOfWeek == 7)
            {
                Date = DateTime.Today.AddDays(-2);
            }
            else if((int)DateTime.Now.DayOfWeek == 6)
            {
                Date = DateTime.Today.AddDays(-1);
            }
            //check if stock exchange opened if not use yesterdays values
            else if (DateTime.Now.Hour < 17)
            {
                Date = DateTime.Today.AddDays(-1);
            }
            //get todays data
            else
            {
                Date = DateTime.Today;
            }

            return Date;
        }

        //determines if the course is rising compared to yesterdays data
        public static bool courseIsRising(string closingCourse, string yesterdayClosingValue)
        {
            bool IsRising = true;
            if (Double.Parse(closingCourse) < Double.Parse(yesterdayClosingValue))
            {
                IsRising = false;
            }

            return IsRising;
        }

        //calculates % difference from yesterdays course
        public static string coursePercentageDeviance(string closingCourse, string yesterdayClosingValue)
        {
            double current = Double.Parse(closingCourse);
            double yesterday = Double.Parse(yesterdayClosingValue);
            double difference = current - yesterday;
            string percentage = "0";
            double yesterdayOnePercent = yesterday / 100;
            double percent = Math.Round(Math.Abs(difference) / yesterdayOnePercent,2);


            if (difference < 0)
            {
                return percentage = "-" + percent.ToString();
            }

            return percentage = "+" + percent.ToString();
        }

        //sets data received from the API and returns it to WelcomePage to update the UI
        public static async Task<ApiData> getApiData(string Company, string ApiKey)
        {
            //get dates for the JSON data
            string today = GetDate().ToString("yyyy-MM-dd");
            string yesterday = GetDate().AddDays(-1).ToString("yyyy-MM-dd");
            string yesterdayClosing;

            JObject dailyData = await getDailyJSONData(Company, ApiKey);

            ApiData datas = new ApiData();
            //convert relevant JSON data to strings
            datas.closingCourse = dailyData["Time Series (Daily)"][today]["4. close"].ToString();
            datas.dailyHigh = dailyData["Time Series (Daily)"][today]["2. high"].ToString();
            datas.dailyLow = dailyData["Time Series (Daily)"][today]["3. low"].ToString();
            yesterdayClosing = dailyData["Time Series (Daily)"][yesterday]["4. close"].ToString();
            //check if course is rising and add %
            if(!String.IsNullOrEmpty(datas.closingCourse) && !String.IsNullOrEmpty(yesterdayClosing))
            {
                datas.isHigher = courseIsRising(datas.closingCourse, yesterdayClosing);
                datas.percentageDifference = coursePercentageDeviance(datas.closingCourse, yesterdayClosing);
            }
            //reformat strings to a better format, usually two decimals and a currency sign
            datas.closingCourse = ReformatStockValueString(datas.closingCourse);
            datas.dailyHigh = ReformatStockValueString(datas.dailyHigh);
            datas.dailyLow = ReformatStockValueString(datas.dailyLow);
            datas.percentageDifference += "%";

            return datas;
        }

        //returns JSON data for further formatting
        public static async Task<JObject> getDailyJSONData(string Company, string ApiKey)
        {
            string HttpRequest = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=" + GetStockHandle(Company) + "&apikey=" + ApiKey;
            return JsonConvert.DeserializeObject<JObject>(await MakeRequest(HttpRequest));
        }

        //Sends request to Alpha Vantage API via HTTP and receives JSON data
        public static async Task<string> MakeRequest(string Request)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    HttpResponseMessage RMessage = await client.GetAsync(Request);
                    RMessage.EnsureSuccessStatusCode();
                    return await RMessage.Content.ReadAsStringAsync();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Class containing all relevant data to update the UI
        public class ApiData
        {
            public string dailyHigh { get; set; }
            public string dailyLow { get; set; }
            public string closingCourse { get; set; }
            public string percentageDifference { get; set; }
            public bool isHigher { get; set; }

            //standard constructor
            public ApiData()
            {
                percentageDifference = "0";
                isHigher = false;
            }

            //copy constructor
            public ApiData(ApiData data)
            {
                dailyHigh = data.dailyHigh;
                dailyLow = data.dailyLow;
                closingCourse = data.closingCourse;
                percentageDifference = data.percentageDifference;
                isHigher = data.isHigher;
            }
        }

        //converts title of Companies to handles used by Stock Exchanges
        //for this project a few(not all) companies are included
        public static string GetStockHandle(string UserEntry)
        {
            string Handle = null;
            switch (UserEntry)
            {
                case "Microsoft":
                    Handle = "MSFT";
                    break;
                case "Apple":
                    Handle = "AAPL";
                    break;
                case "Google":
                    Handle = "GOOG";
                    break;
                case "Amazon":
                    Handle = "AMZN";
                    break;
                case "Facebook":
                    Handle = "FB";
                    break;
                case "Verizon":
                    Handle = "VZ";
                    break;
                case "Disney":
                    Handle = "DIS";
                    break;
            }

            return Handle;
        }
    }
}
