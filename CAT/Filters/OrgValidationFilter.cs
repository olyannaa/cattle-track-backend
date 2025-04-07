using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CAT.Filters
{
    public class OrgValidationFilter : Attribute, IActionFilter
    {
        private readonly Guid _localAdminId;
        private readonly bool _checkOrg;
        private readonly bool _checkAdmin;

        public OrgValidationFilter(IConfiguration config, bool checkAdmin = default, bool checkOrg = default)
        {
            _checkAdmin = checkAdmin;
            _checkOrg = checkOrg;
            _localAdminId = config.GetValue<Guid>("Enviroment:LocalAdminId");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (_checkOrg)
            {
                var responseOrgId = context.HttpContext.Request.Headers["OrganizationId"].ToString();
                if (responseOrgId is null || responseOrgId == String.Empty)
                {
                    context.Result = GetOrgIdHeaderResult();
                    return;
                }

                var authOrgId = GetUserClaims(context).Find(x => x.Type == "Organization")?.Value;
                if(responseOrgId != authOrgId)
                {
                    context.Result = GetOrgIdNotEqualsResult();
                    return;
                } 
            }

            if (_checkAdmin)
            {
                if (_localAdminId == Guid.Empty)
                {
                    context.Result = GetNotImplementedResult();
                    return;
                }
                var userRoleId = GetUserClaims(context).Find(x => x.Type == ClaimTypes.Role)?.Value;
                if (!Guid.TryParse(userRoleId, out var roleId) || roleId != _localAdminId)
                {
                     context.Result = GetNotLocalAdminResult();
                     return;
                }   
            }
        }

        public void OnActionExecuted(ActionExecutedContext context){}

        private List<Claim> GetUserClaims(ActionExecutingContext context)
        {
            return context.HttpContext.User.Claims.ToList();
        }

        private ContentResult GetOrgIdHeaderResult()
        {
            return new ContentResult{ 
                StatusCode=403,
                Content="Для выполнения запроса необходимо указать id организации в заголовке"
            };
        }

        private ContentResult GetOrgIdNotEqualsResult()
        {
            return new ContentResult{ 
                StatusCode=403,
                Content="Нет доступа к информации чужой организации"
            };
        }

        private ContentResult GetNotLocalAdminResult()
        {
            return new ContentResult{ 
                StatusCode=403,
                Content="Не достаточно прав внутри организации"
            };
        }

        private ContentResult GetNotImplementedResult()
        {
            return new ContentResult{ 
                StatusCode=500,
                Content="ID локального администратора не указан в конфигурационном файле"
            };
        }
    }
}
