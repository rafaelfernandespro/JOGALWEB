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

            return View(
                usuarios.ToList());
        }

        // ABRE CREATE
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Tipos =
                db.TipoPessoas.ToList();

            return View();
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

            // EMAIL REPETIDO
            var emailExiste = db.Pessoas
                .FirstOrDefault(u =>
                    u.Email == p.Email);

            if(emailExiste != null)
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

            // ATUALIZA CAMPOS
            usuarioBanco.Nome = System.Globalization
                .CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(p.Nome.ToLower());

            usuarioBanco.Cpf = p.Cpf;
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

        // DELETE
        public IActionResult Delete(int id)
        {
            string tipo =
                HttpContext.Session.GetString("tipo");

            var usuario = db.Pessoas
                .FirstOrDefault(u => u.Id == id);

            if(usuario == null)
            {
                return RedirectToAction("Index");
            }

            // FUNCIONÁRIO NÃO REMOVE ADMIN/FUNC
            if(tipo == "3"
                && usuario.Tipo != 1)
            {
                return RedirectToAction("Index");
            }

            // REMOVE CLIENTE
            var cliente = db.Clientes
                .FirstOrDefault(c => c.Id == id);

            if(cliente != null)
            {
                db.Clientes.Remove(cliente);
            }

            // REMOVE OPERADOR
            var operador = db.Operadores
                .FirstOrDefault(o => o.Id == id);

            if(operador != null)
            {
                db.Operadores.Remove(operador);
            }

            db.Pessoas.Remove(usuario);

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}