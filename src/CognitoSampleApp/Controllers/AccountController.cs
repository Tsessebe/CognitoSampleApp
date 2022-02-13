using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using CognitoSampleApp.Models;
using CognitoSampleApp.Models.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CognitoSampleApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAmazonCognitoIdentityProvider identityProvider;
        private readonly CognitoUserPool userPool;
        private readonly AwsCognitoConfig cognitoConfig;

        public AccountController(IAmazonCognitoIdentityProvider identityProvider, CognitoUserPool userPool, AwsCognitoConfig cognitoConfig)
        {
            this.identityProvider = identityProvider;
            this.userPool = userPool;
            this.cognitoConfig = cognitoConfig;
        }

        [HttpPost]
        [Route("login")]
        public async Task<TokenResponseModel> LoginAsync([FromBody]UserLoginModel model)
        {
            var user = new CognitoUser(
                model.Email,
                this.cognitoConfig.UserPoolClientId,
                this.userPool,
                this.identityProvider,
                clientSecret: this.cognitoConfig.UserPoolClientSecret
            );
            var authRequest = new InitiateSrpAuthRequest()
            {
                Password = model.Password,
            };
            var authResponse = await user.StartWithSrpAuthAsync(authRequest);

            if (authResponse.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
            {
                throw new Exception("Update your temporary password.");
            }

            var timespan = TimeSpan.FromSeconds(authResponse.AuthenticationResult.ExpiresIn);
            var expiry = DateTimeOffset.UtcNow + timespan;

            return new TokenResponseModel
            {
                RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                Token = authResponse.AuthenticationResult.AccessToken,
                Expiry = expiry,
            };
        }

        [HttpPost]
        [Route("updatepassword")]
        public async Task<string> ChangePasswordAsync([FromBody]UpdatePasswordModel model)
        {
            var user = new CognitoUser(
                model.Email,
                this.cognitoConfig.UserPoolClientId,
                this.userPool,
                this.identityProvider,
                clientSecret: this.cognitoConfig.UserPoolClientSecret
            );
            var authRequest = new InitiateSrpAuthRequest()
            {
                Password = model.Password,
            };
            var authResponse = await user.StartWithSrpAuthAsync(authRequest);
            var response = await user.RespondToNewPasswordRequiredAsync( new RespondToNewPasswordRequiredRequest
            {
                SessionID = authResponse.SessionID,
                NewPassword = model.NewPassword,
            });

            return "Password changed successfully.";
        }
    }
}
