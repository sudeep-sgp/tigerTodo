using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TigerTodoAp.Services;
using TigerTodoAp.DTOs;
using MongoDB.Bson;

namespace TigerTodoAp.Controllers
{

    public class HomeController : Controller
    {
        public TodoServices<TodoDTO> todoServices = new TodoServices<TodoDTO>();

        public ActionResult Index()
        {
            ViewBag.Title = "Tiger Home";

            //return Json(todoServices.GetAllTodos(), JsonRequestBehavior.AllowGet);  
            return View();

        }


        public ActionResult Todos()
        {
            ViewBag.Title = "Tiger Home";

            return View();

        }
        [HttpGet]
        public ActionResult AllTodos()
        {

            return Json(todoServices.GetAllTodos(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult Confirm()
        {
            return View();
        }

        public ActionResult Pagination()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(TodoDTO todoModal)
        {
            //todoModal.created = DateTime.Parse(todoModal.created.ToString("d MMM, yyyy"));
            //todoModal.lastModified = DateTime.Parse(todoModal.lastModified.ToString("d MMM, yyyy"));
            var savedTodo = todoServices.SaveNewTodo(todoModal);

            var created = DateTime.Parse(savedTodo.created).ToString("d MMM, yyyy");
            var lastModified = DateTime.Parse(savedTodo.lastModified).ToString("d MMM, yyyy");
            var jsonTodoObj = new
            {
                id = savedTodo.id,
                isSelected = savedTodo.isSelected,
                lastModified = lastModified,
                created = created,
                todoName = savedTodo.todoName,
                todoNote = savedTodo.todoNote
            };

            return Json(jsonTodoObj);

        }

        [HttpPost]
        public ActionResult Update(TodoDTO todoModal)
        {
            var updatedTodo = todoServices.SaveNewTodo(todoModal);
            var created = DateTime.Parse(todoModal.created).ToString("d MMM, yyyy");
            var lastModified = DateTime.Parse(todoModal.lastModified).ToString("d MMM, yyyy");
            var jsonTodoObj = new
            {
                id = todoModal.id,
                isSelected = todoModal.isSelected,
                lastModified = lastModified,
                created = todoModal.created,
                todoName = todoModal.todoName,
                todoNote = todoModal.todoNote
            };

            return Json(jsonTodoObj);
        }



        [HttpPost]
        public ActionResult Delete(string Id)
        {
            var isDeleted = todoServices.DeleteDoneTodos(Id);

            return Json(new { id = Id, success = isDeleted });

        }

        [HttpPost]
        public ActionResult DeleteAll()
        {
            var isDeleted = todoServices.DeleteDoneTodos();

            return Json(new { success = isDeleted });

        }

        [HttpPost]
        public ActionResult SwitchStat(string id)
        {
            var isSwitched = todoServices.SwitchDoneStatus(id);

            return Json(new { id = id, success = isSwitched });

        }

        public ActionResult DoneAll()
        {
            var isDoneAll = todoServices.DoneAllTodos();

            return Json(new { success = isDoneAll });

        }


        public ActionResult UnDoneAll()
        {
            var isUnDoneAll = todoServices.UnDoneAllTodos();

            return Json(new { success = isUnDoneAll });

        }

        public ActionResult AllTodos(string Id)
        {
            var allTodos = todoServices.GetAllTodos();
            return Json(new { allTodos = allTodos });

        }

    }
}
