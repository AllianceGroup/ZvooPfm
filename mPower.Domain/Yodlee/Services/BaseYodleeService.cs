using System;
using com.yodlee.common;
using com.yodlee.core.login;
using com.yodlee.ext.login;
using com.yodlee.soap.core.login;

namespace mPower.Domain.Yodlee.Services
{
    public class BaseYodleeService
    {
        protected CobrandContext _cobrandContext { get; set; }
        protected bool IsConnected { get; set; }

        protected void ConnectToYodlee()
        {
            _cobrandContext = new CobrandContext();

            try
            {
                var passwordCredentials = new CobrandPasswordCredentials
                                              {
                                                  password = YodleeSettings.CobrandPassword,
                                                  loginName = YodleeSettings.CobrandUsername,
                                              };

                System.Environment.SetEnvironmentVariable("com.yodlee.soap.services.url", YodleeSettings.SoapUrl);
                var cobrandLoginProxy = new CobrandLoginService();

                _cobrandContext = cobrandLoginProxy.loginCobrand(YodleeSettings.CobrandId, YodleeSettings.ApplicationId,
                                                                 YodleeSettings.Locale, YodleeSettings.TncVersion,
                                                                 passwordCredentials);
                IsConnected = true;
            }
            catch(Exception e)
            {
                throw new Exception("Error in Yodlee connection. Message :" + e.Message);
            }
        }

        protected UserInfo LoginUser(string username, string password)
        {
            var credentials = new PasswordCredentials
                                  {
                                      loginName = username,
                                      password = password
                                  };

            var loginService = new LoginService();
            return loginService.login(_cobrandContext, credentials);
        }
    }
}