//EVNT_Event
private GenericRepository<EVNT_Event> _EVNT_EventRepository;
public GenericRepository<EVNT_Event> EVNT_EventRepository
{
    get
    {
        if (this._EVNT_EventRepository == null)
        {
            this._EVNT_EventRepository = new GenericRepository<EVNT_Event>(_context);
        }
        return this._EVNT_EventRepository;
    }
}

private Mapper _EVNT_Event;
public Mapper EVNT_Event
{
    get
    {
        if (this._EVNT_Event == null)
        {
            var _EVNT_EventBLLConfig = new MapperConfiguration(cfg => cfg.CreateMap<EVNT_Event, EVNT_EventDTO>().ReverseMap());
            this._EVNT_Event = new Mapper(_EVNT_EventBLLConfig);
        }
        return this._EVNT_Event;
    }
}

[JsonProperty("genderId")]
[JsonPropertyName("genderId")]

services.AddScoped<IEVNT_Event, EVNT_EventBLL>();
services.AddScoped<IEVNT_EventBLL, EVNT_EventBLL>();


CurrentUserClaims CurrentUser { get; set; }
Task<CustomHttpResponse> GetAll();
Task<CustomHttpResponse> GetAllWithSearchParam(string SearchParam);
Task<CustomHttpResponse> GetAllWithPagination(int PageNumber);
Task<CustomHttpResponse> GetAllWithPaginationAndSearchParam(int PageNumber, string SearchParam);
Task<CustomHttpResponse> GetById(int Id);
Task<CustomHttpResponse> Create(EVNT_EventDTO newItem);
Task<CustomHttpResponse> Update(EVNT_EventDTO toEdit);

CurrentUserClaims CurrentUser { get; set; }
Task<CustomHttpResponse> GetAll(string SearchParam);
Task<CustomHttpResponse> GetAllWithPagination(int PageNumber, string SearchParam);
Task<CustomHttpResponse> GetById(int Id);
Task<CustomHttpResponse> Create(EVNT_EventDTO newItem);
Task<CustomHttpResponse> Update(EVNT_EventDTO toEdit);

CurrentUserClaims CurrentUser { get; set; }
Task<CustomHttpResponse> GetAll(string SearchParam);
Task<CustomHttpResponse> GetAllWithPagination(int PageNumber, string SearchParam);
Task<CustomHttpResponse> GetById(long Id);
Task<CustomHttpResponse> Create(EVNT_EventDTO newItem);
Task<CustomHttpResponse> Update(EVNT_EventDTO toEdit);



private Expression<Func<EVNT_Event, bool>> GetFilter(
    string SearchParam
)
{
    var filter = PredicateBuilder.New<EVNT_Event>(true);
    filter = filter.And(q => q.Status == true);
    if (SearchParam != null && SearchParam != String.Empty)
    {
        filter = filter.And(q => q.Name.Contains(SearchParam));
    }
    return filter;
}

operations

public EVNT_EventDTO MapEntity(EVNT_Event toMap)
{
    return Mappers.EVNT_Event.Map<EVNT_EventDTO>(toMap);
}

public EVNT_Event ReverseMapEntity(EVNT_EventDTO toMap)
{
    return Mappers.EVNT_Event.Map<EVNT_Event>(toMap);
}