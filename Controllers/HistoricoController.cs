using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using inter.Data;

namespace inter.Controllers
{
    public class HistoricoController : Controller
    {
        private readonly DatabaseContext db;

        public HistoricoController(DatabaseContext context)
        {
            db = context;
        }

        // LISTAR PEDIDOS DO CLIENTE
        public IActionResult Index()
        {
            int clienteId =
                Convert.ToInt32(
                    HttpContext.Session
                    .GetInt32("clienteId"));

            var pedidos =
                db.Pedidos
                .Include(p => p.Status)
                .Where(p =>
                    p.ClienteId == clienteId)
                .OrderByDescending(p =>
                    p.DataPedido)
                .ToList();

            return View(pedidos);
        }

        // DETALHES
        public IActionResult Detalhes(int id)
        {
            var pedido =
                db.Pedidos
                .Include(p => p.Status)
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefault(p =>
                    p.Id == id);

            if(pedido == null)
            {
                return RedirectToAction("Index");
            }

            return View(pedido);
        }

        // CANCELAR
        public IActionResult Cancelar(int id)
        {
            var pedido =
                db.Pedidos
                .FirstOrDefault(p =>
                    p.Id == id);

            if(pedido == null)
            {
                return RedirectToAction("Index");
            }

            // SOMENTE PENDENTE
            if(pedido.StatusId != 1)
            {
                TempData["Erro"] =
                    "Esse pedido não pode mais ser cancelado.";

                return RedirectToAction("Index");
            }

            // STATUS CANCELADO
            TempData["PedidoCancelado"] = true;

            // DEVOLVER ESTOQUE
            var itens =
                db.ItensPedido
                .Where(i =>
                    i.PedidoId == pedido.Id)
                .ToList();

            foreach(var item in itens)
            {
                var produto =
                    db.Produtos
                    .FirstOrDefault(p =>
                        p.Id == item.ProdutoId);

                if(produto != null)
                {
                    produto.Qtd += item.Quantidade;
                }
            }

            db.SaveChanges();

            TempData["Sucesso"] =
                "Pedido cancelado.";

            return RedirectToAction("Index");
        }
    }
}