using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ValidadorXml.Models.DB;

namespace ValidadorXml.Controllers
{
    public class InformacionController : Controller
    {
        private readonly ValidadorXmlContext _context;

        public InformacionController(ValidadorXmlContext context)
        {
            _context = context;
        }

        // GET: Informacion
        public async Task<IActionResult> Index()
        {
            var validadorXmlContext = _context.Informacions.Include(i => i.Estatus);
            return View(await validadorXmlContext.ToListAsync());
        }

        // GET: Informacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var informacion = await _context.Informacions
                .Include(i => i.Estatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (informacion == null)
            {
                return NotFound();
            }

            return View(informacion);
        }

        // GET: Informacion/Create
        public IActionResult Create()
        {
            ViewData["EstatusId"] = new SelectList(_context.Estatuses, "EstatusId", "EstatusId");
            return View();
        }

        // POST: Informacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RfcEmisor,RfcReceptor,FolioFiscal,FechaEmision,Total,EstatusId")] Informacion informacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(informacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EstatusId"] = new SelectList(_context.Estatuses, "EstatusId", "EstatusId", informacion.EstatusId);
            return View(informacion);
        }

        // GET: Informacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var informacion = await _context.Informacions.FindAsync(id);
            if (informacion == null)
            {
                return NotFound();
            }
            ViewData["EstatusId"] = new SelectList(_context.Estatuses, "EstatusId", "EstatusId", informacion.EstatusId);
            return View(informacion);
        }

        // POST: Informacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RfcEmisor,RfcReceptor,FolioFiscal,FechaEmision,Total,EstatusId")] Informacion informacion)
        {
            if (id != informacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(informacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InformacionExists(informacion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EstatusId"] = new SelectList(_context.Estatuses, "EstatusId", "EstatusId", informacion.EstatusId);
            return View(informacion);
        }

        // GET: Informacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var informacion = await _context.Informacions
                .Include(i => i.Estatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (informacion == null)
            {
                return NotFound();
            }

            return View(informacion);
        }

        // POST: Informacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var informacion = await _context.Informacions.FindAsync(id);
            if (informacion != null)
            {
                _context.Informacions.Remove(informacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InformacionExists(int id)
        {
            return _context.Informacions.Any(e => e.Id == id);
        }
    }
}
