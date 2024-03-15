using Microsoft.AspNetCore.Mvc;
using MvcNetCorePracticaZapatillas.Models;
using MvcNetCorePracticaZapatillas.Repositories;

namespace MvcNetCorePracticaZapatillas.Controllers
{
    public class ZapatillasController : Controller
    {
        private RepositoryZapatillas repo;

        public ZapatillasController(RepositoryZapatillas repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Zapatilla> zapatillas = await this.repo.GetZapatillasAsync();
            return View(zapatillas);
        }

        public async Task<IActionResult> Details(int idzapatilla)
        {
            PaginacionZapatillas model =
                await this.repo.GetPaginacionZapatillasAsync(1, idzapatilla);
            ViewData["ZAPATILLA"] = model.Zapatilla;
            return View();
        }

        public async Task<IActionResult> _PaginacionImagenZapas
            (int? posicion, int idzapatilla)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            PaginacionZapatillas model =
                await this.repo.GetPaginacionZapatillasAsync(posicion.Value, idzapatilla);
            int numRegistros = model.NumRegistros;
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
                siguiente = numRegistros;
            int anterior = posicion.Value - 1;
            if (anterior < 1)
                anterior = 1;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["ULTIMO"] = model.NumRegistros;
            ViewData["ZAPATILLA"] = model.Zapatilla;
            ViewData["POSICION"] = posicion;
            return PartialView("_PaginacionImagenZapas", model.Imagen);
        }

        public async Task<IActionResult> InsertImagenes()
        {
            List<Zapatilla> zapatillas = await this.repo.GetZapatillasAsync();
            return View(zapatillas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertImagenes
            (List<string> imagenes, int idzapatilla)
        {
            await this.repo.InsertImagenes(imagenes, idzapatilla);
            return RedirectToAction("Details", new { idzapatilla = idzapatilla });
        }
    }
}
