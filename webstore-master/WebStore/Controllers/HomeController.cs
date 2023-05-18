using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebStore.Models;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using WebStore.Config;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration Configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignInProcess(SignInModel signin)
        {
            if (ModelState.IsValid && signin != null)
            {
                try
                {
                    var config = Configuration.GetSection("LDAP").Get<LdapConfig>();
                    DirectoryEntry rootEntry = new DirectoryEntry(Configuration.GetSection("LDAP").GetValue<string>("server"), Configuration.GetValue<string>("LDAP:bindUsername"), 
                        Configuration.GetValue<string>("LDAP:bindPassword"));
                    DirectorySearcher searcher = new DirectorySearcher(rootEntry);
                    var queryFormat = "(&(sAMAccountName=" + signin.Username + "))";
                    searcher.Filter = queryFormat;
                    var r = searcher.FindOne();
                    var e = r.GetDirectoryEntry();
                    if (r == null)
                    {
                        // Maybe we should check the password?
                        return RedirectToAction("SignIn");
                    }
                    // var groups = GetUserGrouops(user)
                    // if (groups != null)
                    // {
                    //     if (groups.Contains("Warehouse Manager"))
                    //     {
                    //         HttpContext.Response.Cookies.Append("role", "Warehouse Manager", new CookieOptions
                    //         {
                    //             Expires = DateTimeOffset.Now.AddDays(60),
                    //             Path = "/",
                    //             HttpOnly = false
                    //         });
                    //     }
                    //
                    
                    // Ran out of time, maybe we should not give everyone CEO ac, just a thought
                    HttpContext.Response.Cookies.Append("role", "CEO", new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(60),
                        Path = "/",
                        HttpOnly = false
                    });
                    
                    rootEntry.Dispose();
                    HttpContext.Response.Cookies.Append("username", signin.Username, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(60),
                        Path = "/",
                        HttpOnly = false
                    });
                    return RedirectToAction("Index");
                }
                catch
                {
                    return RedirectToAction("SignIn");
                }
            }
            else
            {
                return RedirectToAction("SignIn");
            }
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUpProcess(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var config = Configuration.GetSection("LDAP").Get<LdapConfig>();
                    DirectoryEntry directoryEntry =
                        new DirectoryEntry(config.Server, config.BindUserName, config.BindPassword);
                    DirectoryEntry ouEntry = directoryEntry.Children.Find("cn=Users");
                    var childEntry = ouEntry.Children.Add($"cn={model.FirstName} {model.LastName}", "user");
                    childEntry.Properties["samAccountName"].Value = model.Username;
                    childEntry.Properties["mail"].Value = model.Email;
                    childEntry.Properties["givenName"].Value = model.FirstName;
                    childEntry.Properties["sn"].Value = model.LastName;
                    childEntry.Properties["streetAddress"].Value = model.Address;
                    childEntry.Properties["postalCode"].Value = model.PostalCode;
                    childEntry.Properties["l"].Value = model.PostalCode;
                    childEntry.Properties["St"].Value = model.State;
                    childEntry.Properties["userPrincipalName"].Value = $"{model.Username}@${config.Domain}";
                    childEntry.Properties["userAccountControl"].Value = 0x220;
                    childEntry.CommitChanges();
                    directoryEntry.CommitChanges();
                    childEntry.Invoke("SetPassword", new object[] { model.Password });
                    childEntry.CommitChanges();
                    // We really shouldn't be giving everyone Domain Admin
                    // Note to blue team: edit this to place users in the correct group
                    var userGroup = ouEntry.Children.Find("cn=Domain Admins");
                    string userPath = childEntry.Path;
                    userGroup.Invoke("Add", new object[] { userPath });
                    userGroup.CommitChanges();
                    return RedirectToAction("SignIn");
                }
                catch
                {
                    return RedirectToAction("SignUp");
                }
            }
            else
            {
                return RedirectToAction("SignUp");
            }
        }

        public IActionResult SignOut()
        {
            Response.Cookies.Delete("username");
            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}