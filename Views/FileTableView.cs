using System.ComponentModel;
using System.Data;
using Terminal.Gui;

namespace App.Views
{
    class FileTableView : TableView
    {
        string cwd = Directory.GetCurrentDirectory();
        string sort = "Name";
        List<string> dataTableColumns = ["Name", "Props", "Own"];
        Dictionary<string, bool> columns = new Dictionary<string, bool>
        {
            { "Name", true },
            { "Type", true },
            { "Size", true },
            // { "Permission", true },
        };

        public FileTableView()
        {
            this.KeyPress += OnKeyPress;
        }

        // Name
        // Type
        // Size
        // Permisson

        public void InitData()
        {

            var dt =  new DataTable();

            foreach (var column in columns)
            {
                if (column.Value)
                {
                    dt.Columns.Add(column.Key);
                }
            }

            

            Table = dt;

            // Table = 
        }

        public void ScanCurrentDir()
        {
            Table.Rows.Clear();
            
            foreach (var file in GetFiles())
            {
                FileInfo fi = new FileInfo(file);

                List<string> row = new List<string>();

                if (columns["Name"])
                {
                    row.Add(fi.Name);
                }

                if (columns["Type"])
                {
                    row.Add(fi.Extension);
                }

                if (columns["Size"])
                {
                    row.Add(fi.Length.ToString());
                }

                Table.Rows.Add(row.ToArray());
            }
        }

        string[] GetFiles()
        {
            return Directory.GetFiles(cwd);
        }

        void ChangeDirToParent()
        {
            string parent = Path.GetDirectoryName(cwd);
            if (!string.IsNullOrEmpty(parent))
            {
                cwd = parent;
            }
        }

        string GetFileName() { return ""; }
        string GetFileType() { return ""; }
        string GetFileSize() { return ""; }
        string GetFilePermission() { return ""; }

        private void OnKeyPress(View.KeyEventEventArgs e)
        {
            if (e.KeyEvent.Key == Key.K || e.KeyEvent.Key == Key.k)
            {
                if (this.SelectedRow > 0)
                    this.SelectedRow--;
                e.Handled = true;
            }
            else if (e.KeyEvent.Key == Key.J || e.KeyEvent.Key == Key.j)
            {
                if (this.SelectedRow < this.Table.Rows.Count - 1)
                    this.SelectedRow++;
                e.Handled = true;
            }
            else if (e.KeyEvent.Key == Key.h)
            {
                ChangeDirToParent();
                ScanCurrentDir();
                e.Handled = true;
            }
        }
    }
}