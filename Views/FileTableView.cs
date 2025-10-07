using System.ComponentModel;
using System.Data;
using Terminal.Gui;

namespace App.Views
{
    class FileTableView : TableView
    {
        string cwd;
        string sort = "Name";
        List<string> dataTableColumns = ["Name", "Props", "Own"];
        Dictionary<string, bool> columns = new Dictionary<string, bool>
        {
            { "Name", true },
            { "Type", true },
            { "Size", true },
            { "Permission", true },
        };

        // Name
        // Type
        // Size
        // Permisson

        void InitData()
        {
            var qwe = new DataTable();

            foreach (var column in columns) {
                if (column.Value)
                {
                    qwe.Columns.Add(column.Key);
                }
            }

            // Table = 
        }
    }
}