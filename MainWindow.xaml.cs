using System;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PershingLayoutReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = "";

        StringBuilder sbRowAdd = new StringBuilder();

        StringBuilder sbListToDT = new StringBuilder();

        StringBuilder sbCreateDT = new StringBuilder();

        StringBuilder sbCreateSQL = new StringBuilder();

        StringBuilder combinedSb = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
            txtEditor.Text = "1. Click Browse and open a pershing layout in a .txt format \n" +
                             "2. Once the file is displayed on the screen, click Process and wait \n" +
                             "3. From the drop-down menu, select one of the available views. If left at combined, the whole extraction will be shown \n";            
        }
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            
            sbRowAdd.Clear();
            sbListToDT.Clear();
            sbCreateDT.Clear();
            sbCreateSQL.Clear();
            combinedSb.Clear();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Text|*.txt|All|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
                filePath = openFileDialog.FileName;
            }
            Mouse.OverrideCursor = null;
        }

        private void btnProcessFile_Click(object sender, RoutedEventArgs e)
        {            
            Mouse.OverrideCursor = Cursors.Wait;

            sbRowAdd.Clear();
            sbListToDT.Clear();
            sbCreateDT.Clear();
            sbCreateSQL.Clear();
            combinedSb.Clear();

            if (filePath.Length > 0)
            {
                try
                {
                    string strLine = null;      //string used to hold the whole line unprocessed

                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        bool recordHeader = false;
                        bool recordFlag = false;      //Flag used to help with seperating the records     
                        int listCount = 0;
                        int rowCount = -1;

                        List<string> repeatedColumn = new List<string>();     //Used to check if column name is repeated                        

                        sbRowAdd.AppendLine();
                        sbRowAdd.AppendLine();
                        sbRowAdd.Append("/************************************************************************************************/");
                        sbRowAdd.AppendLine();
                        sbRowAdd.Append("FILE TO SUBSTRING");
                        sbRowAdd.AppendLine();
                        sbRowAdd.Append("/************************************************************************************************/");
                        sbRowAdd.AppendLine();
                        sbRowAdd.AppendLine();

                        sbListToDT.AppendLine();
                        sbListToDT.AppendLine();
                        sbListToDT.Append("/************************************************************************************************/");
                        sbListToDT.AppendLine();
                        sbListToDT.Append("LIST TO DATATABLE");
                        sbListToDT.AppendLine();
                        sbListToDT.Append("/************************************************************************************************/");
                        sbListToDT.AppendLine();
                        sbListToDT.AppendLine();

                        sbCreateDT.AppendLine();
                        sbCreateDT.AppendLine();
                        sbCreateDT.Append("/************************************************************************************************/");
                        sbCreateDT.AppendLine();
                        sbCreateDT.Append("CREATE DATATABLE");
                        sbCreateDT.AppendLine();
                        sbCreateDT.Append("/************************************************************************************************/");
                        sbCreateDT.AppendLine();
                        sbCreateDT.AppendLine();

                        sbCreateSQL.AppendLine();
                        sbCreateSQL.AppendLine();
                        sbCreateSQL.Append("/************************************************************************************************/");
                        sbCreateSQL.AppendLine();
                        sbCreateSQL.Append("CREATE SQL");
                        sbCreateSQL.AppendLine();
                        sbCreateSQL.Append("/************************************************************************************************/");
                        sbCreateSQL.AppendLine();
                        sbCreateSQL.AppendLine();




                        while ((strLine = reader.ReadLine()) != null)
                        {
                            // if(strLine.Contains("DETAIL RECORD"))
                            // Console.WriteLine(Regex.Replace(strLine.Substring(0, 14), @"\s+", ""));
                            string[] firstFourWords = strLine.Substring(0).Split(new char[] {' ', '\t' }).Take(6).ToArray();                          
                        
                            string recordName = "";

                            string combinedHeader = string.Join("", firstFourWords);

                            if (combinedHeader.Contains("DETAILRECORD") && !combinedHeader.Contains("ALTERNATEDETAILRECORD"))
                            {
                                recordName = Regex.Match(strLine, @"[^ \t]+[ \t]+[^ \t]+[ \t]+[^ \t]+", RegexOptions.Compiled).ToString();

                                recordHeader = true;

                                recordName = ToTitleCase(recordName);
                                recordName = Regex.Replace(recordName, @"\s+", "");
                                
                                rowCount++;
                            }
                            else if (combinedHeader.Contains("RECORD") && !combinedHeader.Contains("ALTERNATEDETAILRECORD") && !combinedHeader.Contains("ALTERNATERECORD") 
                                    && !combinedHeader.Contains("ASSETLEVELRECORD") && !combinedHeader.Contains("TRAILERRECORD") && !combinedHeader.Contains("HEADER"))
                            {
                                recordName = Regex.Match(strLine, @"[^ \t]+[ \t]+[^ \t]+", RegexOptions.Compiled).ToString();

                                recordHeader = true;

                                recordName = ToTitleCase(recordName);
                                recordName = Regex.Replace(recordName, @"\s+", "");

                                rowCount++;
                            }
                            else if(combinedHeader.Contains("ALTERNATEDETAILRECORD"))
                            {
                                recordName = Regex.Match(strLine, @"[^ \t]+[ \t]+[^ \t]+[ \t]+[^ \t]+[ \t]+[^ \t]+", RegexOptions.Compiled).ToString();

                                recordHeader = true;

                                recordName = ToTitleCase(recordName);
                                recordName = Regex.Replace(recordName, @"\s+", "");
                                
                                rowCount++;
                            }
                            else if(combinedHeader.Contains("ALTERNATERECORD"))
                            {
                                recordName = Regex.Match(strLine, @"[^ \t]+[ \t]+[^ \t]+[ \t]+[^ \t]", RegexOptions.Compiled).ToString();

                                recordHeader = true;

                                recordName = ToTitleCase(recordName);
                                recordName = Regex.Replace(recordName, @"\s+", "");

                                
                                rowCount++;
                            }
                            else if (combinedHeader.Contains("ASSETLEVELRECORD"))
                            {
                                recordName = Regex.Match(strLine, @"[^ \t]+[ \t]+[^ \t]+[ \t]+[^ \t]+[ \t]+[^ \t]+[ \t]+[^ \t]+", RegexOptions.Compiled).ToString();

                                recordHeader = true;

                                recordName = ToTitleCase(recordName);
                                recordName = Regex.Replace(recordName, @"\s+", "");

                                rowCount++;
                            }
                            else if(combinedHeader.Contains("TRAILERRECORD"))
                            {
                                recordFlag = false;
                                recordHeader = false;                            
                            }
                            else
                            {
                                recordHeader = false;
                            }
                                                      
                            

                            if (recordHeader)   //Decect Record flag in string line
                            {                               

                                if (recordFlag)
                                {
                                    sbRowAdd.Append("/************************************************************************************************/");
                                    sbRowAdd.AppendLine();
                                    sbRowAdd.AppendLine();
                                    sbRowAdd.AppendLine();

                                    sbListToDT.Append("/************************************************************************************************/");
                                    sbListToDT.AppendLine();
                                    sbListToDT.AppendLine();
                                    sbListToDT.AppendLine();
                              
                                    sbCreateDT.AppendLine();
                                    sbCreateDT.Append("/************************************************************************************************/");
                                    sbCreateDT.AppendLine();
                                    sbCreateDT.AppendLine();
                                    sbCreateDT.AppendLine();

                                    sbCreateSQL.Append(");");
                                    sbCreateSQL.AppendLine();
                                    sbCreateSQL.Append("/************************************************************************************************/");
                                    sbCreateSQL.AppendLine();
                                    sbCreateSQL.AppendLine();
                                    sbCreateSQL.AppendLine();
                                  
                                }

                                repeatedColumn.Clear();     //Clear repeats for each record

                                sbRowAdd.Append("//" + recordName);
                                sbRowAdd.AppendLine();
                                sbRowAdd.Append("/************************************************************************************************/");
                                sbRowAdd.AppendLine();

                                sbRowAdd.Append("\t" + "rowArr.Add(\"LineNumber" + "_" + rowCount + "\", LineNumber.ToString());" + "\t" + "\t" + "//LineNumber");
                                sbRowAdd.AppendLine();




                                sbListToDT.Append("//" + recordName);
                                sbListToDT.AppendLine();
                                sbListToDT.Append("/************************************************************************************************/");
                                sbListToDT.AppendLine();

                                sbListToDT.Append("\t" + "row" + rowCount + "[\"LineNumber" + rowCount + "\"] = rowArr[\"LineNumber" + "_" + rowCount + "\"];");
                                sbListToDT.AppendLine();



                                sbCreateDT.Append("//" + recordName);
                                sbCreateDT.AppendLine();
                                sbCreateDT.Append("/************************************************************************************************/");
                                sbCreateDT.AppendLine();

                                sbCreateDT.Append("\t" + "dc" + rowCount + " = new DataColumn();");
                                sbCreateDT.AppendLine();
                                sbCreateDT.Append("\t" + "dc" + rowCount + ".DataType = Type.GetType(\"System.String\");");
                                sbCreateDT.AppendLine();
                                sbCreateDT.Append("\t" + "dc" + rowCount + ".ColumnName = " + "\"LineNumber" + rowCount + "\";");
                                sbCreateDT.AppendLine();
                                sbCreateDT.Append("\t" + "dt" + rowCount + ".Columns.Add(dc" + rowCount + ");");
                                sbCreateDT.AppendLine();
                                sbCreateDT.AppendLine();




                                sbCreateSQL.Append("//" + recordName);
                                sbCreateSQL.AppendLine();
                                sbCreateSQL.Append("/************************************************************************************************/");
                                sbCreateSQL.AppendLine();
                                sbCreateSQL.Append("CREATE TABLE " + recordName + "(");
                                sbCreateSQL.AppendLine();

                                sbCreateSQL.Append("\t" + "LineNumber" + rowCount + " varchar(255),");
                                sbCreateSQL.AppendLine();

                                recordFlag = true;
                                listCount++;
                            }


                            if (recordFlag)
                            {
                                string subStrPos = "";      //Set initial var values
                                string subStrPic = ""; ;
                                string columnName = "";
                                string[] singleCharacterCheck = { "" };


                                if (strLine.Length >= 15)        //Get the first 15 chars from each line and get only the numbers
                                {
                                    subStrPos = Regex.Replace(strLine.Substring(0, 15), "[^0-9]", "");
                                }

                                if (subStrPos.Length == 6 || subStrPos.Length == 8)   //If the numbers retreived equal to six, then thats the positon given by the file
                                {
                                    //Position/Start of string
                                    /**************************************************************************************/
                                    int strStart = Int32.Parse(subStrPos.Substring(0, subStrPos.Length / 2));  //Set the start of the substrim trim    
                                    strStart--;
                                    /**************************************************************************************/


                                    //Get Picture/Name of column and append to the string builder
                                    /**************************************************************************************/
                                    subStrPic = strLine.Substring(15, 18);     //Get 18 characters from position 16 to get the picture number

                                    subStrPic = Regex.Replace(subStrPic, @"\s+", "");   //Remove any whitespace

                                    if (subStrPic.Length <= 10)       //If the picture is not split (no decimal position) then go here
                                    {
                                        //Return the number in the parentheses 
                                        subStrPic = Regex.Match(subStrPic.Substring(0, subStrPic.Length), @"\(([^)]*)\)").Groups[1].Value;

                                        //Convert to int(also removes any leading zeroes)
                                        int strEnd = Int32.Parse(subStrPic);

                                        //Split column at specified characters
                                        columnName = strLine.Split(',', ';')[0].Substring(28);

                                        //Convert text to title case
                                        columnName = ToTitleCase(columnName);

                                        //Remove first word
                                        columnName = Regex.Replace(columnName, @"^([^\s]+)\s+", "");

                                        //Remove whitespace
                                        columnName = Regex.Replace(columnName, @"\s+", "");

                                        string currentColumn = columnName;      //Temp hold of current column string

                                        if (repeatedColumn.Contains(currentColumn))     //Check for repeats and get count
                                        {
                                            columnName += repeatedColumn.Where(x => x.Equals(currentColumn)).Count();
                                        }

                                        repeatedColumn.Add(currentColumn);

                                        //Append to the stringbuilder         
                                        
                                        sbRowAdd.Append("\t" + "rowArr.Add(\"" + columnName + "_" + rowCount + "\", strLine.Substring(" + strStart + ", " + strEnd + "));" + "\t" + "\t" + "//" + columnName);
                                        sbRowAdd.AppendLine();

                                        sbListToDT.Append("\t" + "row" + rowCount + "[\"" + columnName + "\"] = rowArr[\"" + columnName + "_" + rowCount + "\"];");
                                        sbListToDT.AppendLine();
                                       

                                        sbCreateDT.Append("\t" + "dc" + rowCount + " = new DataColumn();");
                                        sbCreateDT.AppendLine();
                                        sbCreateDT.Append("\t" + "dc" + rowCount + ".DataType = Type.GetType(\"System.String\");");
                                        sbCreateDT.AppendLine();
                                        sbCreateDT.Append("\t" + "dc" + rowCount + ".ColumnName = " + "\"" + columnName + "\"" + ";");
                                        sbCreateDT.AppendLine();
                                        sbCreateDT.Append("\t" + "dt" + rowCount + ".Columns.Add(dc" + rowCount + ");");
                                        sbCreateDT.AppendLine();
                                        sbCreateDT.AppendLine();

                                        sbCreateSQL.Append("\t" + columnName + " varchar(255),");
                                        sbCreateSQL.AppendLine();

                                        listCount++;
                                    }
                                    else if (subStrPic.Length >= 11)   //If the picture is split (has decimal positions) then go here
                                    {
                                        string strLeft = "";
                                        string strRight = "";

                                        int left = 0;
                                        int right = 0;

                                        strLeft = Regex.Match(subStrPic.Substring(0, 7), @"\(([^)]*)\)").Groups[1].Value;       //Returns number in parenthesis only

                                        strRight = Regex.Match(subStrPic.Substring(6), @"\(([^)]*)\)").Groups[1].Value;         //Returns number in parenthesis only

                                        //Convert to int(also removes any leading zeroes)
                                        left = Int32.Parse(strLeft);
                                        right = Int32.Parse(strRight);

                                        //Split column at specified characters
                                        columnName = strLine.Split(',', ';')[0].Substring(28);

                                        //Convert text to title case
                                        columnName = ToTitleCase(columnName);

                                        //Remove first word
                                        columnName = Regex.Replace(columnName, @"^([^\s]+)\s+", "");

                                        //Remove whitespace
                                        columnName = Regex.Replace(columnName, @"\s+", "");

                                        string currentColumn = columnName;      //Temp hold of current column string

                                        if (repeatedColumn.Contains(currentColumn))     //Check for repeats and get count
                                        {
                                            columnName += repeatedColumn.Where(x => x.Equals(currentColumn)).Count();
                                        }

                                        repeatedColumn.Add(currentColumn);     
                                        

                                        sbRowAdd.Append("\t" + "rowArr.Add(\"" + columnName + "_" + rowCount + "\", strLine.Substring(" + strStart + ", " + left + ").TrimStart(new Char[] { '0' }) + '.' + strLine.Substring(" + (strStart += left) + ", " + right + "));" + "\t" + "\t" + "//" + columnName);
                                        sbRowAdd.AppendLine();

                                        sbListToDT.Append("\t" + "row" + rowCount + "[\"" + columnName + "\"] = rowArr[\"" + columnName + "_" + rowCount + "\"];");
                                        sbListToDT.AppendLine();

                                        sbCreateDT.Append("\t" + "dc" + rowCount + " = new DataColumn();");
                                        sbCreateDT.AppendLine();
                                        sbCreateDT.Append("\t" + "dc" + rowCount + ".DataType = Type.GetType(\"System.String\");");
                                        sbCreateDT.AppendLine();
                                        sbCreateDT.Append("\t" + "dc" + rowCount + ".ColumnName = " + "\"" + columnName + "\"" + ";");
                                        sbCreateDT.AppendLine();
                                        sbCreateDT.Append("\t" + "dt" + rowCount + ".Columns.Add(dc" + rowCount + ");");
                                        sbCreateDT.AppendLine();
                                        sbCreateDT.AppendLine();

                                        sbCreateSQL.Append("\t" + columnName + " varchar(255),");
                                        sbCreateSQL.AppendLine();

                                        listCount++;
                                    }
                                    /**************************************************************************************/
                                }
                            }
                        }
                        
                        reader.Close();
                                            
                        sbRowAdd.Append("/************************************************************************************************/");
                        sbRowAdd.AppendLine();
                        sbRowAdd.AppendLine();
                        sbRowAdd.AppendLine();

                        sbListToDT.Append("/************************************************************************************************/");
                        sbListToDT.AppendLine();
                        sbListToDT.AppendLine();
                        sbListToDT.AppendLine();
                  
                        sbCreateDT.AppendLine();
                        sbCreateDT.Append("/************************************************************************************************/");
                        sbCreateDT.AppendLine();
                        sbCreateDT.AppendLine();
                        sbCreateDT.AppendLine();

                        sbCreateSQL.Append(");");
                        sbCreateSQL.AppendLine();
                        sbCreateSQL.Append("/************************************************************************************************/");
                        sbCreateSQL.AppendLine();
                        sbCreateSQL.AppendLine();
                        sbCreateSQL.AppendLine();

                        combinedSb.Append(sbRowAdd.ToString());
                        combinedSb.Append(sbListToDT.ToString());
                        combinedSb.Append(sbCreateDT.ToString());
                        combinedSb.Append(sbCreateSQL.ToString());


                        txtEditor.Text = combinedSb.ToString();

                        if (cmbSelect.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() != null)
                        {
                            switch (cmbSelect.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
                            {
                                case "Combined":
                                    if (combinedSb.ToString() != "")
                                    {
                                        txtEditor.Text = combinedSb.ToString();
                                    }
                                    break;
                                case "StrToList":
                                    if (sbRowAdd.ToString() != "")
                                    {
                                        txtEditor.Text = sbRowAdd.ToString();
                                    }
                                    break;
                                case "ListToDt":
                                    if (sbListToDT.ToString() != "")
                                    {
                                        txtEditor.Text = sbListToDT.ToString();
                                    }
                                    break;
                                case "DtModel":
                                    if (sbCreateDT.ToString() != "")
                                    {
                                        txtEditor.Text = sbCreateDT.ToString();
                                    }
                                    break;
                                case "SQL":
                                    if (sbCreateSQL.ToString() != "")
                                    {
                                        txtEditor.Text = sbCreateSQL.ToString();
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            Mouse.OverrideCursor = null;
        }

        private bool handle = true;
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (handle) Handle();
            handle = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handle = !cmb.IsDropDownOpen;
            Handle();
        }

        private void Handle()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (cmbSelect.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() != null)
            {             
                switch (cmbSelect.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
                {
                    case "Combined":
                        if (combinedSb.ToString() != "")
                        {
                            txtEditor.Text = combinedSb.ToString();
                        }
                        break;
                    case "StrToList":
                        if (sbRowAdd.ToString() != "")
                        {
                            txtEditor.Text = sbRowAdd.ToString();
                        }
                        break;
                    case "ListToDt":
                        if (sbListToDT.ToString() != "")
                        {
                            txtEditor.Text = sbListToDT.ToString();
                        }
                        break;
                    case "DtModel":
                        if (sbCreateDT.ToString() != "")
                        {
                            txtEditor.Text = sbCreateDT.ToString();
                        }
                        break;
                    case "SQL":
                        if (sbCreateSQL.ToString() != "")
                        {
                            txtEditor.Text = sbCreateSQL.ToString();
                        }
                        break;
                }
            }
            Mouse.OverrideCursor = null;
        }

        public static string ToTitleCase(string str)
        {
            Regex pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return new CultureInfo("en-US", false)
            .TextInfo
            .ToTitleCase(
                string.Join(" ", pattern.Matches(str)).ToLower()
            );
        }

    }
}

