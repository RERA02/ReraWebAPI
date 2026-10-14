using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReraCore
{
    public interface ISqlEditorRepository
    {
        Task<DataTable> GetSqlEditorList(string _sqlEditorText);
        Task<DataTable> GetSqlEditor_InsertUpdate(string _sqlEditorText);
    }
}
