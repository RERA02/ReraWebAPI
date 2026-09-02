using ReraModel.CommonFunction;
using ReraModel.CommonModel;
using System.Data;



namespace ReraCore.Interfaces
{
    public interface ICommonFunctionRepository
    {        
       

        Task<DataTable> GetComponentsNew(int? sewadwar_State_ID, int? TypeId);
        Task<DataTable> GetProjectType();
        Task<DataTable> GetQuarterList(int? FinYearId);
        Task<DataTable> GetFinancialyearList();
        Task<DataTable> GetPromoterforddl();
        Task<DataTable> GetProjectList(int? UserId);
        Task<DataTable> GetComponents(int? parentID, int? TypeId);
        Task<DataTable> GetVistorCount(string Actionname , string ip);
        Task<DataTable> CommonDetails(CommonModel searchParams);
        Task<DataTable> GetAllAgentDocumentUploadList(AgentDocumentModel searchParams);
        Task<DataTable> GetYear();

        Task<DataTable> GetDeflauterlist(int? sewadwar_State_ID, int? TypeId);
    }
}
