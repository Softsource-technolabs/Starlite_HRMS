using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using StarLine.Core.Common.EmailModel;
using System.Text;

namespace StarLine.Core.Common
{
    public class StarliteEmailService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public StarliteEmailService(UserManager<IdentityUser> userManager, IEmailSender emailSender, IHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }
        public async Task SendConfirmationAccountEmail(IdentityUser user, string password, string UserName)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var path = _hostEnvironment.ContentRootPath + "/EmailTemplates/ConfirmAccountTemplate.html";
            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

            var request = _httpContextAccessor.HttpContext.Request;

            var confirmationUrl = _linkGenerator.GetUriByAction(
                httpContext: _httpContextAccessor.HttpContext,
                action: "ConfirmEmail",
                controller: "Account",
                values: new { userId = user.Id, code = code, area = "" },
                scheme: request?.Scheme
            );

            var emailBody = await CommonFunctions.ReplacePlaceholders(path, new ConfirmationEmailTemplate
            {
                CompanyName = "Starlite Pharmacy",
                ConfirmationLink = confirmationUrl ?? "",
                Password = password,
                Subject = "Confirm your email",
                UserName = UserName,
            });
            await _emailSender.SendEmailAsync(user.Email ?? "", "Confirm your email", emailBody);
        }
    }
}
