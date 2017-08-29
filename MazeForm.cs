using MazeSolution.Entities;
using MazeSolution.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MazeSolution
{
    public partial class MazeSolution : Form
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        String  fileName = String.Empty; 
        public MazeSolution()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            log.Info("Getting file");
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|Tsv (*.tsv)|*.tsv";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK) // Test result.
            {
                fileName = openFileDialog1.FileName;
                textBox2.Text = fileName;
                log.Info("Getting file done!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fileName.Length != 0)
            {
                using (TextReader tr = File.OpenText(fileName))
                {
                    try
                    {
                        string x0 = textBox4.Text;
                        string y0 = textBox3.Text;
                        string x1 = textBox5.Text;
                        string y1 = textBox6.Text;
                        
                        IFileReading ifl = new FileReading();
                        int[,] matrx = ifl.CreateMatrixFromFile(tr);

                            if (validateIfIsNumeric(x0, matrx.GetLength(0)) && validateIfIsNumeric(x1, matrx.GetLength(0)) && validateIfIsNumeric(y0, matrx.GetLength(1)) && validateIfIsNumeric(y1, matrx.GetLength(1)))
                            {
                                if ((matrx[Convert.ToInt32(x0), Convert.ToInt32(y0)] != 0) && (matrx[Convert.ToInt32(x1), Convert.ToInt32(y1)] != 0))
                                {

                                    IAStartPathFinding aspf = new AStarPathfinding();
                                    List<Location> closedList = aspf.GetPath(matrx, Convert.ToInt32(x0), Convert.ToInt32(y0), Convert.ToInt32(x1), Convert.ToInt32(y1));

                                    String output = "";
                                    foreach (var p in closedList)
                                    {
                                        output += String.Concat("[", p.X.ToString(), ",", p.Y.ToString(), "]", ",");
                                    }
                                    textBox1.Text = "[" + output.Substring(0, output.Length - 1) + "]";
                                    log.Info("Printing maze solution");
                                    PaintMazeGrid(matrx, closedList, Convert.ToInt32(x0), Convert.ToInt32(y0), Convert.ToInt32(x1), Convert.ToInt32(y1));
                                    log.Info("All done!");
                                }
                                else
                                {
                                    MessageBox.Show("Please enter a valid Coords", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                        }
                        else
                        {
                            MessageBox.Show("Coords must be numeric and size must be a non negative value <= to matrix lenght", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An unexpected error have been. Please try again", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                        log.Error("Unexpected error: "+ex.StackTrace);
                    }
                }
            }
            else
            {
                MessageBox.Show("File can't be empty", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }


        /// <summary>
        /// paint maze grid
        /// </summary>
        /// <param name="matrx">Maze grid matrix</param>
        /// <param name="closedList">closed list path</param>
        /// <param name="x0">initial point c</param>
        /// <param name="y0">initial point y</param>
        /// <param name="x1">end point x</param>
        /// <param name="y1">end point y</param>
        private void PaintMazeGrid(int[,] matrx, List<Location> closedList, int x0, int y0, int x1, int y1)
        {
            panel1.Refresh();

            foreach (var p in closedList)
            {
                matrx[p.X, p.Y] = 4;
            }

            matrx[x0, y0] = 2;
            matrx[x1, y1] = 3;

            Graphics gr = panel1.CreateGraphics();
            int xLines = matrx.GetLength(0);
            int yLines = matrx.GetLength(1);
            Pen pen = new Pen(Color.Black, 1);
            float x = 0f;
            float y = 0f;

            float xSpace = panel1.Width / yLines;
            float ySpace = panel1.Height / xLines;

            Font font = new Font("Arial", 10);

            x = 0f;
            y = 0f;
            SolidBrush myBrush = new SolidBrush(System.Drawing.Color.Red);

            for (int i = 0; i < xLines; i++)
            {
                for (int j = 0; j < yLines; j++)
                {
                    gr.FillRectangle(colorCell(matrx[i, j]), new Rectangle(Convert.ToInt32(x) + 1, Convert.ToInt32(y) + 1, Convert.ToInt32(xSpace) - 2, Convert.ToInt32(ySpace) - 2));
                    x += xSpace;
                }
                y += ySpace;
                x = 0;
            }
        }

        /// <summary>
        /// return brush color for a given number
        /// </summary>
        /// <param name="value">cell value</param>
        /// <returns>brush color</returns>
        private SolidBrush colorCell(int value)
        {
            SolidBrush myBrush = new SolidBrush(System.Drawing.Color.Red);
            switch (value)
            {
                case 0:
                    myBrush = new SolidBrush(System.Drawing.Color.Black);
                    break;
                case 2:
                    myBrush = new SolidBrush(System.Drawing.Color.Green);
                    break;
                case 3:
                    myBrush = new SolidBrush(System.Drawing.Color.Blue);
                    break;
                case 4:
                    myBrush = new SolidBrush(System.Drawing.Color.Red);
                    break;
                default:
                    myBrush = new SolidBrush(System.Drawing.Color.White);
                    break;
            }

            return myBrush;

        }

        /// <summary>
        /// Validates if a text is a number value type
        /// </summary>
        /// <param name="text">text value</param>
        /// <param name="size">matrix lenght</param>
        /// <returns>true if a text is numeric</returns>
        private bool validateIfIsNumeric(string text, int size)
        {
            if (Regex.IsMatch(text, @"^\d+$"))
            {
                if (Convert.ToInt32(text) <= size - 1 && Convert.ToInt32(text) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void MazeSolution_Load(object sender, EventArgs e)
        {

        }
    }
}
