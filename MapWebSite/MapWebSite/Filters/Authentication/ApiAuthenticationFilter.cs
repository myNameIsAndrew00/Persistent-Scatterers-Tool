using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Linq;
using System.Web.Http.Results;

namespace MapWebSite.Filters
{
    public class ApiAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        private SiteAuthenticationFilter siteFilter = null;

        public ApiAuthenticationFilter()
        {
            siteFilter = new SiteAuthenticationFilter();
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {

            CookieHeaderValue cookie = context.Request.Headers.GetCookies(AuthenticationHandler.authenticationCookieName).FirstOrDefault();

            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Thread.CurrentPrincipal = AuthenticationHandler.CheckCookie(cookie[AuthenticationHandler.authenticationCookieName].Value);
                    HttpContext.Current.User = Thread.CurrentPrincipal;
                }
                catch
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}