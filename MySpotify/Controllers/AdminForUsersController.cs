using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySpotify.Models;
using MySpotify.Repository;

namespace MySpotify.Controllers
{
    public class AdminForUsersController : Controller
    {
        private readonly IRepositoryMedia _contextMedia;
        private readonly IRepositoryUser _contextUser;
       


        
        public AdminForUsersController(IRepositoryMedia context, IRepositoryUser repositoryUser)
        {
            _contextMedia = context;
            _contextUser = repositoryUser;
           
        }



        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public  ActionResult GetUserList()
        {
           var userCollection = _contextUser.GetUserList();

           
            var config = new MapperConfiguration(cnf => cnf.CreateMap<User, UsersForMapping>());
            var mapper = new Mapper(config);
            var res = mapper.Map<IEnumerable<User>, IEnumerable<UsersForMapping>>(userCollection.Result);

            return Json(res) ;
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _contextUser.GetUserList().Result.FirstOrDefault(i => i.id == id);
            if (user == null)
            {
                return NotFound();
            }
          
            return PartialView("EditAccount", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _contextUser.UpdateUser(user);
                    await _contextUser.SaveDb();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return Ok();
            }
            return NotFound();
        }


        public ActionResult DeleteAcc(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _contextUser.GetUserList().Result.FirstOrDefault(i => i.id == id);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("DeleteAcc", user);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAcc(int id, User user)
        {


            if (id != user.id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                   await _contextUser.DeleteUser(id);
                   await _contextUser.SaveDb();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_contextUser.GetUserList().Result.Any(x=>x.id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return NotFound();
        }




    }
}
