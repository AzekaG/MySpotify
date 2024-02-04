using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySpotify.Models;
using MySpotify.Repository;
using NuGet.Protocol.Core.Types;

namespace MySpotify.Controllers
{
    public class UsersController : Controller
    {
        private readonly IRepositoryUser _context;
   
        public UsersController(IRepositoryUser context)
        {
            _context = context;

        }

    
        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (!isLogged())
                return RedirectToAction("Login");
            return View();
        }

        void AddMediaToPlaylist(Media media)
        {
          var us =  _context.GetUserList().Result.Where(x=>x.id == int.Parse(HttpContext.Session.GetString("Id"))).FirstOrDefault();
            us.MediaFiles.Add(media);
            _context.SaveDb();
        }

        // GET: Users/Details/5
        public ActionResult Login()
        {
            return View();
        }

        bool isLogged()
        {
            if (HttpContext.Session.GetString("Id")!=null)
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel logon)
        {
            if (ModelState.IsValid)
            {

                if (_context.GetUserList().Result.Count == 0)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return View(logon);
                }
                var users = _context.GetUserList().Result.Where(a => a.Email == logon.Email);

                if (users.ToList().Count == 0)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return View(logon);
                }
                
                var user = users.First();
                if(!user.isActive())
                {
                    ModelState.AddModelError("", "Ожидание активации пользователя");
                    return View(logon);
                }
                string? salt = user.Salt;

                //переводим пароль в байт-массив  
                byte[] password = Encoding.Unicode.GetBytes(salt + logon.Password);

                //создаем объект для получения средств шифрования  
                var md5 = MD5.Create();

                //вычисляем хеш-представление в байтах  
                byte[] byteHash = md5.ComputeHash(password);

                StringBuilder hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));

                if (user.Password != hash.ToString())
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return View(logon);
                }
                
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);
                HttpContext.Session.SetString("Id", user.id.ToString());
                return RedirectToAction("Index", "Media");
            }

            return View(logon);
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(RegistrationModel reg)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.FirstName = reg.FirstName;
                user.LastName = reg.LastName;
                user.Email = reg.Email;

                byte[] saltbuf = new byte[16];

                RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(saltbuf);

                StringBuilder sb = new StringBuilder(16);
                for (int i = 0; i < 16; i++)
                    sb.Append(string.Format("{0:X2}", saltbuf[i]));
                string salt = sb.ToString();

                //переводим пароль в байт-массив  
                byte[] password = Encoding.Unicode.GetBytes(salt + reg.Password);

                //создаем объект для получения средств шифрования  
                var md5 = MD5.Create();

                //вычисляем хеш-представление в байтах  
                byte[] byteHash = md5.ComputeHash(password);

                StringBuilder hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));

                user.Password = hash.ToString();
                user.Salt = salt;
                _context.CreateUser(user);
                _context.SaveDb();
                return RedirectToAction("Login");
            }

            return View(reg);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userList = await _context.GetUserList();
            var user = userList.FirstOrDefault(x=>x.id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,FirstName,LastName,Email,Password,Salt,Status")] User user)
        {
            if (ModelState.IsValid)
            {

               await _context.CreateUser(user);       
               await _context.SaveDb();
               return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userList = _context.GetUserList().Result;
            var user = userList.FirstOrDefault(x=>x.id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,FirstName,LastName,Email,Password,Salt,Status")] User user)
        {
            if (id != user.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.UpdateUser(user);
                    await _context.SaveDb();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userList = _context.GetUserList().Result;
            var user = userList.FirstOrDefault(x => x.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userList = _context.GetUserList().Result;
            var user = userList.FirstOrDefault(x => x.id == id);

            
            if (user != null)
            {
                await _context.DeleteUser(user.id);
            }

            await _context.SaveDb();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            var userList = _context.GetUserList().Result;
            return userList.Any(x => x.id == id);
        }

        
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }
}
