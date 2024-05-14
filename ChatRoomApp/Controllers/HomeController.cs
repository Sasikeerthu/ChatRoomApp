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
                string escapedName = cr.name.Replace("'", "''");
                string escapedPassword = cr.password.Replace("'", "''");
                string escapedConfirmPassword = cr.Confirm_password.Replace("'", "''");

                // Check if the username already exists in the database
                string checkUsernameQuery = $"SELECT COUNT(*) FROM [ChatRoomApp].[dbo].[SignUp_Credential_Validation] WHERE Name = '{escapedName}'";
                int existingUsernameCount = (int)DbHelper.ExecuteScalar(checkUsernameQuery);

                // Check if the password has been used before
                string checkPasswordQuery = $"SELECT COUNT(*) FROM [ChatRoomApp].[dbo].[SignUp_Credential_Validation] WHERE Password = '{escapedPassword}'";
                int existingPasswordCount = (int)DbHelper.ExecuteScalar(checkPasswordQuery);

                if (existingUsernameCount == 0 && existingPasswordCount == 0)
                {
                    // Username and password are unique, proceed with insertion
                    // Construct the SQL query with case-sensitive comparison
                    string insertQuery = $"INSERT INTO [ChatRoomApp].[dbo].[SignUp_Credential_Validation] (Name, Password, Confirm_Password) VALUES ('{escapedName}' COLLATE Latin1_General_CS_AS, '{escapedPassword}' COLLATE Latin1_General_CS_AS, '{escapedConfirmPassword}' COLLATE Latin1_General_CS_AS)";
                    DbHelper.ExecuteNonQuery(insertQuery);

                    // Store username in session
                    HttpContext.Session.SetString("UserName", cr.name);

                    return RedirectToAction("SignIn_To_ChatRoom");
                }
                else
                {
                    // Either username or password (or both) already exists, handle accordingly (e.g., display error message to user)
                    if (existingUsernameCount > 0)
                    {
                        ModelState.AddModelError("", "Username is already taken. Please choose a different username.");
                    }
                    if (existingPasswordCount > 0)
                    {
                        ModelState.AddModelError("", "Password has been used before. Please choose a different password.");
                    }
                }
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

            string query = $"SELECT * FROM SignUp_Credential_Validation WHERE Name COLLATE Latin1_General_CS_AS = '{escapedName}' AND Password COLLATE Latin1_General_CS_AS = '{escapedPassword}'";

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
