using ImportData.Entities.Databooks;
using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Список адресатов служебной записки")]
  public class IMemosAddresseess : IEntityBase
  {
    [PropertyOptions("Id служебной записки в DirectumRX", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IMemos MemoDocumentBase { get; set; }

    [PropertyOptions("Адресат", RequiredType.NotRequired, PropertyType.Entity)]
    public IEmployees Addressee { get; set; }

    [PropertyOptions("Подразделение", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IDepartments Department { get; set; }
    
    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Note { get; set; }

    new public static IEntityBase FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      return null;
    }

    new public static IEntityBase CreateOrUpdate(IEntityBase entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var memoAddresseess = (IMemosAddresseess)entity;
      var memo = memoAddresseess.MemoDocumentBase;
      var addressee = new IMemosAddresseess
      {
        Addressee = memoAddresseess.Addressee,
        MemoDocumentBase = memo,
        Department = memoAddresseess.Department,
        Note = memoAddresseess.Note,
      };
      memo.CreateAddressee(addressee, logger, isBatch);
      return addressee;
    }
  }
}
