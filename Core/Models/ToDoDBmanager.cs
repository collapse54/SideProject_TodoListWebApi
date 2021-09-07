using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Core.Models
{
    public class ToDoDBmanager : IToDoDBmanager
    {
        private readonly IConfiguration _configuration;
        SqlConnection ToDoListDB;
        public ToDoDBmanager(IConfiguration configuration)
        {
            _configuration = configuration;
            ToDoListDB = new SqlConnection(configuration.GetConnectionString("ToDoListDBConnection"));
        }

        public List<Thing> GetThing(string UserName)
        {
            List<Thing> things = new List<Thing>();

            string select = @"SELECT * FROM ToDo WHERE UserId=(SELECT UserId FROM UserInfo WHERE UserName=@UserName)";
            SqlCommand SearchCommand = new SqlCommand(select, ToDoListDB);
            SearchCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;
            try
            {
                ToDoListDB.Open();
                SqlDataReader SqlData = SearchCommand.ExecuteReader();
                if (SqlData.HasRows)
                {
                    while (SqlData.Read())
                    {
                        Thing thing = new Thing
                        {
                            Title = SqlData.GetString(SqlData.GetOrdinal("Title")),
                            Description = SqlData.GetString(SqlData.GetOrdinal("Description")),
                            Finish = SqlData.GetInt32(SqlData.GetOrdinal("Finish")),
                            AddDate = SqlData.GetDateTime(SqlData.GetOrdinal("AddDate"))
                        };
                        things.Add(thing);
                    }
                }
                SqlData.Close();
                ToDoListDB.Close();
            }
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return things;
        }
        public bool NewThing(Thing thing,string UserName)
        {
            string insertinto = @"INSERT INTO ToDo (UserId, Title , Description , AddDate , Finish , Recycle)
                        VALUES ((SELECT UserId FROM UserInfo WHERE UserName=@UserName) , @Title , @Description , @AddDate , @Finish , @Recycle )";
            SqlCommand insert = new SqlCommand(insertinto, ToDoListDB);
            insert.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;
            insert.Parameters.Add("@Title", SqlDbType.NVarChar).Value = thing.Title;
            insert.Parameters.Add("@Description", SqlDbType.NVarChar).Value = thing.Description;
            insert.Parameters.Add("@AddDate", SqlDbType.Date).Value = DateTime.Today; 
            insert.Parameters.Add("@Finish", SqlDbType.Int).Value = 0;
            insert.Parameters.Add("@Recycle", SqlDbType.Int).Value = 0;

            try
            {
                ToDoListDB.Open();
                insert.ExecuteNonQuery();
                ToDoListDB.Close();
                return true;
            }
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return false;
        }
        public bool ChangeThingByTitle(Thing thing, string UserName)
        {
            string UPDATE = @"UPDATE ToDo SET 
                                Description = @Description,AddDate = @AddDate,Finish = @Finish, Recycle = @Recycle Where
                                ListId = (Select ListId From ToDo where
                                UserId = (SELECT UserId FROM UserInfo WHERE UserName = @UserName) AND Title = @Title)";
            //ListId 條件=> 用UserName找UserId then 在 ToDo找匹配Title跟UserId的ListId
            SqlCommand update = new SqlCommand(UPDATE, ToDoListDB);
            update.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;
            update.Parameters.Add("@Title", SqlDbType.NVarChar).Value = thing.Title;
            update.Parameters.Add("@Description", SqlDbType.NVarChar).Value = thing.Description;
            update.Parameters.Add("@AddDate", SqlDbType.Date).Value = DateTime.Today;
            update.Parameters.Add("@Finish", SqlDbType.Int).Value = 0;
            update.Parameters.Add("@Recycle", SqlDbType.Int).Value = 0;

            try
            {
                ToDoListDB.Open();
                update.ExecuteNonQuery();
                ToDoListDB.Close();
                return true;
            }
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return false;
        }
        public bool RecycleByTitle(RecycleThing title, string UserName)
        {
            string DELETE = @"DELETE FROM Todo Where
                                ListId = (Select ListId From ToDo where
                                UserId = (SELECT UserId FROM UserInfo WHERE UserName = @UserName) AND Title = @Title)";
            SqlCommand delete = new SqlCommand(DELETE, ToDoListDB);
            delete.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;
            delete.Parameters.Add("@Title", SqlDbType.NVarChar).Value =title.Title;
            try
            {
                ToDoListDB.Open();
                delete.ExecuteNonQuery();
                ToDoListDB.Close();
                return true;
            }
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return false;
        }
    }
}
