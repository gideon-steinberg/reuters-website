using System.Web.Mvc;
using System.Linq;

namespace RetuersStockWebApp.Controllers
{
    public class StocksController : Controller
    {
        private string SanitizeString(string input)
          => new string(input.ToCharArray().Where(c => char.IsLetterOrDigit(c) || c == '.').ToArray());

        public ActionResult Stocks()
        {
            return View();
        }

        public JsonResult Info()
        {
            var stock = Request.Params["stock"];
            if (stock == null)
            {
                Response.Redirect("/");
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            stock = SanitizeString(stock);
            var data = ReturesLibrary.StockValues(stock);
            return new JsonResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult AddStock()
        {
            var stock = Request.Params["stock"];
            Response.Redirect("/Stocks/Stocks");
            if (stock == null)
            {
                return new EmptyResult();
            }
            stock = SanitizeString(stock);
            FileDatabase.AddStock(stock);
            return new EmptyResult();
        }

        public ActionResult RemoveStock()
        {
            var stock = Request.Params["stock"];
            Response.Redirect("/Stocks/Stocks");
            if (stock == null)
            {
                return new EmptyResult();
            }
            stock = SanitizeString(stock);
            FileDatabase.RemoveStock(stock);
            return new EmptyResult();
        }

        public JsonResult StockList()
        {
            var category = Request.Params["category"];
            if (category == null)
            {
                return new JsonResult { Data = FileDatabase.Stocks().OrderBy(a => a), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            category = SanitizeString(category);
            return new JsonResult { Data = FileDatabase.StocksForCategory(category).OrderBy(a => a), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CategoryList()
            => new JsonResult { Data = FileDatabase.Categories(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        public ActionResult AddCategory()
        {
            var category = Request.Params["category"];
            Response.Redirect("/Stocks/Stocks");
            if (category == null)
            {
                return new EmptyResult();
            }
            category = SanitizeString(category);
            FileDatabase.AddCategory(category);
            return new EmptyResult();
        }

        public ActionResult RemoveCategory()
        {
            var category = Request.Params["category"];
            Response.Redirect("/Stocks/Stocks");
            if (category == null)
            {
                return new EmptyResult();
            }
            category = SanitizeString(category);
            FileDatabase.RemoveCategory(category);
            return new EmptyResult();
        }

        public ActionResult AddStockCategory()
        {
            var category = Request.Params["category"];
            Response.Redirect("/Stocks/Stocks");
            if (category == null)
            {
                return new EmptyResult();
            }
            category = SanitizeString(category);
            var stock = Request.Params["stock"];
            if (stock == null)
            {
                return new EmptyResult();
            }
            stock = SanitizeString(stock);

            FileDatabase.AddStockCategory(stock, category);
            return new EmptyResult();
        }

        public ActionResult RemoveStockCategory()
        {
            Response.Redirect("/Stocks/Stocks");
            var category = Request.Params["category"];
            var stock = Request.Params["stock"];

            if (category == null && stock == null)
            {
                return new EmptyResult();
            }
            if (category == null)
            {
                stock = SanitizeString(stock);
                FileDatabase.RemoveStockCategoryForStock(stock);
                return new EmptyResult();
            }
            if (stock == null)
            {
                category = SanitizeString(category);
                FileDatabase.RemoveStockCategoryForCategory(category);
                return new EmptyResult();
            }

            stock = SanitizeString(stock);
            category = SanitizeString(category);
            FileDatabase.RemoveStockCategory(stock, category);
            return new EmptyResult();
        }

        public ActionResult AddNZX50()
        {
            var nzx50 = ReturesLibrary.NZX50();

            FileDatabase.RemoveCategory("nzx50");
            FileDatabase.AddCategory("nzx50");
            foreach (var item in nzx50)
            {
                FileDatabase.AddStock(item);
                FileDatabase.AddStockCategory(item, "nzx50");
            }

            Response.Redirect("/Stocks/Stocks");
            return new EmptyResult();
        }

        public ActionResult AddNZX()
        {
            var nzx = ReturesLibrary.NZX();

            FileDatabase.RemoveCategory("nzx");
            FileDatabase.AddCategory("nzx");
            foreach (var item in nzx)
            {
                FileDatabase.AddStock(item);
                FileDatabase.AddStockCategory(item, "nzx");
            }

            Response.Redirect("/Stocks/Stocks");
            return new EmptyResult();
        }
    }
}