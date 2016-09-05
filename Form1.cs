using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using FastColoredTextBoxNS;
using System.Xml;

namespace Love_Design
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ImageList imgRes = new ImageList();
            imgRes.Images.Add(Image.FromFile("res\\16x16\\love_1.png"));
            imgRes.Images.Add(Image.FromFile("res\\32x32\\love.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\folder.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\file.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\script.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\image.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\png.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\jpg.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\gif.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\mp3.png"));
            imgRes.Images.Add(Image.FromFile("res\\16x16\\wav.png"));

            treeView1.ImageList = imgRes;
            treeView1.SelectedImageIndex = 0;
            treeView1.TreeViewNodeSorter = new NodeSorter();

            /*
            treeView1.Nodes[0].Text = "Project1.love";
            
            treeView1.Nodes[0].Nodes[0].ImageIndex = 2;
            treeView1.Nodes[0].Nodes[0].SelectedImageIndex = 2;
            treeView1.Nodes[0].Nodes[1].ImageIndex = 3;
            */
            //Item.add(2,"lol.lua","path");
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            Project.loadSettings();
            Project.loadEditorSettings();
        }
        public class NodeSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode tx = x as TreeNode;
                TreeNode ty = y as TreeNode;

                //MessageBox.Show(tx.ImageIndex.ToString()+" | "+ty.ImageIndex.ToString(), "");
                return tx.Text.CompareTo(ty.Text);
            }
        }
        private void createFolder() 
        {
            if (treeView1.SelectedNode != null && (treeView1.SelectedNode.ImageIndex == 2 || treeView1.SelectedNode.ImageIndex == 0))
            {
                string value = "folder";
                if (Project.InputBox("Folder Name", ref value) == DialogResult.OK)
                {
                    Item.add(1, value, treeView1.SelectedNode.FullPath);
                }
            }
        }
        private void createScript() 
        {
            if (treeView1.SelectedNode != null && (treeView1.SelectedNode.ImageIndex == 2 || treeView1.SelectedNode.ImageIndex == 0))
                {
                string value = "script.lua";
                if (Project.InputBox("Folder Name", ref value) == DialogResult.OK)
                {
                    Item.add(3, value, treeView1.SelectedNode.FullPath);
                }
            }
        }
        private void createFile()
        {
            if (treeView1.SelectedNode != null && (treeView1.SelectedNode.ImageIndex == 2 || treeView1.SelectedNode.ImageIndex == 0))
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string sFileName = openFileDialog1.FileName;
                    string[] arrAllFiles = openFileDialog1.FileNames;

                    foreach (string fileName in arrAllFiles)
                    {
                        Item.add(2, fileName, treeView1.SelectedNode.FullPath);
                    }
                }
            }
        }
        private void deleteFile()
        {
            if (treeView1.SelectedNode != null &&  treeView1.SelectedNode.ImageIndex != 0)
            {
                if (treeView1.SelectedNode.ImageIndex == 4)
                {
                    Project.edited_saved[treeView1.SelectedNode.FullPath] = true;
                    Project.edited_files[treeView1.SelectedNode.FullPath] = null;
                    Project.current_file_path = null;
                    toolStripStatusLabel1.Text = "Ready";
                    toolStripStatusLabel2.Text = Project.countEdited();
                }
                if (treeView1.SelectedNode.ImageIndex == 2)
                {
                    Directory.Delete(Project.project_path.Replace(Project.project_name, "") + treeView1.SelectedNode.FullPath, true);
                }
                else
                {
                    File.Delete(Project.project_path.Replace(Project.project_name, "") + treeView1.SelectedNode.FullPath);
                }
                treeView1.SelectedNode.Remove();
            }
        }
        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this, e.Location);
            }
        }

        private void fastColoredTextBox1_Load(object sender, EventArgs e)
        {

        }

        Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        Style StringStyle = new TextStyle(Brushes.Peru, null, FontStyle.Regular);
        Style BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        Style RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        Style PurpleStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        Style NavyStyle = new TextStyle(Brushes.Navy, null, FontStyle.Regular);

        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(GreenStyle);
            e.ChangedRange.ClearStyle(StringStyle);
            e.ChangedRange.ClearStyle(BlueStyle);
            e.ChangedRange.ClearStyle(RedStyle);
            e.ChangedRange.SetStyle(GreenStyle, @"--.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(StringStyle, @"((\u0022.+?\u0022)|('.+?'))");
            e.ChangedRange.SetStyle(BlueStyle, @"\b(function|local|require|if|else|then|elseif|for|end|do|while|in|nil)\b");
            e.ChangedRange.SetStyle(RedStyle, @"(\(|\)|\[|\]|\=|\+|\-|\*|\/|\>|\<|\{|\}|\,)|\:");
            e.ChangedRange.SetStyle(PurpleStyle, @"[0-9]*\.*[0-9]+");
            e.ChangedRange.SetStyle(NavyStyle, @"(\bself\b)|(math.+?\()|(love.+?\()");

            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers(@"\bfunction\b", @"\bend\b");
            e.ChangedRange.SetFoldingMarkers(@"\bif\b", @"\bend\b");
            e.ChangedRange.SetFoldingMarkers(@"\bfor\b", @"\bend\b");
            e.ChangedRange.SetFoldingMarkers(@"\bwhile\b", @"\bend\b");

            /*----------------------------------------------------------------*/
            //toolStripStatusLabel4.Text = "Line " + fastColoredTextBox1.

            if (Project.current_file_path != null)
            {
                Project.edited_files[Project.current_file_path] = fastColoredTextBox1.Text;
                Project.edited_saved[Project.current_file_path] = false;
                toolStripStatusLabel1.Text = "Edit";
                toolStripStatusLabel2.Text = Project.countEdited();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MessageBox.Show(e.Node.FullPath, "");
            switch (e.Node.ImageIndex)
            {
                case 4:
                    //Project.current_file_path = e.Node.FullPath.Split('\\').Last();
                    Project.current_file_path = e.Node.FullPath;
                    if (Project.edited_saved[Project.current_file_path])
                    {
                        StreamReader reader = new StreamReader(Project.project_path.Replace(Project.project_name, "") + e.Node.FullPath);
                        fastColoredTextBox1.Text = reader.ReadToEnd();
                        reader.Close();
                        Project.edited_files[Project.current_file_path] = fastColoredTextBox1.Text;
                        Project.edited_saved[Project.current_file_path] = true;
                        toolStripStatusLabel1.Text = "Saved";
                        toolStripStatusLabel2.Text = Project.countEdited();
                    }
                    else
                    {
                        fastColoredTextBox1.Text = Project.edited_files[Project.current_file_path];
                        Project.edited_saved[Project.current_file_path] = false;
                        toolStripStatusLabel1.Text = "Edit";
                        toolStripStatusLabel2.Text = Project.countEdited();
                    }
                    break;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Select Project Folder";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                Project.create(path);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Select Project Folder";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                Project.open(path);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Project.save();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Select Project Folder";
            if ( folderBrowserDialog1.ShowDialog() == DialogResult.OK ) {
                string path = folderBrowserDialog1.SelectedPath;
                Project.create(path);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            createScript();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            createFile();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        private void createFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            createFolder();
        }

        private void addScriptToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            createScript();
        }

        private void addResourceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            createFile();
        }

        private void createFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createFolder();
        }

        private void addScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createScript();
        }

        private void addResourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createFile();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        private void fastColoredTextBox1_TextChanging(object sender, TextChangingEventArgs e)
        {
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.save();
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.saveAll();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.uptadeSettings();
            Project.updateEditorSettings();
            Environment.Exit(0);
        }

        private void toolStripStatusLabel4_Click(object sender, EventArgs e)
        {

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Paste();
        }

        private void fastColoredTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(this, e.Location);
            }
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Cut();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Copy();
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.SelectAll();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteFile();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (treeView1.SelectedNode.ImageIndex == 2)
            {
                Directory.Move(Project.project_path.Replace(Project.project_name, "") + e.Node.FullPath, Project.project_path.Replace(Project.project_name, "") + e.Node.FullPath.Replace(e.Node.FullPath.Split('\\').Last(),e.Label));
            }
            else
            {
                string plabel = "";
                if (treeView1.SelectedNode.ImageIndex == 4)
                {
                    if (e.Label.Split('.').Last() == "lua")
                    {
                        plabel = e.Label;
                    }
                    else
                    {
                        plabel = e.Label + ".lua";
                    }
                    Project.edited_saved[e.Node.FullPath.Replace(e.Node.FullPath.Split('\\').Last(), plabel)] = Project.edited_saved[e.Node.FullPath];
                    Project.edited_files[e.Node.FullPath.Replace(e.Node.FullPath.Split('\\').Last(), plabel)] = Project.edited_files[e.Node.FullPath];
                }
                File.Move(Project.project_path.Replace(Project.project_name, "") + e.Node.FullPath, Project.project_path.Replace(Project.project_name, "") + e.Node.FullPath.Replace(e.Node.FullPath.Split('\\').Last(), plabel));
                this.BeginInvoke((MethodInvoker)delegate { treeView1.SelectedNode.Text = plabel; });
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.ImageIndex != 0)
            {
                treeView1.SelectedNode.BeginEdit();
            }
        }

        private void pathToLOVEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Select Love installation folder";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Project.path_to_love = folderBrowserDialog1.SelectedPath;
                Project.uptadeSettings();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Project.uptadeSettings();
            Project.updateEditorSettings();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowFindDialog();
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.BackColor = colorDialog1.Color;
            }
        }

        private void lineNumberColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.LineNumberColor = colorDialog1.Color;
            }
        }

        private void caretColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.CaretColor = colorDialog1.Color;
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.Font = fontDialog1.Font;
            }   
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.run();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Project.run();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (Project.project_path != null)
            {
                saveFileDialog1.Filter = "LOVE|*.love";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Project.buildLove(saveFileDialog1.FileName);
                }
            }
        }

        private void buildLOVEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Project.project_path != null)
            {
                saveFileDialog1.Filter = "LOVE|*.love";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Project.buildLove(saveFileDialog1.FileName);
                }
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (Project.project_path != null)
            {
                saveFileDialog1.Filter = "Executable|*.exe";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Project.buildGame(saveFileDialog1.FileName);
                }
            }
        }

        private void fontColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.ForeColor = colorDialog1.Color;
            }
        }
    }
}
