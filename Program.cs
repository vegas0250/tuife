using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Terminal.Gui;
using App.Views;

class FileExplorer {
    static string cwd = Directory.GetCurrentDirectory();
    static ListView listView = null!;
    static Label pathLabel = null!;
    static List<string> entries = new List<string>();

    public static TableView tableView { get; private set; }
    public static FileTableView fileTableView;

    static void Main() {
        Application.Init();
        var top = Application.Top;

        var win = new Window("TUI File Explorer — hjkl navigation") {
            X = 0,
            Y = 1, // leave row for menu/status
            Width = Dim.Fill(),
            Height = Dim.Fill() - 5
        };

        pathLabel = new Label(cwd) { X = 0, Y = 0, Width = Dim.Fill() };
        top.Add(pathLabel);

        entries = GetEntries(cwd).ToList();

        var table = new DataTable();
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Type", typeof(string));
        table.Columns.Add("Size", typeof(long));
        table.Columns.Add("Permissions", typeof(string));

        // Добавляем строки
        table.Rows.Add("readme.txt", ".txt", 1024, "rw-r--r--");
        table.Rows.Add("Program.cs", ".cs", 2048, "rw-r--r--");

        tableView = new TableView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Table = table,
            FullRowSelect = true,
        };

        fileTableView = new FileTableView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            FullRowSelect = true,
        };

        fileTableView.InitData();
        fileTableView.ScanCurrentDir();


        win.Add(fileTableView);

        /*
        listView = new ListView(entries) {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        win.Add(listView);
        */

        // Instructions at bottom
        var help = new Label("hjkl — j:down k:up l:open h:up-dir  Enter:open  q:quit  r:refresh") {
            X = 0,
            Y = Pos.Bottom(win),
            Width = Dim.Fill()
        };
        top.Add(help);

        var debug = new Label("Hello") {
          X = 0,
          Y = Pos.Bottom(win)+1,
          Width = Dim.Fill()
        };
        top.Add(debug);


        // Key handling on top so it works even when ListView has focus
        top.KeyDown += (e) => {
            var key = e.KeyEvent.Key;
            var rune = e.KeyEvent.KeyValue;

            if (key == Key.J || rune == 'j')
            {
                //Move(1);
                e.Handled = true;
                debug.Text = "j";
                
                tableView.Table.Rows.Add("Program.cs", ".cs", 2048, "rw-r--r--");
            }
            else if (key == Key.K || rune == 'k')
            {
                //Move(-1);
                e.Handled = true;
                debug.Text = "k";
            }
            else if (key == Key.L || rune == 'l' || key == Key.Enter)
            {
                //OpenSelected();
                e.Handled = true;
                debug.Text = "l";
            }
            else if (key == Key.H || rune == 'h')
            {
                //GoUp();
                e.Handled = true;
                debug.Text = "h";
            }
            else if (rune == 'q')
            {
                Application.RequestStop();
                e.Handled = true;
            }
            else if (rune == 'r')
            {
                // Refresh();
                e.Handled = true;
            }
        };

        // Double-click or Enter on ListView
        /*
        listView.OpenSelectedItem += (args) => {
            OpenSelected();
        };
        */

        // Initial focus
        top.Add(win);
        // listView.SetFocus();

        Application.Run();
        Application.Shutdown();
    }

    static void Move(int delta) {
        if (entries == null || entries.Count == 0) return;
        var newIdx = listView.SelectedItem + delta;
        if (newIdx < 0) newIdx = 0;
        if (newIdx >= entries.Count) newIdx = entries.Count - 1;
        listView.SelectedItem = newIdx;
    }

    static void OpenSelected() {
        if (entries == null || entries.Count == 0) return;
        var name = entries[listView.SelectedItem];
        if (name == null) return;

        // strip trailing slash used to mark directories
        var cleanName = name.TrimEnd(Path.DirectorySeparatorChar, '/');
        var path = Path.Combine(cwd, cleanName);
        try {
            if (Directory.Exists(path)) {
                cwd = Path.GetFullPath(path);
                Refresh();
            } else if (File.Exists(path)) {
                TryOpenWithSystem(path);
            }
        } catch (Exception ex) {
            MessageBox.ErrorQuery(40, 7, "Error", ex.Message);
        }
    }

    static void GoUp() {
        var parent = Directory.GetParent(cwd);
        if (parent != null) {
            cwd = parent.FullName;
            Refresh();
        }
    }

    static void Refresh() {
        pathLabel.Text = cwd;
        entries = GetEntries(cwd).ToList();
        listView.SetSource(entries);
        if (entries.Count > 0) listView.SelectedItem = 0;
    }

    static IEnumerable<string> GetEntries(string dir) {
        try {
            var dirs = Directory.GetDirectories(dir).Select(d => Path.GetFileName(d) + "/");
            var files = Directory.GetFiles(dir).Select(f => Path.GetFileName(f));
            return dirs.Concat(files);
        } catch (Exception) {
            // ignore access errors
            return new[] { "(access denied)" };
        }
    }

    static void TryOpenWithSystem(string path) {
        try {
            var psi = new ProcessStartInfo {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(psi);
        } catch (Exception ex) {
            MessageBox.ErrorQuery(60, 7, "Open failed", ex.Message);
        }
    }
}

