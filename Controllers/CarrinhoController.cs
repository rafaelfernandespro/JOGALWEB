using inter.Data;
using inter.Helpers;
using inter.Models;

using Microsoft.AspNetCore.Mvc;

namespace inter.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly DatabaseContext db;

        public CarrinhoController(DatabaseContext context)
        {
            db = context;
        }

        // MOSTRAR CARRINHO
        public IActionResult Index()
        {
            var carrinho =
                HttpContext.Session
                .GetObjectFromJson<List<CarrinhoItem>>("Carrinho")
                ?? new List<CarrinhoItem>();

            return View(carrinho);
        }

        // ADICIONAR PRODUTO
        [HttpPost]
        public IActionResult Adicionar(
            int produtoId,
            int quantidade)
        {
            var produto =
                db.Produtos
                .FirstOrDefault(p => p.Id == produtoId);

            if(produto == null)
            {
                return NotFound();
            }

            var carrinho =
                HttpContext.Session
                .GetObjectFromJson<List<CarrinhoItem>>("Carrinho")
                ?? new List<CarrinhoItem>();

            var itemExistente =
                carrinho.FirstOrDefault(i =>
                    i.ProdutoId == produtoId);

            // SE JÁ EXISTE
            if(itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                carrinho.Add(new CarrinhoItem
                {
                    ProdutoId = produto.Id,
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    Quantidade = quantidade,
                    Imagem = produto.Imagem
                });
            }

            HttpContext.Session
                .SetObjectAsJson("Carrinho", carrinho);

            return Ok();
        }

        // AUMENTAR QUANTIDADE
        [HttpPost]
        public IActionResult Aumentar(int produtoId)
        {
            var carrinho =
                HttpContext.Session
                .GetObjectFromJson<List<CarrinhoItem>>("Carrinho")
                ?? new List<CarrinhoItem>();

            var item =
                carrinho.FirstOrDefault(i =>
                    i.ProdutoId == produtoId);

            if(item != null)
            {
                item.Quantidade++;
            }

            HttpContext.Session.SetObjectAsJson(
                "Carrinho",
                carrinho);

           return Ok();
        }

        // DIMINUIR QUANTIDADE
        [HttpPost]
        public IActionResult Diminuir(int produtoId)
        {
            var carrinho =
                HttpContext.Session
                .GetObjectFromJson<List<CarrinhoItem>>("Carrinho");

            if(carrinho != null)
            {
                var item =
                    carrinho.FirstOrDefault(i =>
                        i.ProdutoId == produtoId);

                if(item != null)
                {
                    item.Quantidade--;

                    // REMOVE SE CHEGAR EM 0
                    if(item.Quantidade <= 0)
                    {
                        carrinho.Remove(item);
                    }
                }

                HttpContext.Session
                    .SetObjectAsJson("Carrinho", carrinho);
            }

            
            return Ok();
        }

        // REMOVER ITEM
        [HttpPost]
        public IActionResult Remover(int produtoId)
        {
            var carrinho =
                HttpContext.Session
                .GetObjectFromJson<List<CarrinhoItem>>("Carrinho");

            if(carrinho != null)
            {
                var item =
                    carrinho.FirstOrDefault(i =>
                        i.ProdutoId == produtoId);

                if(item != null)
                {
                    carrinho.Remove(item);
                }

                HttpContext.Session
                    .SetObjectAsJson("Carrinho", carrinho);
            }

            return Ok();
        }

        // LIMPAR CARRINHO
        [HttpPost]
        public IActionResult Limpar()
        {
            HttpContext.Session.Remove("Carrinho");

            return Ok();
        }


        // FINALIZAR PEDIDO
        [HttpPost]
        public IActionResult FinalizarPedido()
        {
            // PEGAR CARRINHO
            var carrinho =
                HttpContext.Session
                .GetObjectFromJson<List<CarrinhoItem>>("Carrinho");

            if(carrinho == null || !carrinho.Any())
            {
                return RedirectToAction(
                    "Index",
                    "Catalogo");
            }

            // PEGAR CLIENTE LOGADO
            int clienteId =
                Convert.ToInt32(
                    HttpContext.Session.GetInt32("clienteId"));

            // CALCULAR TOTAL
            decimal total =
                carrinho.Sum(i =>
                    i.Preco * i.Quantidade);


            foreach(var item in carrinho)
            {
                var produto =
                    db.Produtos
                    .FirstOrDefault(p =>
                        p.Id == item.ProdutoId);

                if(produto == null)
                {
                    TempData["Erro"] =
                        "Produto não encontrado.";

                    return RedirectToAction(
                        "Index",
                        "Catalogo");
                }

                if(produto.Qtd < item.Quantidade)
                {
                    TempData["Erro"] =
                        $"Estoque insuficiente para {produto.Nome}.";

                    return RedirectToAction(
                        "Index",
                        "Catalogo");
                }
            }

            // CRIAR PEDIDO
            Pedidos pedido = new Pedidos
            {
                ClienteId = clienteId,

                DataPedido = DateTime.Now,

                Total = total,

                StatusId = 1
            };

            db.Pedidos.Add(pedido);

            db.SaveChanges();

            // CRIAR ITENS
            foreach(var item in carrinho)
            {
                ItensPedido novoItem =
                    new ItensPedido
                    {
                        PedidoId = pedido.Id,

                        ProdutoId = item.ProdutoId,

                        Quantidade = item.Quantidade,

                        PrecoUnitario = item.Preco
                    };

                db.ItensPedido.Add(novoItem);

                // BAIXAR ESTOQUE
                var produto =
                    db.Produtos
                    .FirstOrDefault(p =>
                        p.Id == item.ProdutoId);

                if(produto != null)
                {
                    produto.Qtd -= item.Quantidade;
                }
            }

            db.SaveChanges();

            // LIMPAR CARRINHO
            HttpContext.Session.Remove("Carrinho");

            TempData["PedidoSucesso"] = true;

            return RedirectToAction(
                "Index",
                "Catalogo");
        }
    }
}