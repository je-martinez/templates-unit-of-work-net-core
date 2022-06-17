        //EVNT_Event
        public CurrentUserClaims CurrentUser { get; set; }
        private AutoMappers mappers;
        protected IHttpContextAccessor _httpContextAccessor;

        public EVNT_EventBLL(IHttpContextAccessor httpContextAccessor)
        {
            mappers = new AutoMappers();
            _httpContextAccessor = httpContextAccessor;
            CurrentUser = new CurrentUserClaims(_httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity);
        }

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
                                return MapEntity(item);
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
                    ErrorsConstants.GetAll_EVNT_Event,
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
                                return MapEntity(item);
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
                    ErrorsConstants.GetAll_EVNT_Event,
                    HttpStatusCode.Conflict
                );
            }
            return response;
        }

        public async Task<CustomHttpResponse> GetById(int Id)
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
                        response.Message = ErrorsConstants.GetById_NF_EVNT_Event;
                        response.Success = false;
                        response.StatusCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        response.Data = MapEntity(entityFind);
                        response.Success = true;
                        response.StatusCode = HttpStatusCode.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleErrorWithResponse(
                    ex,
                    response,
                    ErrorsConstants.GetById_EVNT_Event,
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
                EVNT_Event newRecord = ReverseMapEntity(newItem);
                newRecord.CreatedAt = DateTime.Now;
                newRecord.CreatedBy = CurrentUser.UserId;
                newRecord.CreatedByUserName = CurrentUser.UserName;
                newRecord.Alias = !(newRecord.Alias == null || newRecord.Alias == String.Empty) ? GeneralHelpers.formatAlias(newRecord.Alias) : GeneralHelpers.RandomString(8);
                using (UnitOfWork logic = new UnitOfWork())
                {
                    await logic.EVNT_EventRepository.InsertAsync(newRecord);
                    await logic.SaveAsync();
                    response.Success = true;
                    response.Data = MapEntity(newRecord);
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = MessagesConstants.Create_Success;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleErrorWithResponse(
                    ex,
                    response,
                    ErrorsConstants.Create_EVNT_Event,
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
                        response.Message = ErrorsConstants.GetById_NF_EVNT_Event;
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
                    response.Data = MapEntity(recordToEdit);
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
                    ErrorsConstants.Update_EVNT_Event,
                    HttpStatusCode.Conflict
                );
            }
            return response;
        }


        private EVNT_EventDTO MapEntity(EVNT_Event toMap)
        {
            return mappers.EVNT_Event.Map<EVNT_EventDTO>(toMap);
        }

        private EVNT_Event ReverseMapEntity(EVNT_EventDTO toMap)
        {
            return mappers.EVNT_Event.Map<EVNT_Event>(toMap);
        }