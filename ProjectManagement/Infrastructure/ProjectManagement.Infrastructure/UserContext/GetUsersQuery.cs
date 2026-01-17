using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.Infrastructure.UserContext;

public sealed record GetUsersQuery(
    Guid? Id = null,
    string? Login = null,
    string? Email = null,
    string? Phone = null,
    string? Status = null,
    string? OrderMode = null,
    int? PageSize = null,
    int? Page = null,
    IEnumerable<string>? SortFields = null)
{
    public IQueryable<User> ApplyFilters(IQueryable<User> queryable)
    {
        queryable = ApplyLoginFilter(queryable);
        queryable = ApplyEmailFilter(queryable);
        queryable = ApplyPhoneFilter(queryable);
        queryable = ApplyStatusFilter(queryable);
        queryable = ApplySorting(queryable);
        queryable = ApplyPagination(queryable);
        queryable = ApplyIdFilter(queryable);
        return queryable;
    }

    private IQueryable<User> ApplyLoginFilter(IQueryable<User> queryable)
    {
        return string.IsNullOrWhiteSpace(Login) 
            ? queryable 
            : queryable.Where(u => u.AccountData.Login == Login);
    }

    private IQueryable<User> ApplyEmailFilter(IQueryable<User> queryable)
    {
        return string.IsNullOrWhiteSpace(Email) 
            ? queryable 
            : queryable.Where(u => u.AccountData.Email == Email);
    }

    private IQueryable<User> ApplyPhoneFilter(IQueryable<User> queryable)
    {
        return string.IsNullOrWhiteSpace(Phone)
            ? queryable
            : queryable.Where(u => u.PhoneNumber.Phone == Phone);
    }

    private IQueryable<User> ApplyStatusFilter(IQueryable<User> queryable)
    {
        return string.IsNullOrWhiteSpace(Status)
            ? queryable
            : queryable.Where(u => u.Status.Name == Status);
    }

    public IQueryable<User> ApplyIdFilter(IQueryable<User> queryable)
    {
        return Id.HasValue
            ? queryable.Where(u => u.UserId.Value == Id.Value)
            : queryable;
    }
    
    private IQueryable<User> ApplySorting(IQueryable<User> queryable)
    {
        if (string.IsNullOrWhiteSpace(OrderMode))
            return queryable;

        if (SortFields == null || !SortFields.Any())
            return queryable;

        foreach (string sortField in SortFields)
        {
            queryable = OrderMode switch
            {
                "ASC" => sortField switch
                {
                    "login" => queryable.OrderBy(u => u.AccountData.Login),
                    "email" => queryable.OrderBy(u => u.AccountData.Email),
                    "phone" => queryable.OrderBy(u => u.PhoneNumber.Phone),
                    "status" => queryable.OrderBy(u => u.Status.Name),
                    _ => queryable
                },
                "DESC" => sortField switch
                {
                    "login" => queryable.OrderByDescending(u => u.AccountData.Login),
                    "email" => queryable.OrderByDescending(u => u.AccountData.Email),
                    "phone" => queryable.OrderByDescending(u => u.PhoneNumber.Phone),
                    "status" => queryable.OrderByDescending(u => u.Status.Name),
                    _ => queryable
                },
                _ => queryable
            };
        }
        
        return queryable;
    }

    private IQueryable<User> ApplyPagination(IQueryable<User> queryable)
    {
        int page = Page ?? 1;
        int pageSize = PageSize ?? 30;
        int offset = (page - 1) * pageSize;
        return queryable.Skip(offset).Take(pageSize);
    }
}