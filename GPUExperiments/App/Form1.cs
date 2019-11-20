using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPUExperiments
{
    public partial class Form1 : Form
    {
	    private GLView view;
        public Form1()
        {
            InitializeComponent();
			view = new GLView(glControl1);
        }
		

        private void btNext_Click(object sender, EventArgs e)
        {

        }
    }
}
