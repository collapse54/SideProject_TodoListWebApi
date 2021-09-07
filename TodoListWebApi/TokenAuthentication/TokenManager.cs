using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace ToDoListApi.TokenAuthentication
{
    public class TokenManager : ITokenManager
    {
        private List<Token> listTokens;
        private readonly IConfiguration _configuration;
        SqlConnection ToDoListDB;
        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
            listTokens = new List<Token>();
            ToDoListDB = new SqlConnection(_configuration.GetConnectionString("ToDoListDBConnection"));
        }
        public bool Authenticate(string Username, string Password)
        {
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) &&
                CheckUserInfoInDataBase(Username, Password))
                return true;
            else
                return false;
        }
        public bool CheckUserInfoInDataBase(string Username, string password)
        {
            //測試用
            if (Username.ToLower() == "admin" && password == "password") return true;

            string Select = @"SELECT passwordHash FROM UserInfo where UserName =@Username";
            SqlCommand SearchCommand = new SqlCommand(Select, ToDoListDB);
            SearchCommand.Parameters.Add("@Username", SqlDbType.VarChar).Value = Username;
            try
            {
                ToDoListDB.Open();
                SqlDataReader SqlData = SearchCommand.ExecuteReader();
                if (SqlData.HasRows && SqlData.Read())
                {
                    string  passwordHash = SqlData["passwordHash"].ToString();
                    SqlData.Close();
                    ToDoListDB.Close();
                    return (BCrypt.Net.BCrypt.Verify(password, passwordHash));
                }
                SqlData.Close();
                ToDoListDB.Close();
                return false;
            }
            //輸出SQL有關的錯誤訊息 &其他錯誤訊息
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
            
            return false;
        }
        
        public Token NewToken(string User)
        {
            //生成新TOKEN
            var token = new Token
            { //GUID 輸出 36字元 資料庫裡的類型為nchar(36)
                TokenValue = Guid.NewGuid().ToString(),
                RefreshTokenValue = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.Now.AddMinutes(10)
            };
            //add new token in list
            listTokens.Add(token);
            //測試用 只存LIST 不存資料庫
            if(User.ToLower() == "admin")
            {
                return token;
            }
            //add Token Value in datebase
            string update = @"UPDATE UserInfo SET TokenValue=@TokenValue,RefreshTokenValue=@RefreshTokenValue";
            SqlCommand SearchCommand = new SqlCommand(update , ToDoListDB);
            SearchCommand.Parameters.Add("@TokenValue", SqlDbType.VarChar).Value = token.TokenValue;
            SearchCommand.Parameters.Add("@RefreshTokenValue", SqlDbType.VarChar).Value = token.RefreshTokenValue;
            try
            {
                ToDoListDB.Open();
                SearchCommand.ExecuteNonQuery();
                ToDoListDB.Close();
            }
            //輸出SQL有關的錯誤訊息 &其他錯誤訊息
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return token;
        }

        public bool VerifyToken(string token)
        {
            if (listTokens.Any(x => x.TokenValue == token && x.ExpiryDate > DateTime.Now))
                return true;
            else
                return false;
        }
        public bool RefreshTokenCheck(string User,string Refreshtoken)
        {
            
            if (listTokens.Any(x => x.RefreshTokenValue == Refreshtoken && x.ExpiryDate > DateTime.Now))
            {
                ///刪除的部分之後要改寫成非同步
                listTokens.RemoveAll(x => x.RefreshTokenValue == Refreshtoken || x.ExpiryDate < DateTime.Now);
                return true;
            }
            else
            {   
                listTokens.RemoveAll(x => x.ExpiryDate < DateTime.Now);
                return false;
            }
        }
    }
}
