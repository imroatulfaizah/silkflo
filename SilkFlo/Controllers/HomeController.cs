using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilkFlo.Models;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Transactions;

namespace SilkFlo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SilkFloDbContext _context;

        public HomeController(ILogger<HomeController> logger, SilkFloDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Calculator()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public async Task<IActionResult> FormBuilder()
        {

            var formBuilder = new FormBuilder
            {
                Name = "Form builder",
                Description= "Description of form builder",
                
            };

            _context.FormBuilder.Add(formBuilder);
            await _context.SaveChangesAsync();
            ViewBag.Id = formBuilder.Id;

            return View();
        }

        #region calculate the expression

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate(CalculationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            if (!Regex.IsMatch(model.Expression, @"[\+\-\*/\^]|sqrt\(|exp\(|pow\("))
            {
                ModelState.AddModelError("", "The expression must contain at least one arithmetic operator (+, -, *, /, ^), or a function (sqrt(), exp(), pow()).");
                return View("Calculator", model);
            }

            try
            {
                var result = EvaluateExpression(model.Expression);
                ViewBag.Result = result;
                model.Memory = result;
            }
            catch (DivideByZeroException)
            {
                ModelState.AddModelError("", "Division by zero is not allowed.");
                return View("Calculator", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "invalid characters");
                return View("Calculator", model);
            }

            return View("Calculator", model);
        }
        public double EvaluateExpression(string expression)
        {
            NCalc.Expression e = new NCalc.Expression(expression);
            e.EvaluateFunction += delegate (string name, NCalc.FunctionArgs args)
            {
                if (name == "sqrt")
                {
                    args.Result = Math.Sqrt(Convert.ToDouble(args.Parameters[0].Evaluate()));
                }
                else if (name == "exp")
                {
                    args.Result = Math.Exp(Convert.ToDouble(args.Parameters[0].Evaluate()));
                }
                else if (name == "pow")
                {
                    double baseNumber = Convert.ToDouble(args.Parameters[0].Evaluate());
                    double exponent = Convert.ToDouble(args.Parameters[1].Evaluate());
                    args.Result = Math.Pow(baseNumber, exponent);
                }
            };
            return Convert.ToDouble(e.Evaluate());
        }
        #endregion

        #region crud form builder to database
        [HttpPost]
        public async Task<IActionResult> PostElement([FromBody] Element element)
        {
            _context.Elements.Add(element);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetElement", new { id = element.Id }, element);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetElement(int id)
        {
            var element = await _context.Elements.FindAsync(id);

            if (element == null)
            {
                return NotFound();
            }

            return Ok(element);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutElement(int id, [FromBody] Element element)
        {
            if (id != element.Id)
            {
                return BadRequest();
            }

            _context.Entry(element).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ElementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ElementExists(int id)
        {
            return _context.Elements.Any(e => e.Id == id);
        }
        #endregion

        #region load form builder
        [HttpPost]
        public ActionResult LoadFormBuilder()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
   
                var forms = _context.FormBuilder.Select(t => new FormBuilder
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description= t.Description,
                });
   
                recordsTotal = forms.Count();
                var data = forms.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EditFormBuilder(int? Id)
        {
            ViewBag.id = Id;

            try
            {
                var _form = new Element();
                _form = (_context.Elements).Where(a => a.IdFormBuilder == Id).FirstOrDefault();

                //ViewBag.SelectedPosition = _User.Position;

                return View(_form);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("DeleteFormBuilder/{id}")]
        public JsonResult DeleteFormBuilder(int? id)
        {
            if (id == null)
            {
                return Json(new { message = "Not Deleted" });
            }

            var _form = _context.FormBuilder.Find(id);
            if (_form == null)
            {
                return Json(new { message = "form not found" });
            }

            _context.FormBuilder.Remove(_form);

            //delete element
            var elements = _context.Elements.Where(el => el.IdFormBuilder == id).ToList();
            if (!elements.Any())
            {
                return Json(new { message = "Elements not found" });
            }

            foreach (var element in elements)
            {
                _context.Elements.Remove(element);
            }
            _context.SaveChanges();

            return Json(new { message = "Deleted" });
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}