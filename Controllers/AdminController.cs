using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PaulaPresentesWebMVC.Models;
using PaulaPresentesWebMVC.Data;

namespace PaulaPresentesWebMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}