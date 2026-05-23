using Microsoft.AspNetCore.Mvc;
using inter.Models;
using inter.Data;

namespace inter.Controllers
{
    public class PerfilController : Controller
    {
        private readonly DatabaseContext db;

        public PerfilController(DatabaseContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int id = Convert.ToInt32(
                HttpContext.Session.GetString("id"));

            var usuario = db.Pessoas
                .FirstOrDefault(u => u.Id == id);

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Index(Pessoas p)
        {
            int id = Convert.ToInt32(
                HttpContext.Session.GetString("id"));

            var usuario = db.Pessoas
                .FirstOrDefault(u => u.Id == id);

            if(usuario == null)
            {
                return RedirectToAction(
                    "Index",
                    "Catalogo");
            }

            usuario.Email = p.Email;
            usuario.Senha = p.Senha;

            db.SaveChanges();

            TempData["Sucesso"] =
                "Dados alterados com sucesso.";

            return RedirectToAction("Index");
        }
    }
}