using OA.Ultima.Core.Patterns.MVC;

namespace OA.Ultima.Login
{
    class LoginView : AView
    {
        protected new LoginModel Model => (LoginModel)base.Model;

        public LoginView(LoginModel model)
            : base(model) { }
    }
}
