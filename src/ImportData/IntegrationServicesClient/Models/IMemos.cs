using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Xml.Linq;
using NLog;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Служебная записка")]
  public class IMemos : IInternalDocumentBases
  {
    [PropertyOptions("Подписал", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public new IEmployees OurSignatory { get; set; }

    [PropertyOptions("Адресат", RequiredType.Required, PropertyType.EntityWithCreate, AdditionalCharacters.ForSearch)]
    public IEmployees Addressee { get; set; }

    public bool IsManyAddressees { get; set; }
    public string ManyAddresseesPlaceholder { get; set; }
    public string ManyAddresseesLabel { get; set; }

    new public static IMemos CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var subject = propertiesForSearch[Constants.KeyAttributes.Subject];
      var documentKindName = propertiesForSearch[Constants.KeyAttributes.DocumentKind];
      var businessUnitName = propertiesForSearch[Constants.KeyAttributes.BusinessUnit];
      var departmentName = propertiesForSearch[Constants.KeyAttributes.Department];
      var preparedByName = propertiesForSearch[Constants.KeyAttributes.PreparedBy];
      var registrationNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];
      var name = $"{documentKindName} \"{subject}\"";
      var documentKind = BusinessLogic.GetEntityWithFilter<IDocumentKinds>(x => x.Name == documentKindName, exceptionList, logger);
      var businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == businessUnitName, exceptionList, logger);
      var department = BusinessLogic.GetEntityWithFilter<IDepartments>(x => x.Name == departmentName, exceptionList, logger);
      var preparedBy = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == preparedByName, exceptionList, logger);

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate) &&
        documentKind != null && businessUnit != null && department != null && preparedBy != null)
      {
        return BusinessLogic.CreateEntity<IMemos>(new IMemos()
        {
          Name = name,
          Subject = subject,
          DocumentKind = documentKind,
          BusinessUnit = businessUnit,
          Department = department,
          RegistrationDate = registrationDate,
          RegistrationNumber = registrationNumber,
          Created = registrationDate
        }, exceptionList, logger, isBatch);
      }

      return null;
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var subject = propertiesForSearch[Constants.KeyAttributes.Subject];
      var documentKindName = propertiesForSearch[Constants.KeyAttributes.DocumentKind];
      var registrationNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate))
      {
        var name = $"{documentKindName} №{registrationNumber} от {registrationDate.ToString("dd.MM.yyyy")} \"{subject}\"";
        return BusinessLogic.GetEntityWithFilter<IMemos>(x => x.Name == name, exceptionList, logger);
      }

      return null;
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
      {
        var lifeCycleState = ((IMemos)entity).LifeCycleState;
        entity = BusinessLogic.CreateEntity((IMemos)entity, exceptionList, logger, isBatch);
        return ((IMemos)entity)?.UpdateLifeCycleState(lifeCycleState, isBatch);
      }
      else
        return BusinessLogic.UpdateEntity((IMemos)entity, exceptionList, logger);
    }
  }
}
