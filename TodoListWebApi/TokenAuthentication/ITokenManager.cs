namespace ToDoListApi.TokenAuthentication
{
    public interface ITokenManager
    {
        bool Authenticate(string Username, string Password);
        bool CheckUserInfoInDataBase(string Username, string password);
        Token NewToken(string User);
        bool VerifyToken(string token);
        public bool RefreshTokenCheck(string User,string Refreshtoken);
    }
}