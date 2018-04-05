/// Mike Ames - CIS 452 GVSU Winter 2018. Professor: Greg Wolffe
/// This program demonstrates knowledge of memory management techniques by simulating a simple memory
/// management scheme. All data structure choices and logic are by me, but I did reference numerous Stack Overflow articles,
/// youtube videos, and Microsoft articles regarding the specifics of WPF.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class PCB
    {

        const int pageSize = 512;


        int pId;
        int textSize;
        int dataSize;

        int numTextPages;
        int numDataPages;
        int totalNumPages;
        InnerPage innerPage;

        public PCB(int pId, int textSize, int dataSize, InnerPage innerPage) {

            this.pId = pId;
            this.textSize = textSize;
            this.dataSize = dataSize;
            this.innerPage = innerPage;
            this.numTextPages = (int)Math.Ceiling((double)textSize / pageSize);
            this.numDataPages = (int)Math.Ceiling((double)dataSize / pageSize);

            totalNumPages = numTextPages + numDataPages;

        }

        public int PId { get => pId; set => pId = value; }
        public int NumTextPages { get => numTextPages; set => numTextPages = value; }
        public int NumDataPages { get => numDataPages; set => numDataPages = value; }
        internal InnerPage InnerPage { get => innerPage; set => innerPage = value; }
    }
}
