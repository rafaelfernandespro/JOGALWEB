using Microsoft.AspNetCore.Mvc;
using inter.Data;

namespace inter.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly DatabaseContext db;

        public CatalogoController(DatabaseContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var produtos = db.Produtos.ToList();

            return View(produtos);
        }
    }
}