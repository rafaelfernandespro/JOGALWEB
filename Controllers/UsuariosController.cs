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
            var usuarios = db.Pessoas.ToList();

            return View(usuarios);
        }

        // ABRE TELA CREATE
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Tipos = db.TipoPessoas.ToList();

            return View();
        }

        // SALVA USUÁRIO
        [HttpPost]
        public IActionResult Create(Pessoas p)
        {
            // verifica email repetido
            var emailExiste = db.Pessoas
                .FirstOrDefault(u => u.Email == p.Email);

            if(emailExiste != null)
            {
                ViewBag.Erro = "Email já cadastrado.";

                ViewBag.Tipos = db.TipoPessoas.ToList();

                return View(p);
            }

            p.Nome = System.Globalization.CultureInfo
                    .CurrentCulture
                    .TextInfo
                    .ToTitleCase(p.Nome.ToLower());

            // salva pessoa
            db.Pessoas.Add(p);

            db.SaveChanges();

            // CLIENTE
            if(p.Tipo == 1)
            {
                Clientes cli = new Clientes();

                cli.Id = p.Id;

                db.Clientes.Add(cli);
            }

            // OPERADOR
            else if(p.Tipo == 2)
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

            var usuario = db.Pessoas
                .FirstOrDefault(u => u.Id == id);

            ViewBag.Tipos = db.TipoPessoas.ToList();

            return View(usuario);
        }

        // SALVA UPDATE
        [HttpPost]
        public IActionResult Update(Pessoas p)
        {
            p.Nome = System.Globalization.CultureInfo
                    .CurrentCulture
                    .TextInfo
                    .ToTitleCase(p.Nome.ToLower());
                    
            db.Pessoas.Update(p);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var usuario = db.Pessoas
                .FirstOrDefault(u => u.Id == id);

            // remove cliente relacionado
            var cliente = db.Clientes
                .FirstOrDefault(c => c.Id == id);

            if(cliente != null)
            {
                db.Clientes.Remove(cliente);
            }

            // remove operador relacionado
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