using ImportarImagemProjetoCanal.Data;
using ImportarImagemProjetoCanal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImportarImagemProjetoCanal.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private string _caminhoSalvarImagem;
        public HomeController(AppDbContext context, IWebHostEnvironment sistema)
        {
            _context = context;
            _caminhoSalvarImagem = sistema.WebRootPath;
        }

        [HttpGet]
        public async Task<ActionResult<List<ImagemModel>>> Index()
        {
            var imagens = await _context.Imagens.ToListAsync();
            return View(imagens);
        }

        [HttpGet]
        public ActionResult ImportImage()
        {
            return View();
        }

       
        [HttpGet]
        public async Task<ActionResult<ImagemModel>> EditImage(int id)
        {
            var imagem = await _context.Imagens.FirstOrDefaultAsync(img => img.Id == id);
            return View(imagem);
        }

        [HttpGet]
        public async Task<ActionResult<ImagemModel>> RemoveImage(int id)
        {
            var imagem = await _context.Imagens.FirstOrDefaultAsync(img => img.Id == id);
            return View(imagem);
        }


        [HttpPost]
        public async Task<ActionResult> ImportImage(IFormFile foto, string nome)
        {
            try
            {
                if (foto != null && nome != null)
                {

                    var nomeCaminhoImagem = GeraCaminhoImagem(foto);

                    var novaImagem = new ImagemModel
                    {
                        CaminhoImagem = nomeCaminhoImagem,
                        Nome = nome
                    };


                    _context.Add(novaImagem);
                    await _context.SaveChangesAsync();


                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "É necessário incluir a imagem e o título!";
                    return View();
                }



            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }



        }

        [HttpPost]
        public async Task<ActionResult<ImagemModel>> EditImage(IFormFile foto, string nome, int id)
        {
            var imagem = await _context.Imagens.FirstOrDefaultAsync(img => img.Id == id);
            if(foto != null && nome != null)
            {
                var caminhoImagemExiste = _caminhoSalvarImagem + "\\imagem\\" + imagem.CaminhoImagem;

                if (System.IO.File.Exists(caminhoImagemExiste))
                {
                    System.IO.File.Delete(caminhoImagemExiste);
                }

                var nomeCaminhoImagem = GeraCaminhoImagem(foto);

                imagem.Nome = nome;
                imagem.CaminhoImagem = nomeCaminhoImagem;

                _context.Update(imagem);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");

            }
            else
            {
                TempData["MensagemErro"] = "É necessário incluir a imagem e o título!";
                return View(imagem);
            }

        }

        [HttpPost]
        public async Task<ActionResult> RemoveImage(ImagemModel imageRecebida)
        {
            var imagem = await _context.Imagens.FirstOrDefaultAsync(img => img.Id == imageRecebida.Id);

            if (imagem != null)
            {

                var caminhoImagemExistente = _caminhoSalvarImagem + "\\imagem\\" + imagem.CaminhoImagem;

                if (System.IO.File.Exists(caminhoImagemExistente))
                {
                    System.IO.File.Delete(caminhoImagemExistente);
                }

                _context.Remove(imagem);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");


            }
            else
            {
                TempData["MensagemErro"] = "Erro ao realizar o processo de exclusão!";
                return RedirectToAction("Index");
            }
        }

        public string GeraCaminhoImagem(IFormFile foto)
        {

            var codigoUnico = Guid.NewGuid().ToString().ToString();
            var nomeCaminhoImagem = foto.FileName.Replace(" ", "").ToLower() + codigoUnico + ".png";

            string caminhoParaSalvarImagens = _caminhoSalvarImagem + "\\imagem\\";

            if (!Directory.Exists(caminhoParaSalvarImagens))
            {
                Directory.CreateDirectory(caminhoParaSalvarImagens);
            }

            using (var stream = System.IO.File.Create(caminhoParaSalvarImagens + nomeCaminhoImagem))
            {
                foto.CopyToAsync(stream).Wait();
            }

            return nomeCaminhoImagem;
        }


    }
}
