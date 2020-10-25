using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Identity.SignUp
{
    public class SignUpController : Controller
    {
        private readonly SignUpService _signUpService;

        public SignUpController(SignUpService signUpService)
        {
            _signUpService = signUpService;
        }

        public IActionResult Index()
        {
            //return View(new SignUpViewModel
            //{
            //    Email = "tree@gmx.ch",
            //    Mobile = "+41 79 541 56 20"
            //});

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUpAsync(SignUpViewModel vm, CancellationToken cancellationToken)
        {
            await _signUpService.SendSmsCodeAsync(vm.Mobile, cancellationToken);

            return View();
        }
    }
}
