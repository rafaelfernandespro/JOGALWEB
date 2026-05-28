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
        [HttpPost]
        public IActionResult Cancelar(int id)
        {
            var clienteId =
                HttpContext.Session.GetInt32("clienteId");

            if(clienteId == null)
            {
                TempData["Erro"] =
                    "Sessão expirada.";

                return RedirectToAction(
                    "Index",
                    "Login");
            }

            var pedido = db.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefault(p =>
                    p.Id == id
                    && p.ClienteId == clienteId);

            if(pedido == null)
            {
                TempData["Erro"] =
                    "Pedido não encontrado.";

                return RedirectToAction("Index");
            }

            // APENAS PENDENTE
            if(pedido.StatusId != 1)
            {
                TempData["Erro"] =
                    "Apenas pedidos pendentes podem ser cancelados.";

                return RedirectToAction("Index");
            }

            // CANCELA
            pedido.StatusId = 5;

            // DEVOLVE ESTOQUE
            foreach(var item in pedido.Itens)
            {
                var produto = db.Produtos
                    .FirstOrDefault(p =>
                        p.Id == item.ProdutoId);

                if(produto != null)
                {
                    produto.Qtd += item.Quantidade;
                }
            }

            db.SaveChanges();

            TempData["PedidoCancelado"] = true;

            return RedirectToAction("Index");
        }
    }
}