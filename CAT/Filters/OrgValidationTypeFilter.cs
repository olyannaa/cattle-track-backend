using CAT.Filters;
using Microsoft.AspNetCore.Mvc;

public class OrgValidationTypeFilter: TypeFilterAttribute
{
    public OrgValidationTypeFilter(bool checkAdmin = default, bool checkOrg = default)
     : base(typeof(OrgValidationFilter))
    {
        Arguments = new object[] { checkAdmin, checkOrg };
    }
}