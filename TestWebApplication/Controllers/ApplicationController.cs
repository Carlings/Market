using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Data;
using TestWebApplication.Models;

namespace TestWebApplication.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ApplicationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var obj = _db.Type;

            if (obj == null)
                return NotFound();

            return View(obj);
        }
        // create - post
        [HttpPost]
        public IActionResult Create(ApplicationType type)
        {
            if(ModelState.IsValid)
            {
                _db.Add(type);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(type);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var obj = _db.Type.Find(id);

            if (obj == null)
                return NotFound();

            return View(obj);
        }

        [HttpPost]
        public IActionResult Edit(ApplicationType obj)
        {
            if(obj != null)
            {
                _db.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();

        }


        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var obj = _db.Type.Find(id);

            if (obj == null)
                return NotFound();

            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(ApplicationType obj)
        {
            if (obj != null)
            {
                _db.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();

        }
    }
}
