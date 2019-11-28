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
	    private GLOneTriangle _oneTriangleView;
        private GLView _view;
        private GLHexGrid _hexView;
        private GLPolyDraw _polyDrawView;

        public Form1()
        {
            InitializeComponent();
            //_oneTriangleView = new GLOneTriangle(glControl1);
            //_view = new GLView(glControl1);
            //_hexView = new GLHexGrid(glControl1);
            _polyDrawView = new GLPolyDraw(glControl1);
        }
		

        private void btNext_Click(object sender, EventArgs e)
        {

        }
    }
}
