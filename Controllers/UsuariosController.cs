using Microsoft.AspNetCore.Mvc;
using inter.Data;
using inter.Models;

namespace inter.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DatabaseContext db;

        public UsuariosController(DatabaseContext db)
        {
            this.db = db;
        }

        // LISTA USUÁRIOS
        public IActionResult Index()
        {
            string tipo =
                HttpContext.Session.GetString("tipo");

            var usuarios =
                db.Pessoas.AsQueryable();

            // FUNCIONÁRIO SÓ VÊ CLIENTE
            if(tipo == "3")
            {
                usuarios =
                    usuarios.Where(u => u.Tipo == 1);
            }

            usuarios =
                usuarios.OrderByDescending(u => u.Ativo)
                        .ThenBy(u => u.Id);

            return View(
                usuarios.ToList());
        }

        // ABRE CREATE
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Tipos =
                db.TipoPessoas.ToList();

            return View(new Pessoas());
        }

        // SALVA USUÁRIO
        [HttpPost]
        public IActionResult Create(Pessoas p)
        {
            string tipoLogado =
                HttpContext.Session.GetString("tipo");

            // FUNCIONÁRIO SEMPRE CRIA CLIENTE
            if(tipoLogado == "3")
            {
                p.Tipo = 1;
            }

            // TELEFONE REPETIDO
            bool telefoneExiste =
                db.Pessoas.Any(x =>
                    x.Telefone != null &&
                    x.Telefone == p.Telefone);

            if(telefoneExiste)
            {
                ViewBag.Erro =
                    "Telefone já cadastrado.";

                ViewBag.Tipos =
                    db.TipoPessoas.ToList();

                return View(p);
            }

            // EMAIL REPETIDO
            bool emailExiste =
                db.Pessoas.Any(x =>
                    x.Email == p.Email);

            if(emailExiste)
            {
                ViewBag.Erro =
                    "Email já cadastrado.";

                ViewBag.Tipos =
                    db.TipoPessoas.ToList();

                return View(p);
            }

            p.Nome = System.Globalization
                .CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(
                    p.Nome.ToLower());
            
            p.Ativo = true;

            db.Pessoas.Add(p);

            db.SaveChanges();

            // CLIENTE
            if(p.Tipo == 1)
            {
                Clientes cli = new Clientes();

                cli.Id = p.Id;

                db.Clientes.Add(cli);
            }

            // ADMIN OU FUNCIONÁRIO
            else if(p.Tipo == 2 || p.Tipo == 3)
            {
                Operadores op = new Operadores();

                op.Id = p.Id;

                db.Operadores.Add(op);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ABRE UPDATE
        [HttpGet]
        public IActionResult Update(int id)
        {
            string tipo =
                HttpContext.Session.GetString("tipo");

            var usuario = db.Pessoas
                .FirstOrDefault(u => u.Id == id);

            if(usuario == null)
            {
                return RedirectToAction("Index");
            }

            // FUNCIONÁRIO NÃO EDITA ADMIN/FUNC
            if(tipo == "3"
                && usuario.Tipo != 1)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Tipos =
                db.TipoPessoas.ToList();

            return View(usuario);
        }

        // SALVA UPDATE
        [HttpPost]
        public IActionResult Update(Pessoas p)
        {
            string tipoLogado =
                HttpContext.Session.GetString("tipo");

            var usuarioBanco = db.Pessoas
                .FirstOrDefault(u => u.Id == p.Id);

            if(usuarioBanco == null)
            {
                return RedirectToAction("Index");
            }

            // FUNCIONÁRIO NÃO ALTERA ADMIN/FUNC
            if(tipoLogado == "3"
                && usuarioBanco.Tipo != 1)
            {
                return RedirectToAction("Index");
            }

            // FUNCIONÁRIO NÃO MUDA TIPO
            if(tipoLogado == "3")
            {
                p.Tipo = 1;
            }

            // TELEFONE REPETIDO
            bool telefoneExiste =
                db.Pessoas.Any(x =>
                    x.Telefone != null &&
                    x.Telefone == p.Telefone &&
                    x.Id != p.Id);

            if(telefoneExiste)
            {
                TempData["Erro"] =
                    "Telefone já cadastrado.";

                return RedirectToAction(
                    "Update",
                    new { id = p.Id });
            }

            // EMAIL REPETIDO
            bool emailExiste =
                db.Pessoas.Any(x =>
                    x.Email == p.Email &&
                    x.Id != p.Id);

            if(emailExiste)
            {
                TempData["Erro"] =
                    "Email já cadastrado.";

                return RedirectToAction(
                    "Update",
                    new { id = p.Id });
            }

            // ATUALIZA CAMPOS
            usuarioBanco.Nome = System.Globalization
                .CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(p.Nome.ToLower());

            usuarioBanco.Cpf = p.Cpf;
            usuarioBanco.Telefone = p.Telefone;
            usuarioBanco.Endereco = p.Endereco;
            usuarioBanco.Numero = p.Numero;
            usuarioBanco.Email = p.Email;
            usuarioBanco.Senha = p.Senha;
            usuarioBanco.Tipo = p.Tipo;

            // CLIENTE -> OPERADOR/FUNC
            if((p.Tipo == 2 || p.Tipo == 3)
                && !db.Operadores.Any(o => o.Id == p.Id))
            {
                Operadores op = new Operadores();

                op.Id = p.Id;

                db.Operadores.Add(op);
            }

            // OPERADOR/FUNC -> CLIENTE
            if(p.Tipo == 1)
            {
                var operador = db.Operadores
                    .FirstOrDefault(o => o.Id == p.Id);

                if(operador != null)
                {
                    db.Operadores.Remove(operador);
                }

                if(!db.Clientes.Any(c => c.Id == p.Id))
                {
                    Clientes cli = new Clientes();

                    cli.Id = p.Id;

                    db.Clientes.Add(cli);
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //INATIVAR
        public IActionResult Inativar(int id)
        {
            var usuario = db.Pessoas
                .FirstOrDefault(u => u.Id == id);

            if(usuario == null)
            {
                return RedirectToAction("Index");
            }

            usuario.Ativo = false;

            db.SaveChanges();

            TempData["Sucesso"] =
                "Usuário inativado com sucesso.";

            return RedirectToAction("Index");
        }

        //ATIVAR
        public IActionResult Ativar(int id)
        {
            var usuario = db.Pessoas
                .FirstOrDefault(u => u.Id == id);

            if(usuario == null)
            {
                return RedirectToAction("Index");
            }

            usuario.Ativo = true;

            db.SaveChanges();

            TempData["Sucesso"] =
                "Usuário ativado com sucesso.";

            return RedirectToAction("Index");
        }
    }
}