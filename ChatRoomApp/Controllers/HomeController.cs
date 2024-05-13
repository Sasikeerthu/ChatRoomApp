using ChatRoomApp.Models;
using ChatRoomApp.Models.Database;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace ChatRoomApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly MessageRepository _messageRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }
       
        public IActionResult SignUp_To_ChatRoom() {

            return View();
        }

        
        [HttpPost]
       
        public IActionResult SignUp_To_ChatRoom(Registration_Credentials cr)
        {
            if (ModelState.IsValid)
            {
                string query = $"insert into [ChatRoomApp].[dbo].[SignUp_Credential_Validation] (Name,Password ,Confirm_Password ) values('{cr.name}','{cr.password}','{cr.Confirm_password}') ";
                DataTable dataTable = DbHelper.ExecuteQuery(query);
                // Store username in session
                HttpContext.Session.SetString("UserName", cr.name);

                return RedirectToAction("SignIn_To_ChatRoom");
            }
            return View();
        }
        public IActionResult SignIn_To_ChatRoom()
        {

            return View();
        }

       
        [HttpPost]
       
        public IActionResult SignIn_To_ChatRoom(Login_Credentials model)

        {

            if (ModelState.IsValid)

            {
                if (!string.IsNullOrEmpty(model.name) && !string.IsNullOrEmpty(model.password))
                {

                    if (IsValidUser(model.name, model.password))
                    {
                        string userName = model.name; // Get the username from the form

                        // using this to avoid unnessary redirections
                        HttpContext.Session.SetString("LoggedIn", "true");
                        // Store username in session
                        HttpContext.Session.SetString("UserName", model.name);
                        // Redirect to chat room or perform any other action
                        return RedirectToAction("ChatRoom");
                      

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid username or password");
                        //return RedirectToAction("login");
                    }

                }
            }


            return View();
        }
        private bool IsValidUser(string name, string password)

        {
            string escapedName = name.Replace("'", "''");
            string escapedPassword = password.Replace("'", "''");

            string query = $"SELECT * FROM SignUp_Credential_Validation WHERE Name = '{escapedName}' AND Password = '{escapedPassword}'";
            DataTable dataTable = DbHelper.ExecuteQuery(query);
            if (dataTable.Rows.Count > 0)
            {
                return true;

            }

            return false;

        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ChatRoom()
        {
            if (HttpContext.Session.GetString("LoggedIn") == "true")
            {
                var messages = _messageRepository.GetMessages();
                var userName = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = userName;
            return View(messages);

               
            }
            else
            {
                return RedirectToAction("SignIn_To_ChatRoom");
            }
        }



        [HttpPost]
        public IActionResult SendMessage(Message message)
        {
            if (ModelState.IsValid)
            {
                string? userName = HttpContext.Session.GetString("UserName");
                message.SenderName = userName;
                message.Timestamp = DateTime.Now;
                _messageRepository.AddMessage(message);
            }
            return RedirectToAction("ChatRoom");
        }
    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
