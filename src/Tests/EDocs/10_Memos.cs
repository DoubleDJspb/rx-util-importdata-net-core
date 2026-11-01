using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Security.Cryptography;
using ImportData;
using ImportData.IntegrationServicesClient.Models;
using Xunit.Extensions.Ordering;
using DocumentFormat.OpenXml.Office2010.Word;

namespace Tests.EDocs
{
    public partial class Tests
    {
        [Fact, Order(100)]
        public void T10_OrdersImport()
        {
            var xlsxPath = TestSettings.MemosPathXlsx;
            var action = ImportData.Constants.Actions.ImportMemos;
            var sheetName = ImportData.Constants.SheetNames.Memos;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedOrder in items)
            {
                var error = EqualsMemos(expectedOrder);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsMemos(List<string> parameters, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();

            var actualMemo = Common.GetOfficialDocument<IMemos>(parameters[shift + 0], parameters[shift + 1], parameters[shift + 13]);

            var name = Common.GetDocumentName(parameters[shift + 2], parameters[shift + 0], parameters[shift + 1], parameters[shift + 3]);

            if (actualMemo == null)
                return $"Не найдена служебная записка: {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualMemo.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                Common.CheckParam(actualMemo.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                Common.CheckParam(actualMemo.DocumentKind, parameters[shift + 2], "DocumentKind"),
                Common.CheckParam(actualMemo.Subject, parameters[shift + 3], "Subject"),
                Common.CheckParam(actualMemo.BusinessUnit, parameters[shift + 4], "BusinessUnit"),
                Common.CheckParam(actualMemo.Department, parameters[shift + 5], "Department"),
                Common.CheckParam(actualMemo.LastVersion(), parameters[shift + 6], "LastVersion"),
                Common.CheckParam(actualMemo.Addressee, parameters[shift + 7], "Addressee"),
                Common.CheckParam(actualMemo.Assignee, parameters[shift + 8], "Assignee"),
                Common.CheckParam(actualMemo.PreparedBy, parameters[shift + 9], "PreparedBy"),
                Common.CheckParam(actualMemo.OurSignatory, parameters[shift + 10], "OurSignatory"),
                Common.CheckParam(actualMemo.LifeCycleState, BusinessLogic.GetPropertyLifeCycleState(parameters[shift + 11]), "LifeCycleState"),
                Common.CheckParam(actualMemo.Note, parameters[shift + 12], "Note"),
                Common.CheckParam(actualMemo.DocumentRegister, parameters[shift + 13], "DocumentRegister"),
                Common.CheckParam(actualMemo.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 14]), "RegistrationState")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}