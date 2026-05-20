using Microsoft.AspNetCore.Mvc;
using inter.Data;

namespace inter.Controllers
{
    public class LoginController : Controller
    {
        private readonly DatabaseContext _context;

        public LoginController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Entrar(string email, string senha)
        {
            var usuario = _context.Pessoas  // SELECT * FROM PESSOAS WHERE EMAIL == EMAIL AND SENHA == SENHA
                .FirstOrDefault(u =>
                    u.Email == email &&
                    u.Senha == senha);

            if (usuario == null)
            {
                ViewBag.Erro = "Login inválido";
                return View("Index");
            }

            HttpContext.Session.SetString("usuario", usuario.Nome);
            HttpContext.Session.SetString("tipo", usuario.Tipo.ToString());

            if (usuario.Tipo == 2)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return RedirectToAction("Index", "Catalogo");
        }
    }
}