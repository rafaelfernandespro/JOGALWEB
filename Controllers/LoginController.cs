using Microsoft.AspNetCore.Mvc;
using inter.Data;
using inter.Models;

namespace inter.Controllers
{
    public class LoginController : Controller
    {
        private readonly DatabaseContext _context;

        public LoginController(DatabaseContext context)
        {
            _context = context;
        }

        // TELA LOGIN
        public IActionResult Index()
        {
            return View();
        }

        // LOGIN
        [HttpPost]
        public IActionResult Entrar(string email, string senha)
        {
            var usuario = _context.Pessoas
                    .FirstOrDefault(u =>
                        u.Email == email
                        && u.Senha == senha
                        && u.Ativo);

            if(usuario == null)
            {
                ViewBag.Erro = "Login inválido";

                return View("Index");
            }

            HttpContext.Session.SetString(
                "id",
                usuario.Id.ToString());

            HttpContext.Session.SetString(
                "usuario",
                usuario.Nome);

            HttpContext.Session.SetString(
                "tipo",
                usuario.Tipo.ToString());

            // ID CLIENTE
            HttpContext.Session.SetInt32(
                "clienteId",
                usuario.Id);

            // ADMIN / FUNCIONÁRIO
            if(usuario.Tipo == 2 || usuario.Tipo == 3)
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }

            // CLIENTE
            return RedirectToAction(
                "Index",
                "Catalogo");
        }

        // TELA CADASTRO
        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

       // CADASTRAR CLIENTE
        [HttpPost]
        public IActionResult Cadastro(
            string nome,
            string cpf,
            string endereco,
            string numero,
            string email,
            string senha,
            string telefone)
        {
            nome = System.Globalization.CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(nome.ToLower());

            endereco = System.Globalization.CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(endereco.ToLower());

            // VERIFICA EMAIL
            bool emailExiste =
                _context.Pessoas.Any(p =>
                    p.Email == email);

            if(emailExiste)
            {
                ViewBag.Erro =
                    "Este email já está cadastrado.";

                return View();
            }

            // VERIFICA CPF
            bool cpfExiste =
                _context.Pessoas.Any(p =>
                    p.Cpf == cpf);

            if(cpfExiste)
            {
                ViewBag.Erro =
                    "Este CPF já está cadastrado.";

                return View();
            }

            Pessoas novaPessoa = new Pessoas
            {
                Nome = nome,
                Cpf = cpf,
                Endereco = endereco,
                Numero = numero,
                Email = email,
                Senha = senha,
                Telefone = telefone,

                // CLIENTE
                Tipo = 1,

                Ativo = true
            };

            // SALVA PESSOA
            _context.Pessoas.Add(novaPessoa);

            _context.SaveChanges();

            // CRIA CLIENTE
            Clientes novoCli = new Clientes
            {
                Id = novaPessoa.Id
            };

            _context.Clientes.Add(novoCli);

            // SALVA CLIENTE
            _context.SaveChanges();

            TempData["Sucesso"] =
                "Conta criada com sucesso.";

            return RedirectToAction("Index");
        }
    }
}