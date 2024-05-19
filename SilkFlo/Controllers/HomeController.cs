using Microsoft.AspNetCore.Mvc;
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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult FormBuilder()
        {
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
                return View("Index", model);
            }

            try
            {
                var result = EvaluateExpression(model.Expression);
                ViewBag.Result = result;
            }
            catch (DivideByZeroException)
            {
                ModelState.AddModelError("", "Division by zero is not allowed.");
                return View("Index", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "invalid characters");
                return View("Index", model);
            }

            return View("Index", model);
        }
        private double EvaluateExpression(string expression)
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}