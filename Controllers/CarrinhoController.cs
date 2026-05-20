using Microsoft.AspNetCore.Mvc;
using inter.Models;
using System.Text.Json;

namespace inter.Controllers
{
    public class CarrinhoController : Controller
    {
        private const string CHAVE_CARRINHO = "carrinho";

        // Pega o carrinho da Session
        private List<CarrinhoItem> GetCarrinho()
        {
            var carrinhoJson = HttpContext.Session.GetString(CHAVE_CARRINHO);

            if (string.IsNullOrEmpty(carrinhoJson))
                return new List<CarrinhoItem>();

            return JsonSerializer.Deserialize<List<CarrinhoItem>>(carrinhoJson);
        }

        // Salva o carrinho na Session
        private void SalvarCarrinho(List<CarrinhoItem> carrinho)
        {
            var json = JsonSerializer.Serialize(carrinho);
            HttpContext.Session.SetString(CHAVE_CARRINHO, json);
        }

        // Exibir carrinho (View)
        public IActionResult Index()
        {
            var carrinho = GetCarrinho();
            return View(carrinho);
        }

        // Adicionar item no carrinho
        [HttpPost]
        public IActionResult Adicionar(CarrinhoItem item)
        {
            var carrinho = GetCarrinho();

            var itemExistente = carrinho.FirstOrDefault(x => x.ProdutoId == item.ProdutoId);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += item.Quantidade;
            }
            else
            {
                carrinho.Add(item);
            }

            SalvarCarrinho(carrinho);

            return Json(carrinho);
        }

        // Remover item
        [HttpPost]
        public IActionResult Remover(int produtoId)
        {
            var carrinho = GetCarrinho();

            var item = carrinho.FirstOrDefault(x => x.ProdutoId == produtoId);

            if (item != null)
            {
                carrinho.Remove(item);
            }

            SalvarCarrinho(carrinho);

            return Json(carrinho);
        }

        // Atualizar quantidade
        [HttpPost]
        public IActionResult Atualizar(int produtoId, int quantidade)
        {
            var carrinho = GetCarrinho();

            var item = carrinho.FirstOrDefault(x => x.ProdutoId == produtoId);

            if (item != null)
            {
                item.Quantidade = quantidade;

                if (item.Quantidade <= 0)
                    carrinho.Remove(item);
            }

            SalvarCarrinho(carrinho);

            return Json(carrinho);
        }

        // Limpar carrinho
        [HttpPost]
        public IActionResult Limpar()
        {
            HttpContext.Session.Remove(CHAVE_CARRINHO);
            return Json(new { sucesso = true });
        }
    }
}