using System.Data;
using Teltonika.DataParser.Client.Infrastructure.Interfaces;
using Teltonika.DataParser.Client.Models;

namespace Teltonika.DataParser.Client.Infrastructure.Visitor
{
    public class TransposedAvlDataVisitor : IVisitor
    {
        private DataRow _currentRow;
        private string _currentId;
        public TransposedAvlDataVisitor()
        {
            DataTable = new DataTable();
        }

        public DataTable DataTable { get; }

        public void Visit(BaseData componentData)
        {
            switch (componentData.Name)
            {
                case "Timestamp":
                    CreateColumn(componentData.Name);
                  
                    _currentRow = DataTable.NewRow();
                    DataTable.Rows.Add(_currentRow);

                    _currentRow[componentData.Name] = componentData.Value;
                   
                    break;
                case "Priority":
                case "Longitude":
                case "Latitude":
                case "Angle":
                case "Speed":
                case "Event ID":
                case "Altitude":
                case "Satellites":
                    CreateColumn(componentData.Name);
                    _currentRow[componentData.Name] = componentData.Value;
                    break;
                case "ID":
                    CreateColumn(componentData.Value);
                    _currentId = componentData.Value;
                    break;
                case "Value":
                    _currentRow[_currentId] = componentData.Value;
                    break;
                default:
                    break;
            }
        }
        private void CreateColumn(string columnName)
        {
            DataColumnCollection columns = DataTable.Columns;
            if (columns.Contains(columnName) == false)
            {
                var column = new DataColumn
                {
                    ColumnName = columnName,
                    Caption = columnName
                };

                DataTable.Columns.Add(column);
            }
        }

        public void Visit(CompositeData compositeData)
        {
            foreach (var item in compositeData.Data)
            {
                item.Accept(this);
            }
        }
    }
}
