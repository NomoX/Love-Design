using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Diagnostics;


namespace Love_Design
{
    class Project
    {
        public static string path_to_love = "";
        public static string current_build;
        public static string project_path;
        public static string project_name;
        public static string current_file_path;
        public static Dictionary<string, string> edited_files = new Dictionary<string, string>();
        public static Dictionary<string, string> edited_files_path = new Dictionary<string, string>();
        public static Dictionary<string, bool> edited_saved = new Dictionary<string, bool>();

        public static void buildLove(string path)
        {
            var mainForm = Form.ActiveForm as Form1;
            if (project_path != null)
            {
                //current_build = project_name + "_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".love";
                mainForm.toolStripStatusLabel1.Text = "Zipping...";
                ZipFile.CreateFromDirectory(project_path, path);
                mainForm.toolStripStatusLabel1.Text = "Zipped!";
            }
        }
        public static void buildGame(string path)
        {
            var mainForm = Form.ActiveForm as Form1;
            if (project_path != null)
            {
                current_build = project_name + "_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".love";
                mainForm.toolStripStatusLabel1.Text = "Zipping...";
                ZipFile.CreateFromDirectory(project_path, @"build\" + current_build);
                mainForm.toolStripStatusLabel1.Text = "Zipped!";

                //string b = @"/b" + " " + '"' + path_to_love + @"\love.exe" + '"' + "+" + '"' + Directory.GetCurrentDirectory() + @"\build\" + current_build + '"' + " " + '"' + path + '"';
                
                string b = String.Format(@"/C copy /b ""{0}\love.exe""+""{1}\build\{2}"" ""{3}""", path_to_love, Directory.GetCurrentDirectory(), current_build, path);

                Process.Start("cmd.exe", b);
                mainForm.toolStripStatusLabel1.Text = "Builded! (" + path + ")";
            }
        }
        public static void run()
        {
            var mainForm = Form.ActiveForm as Form1;
            if (project_path != null)
            {
                Process.Start(path_to_love + "\\love.exe", '"' + project_path + '"');
                mainForm.toolStripStatusLabel1.Text = "Runing";
            }
        }
        public static void create(string path)
        {
            var mainForm = Form.ActiveForm as Form1;
            mainForm.treeView1.Nodes.Clear();
            project_path = path;
            string[] array = path.Split('\\');
            project_name = array.Last();
            File.Copy(@"project\main.lua", project_path + @"\main.lua", true);
            File.Copy(@"project\conf.lua", project_path + @"\conf.lua", true);
            if (array.Last() != "")
                Item.add(0, array.Last(), "");
            CreatePath(mainForm.treeView1.Nodes, project_name + @"\main.lua", 4);
            CreatePath(mainForm.treeView1.Nodes, project_name + @"\conf.lua", 4);
        }
        private static void buildTree(string path)
        {
            Item.add(5, "", path.Substring(path.IndexOf(project_name)));

            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
            {
                Item.add(4, f.Split('\\').Last(), path.Substring(path.IndexOf(project_name)));
            }
            string[] directories = Directory.GetDirectories(path);
            foreach (string d in directories)
            {
                buildTree(d);
            }
        }
        public static void open(string path)
        {
            var mainForm = Form.ActiveForm as Form1;
            mainForm.treeView1.Nodes.Clear();
            project_path = path;
            string[] array = path.Split('\\');
            project_name = array.Last();
            if (array.Last() != "")
                Item.add(0, array.Last(), "");
            buildTree(path);
        }
        public static void save()
        {
            var mainForm = Form.ActiveForm as Form1;

            if (Project.current_file_path != null && !Project.edited_saved[Project.current_file_path])
            {
                StreamWriter writer = new StreamWriter(Project.project_path.Replace(Project.project_name, "") + current_file_path);
                writer.Write(edited_files[current_file_path]);
                edited_saved[Project.current_file_path] = true;
                mainForm.toolStripStatusLabel1.Text = "Saved (" + current_file_path.Split('\\').Last() +")";
                mainForm.toolStripStatusLabel2.Text = Project.countEdited();
                writer.Close();
            }         
        }
        public static void saveAll()
        {
            var mainForm = Form.ActiveForm as Form1;

            int countEdFiles = 0;

            foreach (KeyValuePair<string, bool> cf in edited_saved.ToList())
            {
                if (cf.Key != null && !cf.Value)
                {
                    countEdFiles++;
                    StreamWriter writer = new StreamWriter(Project.project_path.Replace(Project.project_name, "") + cf.Key);
                    writer.Write(edited_files[cf.Key]);
                    edited_saved[cf.Key] = true;
                    mainForm.toolStripStatusLabel1.Text = "Saved All (" + countEdFiles + ")";
                    mainForm.toolStripStatusLabel2.Text = Project.countEdited();
                    writer.Close();
                }
            }
        }
        public static string countEdited()
        {
            int fp = 0;
            foreach (KeyValuePair<string, bool> cf in edited_saved.ToList())
            {
                if (cf.Key != null && !cf.Value)
                {
                    fp++;
                }
            }
            return fp + " edited";

        }
        public static void loadSettings()
        {
            var mainForm = Form.ActiveForm as Form1;

            XmlReader reader = XmlReader.Create("data\\config.xml");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element
                && reader.Name == "lovedesign")
                {
                    mainForm.Width = Convert.ToInt32(reader.GetAttribute(0));
                    mainForm.Height = Convert.ToInt32(reader.GetAttribute(1));
                    FormWindowState fws = (FormWindowState)Enum.Parse(typeof(FormWindowState), reader.GetAttribute(2));
                    mainForm.WindowState = fws;
                    mainForm.Left = Convert.ToInt32(reader.GetAttribute(3));
                    mainForm.Top = Convert.ToInt32(reader.GetAttribute(4));

                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name == "pathToLove")
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    path_to_love = reader.Value;
                                }
                            }
                            reader.Read();
                        }
                    }
                }
            }
        }
        public static void uptadeSettings()
        {
            var mainForm = Form.ActiveForm as Form1;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create("data\\config.xml", settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("lovedesign");
            writer.WriteAttributeString("windowWidth", mainForm.Width.ToString());
            writer.WriteAttributeString("windowHeight", mainForm.Height.ToString());
            writer.WriteAttributeString("windowState", mainForm.WindowState.ToString());
            writer.WriteAttributeString("positionX", mainForm.Left.ToString());
            writer.WriteAttributeString("positionY", mainForm.Top.ToString());
            writer.WriteElementString("pathToLove", path_to_love);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }
        public static void loadEditorSettings()
        {
            var mainForm = Form.ActiveForm as Form1;

            XmlReader reader = XmlReader.Create("data\\editor.xml");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element
                && reader.Name == "editor")
                {
                    mainForm.fastColoredTextBox1.Font = new Font(reader.GetAttribute(0), Convert.ToSingle(reader.GetAttribute(1)), FontStyle.Regular);
                    mainForm.fastColoredTextBox1.ForeColor = Color.FromArgb(Convert.ToInt32(reader.GetAttribute(2)));
                    mainForm.fastColoredTextBox1.BackColor = Color.FromArgb(Convert.ToInt32(reader.GetAttribute(3)));
                    mainForm.fastColoredTextBox1.LineNumberColor = Color.FromArgb(Convert.ToInt32(reader.GetAttribute(4)));
                    mainForm.fastColoredTextBox1.CaretColor = Color.FromArgb(Convert.ToInt32(reader.GetAttribute(5)));
                }
            }
        }
        public static void updateEditorSettings()
        {
            var mainForm = Form.ActiveForm as Form1;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create("data\\editor.xml", settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("editor");
            writer.WriteAttributeString("fontFamily", mainForm.fastColoredTextBox1.Font.FontFamily.Name);
            writer.WriteAttributeString("fontSize", mainForm.fastColoredTextBox1.Font.Size.ToString());
            writer.WriteAttributeString("fontColor", mainForm.fastColoredTextBox1.ForeColor.ToArgb().ToString());
            writer.WriteAttributeString("backGroundColor", mainForm.fastColoredTextBox1.BackColor.ToArgb().ToString());
            writer.WriteAttributeString("lineNumberColor", mainForm.fastColoredTextBox1.LineNumberColor.ToArgb().ToString());
            writer.WriteAttributeString("caretColor", mainForm.fastColoredTextBox1.CaretColor.ToArgb().ToString());
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }
        public static void CreatePath(TreeNodeCollection nodeList, string path,int imgIdx)
        {
            TreeNode node = null;
            string folder = string.Empty;

            int p = path.IndexOf('\\');

            if (p == -1)
            {
                folder = path;
                path = "";
            }
            else
            {
                folder = path.Substring(0, p);
                path = path.Substring(p + 1, path.Length - (p + 1));
            }

            node = null;

            foreach (TreeNode item in nodeList)
            {
                if (item.Text == folder)
                {
                    node = item;
                }
            }

            if (node == null)
            {
                node = new TreeNode(folder);
                nodeList.Add(node);
                node.ImageIndex = imgIdx;
                node.SelectedImageIndex = imgIdx;
            }

            if (path != "")
            {
                CreatePath(node.Nodes, path, imgIdx);
            }
        }
        public static DialogResult InputBox(string title, ref string value)
        {
            Form form = new Form();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();

            form.Text = title;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;

            textBox.SetBounds(12, 12, 342, 20);
            buttonOk.SetBounds(271, 38, 83, 23);

            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(366, 72);
            form.Controls.AddRange(new Control[] { textBox, buttonOk });
            form.ClientSize = new System.Drawing.Size(366, 72);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}
