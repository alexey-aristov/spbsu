﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mathematics.Delegates;
using DynamicCompiling;
using Mathematics.Intergration;
using Mathematics;

namespace Graph {
	public partial class GraphDynamicType : Graph {

		public double t0 {
			get;
			set;
		}
		public Dictionary<string , double> f0 {
			get;
			set;
		}
		public Dictionary<string , functionD> functionsD {
			get;
			set;
		}
		public bool UseDynamicFunctions {
			get;
			set;
		}

		public Dictionary<string , List<double>> Solutions {
			get;
			set;
		}

		public IntegrationType IntegrationType {
			get;
			set;
		}
		public Dictionary<string , double> Parameters {
			get;
			set;
		}
		protected override void calculate () {
			//base.calculate ();
			if ( UseDynamicFunctions ) {
				Dictionary<string , List<double>> solution = new Dictionary<string , List<double>> () ;
				switch ( this.IntegrationType ) {
					case IntegrationType.RungeKutta4:
						solution = RungeKutta.Integrate4 ( this.functionsD , t0 , f0,Parameters );
						break;
					case IntegrationType.EulerMethod:
						solution = Euler.Integrate ( this.functionsD , t0 , f0,Parameters);
						break;
					case Mathematics.Intergration.IntegrationType.EulerMethodSymplectic:
						solution = Euler.IntegrateSymplectic ( this.functionsD , t0 , f0,Parameters );
						break;
					case Mathematics.Intergration.IntegrationType.Iterative:
						solution = Iterative.Integrate ( this.functionsD , t0 , f0 , Parameters );
						break;
					case Mathematics.Intergration.IntegrationType.PoincareMap:
						solution = Poincare.Calculate ( this.functionsD , t0 , f0 , Parameters );
						break;

					default: break;
				}
				this.Solutions = solution;
				dataX = solution[this.AxisXlabel].ToArray ();
				dataY = solution[this.AxisYlabel].ToArray ();
			}
			
		}
		public void InitFunctionsD ( Dictionary<string , string> functions, Dictionary<string , double> parameters) {
			this.UseDynamicFunctions = true;
			Compilator compilator = new Compilator ( functions , parameters );
			this.Parameters = parameters;
			this.functionsD = compilator.GetFuncs ();
		}

		public void SetAxisToShow (string x="t", string y="x") {
			this.AxisXlabel = x;
			this.AxisYlabel = y;
		}

		public GraphDynamicType () {
			InitializeComponent ();
		}

		private void GraphDynamicType_Load ( object sender , EventArgs e ) {

		}
		public List<double> GetAxisData ( Axes axis ) {

			switch ( axis ) {
				case Axes.x: return new List<double> ( this.dataX );
				case Axes.y: return new List<double> ( this.dataY );
				default: return null;
			}
		}

		private void checkBoxScatter_CheckedChanged ( object sender , EventArgs e ) {
			if ( this.checkBoxScatter.Checked ) this.Scatter = true;
			else this.Scatter = false;
			this.Redraw ();
		}

		private void buttonZoom100_Click ( object sender , EventArgs e ) {
			
			
				//if ( Double.IsInfinity ( xMinValue ) || Double.IsNaN ( xMinValue ) ) {
				//	xMinValue = -1000;
				//	xMinValue = dataX.Where(a=)
				//}
				//if ( Double.IsInfinity ( xMaxValue ) || Double.IsNaN ( xMaxValue ) ) {
				//	xMaxValue = 1000;
				//}
				//if ( Double.IsInfinity ( yMinValue ) || Double.IsNaN ( yMinValue ) ) {
				//	yMinValue = -1000;
				//}
				//if ( Double.IsInfinity ( yMaxValue ) || Double.IsNaN ( yMaxValue ) ) {
				//	yMaxValue = 1000;
				//}
			
			this.Redraw ();
		}
	}
}
