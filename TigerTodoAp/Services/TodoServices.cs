using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TigerTodoAp.Properties;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using System.Web;
using System.Web.Mvc;
using TigerTodoAp.DTOs;

namespace TigerTodoAp.Services
{
    public class TodoServices<T> : ITodoServices<T> where T : TodoDTO
    {
        public MongoDatabase MongoTigerDB;
        public MongoCollection TodosCollections;
        public bool isDBReady = true;

        public TodoServices()
        {
            var mongoClient = new MongoClient(Settings.Default.TodoMongoConnectionStr);
            var mongServer = mongoClient.GetServer();
            MongoTigerDB = mongServer.GetDatabase(Settings.Default.Database);
            TodosCollections = MongoTigerDB.GetCollection<T>(typeof(T).Name.ToLower() + "Tb");
            try
            {
                MongoTigerDB.Server.Ping();
            }
            catch (Exception e)
            {
                isDBReady = false;
            }
        }

        private List<T> todosList = new List<T>();

        public T SaveNewTodo(T entity)
        {
            entity.id = ObjectId.GenerateNewId().ToString();
            entity.created = DateTime.UtcNow.ToString("d MMM, yyyy");
            entity.lastModified = DateTime.UtcNow.ToString("d MMM, yyyy");

            TodosCollections.Save(entity);

            return entity;
        }

        public virtual bool DeleteDoneTodos(string id)
        {
            WriteConcernResult result = TodosCollections.Remove(Query.EQ("_id", id));
            return result.DocumentsAffected == 1;
        }

        public virtual bool DeleteDoneTodos()
        {
            WriteConcernResult result = TodosCollections.Remove(Query.EQ("isSelected", true));
            return result.DocumentsAffected > 0;
        }

        public T UpdateTodo(T entity)
        {
            var query = Query.EQ("_id", entity.id);
            entity.lastModified = DateTime.UtcNow.ToString("d MMM, yyyy");
            UpdateBuilder updateBuilder = MongoDB.Driver.Builders.Update               
               .Set("isSelected", entity.isSelected)
               .Set("todoName", entity.todoName)
               .Set("todoNote", entity.todoNote)
               .Set("lastModified", entity.lastModified);
            WriteConcernResult result = TodosCollections.Update(query, updateBuilder);


            return entity;

        }

        public virtual bool SwitchDoneStatus(string id)
        {
            var query = Query.EQ("_id", id);
            var selectedTodoList = TodosCollections.FindAs(typeof(T), query);

            UpdateBuilder updateBuilder;
            foreach (T todo in selectedTodoList)
            {
                if (todo.isSelected == true)
                {
                    updateBuilder = MongoDB.Driver.Builders.Update
                    .Set("isSelected", false);
                    TodosCollections.Update(Query.EQ("_id", id), updateBuilder);
                }
                else
                {
                    updateBuilder = MongoDB.Driver.Builders.Update
                    .Set("isSelected", true);
                    TodosCollections.Update(Query.EQ("_id", id), updateBuilder);
                }
            }


            return true;
        }

        public virtual bool DoneAllTodos()
        {

            bool isUpdated = false;

            var allTodos = TodosCollections.FindAs(typeof(T), Query.EQ("isSelected", false));
            if (allTodos.Count() > 0)
            {
                foreach (T todo in allTodos)
                {
                    if (todo.isSelected != true)
                    {
                        todo.isSelected = true;
                    }
                    WriteConcernResult result = TodosCollections.Save(todo);
                    isUpdated = result.UpdatedExisting;
                }
            }

            return isUpdated;
        }

        public virtual bool UnDoneAllTodos()
        {
            bool isUpdated = false;

            var allTodos = TodosCollections.FindAs(typeof(T), Query.EQ("isSelected", true));
            if (allTodos.Count() > 0)
            {
                foreach (T todo in allTodos)
                {
                    if (todo.isSelected != false)
                    {
                        todo.isSelected = false;
                    }
                    WriteConcernResult result = TodosCollections.Save(todo);
                    isUpdated = result.UpdatedExisting;
                }
            }

            return isUpdated;
        }

        public IEnumerable<T> GetAllTodos()
        {
            if (isDBReady && Convert.ToInt32(TodosCollections.Count()) > 0)
            {
                todosList.Clear();
                var allTodos = TodosCollections.FindAs(typeof(T), Query.NE("_id", "null"));
                if (allTodos.Count() > 0)
                {
                    foreach (T todo in allTodos)
                    {
                        todo.created = DateTime.Parse(todo.created).ToString("d MMM, yyyy");
                        todo.lastModified = DateTime.Parse(todo.lastModified).ToString("d MMM, yyyy");
                        todosList.Add(todo);
                    }
                }

            }
            else
            {
                return null;

            }

            var result = todosList.AsQueryable<T>();
            return result;

        }


    }
}