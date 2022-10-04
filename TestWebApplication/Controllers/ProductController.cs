using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.Data;
using TestWebApplication.Models;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product.ToArray();

            foreach (var item in objList)
            {
                item.Category = _db.Category.FirstOrDefault(category => category.Id == item.CategoryId);
                item.Type = _db.Type.FirstOrDefault(appType => appType.Id == item.ApplicationTypeId);
            };

            return View(objList);
        }

        // get - upsert
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),

                CategorySelectList  = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

                 ApplicationTypeSelectList = _db.Type.Select(i => new SelectListItem
                 {
                     Text = i.Name,
                     Value = i.Id.ToString()
                 })
            };



            if(id == null)
            {
                // for create
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Product.Find(id);

                if (productVM.Product == null)
                    NotFound();

                return View(productVM);
            }
        }

        // post - upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(obj.Product?.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extention = Path.GetExtension(files[0].FileName);

                    using(var fileStream = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    obj.Product.Image = fileName + extention;

                    _db.Product.Add(obj.Product);
                }
                else
                {
                    //Updating
                    var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(product => product.Id == obj.Product.Id);

                    if(files.Count() > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extention = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        obj.Product.Image = fileName + extention;
                    }
                    else
                    {
                        obj.Product.Image = objFromDb.Image;
                    }
                    _db.Product.Update(obj.Product);
                }


                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            obj.CategorySelectList = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });


            obj.ApplicationTypeSelectList = _db.Type.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(obj);
        }


        // get - delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Product product = _db.Product.Include(u => u.Category).Include(u => u.Type).FirstOrDefault(u => u.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // post - delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Product.Find(id);

            if (obj != null)
            {
                string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
                var oldFile = Path.Combine(upload, obj.Image);

                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
                _db.Product.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound(); 
            }

            //return View(obj);
        }
    }
}
