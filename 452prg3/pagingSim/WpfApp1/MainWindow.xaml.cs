/// Mike Ames - CIS 452 GVSU Winter 2018. Professor: Greg Wolffe
/// This program demonstrates knowledge of memory management techniques by simulating a simple memory
/// management scheme. All data structure choices and logic are by me, but I did reference numerous Stack Overflow articles,
/// youtube videos, and Microsoft articles regarding the specifics of WPF.



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        
        int selectedInstruction = 0;
        private int[][] fileData;
        int numInstructions;
        

        //Used to populate the instruction list in the view.
        List<InstructionViewRow> instructionListRowData;
        List<PhysicalMViewRow> physicalMListRowData;
        Dictionary<string, List<PageCellRow>> cellRowDict;

        OS os;

        public MainWindow()
        {
            
            instructionListRowData = new List<InstructionViewRow>();
            physicalMListRowData = new List<PhysicalMViewRow>();
            cellRowDict = new Dictionary<string, List<PageCellRow>>();

            InitializeComponent();
        }

        private void readFile(string fileName)
        {
            os = new OS(physicalMListRowData, cellRowDict);
            int counter = 0;
            int numLines = numInstructions = File.ReadLines(fileName).Count();
            fileData = new int[numLines][];
            string line;

            StreamReader file = new StreamReader(fileName);

            //Read file line by line
            while ((line = file.ReadLine()) != null)
            {

                string[] tokens = line.Split(' ');

                //Line contains a Halt command.
                if (tokens.Length == 2)
                {
                    //Create a new array and add it to the File Data jagged array
                    fileData[counter] = new int[1];
                    //Parse string to int
                    Int32.TryParse(tokens[0], out int temp);
                    //Assign the array to the File Data jagged array
                    fileData[counter][0] = temp;

                    //Store instruction data for the GUI
                    instructionListRowData.Add(new InstructionViewRow(fileData[counter][0], "Halt", ""));
                    
                }
                else
                {
                    fileData[counter] = new int[3];
                    for(int i = 0; i < 3; ++i)
                    {
                        Int32.TryParse(tokens[i], out int temp);
                        fileData[counter][i] = temp;
                    }
                    //Store instruction data for the GUI
                    instructionListRowData.Add(new InstructionViewRow(fileData[counter][0], fileData[counter][1].ToString(), fileData[counter][2].ToString()));
                }

                counter++;
            }

            file.Close();
           
            populateGUIInstructionTable();
            createCellRowDict();
        }

        private void populateGUIInstructionTable()
        {
            foreach (var row in instructionListRowData)
            {
                instructionsXAML.Items.Add(row);
            }

            instructionsXAML.SelectedItem = instructionListRowData[0];
            
        }

        private void open_clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                readFile(openFileDialog.FileName);
            }


            if (selectedInstruction > 0)
            {
                physicalMListRowData.Clear();
                PhysicalMemXAMLTable.Items.Refresh();
                populatePhysicalMView();
                PhysicalMemXAMLTable.Items.Refresh();

            }
            else {
                populatePhysicalMView();
                PhysicalMemXAMLTable.Items.Refresh();
            }
                
            selectedInstruction = 0;
            nextButton.IsEnabled = true;

        }

        private void exit_clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void next_clicked(object sender, RoutedEventArgs e)
        {
           
            

            if (selectedInstruction >= numInstructions) {
                nextButton.IsEnabled = false;
                return;
            }

            instructionsXAML.SelectedItem = instructionListRowData[selectedInstruction];

            //Get the pId for the next event.
            int pId = fileData[selectedInstruction][0];

            int textSegmentSize;
            int dataSegmentSize;

            if (fileData[selectedInstruction].Length == 1)
            {
                textSegmentSize = 0;
                dataSegmentSize = 0;
            }
            else
            {
                textSegmentSize = fileData[selectedInstruction][1];
                dataSegmentSize = fileData[selectedInstruction][2];
            }
           
            os.executeNextEvent(pId, textSegmentSize, dataSegmentSize);
            

            ++selectedInstruction;

            PhysicalMemXAMLTable.Items.Refresh();
        }

        public void populatePhysicalMView()
        {
            for (int i = 0; i < 8; ++i)
            {
                physicalMListRowData.Add(new PhysicalMViewRow(i, ""));
            }

            foreach (var row in physicalMListRowData)
            {
                PhysicalMemXAMLTable.Items.Add(row);
            }
        }

        public void createCellRowDict() {

            for (int i = 0; i < 5; ++i) {

                for (int j = 0; j < 5; ++j) {

                    string key = i.ToString() + j.ToString();


                    cellRowDict.Add(key, new List<PageCellRow>());

                }
            }

        }

        
    }


    //Used to bind data to the view
    public class InstructionViewRow
    {
        public int pId { get; set; }
        public string text { get; set; }
        public string data { get; set; }

        public InstructionViewRow(int pId, string text, string data) {
            this.pId = pId;
            this.text = text;
            this.data = data;
        }
      
    }

    //Used to bind data to the view
    public class PhysicalMViewRow 
    {
        public string frameNumXAML { get; set; }
        public string frameInfoXAML { get; set; }
        public PhysicalMViewRow(int frameNumXAML, string frameInfoXAML)
        {
            this.frameNumXAML = "Frame " + frameNumXAML.ToString();
            this.frameInfoXAML = frameInfoXAML;
        }
    }

    //Used to bind data to the view
    public class PageCellRow
    {
        public string title { get; set; }

        public int page { get; set; }
        public int frame { get; set; }

        public PageCellRow(int page, int frame)
        {
            this.page = page;
            this.frame = frame;
        }

    }

}
