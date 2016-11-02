using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace RetuersStockWebApp
{
    public class FileDatabase
    {
        private const string STOCKSFILEPATH = "c:\\db\\stocks.txt";
        private const string CATEGORIESFILEPATH = "c:\\db\\categories.txt";
        private const string STOCKCATEGORYFILEPATH = "c:\\db\\stock_categories.txt";

        public static IEnumerable<string> Stocks()
            => File.ReadAllLines(STOCKSFILEPATH);

        public static IEnumerable<string> Categories()
            => File.ReadAllLines(CATEGORIESFILEPATH);

        public static IEnumerable<string[]> StockCategories()
            => File.ReadAllLines(STOCKCATEGORYFILEPATH).ToList().Select(line => line.Split(','));

        public static IEnumerable<string> CategoriesForStock(string stock)
            => StockCategories().Where(arr => arr[0] == stock).Select(arr => arr[1]);

        public static IEnumerable<string> StocksForCategory(string category)
            => StockCategories().Where(arr => arr[1] == category).Select(arr => arr[0]);

        public static void AddStock(string stock)
        {
            if (!File.Exists(STOCKSFILEPATH)) File.WriteAllText(STOCKSFILEPATH, "");
            if (!Stocks().Contains(stock))
            File.AppendAllText(STOCKSFILEPATH, Environment.NewLine + stock);
        }

        public static void RemoveStock(string stock)
        {
            var stocks = Stocks();
            stocks.ToList().Remove(stock);
            File.WriteAllText(STOCKSFILEPATH, string.Join(Environment.NewLine, stocks));
            RemoveStockCategoryForStock(stock);
        }

        public static void AddCategory(string category)
        {
            if (!File.Exists(CATEGORIESFILEPATH)) File.WriteAllText(CATEGORIESFILEPATH, "");
            if (!Categories().Contains(category))
            File.AppendAllText(CATEGORIESFILEPATH, Environment.NewLine + category);
        }

        public static void RemoveCategory(string category)
        {
            var categories = Categories();
            categories.ToList().Remove(category);
            File.WriteAllText(CATEGORIESFILEPATH, string.Join(Environment.NewLine, categories));
            RemoveStockCategoryForCategory(category);
        }

        public static void AddStockCategory(string stock, string category)
        {
            if (!File.Exists(STOCKCATEGORYFILEPATH)) File.WriteAllText(STOCKCATEGORYFILEPATH, "");
            if (!StockCategories().Any(arr => arr[0] == stock && arr[1] == category))
                File.AppendAllText(STOCKCATEGORYFILEPATH, Environment.NewLine + stock + "," + category);
        }

        public static void RemoveStockCategory(string stock, string category)
        {
            var stockCategories = StockCategories().
                Where(arr => arr.Length < 2 || arr[0] != stock || arr[1] != category)
                .Select(arr => arr[0] + "," + arr[1]).ToList();
            File.WriteAllText(STOCKCATEGORYFILEPATH, string.Join(Environment.NewLine, stockCategories));
        }

        public static void RemoveStockCategoryForStock(string stock)
        {
            var stockCategories = StockCategories().
                Where(arr => arr.Length < 2 || arr[0] != stock)
                .Select(arr => arr[0] + "," + arr[1]).ToList();
            File.WriteAllText(STOCKCATEGORYFILEPATH, string.Join(Environment.NewLine, stockCategories));
        }

        public static void RemoveStockCategoryForCategory(string category)
        {
            var stockCategories = StockCategories()
                .Where(arr => arr.Length < 2 || arr[1] != category)
                .Select(arr => arr[0] + "," + arr[1]).ToList();
            File.WriteAllText(STOCKCATEGORYFILEPATH, string.Join(Environment.NewLine, stockCategories));
        }
    }
}