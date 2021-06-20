using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICRUD
    {
        T InsertRecord<T>(string tableName, T record);
        IEnumerable<T> LoadRecords<T>(string tableName);
        T LoadRecordById<T>(string tableName, Guid Id);
        void DeleteRecordById<T>(string tableName, Guid Id);
        T Upsert<T>(string tableName, T record, Guid Id);
    }
}
