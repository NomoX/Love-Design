using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Love_Design
{
    class Item
    {
        /* items type
         0 - project
         1 - folder
         2 - file create(by file type)
         3 - script
         4 - files
         */

 

        public static void add(int type, string name, string path)
        {
            //Form mainForm = Application.OpenForms["Form1"];
            var mainForm = Form.ActiveForm as Form1;
            
            switch (type) {
                case 0:
                    mainForm.treeView1.Nodes.Add(name);
                    mainForm.treeView1.Nodes[0].ImageIndex = 0;
                    mainForm.treeView1.Nodes[0].SelectedImageIndex = 0;
                    break;
                case 1:
                    Project.CreatePath(mainForm.treeView1.Nodes, path + "\\" + name, 2);
                    Directory.CreateDirectory(Project.project_path + path.Replace(path.Split('\\').First(), "") + "\\" + name);
                    break;
                case 2:
                    string[] array = name.Split('\\');
                    string[] fformat = name.Split('.');
                    int ft = 0;
                    switch (fformat.Last()) { 
                        case "lua": 
                            ft = 4;
                            Project.edited_saved[path + "\\" + array.Last()] = true;
                            break;
                        case "jpg":
                        case "jpeg":
                        case "png":
                        case "tga":
                            ft = 5;
                            break;
                        case "mp3":
                            ft = 9;
                            break;
                        case "wav":
                            ft = 10;
                            break;
                        default: 
                            ft = 3;
                            break;
                    }
                    Project.CreatePath(mainForm.treeView1.Nodes, path + "\\" + array.Last(), ft);
                    File.Copy(name, Project.project_path + path.Replace(path.Split('\\').First(), "") + "\\" + array.Last(), true);
                    break;
                case 3:
                    Project.CreatePath(mainForm.treeView1.Nodes, path + "\\" + name, 4);
                    File.Create(Project.project_path + path.Replace(path.Split('\\').First(),"") + "\\" + name);
                    Project.edited_saved[path + "\\" + name] = true;
                    break;
                case 4:
                    string[] array2 = name.Split('\\');
                    string[] fformat2 = name.Split('.');
                    int ft2 = 0;
                    switch (fformat2.Last()) { 
                        case "lua": 
                            ft2 = 4;
                            Project.edited_saved[path + "\\" + array2.Last()] = true;
                            break;
                        case "jpg":
                        case "jpeg":
                        case "png":
                        case "tga":
                            ft2 = 5;
                            break;
                        case "mp3":
                            ft2 = 9;
                            break;
                        case "wav":
                            ft2 = 10;
                            break;
                        default: 
                            ft2 = 3;
                            break;
                    }
                    Project.CreatePath(mainForm.treeView1.Nodes, path + "\\" + array2.Last(), ft2);
                    break;
                case 5:
                    Project.CreatePath(mainForm.treeView1.Nodes, path + "\\" + name, 2);
                    break;
                default:
                    MessageBox.Show("Item type Not Found", "!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
