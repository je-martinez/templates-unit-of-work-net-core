//EVNT_Event
public CurrentUserClaims CurrentUser { get; set; }
private MappersEntities mappers;
protected IHttpContextAccessor _httpContextAccessor;
protected ErrorDefaultMessagesHelpers error;

public EVNT_EventBLL(IHttpContextAccessor httpContextAccessor)
{
    mappers = new MappersEntities();
    _httpContextAccessor = httpContextAccessor;
    error = new ErrorDefaultMessagesHelpers(EntityGenderName.F_Singular, EntityGenderName.F_Plural, "accion", "acciones");
    CurrentUser = new CurrentUserClaims(_httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity);
}

private Expression<Func<EVNT_Event, bool>> GetFilter(
    string SearchParam
)
{
    var filter = PredicateBuilder.New<EVNT_Event>(true);
    filter = filter.And(q => q.Status == true);
    if (SearchParam != null && SearchParam != string.Empty)
    {
        filter = filter.And(q => q.Name.Contains(SearchParam));
    }
    return filter;
}

private Pagination GetPaginationParams(
    int PageNumber
)
{
    return new Pagination()
    {
        SkipValue = PaginationHelpers.GetSkipValue(PageNumber),
        TakeValue = PaginationHelpers.PageSize
    };
}

public async Task<CustomHttpResponse> GetAll(string SearchParam)
{
    CustomHttpResponse response = new CustomHttpResponse();
    try
    {
        using (UnitOfWork logic = new UnitOfWork())
        {
            List<EVNT_Event> listOfEntites = (await logic.EVNT_EventRepository.GetAsync(
                filter: GetFilter(SearchParam)
            ))?.ToList();
            if (listOfEntites != null && listOfEntites.Count > 0 || listOfEntites.Count == 0)
            {
                if (listOfEntites.Count > 0)
                {
                    response.Data = listOfEntites.Select(item =>
                    {
                        return mappers.MapEntity(item);
                    }).ToList();
                }
                else if (listOfEntites.Count == 0)
                {
                    response.Data = new List<object>();
                }
            }
            response.Success = true;
            response.StatusCode = HttpStatusCode.OK;
        }
    }
    catch (Exception ex)
    {
        ErrorHandler.HandleErrorWithResponse(
            ex,
            response,
            error.GetAll,
            HttpStatusCode.Conflict
        );
    }
    return response;
}


public async Task<CustomHttpResponse> GetAllWithPagination(int PageNumber, string SearchParam)
{
    CustomHttpResponse response = new CustomHttpResponse();
    try
    {
        using (UnitOfWork logic = new UnitOfWork())
        {
            List<EVNT_Event> listOfEntites = (await logic.EVNT_EventRepository.GetWithPaginationAsync(
                GetPaginationParams(PageNumber),
                orderBy: q => q.OrderBy(d => d.EVNT_EventId),
                filter: GetFilter(SearchParam)
            ))?.ToList();
            if (listOfEntites != null && listOfEntites.Count > 0 || listOfEntites.Count == 0)
            {
                if (listOfEntites.Count > 0)
                {
                    response.Data = listOfEntites.Select(item =>
                    {
                        return mappers.MapEntity(item);
                    }).ToList();
                }
                else if (listOfEntites.Count == 0)
                {
                    response.Data = new List<object>();
                }
            }
            response.Success = true;
            response.StatusCode = HttpStatusCode.OK;
        }
    }
    catch (Exception ex)
    {
        ErrorHandler.HandleErrorWithResponse(
            ex,
            response,
            error.GetAll,
            HttpStatusCode.Conflict
        );
    }
    return response;
}

public async Task<CustomHttpResponse> GetById(long Id)
{
    CustomHttpResponse response = new CustomHttpResponse();
    try
    {
        using (UnitOfWork logic = new UnitOfWork())
        {
            EVNT_Event entityFind = await logic.EVNT_EventRepository.GetByIdAsync(Id);
            response.Data = entityFind;
            if (entityFind == null)
            {
                response.Message = error.NotFound;
                response.Success = false;
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            response.Data = mappers.MapEntity(entityFind);
            response.Success = true;
            response.StatusCode = HttpStatusCode.OK;
        }
    }
    catch (Exception ex)
    {
        ErrorHandler.HandleErrorWithResponse(
            ex,
            response,
            error.GetById,
            HttpStatusCode.Conflict
        );
    }
    return response;
}

public async Task<CustomHttpResponse> Create(EVNT_EventDTO newItem)
{
    CustomHttpResponse response = new CustomHttpResponse();
    try
    {
        EVNT_Event newRecord = mappers.ReverseMapEntity(newItem);
        newRecord.CreatedAt = DateTime.Now;
        newRecord.CreatedBy = CurrentUser.UserId;
        newRecord.CreatedByUserName = CurrentUser.UserName;
        newRecord.Alias = !(newRecord.Alias == null || newRecord.Alias == String.Empty) ? GeneralHelpers.formatAlias(newRecord.Alias) : GeneralHelpers.RandomString(8);
        using (UnitOfWork logic = new UnitOfWork())
        {
            await logic.EVNT_EventRepository.InsertAsync(newRecord);
            await logic.SaveAsync();
            response.Success = true;
            response.Data = mappers.MapEntity(newRecord);
            response.StatusCode = HttpStatusCode.OK;
            response.Message = MessagesConstants.Create_Success;
        }
    }
    catch (Exception ex)
    {
        ErrorHandler.HandleErrorWithResponse(
            ex,
            response,
            error.Create,
            HttpStatusCode.Conflict
        );
    }
    return response;
}

public async Task<CustomHttpResponse> Update(EVNT_EventDTO toEdit)
{
    CustomHttpResponse response = new CustomHttpResponse();
    try
    {
        using (UnitOfWork logic = new UnitOfWork())
        {
            EVNT_Event recordToEdit = await logic.EVNT_EventRepository.GetByIdAsync(toEdit.EVNT_EventId);
            if (recordToEdit == null)
            {
                response.Message = error.NotFound;
                response.Success = false;
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            recordToEdit.Name = toEdit.Name;
            recordToEdit.Alias = !(toEdit.Alias == null || toEdit.Alias == String.Empty) ? GeneralHelpers.formatAlias(toEdit.Alias) : GeneralHelpers.RandomString(8);
            recordToEdit.Status = toEdit.Status;
            recordToEdit.ModifiedAt = DateTime.Now;
            recordToEdit.ModifiedBy = CurrentUser.UserId;
            recordToEdit.ModifiedByUserName = CurrentUser.UserName;
            await logic.SaveAsync();
            response.Data = mappers.MapEntity(recordToEdit);
            response.Success = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Message = recordToEdit?.Status == false ? MessagesConstants.Delete_Success : MessagesConstants.Update_Success;
        }
    }
    catch (Exception ex)
    {
        ErrorHandler.HandleErrorWithResponse(
            ex,
            response,
            error.Update,
            HttpStatusCode.Conflict
        );
    }
    return response;
}
