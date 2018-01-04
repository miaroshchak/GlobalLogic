using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gl
{
    public partial class Form1 : Form
    {
        List<String> listDir = new List<String>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    Archiv total = new Archiv();
                    total.path = FBD.SelectedPath;
                    listDir.Clear();
                    DirSearch(FBD.SelectedPath);
                    total.DirNameArchiv = listDir;


                    string[] files = Directory.GetFiles(FBD.SelectedPath, "*", SearchOption.AllDirectories);

                    List<FileIN> listFileIN = new List<FileIN>();
                    foreach (string s in files)
                    {
                        FileIN fin = new FileIN();
                        fin.Pathfile = s;

                        FileInfo tFile = new FileInfo(s);
                        fin.FilesInfo = tFile;

                        listFileIN.Add(fin);

                    }

                    total.FileArchiv = listFileIN;


                    //////////// Serialization

                    // create BinaryFormatter
                    BinaryFormatter formatter = new BinaryFormatter();

                    // отримуємо потік, куда будем записувати сериализованний обєкт

                    //MessageBox.Show("Select the location of the save serialize file");

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Dat file|*.dat";
                    saveFileDialog1.Title = "Save File";
                    //saveFileDialog1.ShowDialog();



                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string path_file_Serializ = saveFileDialog1.FileName;

                        using (FileStream fs = new FileStream(path_file_Serializ, FileMode.OpenOrCreate))
                        {
                            formatter.Serialize(fs, total);

                            MessageBox.Show("The object is serialized. Serialization file created - " + path_file_Serializ);
                        }


                    }

                }

                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }


            }

        }


        /// recursion to nested folders
        public void DirSearch(string sDir)
        {
                 if (listDir.Count == 0 )
                    {
                        string[] tfiles = Directory.GetDirectories(sDir);
                        foreach (string s in tfiles)
                            { listDir.Add(s); }
                    };
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetDirectories(d))
                    {
                        listDir.Add(f);
                    }
                    DirSearch(d);
                }
        }


        private void button2_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            Archiv total = new Archiv();

            BinaryFormatter formatter = new BinaryFormatter();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "Dat file|*.dat";


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = openFileDialog.FileName;

                    FolderBrowserDialog FBD = new FolderBrowserDialog();
                    MessageBox.Show("Select a folder to deserialize data");
                    string pathFolder;

                    if (FBD.ShowDialog() == DialogResult.OK)
                    {
                        pathFolder = FBD.SelectedPath;


                        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))

                        {
                            Archiv newArchiv = (Archiv)formatter.Deserialize(fs);

                            if (!File.Exists(newArchiv.path))
                            {


                                // create subfolder
                                foreach (string s in newArchiv.DirNameArchiv)
                                {
                                    Directory.CreateDirectory(s.Replace(newArchiv.path, pathFolder));
                                }

                                // create files
                                FileIN tfilein = new FileIN();
                                foreach (FileIN s in newArchiv.FileArchiv)
                                {
                                    string sit = (s.Pathfile).Replace(newArchiv.path, pathFolder);

                                    //Open the stream and read it back.
                                    using (FileStream fsArchiv = s.FilesInfo.OpenRead())
                                    {

                                      using (FileStream fstream = new FileStream(sit, FileMode.OpenOrCreate))
                                        {
                                            fsArchiv.CopyTo(fstream);
                                            //MessageBox.Show("Текст записан в файл");
                                        }

                                        }
                                    

                                }

                            }
                            else MessageBox.Show("Папка " + newArchiv.path + " десирализації існує");

                            MessageBox.Show("The object is diserialized");

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }


    }
}
