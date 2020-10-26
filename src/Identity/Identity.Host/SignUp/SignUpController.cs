using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Guid sessionId = await _signUpService.SendSmsCodeAsync(
                vm.Email,
                vm.Mobile,
                cancellationToken);

            return View("ValidateMobile", new ValidateMobileViewModel
            {
                SessionId = sessionId
            });
        }

        [HttpPost]
        public async Task<IActionResult> ValidateMobile(
            ValidateMobileViewModel vm,
            CancellationToken cancellationToken)
        {
            bool isValid = await _signUpService.ValidateMobileAsync(
                vm.SessionId,
                vm.Code,
                cancellationToken);

            if (isValid)
            {
                return View("Completed");
            }

            else
            {
                return View("ValidateMobile", new ValidateMobileViewModel
                {
                    SessionId = vm.SessionId,
                    ErrorMessage = "Invalid code"
                });
            }
        }
    }
}
