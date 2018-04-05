/// Mike Ames - CIS 452 GVSU Winter 2018. Professor: Greg Wolffe
/// This program demonstrates knowledge of memory management techniques by simulating a simple memory
/// management scheme. All data structure choices and logic are by me, but I did reference numerous Stack Overflow articles,
/// youtube videos, and Microsoft articles regarding the specifics of WPF.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1
{
    class OS
    {

        int physMem = 4096;
        int pageSize = 512;
        int numFrames;
        List<PhysicalMViewRow> physicalMListRowData;
        Dictionary<string, List<PageCellRow>> cellRowDict;

        string[] frameTable;
        //Since we are not actually accessing pages by address I felt that a Map was the most analgous data structure. 
        //We give a key and get a location, just as we would give an address to get a location.
        Dictionary<int, InnerPage> outerPageTable;
        List<PCB> pcbList;


        public OS(List<PhysicalMViewRow> physicalMListRowData, Dictionary<string, List<PageCellRow>> cellRowDict)
        {
            this.physicalMListRowData = physicalMListRowData;
            this.cellRowDict = cellRowDict;
            numFrames = physMem / pageSize;

            outerPageTable = new Dictionary<int, InnerPage>();
            pcbList = new List<PCB>();
            frameTable = new string[numFrames];
        }

        public void executeNextEvent(int pId, int textSegmentSize, int dataSegmentSize)
        {
            //Check to see if a PCB already exists for the process.
            if (!processExists(pId))
            {

                loadProcess(pId, textSegmentSize, dataSegmentSize);
                //Create a PCB and add it to the PCB list.
                pcbList.Add(new PCB(pId, textSegmentSize, dataSegmentSize, outerPageTable[pId]));
            }
            //If a PCB already exists for this process we know the event must be to halt the process.
            else
            {
                haltProcess(pId);
            }
        }

        public bool processExists(int pId)
        {
            foreach(var pcb in pcbList)
            {
                if (pcb.PId == pId)
                    return true;
            }
            return false;
        }

        public void loadProcess(int pId, int textSegmentSize, int dataSegmentSize)
        {
            int numTextSegmentPages = (int)Math.Ceiling((double)textSegmentSize / pageSize);
            int numDataSegmentPages = (int)Math.Ceiling((double)dataSegmentSize / pageSize);

            //Create page tables for each segment of the process.
            PageTable textTable = new PageTable("text", numTextSegmentPages);
            PageTable dataTable = new PageTable("data", numDataSegmentPages);

            //Create an inner page to hold the two page tables.
            InnerPage innerPage;

            //Find a frame for each text segment and update the page table and the frame table.
            for (int i = 0; i < numTextSegmentPages; ++i)
            {
                for (int j = 0; j < numFrames; ++j)
                {
                    //See if frame is empty.
                    if ( string.IsNullOrEmpty(frameTable[j]))
                    {
                        //Assign the page to an empty frame.
                        frameTable[j] = "P" + pId.ToString() + " Text Page " + i.ToString();
                        physicalMListRowData[j].frameInfoXAML = frameTable[j];
                        textTable.Table[i] = j;
                        break;
                    }
                }

            }

            //Find a frame for each data segment and update the page table and the frame table.
            for (int i = 0; i < numDataSegmentPages; ++i)
            {
                for (int j = 0; j < numFrames; ++j)
                {
                    //See if frame is empty.
                    if (string.IsNullOrEmpty(frameTable[j]))
                    {
                        //Assign the page to an empty frame.
                        frameTable[j] = "P" + pId.ToString() + " Data Page " + i.ToString();
                        physicalMListRowData[j].frameInfoXAML = frameTable[j];
                        dataTable.Table[i] = j;
                        break;
                    }
                }

            }

            //For GUI
            String textKey = pId.ToString() + "0";
            String dataKey = pId.ToString() + "1";

            makeRows(textKey, textTable);
            makeRows(dataKey, dataTable);
            //End for GUI

            //Create and populate the inner page.
            innerPage = new InnerPage(pId, textTable, dataTable);

            //Add the inner page to the outer page table. 
            outerPageTable.Add(pId, innerPage);
        }

        public void haltProcess(int pId)
        {
            //Get a reference to the PCB node for the process.
            PCB PCBNode = findPCBNode(pId);

            //Get references to the processes page tables.
            PageTable textTable = PCBNode.InnerPage.TextTable;
            PageTable dataTable = PCBNode.InnerPage.DataTable;

            //Go through the text page table.
            for (int i = 0; i < PCBNode.NumTextPages; ++i)
            {
                //Find the frame number for each page in the text page table.
                int frame = textTable.Table[i];

                //Clear the frame.
                frameTable[frame] = "";
                physicalMListRowData[frame].frameInfoXAML = frameTable[frame];
            }

            //Go through the data page table.
            for (int i = 0; i < PCBNode.NumDataPages; ++i)
            {
                //Find the frame number for each page in the data page table.
                int frame = dataTable.Table[i];

                //Clear the frame.
                frameTable[frame] = "";
                physicalMListRowData[frame].frameInfoXAML = frameTable[frame];
            }

            //Remove the inner page table from the outer page table.
            outerPageTable.Remove(pId);

            //Remove the PCB from the PCB list. This removes all references to the page tables, so they should be garbage collected. 
            pcbList.Remove(PCBNode);

            String XAMLCellText = "cell" +  pId.ToString() + "0";
            String XAMLCellData = "cell" + pId.ToString() + "1";
            DataGrid gridText = (DataGrid)App.Current.MainWindow.FindName(XAMLCellText);
            DataGrid dataText = (DataGrid)App.Current.MainWindow.FindName(XAMLCellData);
            gridText.Opacity = 0.0;
            dataText.Opacity = 0.0;

        }

        public PCB findPCBNode(int pId)
        {
           foreach (var pcb in pcbList)
            {
                if (pcb.PId == pId)
                    return pcb;
            }

            return null;
        }

        public void makeRows(string key, PageTable pageTable) {

            for (int i = 0; i < pageTable.NumPages; ++i) {
                int page = i;
                int frame = pageTable.Table[i];

                PageCellRow row = new PageCellRow(page, frame);
                cellRowDict[key].Add(row);
            }
            String XAMLCellName = "cell" + key;
            updateCellGUI(XAMLCellName, key);
        }

        public void updateCellGUI(string XAMLCellName, string key)
        {
            foreach (var row in cellRowDict[key])
            {

                DataGrid grid = (DataGrid)App.Current.MainWindow.FindName(XAMLCellName);
                grid.Items.Add(row);
                grid.Items.Refresh();


            }

        }
    }
}
