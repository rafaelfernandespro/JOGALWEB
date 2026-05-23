using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using inter.Data;

namespace inter.Controllers
{
    public class PedidosController : Controller
    {
        private readonly DatabaseContext db;

        public PedidosController(DatabaseContext context)
        {
            db = context;
        }

        // LISTA GERAL
        public IActionResult Index()
        {
            var pedidos = db.Pedidos
                        .Include(p => p.Itens)
                        .Include(p => p.Status)
                        .OrderByDescending(p => p.Id)
                        .ToList();

            ViewBag.Status =
                db.StatusPedido.ToList();

            return View(pedidos);
        }

        // DETALHES
        public IActionResult Detalhes(int id)
        {
            var pedido = db.Pedidos
                        .Include(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                        .Include(p => p.Status)
                        .FirstOrDefault(p => p.Id == id);

            if(pedido == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Status =
                db.StatusPedido.ToList();

            return View(pedido);
        }

        // ALTERAR STATUS
        [HttpPost]
        public IActionResult AlterarStatus(
            int pedidoId,
            int statusId)
        {
            var pedido = db.Pedidos
                .FirstOrDefault(p => p.Id == pedidoId);

            if(pedido == null)
            {
                return RedirectToAction("Index");
            }

            // BLOQUEIO
            if(pedido.StatusId == 4
                || pedido.StatusId == 5)
            {
                TempData["Erro"] =
                    "Pedidos entregues ou cancelados não podem ser alterados.";

                return RedirectToAction(
                    "Detalhes",
                    new { id = pedidoId });
            }

            // CANCELADO
            // DEVOLVE ESTOQUE
            if(pedido.StatusId != 5
                && statusId == 5)
            {
                var itens = db.ItensPedido
                    .Where(i => i.PedidoId == pedidoId)
                    .ToList();

                foreach(var item in itens)
                {
                    var produto = db.Produtos
                        .FirstOrDefault(p =>
                            p.Id == item.ProdutoId);

                    if(produto != null)
                    {
                        produto.Qtd += item.Quantidade;
                    }
                }
            }

            pedido.StatusId = statusId;

            db.SaveChanges();

            TempData["Sucesso"] =
                "Status atualizado com sucesso.";

            return RedirectToAction(
                "Detalhes",
                new { id = pedidoId });
        }
    }
}