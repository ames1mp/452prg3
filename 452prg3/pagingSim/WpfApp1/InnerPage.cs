using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class InnerPage
    {

        int pId;
        PageTable textTable;
        PageTable dataTable;

        public InnerPage(int pId, PageTable textTable, PageTable dataTable)
        {
            this.pId = pId;
            this.textTable = textTable;
            this.dataTable = dataTable;

        }

        public PageTable TextTable { get => textTable; set => textTable = value; }
        public PageTable DataTable { get => dataTable; set => dataTable = value; }
    }

    public class PageTable {

        string dataType;
        int numPages;
        int[] table;

        public PageTable(string dataType, int numPages) {

            this.dataType = dataType;
            this.numPages = numPages;
            table = new int[numPages];
        }

        public int[] Table { get => table; set => table = value; }
        public string DataType { get => dataType; set => dataType = value; }
        public int NumPages { get => numPages; set => numPages = value; }
    }

}

