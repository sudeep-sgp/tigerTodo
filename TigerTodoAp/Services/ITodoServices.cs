using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TigerTodoAp.DTOs;

namespace TigerTodoAp.Services
{
    public interface ITodoServices<T> where T : TodoDTO
    {
        T SaveNewTodo(T entity);
        bool DeleteDoneTodos(string id);
        bool DeleteDoneTodos();
        T UpdateTodo(T entity);
        bool SwitchDoneStatus(string id);
        bool DoneAllTodos();
        bool UnDoneAllTodos();
        IEnumerable<T> GetAllTodos();
    }
  
}
