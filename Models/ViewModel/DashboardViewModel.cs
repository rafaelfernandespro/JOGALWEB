using inter.Models;

namespace inter.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProdutos { get; set; }

        public int TotalPedidos { get; set; }

        public int TotalClientes { get; set; }

        public List<Pedidos> UltimosPedidos { get; set; }
    }
}