﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebStore.Models;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using WebStore.Config;
using BCrypt.Net;

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
          using (
              DirectoryEntry rootEntry = new DirectoryEntry(
                  Configuration.GetSection("LDAP").GetValue<string>("server"),
                  Configuration.GetValue<string>("LDAP:bindUsername"),
                  Configuration.GetValue<string>("LDAP:bindPassword")
              )
          )
          {
            Console.WriteLine("LDAP is successfully bind");

            DirectorySearcher directorySearcher = new DirectorySearcher(rootEntry);
            var filter = "(&(sAMAccountName=" + signin.Username + "))";
            directorySearcher.Filter = filter;
            var searchResult = directorySearcher.FindOne();
           
            if (searchResult == null)
            {
              return RedirectToAction("SignIn");
            }

            var directoryEntry = searchResult.GetDirectoryEntry();


            bool isPasswordValid = false;
            using (var domainContext = new PrincipalContext(ContextType.Domain, config.Domain))
            {
                            Console.WriteLine("Before passworfd validation validation");
                isPasswordValid = domainContext.ValidateCredentials(signin.Username, signin.Password);
                Console.WriteLine("After password validtion");
            }
            
            if (!isPasswordValid)
            {

              return RedirectToAction("SignIn");
            }


            var groups = GetUserGroups(signin.Username);
                        if (groups != null)
                        {
                            

                            if (groups.Contains("Customers"))
                            {
                                
                                HttpContext.Response.Cookies.Append("role", "Customers", new CookieOptions
                                {
                                    Expires = DateTimeOffset.Now.AddDays(60),
                                    Path = "/",
                                    HttpOnly = false
                                });
                            }
                            Console.WriteLine(groups);
                        
                            
                            if (groups.Contains("Warehouse Manager"))
                            {
                              HttpContext.Response.Cookies.Append("role", "Warehouse Manager", new CookieOptions
                              {
                                Expires = DateTimeOffset.Now.AddDays(60),
                                Path = "/",
                                HttpOnly = false
                              });
                            }


                            if (groups.Contains("Warehouse Employees"))
                            {
                              HttpContext.Response.Cookies.Append("role", "Warehouse Manager", new CookieOptions
                              {
                                Expires = DateTimeOffset.Now.AddDays(60),
                                Path = "/",
                                HttpOnly = false
                              });
                            }


                            if (groups.Contains("Warehouse Employees"))
                            {
                              HttpContext.Response.Cookies.Append("role", "Warehouse Manager", new CookieOptions
                              {
                                Expires = DateTimeOffset.Now.AddDays(60),
                                Path = "/",
                                HttpOnly = false
                              });
                            }


                            if (groups.Contains("CEO"))
                            {
                              HttpContext.Response.Cookies.Append("role", "CEO", new CookieOptions
                              {
                                Expires = DateTimeOffset.Now.AddDays(60),
                                Path = "/",
                                HttpOnly = false
                              });
                            }


                            if (groups.Contains("SRE"))
                            {
                              HttpContext.Response.Cookies.Append("role", "SRE", new CookieOptions
                              {
                                Expires = DateTimeOffset.Now.AddDays(60),
                                Path = "/",
                                HttpOnly = false
                              });
                            }



                            if (groups.Contains("Accountants"))
                            {
                              HttpContext.Response.Cookies.Append("role", "Accountants", new CookieOptions
                              {
                                Expires = DateTimeOffset.Now.AddDays(60),
                                Path = "/",
                                HttpOnly = false
                              });
                            }

                            if (groups.Contains("Enterprise Admin"))
                            {
                              HttpContext.Response.Cookies.Append("role", "Enterprise Admin", new CookieOptions
                              {
                                Expires = DateTimeOffset.Now.AddDays(60),
                                Path = "/",
                                HttpOnly = false
                              });
                            }

                          }
                        
                HttpContext.Response.Cookies.Append(
                "username",
                signin.Username,
                new CookieOptions
                {
                  Expires = DateTimeOffset.Now.AddDays(60),
                  Path = "/",
                  HttpOnly = false
                }
            );
          }
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
            DirectoryEntry directoryEntry = new DirectoryEntry(
                config.Server,
                config.BindUserName,
                config.BindPassword
            );
            DirectoryEntry ouEntry = directoryEntry.Children.Find("cn=Users");
            var childEntry = ouEntry.Children.Add(
                $"cn={model.FirstName} {model.LastName}",
                "user"
            );
            childEntry.Properties["samAccountName"].Value = model.Username;
            childEntry.Properties["mail"].Value = model.Email;
            childEntry.Properties["givenName"].Value = model.FirstName;
            childEntry.Properties["sn"].Value = model.LastName;
            childEntry.Properties["streetAddress"].Value = model.Address;
            childEntry.Properties["postalCode"].Value = model.PostalCode;
            childEntry.Properties["l"].Value = model.PostalCode;
            childEntry.Properties["St"].Value = model.State;
            childEntry.Properties["userPrincipalName"].Value =
                $"{model.Username}@${config.Domain}";
            childEntry.Properties["userAccountControl"].Value = 0x220;
            childEntry.CommitChanges();
            directoryEntry.CommitChanges();

            // Hash the password
           // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Set the hashed password
            childEntry.Invoke("SetPassword", new object[] { model.Password });
            childEntry.CommitChanges();

            var userGroup = ouEntry.Children.Find("cn=Customers");
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
      return View(
          new ErrorViewModel
          {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
          }
      );
    }


    public List<string> GetUserGroups(string username)
    {
      List<string> groups = new List<string>();

      try
      {
        using (var context = new PrincipalContext(ContextType.Domain))
        {
          using (var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username))
          {
            if (userPrincipal != null)
            {
              var principalSearchResult = userPrincipal.GetAuthorizationGroups();

              foreach (var principal in principalSearchResult)
              {
                if (principal is GroupPrincipal groupPrincipal)
                {
                  groups.Add(groupPrincipal.Name);
                }
              }
            }
          }
        }
      }
      catch (PrincipalException ex)
      {
        Console.WriteLine("Error retrieving user groups: " + ex.Message);
      }

      return groups;
    }
  }
}
