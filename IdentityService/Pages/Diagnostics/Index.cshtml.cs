using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Diagnostics;

[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    public ViewModel View { get; set; } = default!;

    public async Task<IActionResult> OnGet()
    {
        //Replace with an authorization policy check
        if (HttpContext.Connection.IsRemote())
        {
            ViewData["Message"] = "Your application description page from within a container";
        }

        View = new ViewModel(await HttpContext.AuthenticateAsync());

        return Page();
    }
}
