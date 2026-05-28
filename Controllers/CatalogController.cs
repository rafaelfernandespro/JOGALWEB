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

        public IActionResult Index(
            string busca,
            int? tipoId)
        {
            var produtos =
                db.Produtos
                .Where(p => p.Ativo)
                .AsQueryable();

            // PESQUISA
            if(!string.IsNullOrEmpty(busca))
            {
                produtos =
                    produtos.Where(p =>
                        p.Nome.Contains(busca));
            }

            // FILTRO CATEGORIA
            if(tipoId.HasValue && tipoId > 0)
            {
                produtos =
                    produtos.Where(p =>
                        p.TipoId == tipoId);
            }

            // MANDAR TIPOS PRA VIEW
            ViewBag.Tipos =
                db.Tipos.ToList();

            ViewBag.Busca =
                busca;

            ViewBag.TipoSelecionado =
                tipoId;

            return View(
                produtos.ToList());
        }
    }
}