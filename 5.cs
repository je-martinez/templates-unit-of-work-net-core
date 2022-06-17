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

public EVNT_EventDTO MapEntity(EVNT_Event toMap)
{
    return Mappers.EVNT_Event.Map<EVNT_EventDTO>(toMap);
}

public EVNT_Event ReverseMapEntity(EVNT_EventDTO toMap)
{
    return Mappers.EVNT_Event.Map<EVNT_Event>(toMap);
}

public void ProccessEVNT_EventFks(PAG_PageDTO item, PAG_Page entity, CurrentUserClaims currentUser, bool deactiveOnMissing = false)
{
    ProcessFKPageActions(item.Actions, entity.PAG_ActionXPages, currentUser, deactiveOnMissing);
}

public void ProcessFKPageActions(List<EVNT_EventDTO> toAddOrEdit, ICollection<EVNT_Event> entities, CurrentUserClaims user, bool deactiveOnMissing = false)
{
    List<long> entitiesCreatedOrReactivated = new List<long>();
    if (GeneralHelpersGeneric<EVNT_EventDTO>.IsListNotNullOrEmpty(toAddOrEdit))
    {
        toAddOrEdit.ForEach(item =>
        {
            EVNT_Event detail = entities.Where(q => q.PAG_PageId == item.PAG_PageId && q.PAG_PageId > 0).FirstOrDefault();
            if (detail == null)
            {
                EVNT_Event newItem = _mappers.ReverseMapEntity(item);
                newItem.CreatedAt = DateTime.Now;
                newItem.CreatedBy = user.UserId;
                newItem.CreatedByUserName = user.UserName;
                entities.Add(newItem);
                entitiesCreatedOrReactivated.Add(newItem.PAG_PageId);
            }
            else
            {
                detail.Status = item.Status;
                detail.ModifiedAt = DateTime.Now;
                detail.ModifiedBy = user.UserId;
                detail.ModifiedByUserName = user.UserName;
                entitiesCreatedOrReactivated.Add(detail.PAG_PageId);
            }
        });
    }
    if (deactiveOnMissing)
    {
        foreach (var item in entities)
        {
            if (!toAddOrEdit.Any(x => x.PAG_PageId == item.PAG_PageId && entitiesCreatedOrReactivated.Contains(x.PAG_PageId)))
            {
                item.Status = false;
                item.ModifiedAt = DateTime.Now;
                item.ModifiedBy = user.UserId;
                item.ModifiedByUserName = user.UserName;
            }
        }
    }
}