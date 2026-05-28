using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using inter.Models;
using inter.Data;

namespace inter.Controllers
{
    public class ProdutosController : Controller
    {
        private DatabaseContext db;

        public ProdutosController(DatabaseContext db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            var produtos = db.Produtos
                .OrderByDescending(p => p.Ativo)
                .ThenBy(p => p.Id)
                .ToList();

            foreach(var produto in produtos)
            {
                var tipo = db.Tipos
                    .FirstOrDefault(t => t.Id == produto.TipoId);

                produto.NomeTipo = tipo?.Descricao;
            }

            ViewBag.Tipos = db.Tipos.ToList();

            return View(produtos);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Tipos = db.Tipos.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Create(
            Produtos p,
            IFormFile arquivoImagem)
        {
            p.Nome = System.Globalization.CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(p.Nome.ToLower());

            p.Preco = Convert.ToDecimal(
                Request.Form["Preco"]
                .ToString()
                .Replace(".", ",")
            );

            var produtoExistente = db.Produtos
                .FirstOrDefault(prod =>
                    prod.Nome.ToLower() == p.Nome.ToLower()
                    && prod.TipoId == p.TipoId);

            // JÁ EXISTE
            if(produtoExistente != null)
            {
                ViewBag.Erro =
                    "Já existe um produto com esse nome e tipo.";

                ViewBag.Tipos = db.Tipos.ToList();

                return View(p);
            }

            // QUANTIDADE INVÁLIDA
            if(p.Qtd < 0)
            {
                ViewBag.Erro =
                    "Quantidade inválida.";

                ViewBag.Tipos = db.Tipos.ToList();

                return View(p);
            }

            // SALVAR IMAGEM
            if(arquivoImagem != null
                && arquivoImagem.Length > 0)
            {
                string nomeArquivo =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(
                        arquivoImagem.FileName);

                string caminho =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads",
                        nomeArquivo);

                using(var stream =
                    new FileStream(caminho, FileMode.Create))
                {
                    arquivoImagem.CopyTo(stream);
                }

                p.Imagem = nomeArquivo;
            }

            db.Produtos.Add(p);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Inativar(int id)
        {
            var produto =
                db.Produtos
                .FirstOrDefault(p => p.Id == id);

            if(produto == null)
            {
                return RedirectToAction("Index");
            }

            produto.Ativo = false;

            db.SaveChanges();

            TempData["Sucesso"] =
                "Produto desativado com sucesso.";

            return RedirectToAction("Index");
        }

        public IActionResult Ativar(int id)
        {
            var produto =
                db.Produtos
                .FirstOrDefault(p => p.Id == id);

            if(produto == null)
            {
                return RedirectToAction("Index");
            }

            produto.Ativo = true;

            db.SaveChanges();

            TempData["Sucesso"] =
                "Produto ativado com sucesso.";

            return RedirectToAction("Index");
        }

        // ABRE UPDATE
        [HttpGet]
        public ActionResult Update(int id)
        {
            var produto =
                db.Produtos.Single(p => p.Id == id);

            ViewBag.Tipos =
                db.Tipos.ToList();

            return View(produto);
        }

        // SALVA UPDATE
        [HttpPost]
        public ActionResult Update(
            Produtos p,
            IFormFile arquivoImagem)
        {
            p.Nome = System.Globalization.CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(p.Nome.ToLower());

            p.Preco = Convert.ToDecimal(
                Request.Form["Preco"]
                .ToString()
                .Replace(".", ",")
            );

            // QUANTIDADE INVÁLIDA
            if(p.Qtd < 0)
            {
                ViewBag.Erro =
                    "Quantidade inválida.";

                ViewBag.Tipos = db.Tipos.ToList();

                return View(p);
            }

            var produtoBanco =
                db.Produtos.Single(prod => prod.Id == p.Id);

            // MANTÉM STATUS
            p.Ativo = produtoBanco.Ativo;

            // NOVA IMAGEM
            if(arquivoImagem != null
                && arquivoImagem.Length > 0)
            {
                // APAGA IMAGEM ANTIGA
                if(!string.IsNullOrEmpty(produtoBanco.Imagem))
                {
                    string imagemAntiga =
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/uploads",
                            produtoBanco.Imagem);

                    if(System.IO.File.Exists(imagemAntiga))
                    {
                        System.IO.File.Delete(imagemAntiga);
                    }
                }

                // SALVA NOVA IMAGEM
                string nomeArquivo =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(
                        arquivoImagem.FileName);

                string caminho =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads",
                        nomeArquivo);

                using(var stream =
                    new FileStream(caminho, FileMode.Create))
                {
                    arquivoImagem.CopyTo(stream);
                }

                p.Imagem = nomeArquivo;
            }
            else
            {
                // MANTÉM IMAGEM ANTIGA
                p.Imagem = produtoBanco.Imagem;
            }

            db.Entry(produtoBanco)
                .CurrentValues
                .SetValues(p);

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}