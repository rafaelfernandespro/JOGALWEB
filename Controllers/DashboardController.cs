using Microsoft.AspNetCore.Mvc;
using inter.Data;
using inter.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace inter.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DatabaseContext db;

        public DashboardController(DatabaseContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            DashboardViewModel vm =
                new DashboardViewModel();

            vm.TotalProdutos = db.Produtos.Count();

            vm.TotalPedidos = db.Pedidos.Count();

            vm.TotalClientes = db.Clientes.Count();

            vm.UltimosPedidos = db.Pedidos
                                .Include(p => p.Itens)
                                .Include(p => p.Status)
                                .OrderByDescending(p => p.Id)
                                .Take(5)
                                .ToList();

            return View(vm);
        }
    }
}