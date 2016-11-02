using HtmlAgilityPack;
using System.Collections.Generic;

namespace RetuersStockWebApp
{
    public class ReturesLibrary
    {
        private const string REUTERS_BASE_URL = @"/finance/stocks/analyst?symbol=";
        private const string RATINGS_XPATH = @"//td[@class=""data dataBold""]/text()";
        private const string CONSENSUS_XPATH = @"//*[@id=""content""]/div[2]/div/div[2]/div[1]/div[2]/div[2]/table/tbody/tr[2]/td[1]/text()"; // taken xpath using chrome inbuilt feature
        private const string PREVIOUS_CLOSE_XPATH = @"//*[@id=""headerQuoteContainer""]/div[3]/div[1]/span[2]/text()";
        private const string REUTERS_OVERVIEW_URL = @"/finance/stocks/overview?symbol=";
        private const string DIVIDENDS_XPATH = @"//*[@id=""overallRatios""]/div/div[2]/table/tbody/tr[5]/td[2]/strong/text()";
        private const string PRICE_EARTINGS_XPATH = @"//*[@id=""companyVsIndustry""]/div/div[2]/table/tbody/tr[2]/td[2]/text()";
        private const string MEAN_LAST_MONTH_XPATH = @"//*[@id=""content""]/div[2]/div/div[2]/div[1]/div[4]/div[2]/table/tbody/tr[9]/td[3]/text()";
        private const string DESCRIPTION_XPATH = @"//*[@id=""sectionTitle""]/h1/text()";

        private const string NZX50_BASE_PATH = "http://topforeignstocks.com";
        private const string NZX_BASE_PATH = "https://www.nzx.com";

        private const string NZX50_URL = @"/indices/components-of-the-nzsx-50-index/";
        private const string NZX_URL = @"/markets/NZSX/securities";
 
        private const string NZX50_XPATH = @"//*[@id=""tablepress-915""]/tbody/tr/td[3]/text()";
        private const string NZX_XPATH = @"//*[@id=""instruments""]/table/tbody/tr/td[1]/a/text()";

        private const string REUTUES_BASE_PATH = "http://www.reuters.com";

        private static HtmlNode Response(string stockName, string path = REUTERS_BASE_URL, string basePath = REUTUES_BASE_PATH)
        {
            string url = path + stockName;
            HtmlWeb web = new HtmlWeb();
            return web.Load($"{basePath}{url}").DocumentNode;
        }

        public static Dictionary<string, object> StockValues(string stock_name)
        {
            var base_response = Response(stock_name);
            var overview_response = Response(stock_name, REUTERS_OVERVIEW_URL);
            var basic_values = base_response.SelectNodes(RATINGS_XPATH);
            var result = new Dictionary<string, object>();
            result["code"] = stock_name;
            result["categories"] = FileDatabase.CategoriesForStock(stock_name);

            if (basic_values == null) return result;
            if (basic_values.Count > 0)
            {
                result["buy"] = basic_values[0].InnerText;
                result["outperform"] = basic_values[1].InnerText;
                result["hold"] = basic_values[2].InnerText;
                result["underperform"] = basic_values[3].InnerText;
                result["sell"] = basic_values[4].InnerText;
                result["no_opinion"] = basic_values[5].InnerText;
                result["mean"] = basic_values[6].InnerText;
            }

            var mean_last_month = base_response.SelectNodes(MEAN_LAST_MONTH_XPATH)?[0].InnerText;
            if (mean_last_month == null || mean_last_month == "--") mean_last_month = "0";

            float? difference = float.Parse(result["mean"].ToString()) - float.Parse(mean_last_month);

            if (difference == 0) difference = null;
            else difference = float.Parse(string.Format("{0:F4}", difference));

            result["mean_difference"] = difference.Value.ToString();
            result["consensus"] = base_response.SelectNodes(CONSENSUS_XPATH)?[0].InnerText;
            if (result["consensus"] == null) result["consensus"] = "---";
            result["price_earnings"] = overview_response.SelectNodes(PRICE_EARTINGS_XPATH)[0].InnerText.Trim();

            var parsed_output = overview_response.SelectNodes(DIVIDENDS_XPATH);
            var dividend = "--";
            if (parsed_output != null && parsed_output.Count > 0) dividend = parsed_output[0].InnerText;
            result["dividend"] = dividend;

            parsed_output = base_response.SelectNodes(DESCRIPTION_XPATH);
            if (parsed_output != null && parsed_output.Count > 0) result["description"] = parsed_output[0].InnerText;
            return result;
        }

        public static IEnumerable<string> NZX50()
        {
            var nodes = Response("", NZX50_URL, NZX50_BASE_PATH).SelectNodes(NZX50_XPATH);
            var list = new List<string>();
            foreach (var node in nodes)
            {
                list.Add(node.InnerText);
            }
            return list;
        }

        public static IEnumerable<string> NZX()
        {
            var nodes = Response("", NZX_URL, NZX_BASE_PATH).SelectNodes(NZX_XPATH);
            var list = new List<string>();
            foreach (var node in nodes)
            {
                list.Add(node.InnerText + ".NZ");
            }
            return list;
        }
    }
}