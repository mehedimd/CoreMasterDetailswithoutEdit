using CoreMasterDetails.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace CoreMasterDetails.Controllers
{
    public class FacultyController : Controller
    {
        private readonly FacultyStudentContext context;
        IHostEnvironment environment;
        public FacultyController(FacultyStudentContext context, IHostEnvironment env = null)
        {
            this.context = context;
            this.environment = env;
        }
        public IActionResult Index()
        {
            return View(context.Faculties.ToList());
        }
        public ActionResult Create()
        {
            Faculty faculty = new Faculty();
            faculty.Students.Add(new Student()
            {
                StudentName = "",
                Address = ""
            });
            return View(faculty);
        }
        [HttpPost]
        public IActionResult Create(Faculty faculty, string btn)
        {
            if (btn == "ADD")
            {
                faculty.Students.Add(new Student());
            }
            if (btn == "Create")
            {
                if (ModelState.IsValid)
                {
                    if (faculty.Picture != null)
                    {
                        // var ext = Path.GetExtension(faculty.Picture.FileName);
                        var rootPath = this.environment.ContentRootPath;
                        var fileToSave = Path.Combine(rootPath, "wwwroot/Pictures", faculty.Picture.FileName);
                        using (var fileStream = new FileStream(fileToSave, FileMode.Create))
                        {
                            faculty.Picture.CopyToAsync(fileStream);
                        }
                        faculty.PicPath = "~/Pictures/" + faculty.Picture.FileName;

                        context.Faculties.Add(faculty);
                        if (context.SaveChanges() > 0)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please Provide Profile Picture");
                        return View(faculty);
                    }
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));
                    ModelState.AddModelError("", message);
                }
            }
            return View(faculty);
        }
    }
}
