using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriaController : Controller
    {

        private readonly IUnidadTrabajo _UnidadTrabajo;

        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _UnidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Categoria categoria = new Categoria();

            if (id == null) {
                //crear nueva Categoria
                categoria.Estado = true;
                return View(categoria);
            }
            //actualizamos bodega
            categoria = await _UnidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if(categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (categoria.Id == 0)
                {
                    await _UnidadTrabajo.Categoria.Agregar(categoria);
                    TempData[DS.Exitosa] = "Categoria creada exitosamente";

                }
                else
                {
                    _UnidadTrabajo.Categoria.Actualizar(categoria);
                    TempData[DS.Exitosa] = "Categoria actualizada exitosamente";

                }
                await _UnidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "No se pudo guardar la Categoria";
            return View(categoria);
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _UnidadTrabajo.Categoria.ObtenerTodos();
            return Json(new { data = todos });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaDb = await _UnidadTrabajo.Categoria.Obtener(id);
            if (categoriaDb == null)
            {
                return Json(new { success = false, message = "ERROR AL BORRAR CATEGORIA " });
            }
            _UnidadTrabajo.Categoria.Remover(categoriaDb);
            await _UnidadTrabajo.Guardar();
            return Json(new { success = true, message = "CATEGORIA BORRADA " });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre,int id = 0)
        {
            bool valor = false;
            var lista = await _UnidadTrabajo.Categoria.ObtenerTodos();
            if(id == 0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id !=id);
            }

            if(valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });

        }



        #endregion
    }

}
