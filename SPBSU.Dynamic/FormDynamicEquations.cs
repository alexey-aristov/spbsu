﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DynamicCompiling;
using Graph;
using Graph.Events;
using Mathematics;
using Mathematics.Analysis;
using Mathematics.Intergration;
using SPBSU.Dynamic.Data;

namespace SPBSU.Dynamic {
	public partial class FormDynamicEquations : Form {
		private Dictionary<string,TextBox> Equations;
		public Dictionary<string , TextBox> Initials;
		private Dictionary<string , Label> Dts;
		private Dictionary<string , Label> Var0;
		private Dictionary<string , TextBox> Variables;
		private Dictionary<string , Button> DeleteButtons;

		private Dictionary<string , TrackBar> ParamterTrackBars;
		private Dictionary<string , TextBox> ParamterTextBoxes;
		private Dictionary<string , Button> ParametersButtonsEdit;

		private int SplitBetweenEquations = 30;

		private List<string> VarsNames;

		private bool initedgraphGrabli = false;

		public Dictionary<string , Delegate> Funcs;

		private int EquationElementPosition = 12;

		public const int MaxEquationsCount = 60;

		private int CurrentEqNumerator;

		public List<Dictionary<string , double>> SetOfInitials {
			get;
			set;
		}
		public List<Color> ColorsOfInitials {
			get;
			set;
		}
		public FormDynamicEquations () {
			InitializeComponent ();
			CurrentEqNumerator = 0;
			VarsNames = new List<string> ();
			VarsNames.AddRange ( new string[] { "x" , "y" , "z" } );

			Equations = new Dictionary<string , TextBox> ();
			Initials = new Dictionary<string , TextBox> ();
			Dts = new Dictionary<string , Label> ();
			Variables = new Dictionary<string , TextBox> ();
			Var0 = new Dictionary<string , Label> ();
			DeleteButtons = new Dictionary<string , Button> ();
			//this.EquationElementPosition = 0;

			AddEquation ();
			VariablesChanged (null,new EventArgs());

			this.graphSystemBehavior1.IntegrationType = IntegrationType.RungeKutta4;
		}

		private void Form1_Load ( object sender , EventArgs e ) {
			////Делаем кнопку круглой
			//System.Drawing.Drawing2D.GraphicsPath Button_Path = new System.Drawing.Drawing2D.GraphicsPath ();
			//Button_Path.AddEllipse ( 0 , 0 , this.buttonCalc.Height-5 , this.buttonCalc.Height-5 );
			//Region Button_Region = new Region ( Button_Path );
			//this.buttonCalc.Region = Button_Region;
			this.ParametersButtonsEdit = new Dictionary<string , Button> ();
			this.ParamterTextBoxes = new Dictionary<string , TextBox> ();
			this.ParamterTrackBars = new Dictionary<string , TrackBar> ();

			

			this.ParamterTextBoxes.Add ( "A" , this.textBoxA );
			this.ParamterTextBoxes.Add ( "B" , this.textBoxB );
			this.ParamterTextBoxes.Add ( "C" , this.textBoxC );
			this.ParamterTextBoxes.Add ( "D" , this.textBoxD );
			this.ParamterTextBoxes.Add ( "E" , this.textBoxE );
			this.ParamterTextBoxes.Add ( "F" , this.textBoxF );

			//this.ParamterTrackBars.Add ( "A" , this.trackBarA );
			//this.ParamterTrackBars.Add ( "B" , this.trackBarB );
			//this.ParamterTrackBars.Add ( "C" , this.trackBarC );
			//this.ParamterTrackBars.Add ( "D" , this.trackBarD );
			//this.ParamterTrackBars.Add ( "E" , this.trackBarE );
			//this.ParamterTrackBars.Add ( "F" , this.trackBarF );

			//this.ParametersButtonsEdit.Add ( "A" , this.buttonA );
			//this.ParametersButtonsEdit.Add ( "B" , this.buttonB );
			//this.ParametersButtonsEdit.Add ( "C" , this.buttonC );
			//this.ParametersButtonsEdit.Add ( "D" , this.buttonD );
			//this.ParametersButtonsEdit.Add ( "E" , this.buttonE );
			//this.ParametersButtonsEdit.Add ( "F" , this.buttonF );
			DirectoryInfo dInfo = new DirectoryInfo ( Application.StartupPath + @"\EquationsSets\" );
			if ( !dInfo.Exists ) {
				dInfo.Create ();
			}
			else {
				foreach ( var f in dInfo.GetFiles ( "*.equation" ) ) {
					this.listBoxSystemName.Items.Add ( f.Name.Substring(0,f.Name.Length-9) );
				}

			}

		}

		private void button1_Click ( object sender , EventArgs e ) {

			AddEquation ();
		}

		private void AddEquation () {
			if ( this.Equations.Count < MaxEquationsCount ) {
				string newVarName;
				if ( Equations.Count >= VarsNames.Count ) {
					newVarName = "v" + ( Equations.Count - VarsNames.Count ).ToString ();
				}
				else {
					newVarName = VarsNames[Equations.Count ()];
				}

				TextBox newTextBox = new TextBox ();
				newTextBox.Location = new Point ( 550 , this.EquationElementPosition + (this.Equations.Count == 0 ? 0 : this.SplitBetweenEquations) );

				TextBox newInitial = new TextBox ();
				newInitial.Location = new Point ( 690 , this.EquationElementPosition + (this.Equations.Count == 0 ? 0 : this.SplitBetweenEquations) );
				newInitial.Size = new System.Drawing.Size ( 30 , 20 );
				newInitial.Text = "0";

				Label newDt = new Label ();
				newDt.Location = new Point ( 500 , this.EquationElementPosition + (this.Equations.Count == 0 ? 0 : this.SplitBetweenEquations - 2) );
				newDt.Size = new System.Drawing.Size ( 20 , 30 );
				newDt.Text =
					@"d
					dt";
				Label newVar0 = new Label ();
				newVar0.Location = new Point ( 660 , this.EquationElementPosition + (this.Equations.Count == 0 ? 0 : this.SplitBetweenEquations) );
				newVar0.Size = new System.Drawing.Size ( 40 , 30 );
				newVar0.Text = newVarName + "(t0)=";

				TextBox newVar = new TextBox ();
				newVar.Text = newVarName;
				newVar.Location = new Point ( 520 , this.EquationElementPosition + (this.Equations.Count == 0 ? 0 : this.SplitBetweenEquations) );
				newVar.Size = new System.Drawing.Size ( 20 , 10 );
				newVar.TextChanged += VariablesChanged;

				Button newDelete = new Button ();
				newDelete.Text = "Del";
				newDelete.Location = new Point ( 730 , this.EquationElementPosition + ( this.Equations.Count == 0 ? 0 : this.SplitBetweenEquations ) );
				newDelete.Size = new Size (50,23);


				this.buttonAddEquation.Location = new Point ( this.buttonAddEquation.Location.X , this.buttonAddEquation.Location.Y + (this.Equations.Count == 0 ? 0 : this.SplitBetweenEquations) );
				CurrentEqNumerator++;
				string eqName = "eq" + CurrentEqNumerator.ToString ();
				this.Equations.Add (eqName, newTextBox );
				this.Initials.Add (eqName, newInitial );
				this.Dts.Add (eqName, newDt );
				this.Variables.Add (eqName, newVar );
				this.Var0.Add (eqName, newVar0 );
				this.DeleteButtons.Add ( eqName,newDelete );
				newDelete.Click += buttonDelEquation_Click;

				this.Controls.Add ( newTextBox );
				this.Controls.Add ( newInitial );
				this.Controls.Add ( newDt );
				this.Controls.Add ( newVar );
				this.Controls.Add ( newVar0 );
				this.Controls.Add ( newDelete );
				VariablesChanged ( null , new EventArgs () );
				if ( this.Equations.Count == MaxEquationsCount ) {
					this.buttonAddEquation.Visible = false;
				}
				if(this.Equations.Count !=1)
				this.EquationElementPosition += this.SplitBetweenEquations;
			}
			else {
				this.buttonAddEquation.Visible = false;
			}
		}
		private void DelEquation (string key) {
			this.Controls.Remove(this.Equations[key]);
			this.Equations.Remove ( key );
			this.Controls.Remove ( this.Var0[key] );
			this.Var0.Remove ( key );
			this.Controls.Remove ( this.Initials[key] );
			this.Initials.Remove ( key );

			this.Controls.Remove ( this.Dts[key] );
			this.Dts.Remove ( key );
			this.Controls.Remove ( this.Variables[key] );
			this.Variables.Remove ( key );
			this.Controls.Remove ( this.DeleteButtons[key] );
			this.DeleteButtons.Remove ( key );
			this.EquationElementPosition -= this.SplitBetweenEquations;
			CorrectEquationPosition ();
			this.buttonAddEquation.Location = new Point ( this.buttonAddEquation.Location.X , this.buttonAddEquation.Location.Y - ( this.Equations.Count == 0 ? 0 : this.SplitBetweenEquations ) );
			this.VariablesChanged ( null , null );
		}

		private void CorrectEquationPosition () {
			this.EquationElementPosition = 12;
			for(int i=0;i<this.Equations.Count;i++)
			{
				this.Equations.ElementAt ( i ).Value.Location = new Point ( 550 , this.EquationElementPosition );
				this.Dts.ElementAt ( i ).Value.Location = new Point ( 500 , this.EquationElementPosition );
				this.Initials.ElementAt ( i ).Value.Location = new Point ( 690 , this.EquationElementPosition );
				this.Var0.ElementAt ( i ).Value.Location = new Point ( 660 , this.EquationElementPosition );
				this.Variables.ElementAt ( i ).Value.Location = new Point ( 520 , this.EquationElementPosition );
				this.DeleteButtons.ElementAt ( i ).Value.Location = new Point ( 730 , this.EquationElementPosition );

				this.EquationElementPosition += this.SplitBetweenEquations;
			}
			if ( this.Equations.Count != 0 )
				this.EquationElementPosition -= this.SplitBetweenEquations;
		}
		
		private void VariablesChanged ( object sender , EventArgs e ) {

			try {
				int selectedIndexX = this.listBoxX.SelectedIndex;
				int selectedIndexY = this.listBoxY.SelectedIndex;
				int selectedIndexPoincare = this.comboBoxVarForPoincare.SelectedIndex;
				int selectedIndexHVar = this.comboBoxVarForDetH.SelectedIndex;
				this.listBoxX.Items.Clear ();
				this.listBoxY.Items.Clear ();
				this.comboBoxVarForPoincare.Items.Clear ();
				this.comboBoxVarForDetH.Items.Clear ();

				this.listBoxX.Items.Add ( "t" );
				this.listBoxY.Items.Add ( "t" );
				foreach ( var str in this.Variables.Values ) {
					this.listBoxX.Items.Add (str.Text);
					this.listBoxY.Items.Add ( str.Text );
					this.comboBoxVarForPoincare.Items.Add ( str.Text );
					this.comboBoxVarForDetH.Items.Add ( str.Text );
				}
			

					this.comboBoxVarForPoincare.SelectedIndex = 0;
			
				//for ( int i = 0 ; i < this.Var0.Count ;i++ ) {
				//	this.Var0[i].Text = this.Variables[i].Text+"(t0)=";
				//}
				foreach ( var key in Var0.Keys ) {
					Var0[key].Text = this.Variables[key].Text + "(t0)=";
				}
				if (selectedIndexX != -1) this.listBoxX.SetSelected(selectedIndexX,true);
				else this.listBoxX.SetSelected ( 0 , true );

				if ( selectedIndexY != -1 ) this.listBoxY.SetSelected ( selectedIndexY , true );
				else this.listBoxY.SetSelected ( 1 , true );

				if ( selectedIndexPoincare != -1 ) this.comboBoxVarForPoincare.SelectedIndex = selectedIndexPoincare;
				else this.comboBoxVarForPoincare.SelectedIndex = 0;

				if ( selectedIndexHVar != -1 ) {


					this.comboBoxVarForDetH.SelectedIndex = selectedIndexPoincare;

				}
				else {
				
					this.comboBoxVarForDetH.SelectedIndex = 0;
				
				}

			}
			catch {
			}

			
		}
		public void buttonCalc_Click ( object sender , EventArgs e ) {

			//this.SetOfInitials = null;
			Dictionary<string , double> parameters = new Dictionary<string,double>();
			try {
				Dictionary<string , string> Eques = new Dictionary<string , string> ();
				Dictionary<string , double> initials = new Dictionary<string , double> ();
				//for ( int i = 0 ; i < this.Equations.Count ; i++ ) {
				//	Eques.Add ( Variables[i].Text , Equations[i].Text );
				//	initials.Add ( Variables[i].Text , Convert.ToDouble ( Initials[i].Text ) );
				//}

				foreach ( var key in this.Equations.Keys ) {
					Eques.Add ( Variables[key].Text , Equations[key].Text );
					initials.Add ( Variables[key].Text , Convert.ToDouble ( Initials[key].Text ) );
				}
				parameters = this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble(b.Value.Text) );

				//Compilator compilator = new Compilator ( Eques );
				//var funcs= compilator.GetFuncs ();
				//for ( int i = 0 ; i < this.Initials.Count ; i++ ) {

				//	this.Initials[i].Text = funcs.Skip(i).First().Value.Invoke ( 0 , new Dictionary<string,double>(){{"x",0}}).ToString ();
					
				//}

				//Dictionary<string , double> f0 = new Dictionary<string , double> ();
				//foreach ( var f in funcs ) {
					
				//}
				//var w =RungeKutta.Integrate4 ( funcs , 0 , new Dictionary<string , double> () { { "x" , 0 } } );

				IntegrationParameters integrParam = new IntegrationParameters();
				try{

					 integrParam= new IntegrationParameters {
						IterationsCount = Convert.ToInt32(this.textBoxIterations.Text),
						Step = Convert.ToDouble(this.textBoxStep.Text),
						LeftDirection = this.checkBoxDirectionLeft.Checked,
						RightDirection = this.checkBoxDirectionRight.Checked
						
					};
					if ( this.checkBoxHDet.Checked ) {
						HamiltonianPlot form = new HamiltonianPlot ();

						//Dictionary<string , string> eques = new Dictionary<string , string> ();
						//Dictionary<string , double> initials = new Dictionary<string , double> ();
						//for ( int i = 0 ; i < this.Equations.Count ; i++ ) {
						//	eques.Add ( Variables[i].Text , Equations[i].Text );
						//}
						double variableVal;
						try {
							var temp = this.textBoxH.Text.Split ( '/' );
							if ( temp.Length > 2 || temp.Length == 0 ) throw new IncorrectInputException {
								ErrorMessage = "Wrong input for H!"
							};

						}
						catch ( IncorrectInputException ex ) {
							MessageBox.Show ( ex.ErrorMessage );
							throw;
						}
						string equationWithHForVar = this.textBoxVarEquation.Text.Replace ( "H" , "(double)" + this.textBoxH.Text );
						Dictionary<string , string> EquesForHamiltonian = new Dictionary<string , string> ();
						EquesForHamiltonian.Add ( "H" , equationWithHForVar );

						Compilator compilator = new Compilator ( EquesForHamiltonian ,
																this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble ( b.Value.Text ) ) ,
																Eques.Keys );
						var temt = compilator.GetFuncs ();
						var newDetVar = temt["H"].Invoke ( Convert.ToDouble ( this.textBoxt0.Text ) , initials , parameters );
						if(double.IsNaN(newDetVar)||double.IsInfinity(newDetVar))
						{
							throw new IncorrectInputException{
							ErrorMessage = "impossible to calculate"};
						}
						initials[this.comboBoxVarForDetH.SelectedItem.ToString ()] = newDetVar;
						this.label3.Text = newDetVar.ToString ();
					}
					if ( this.checkBoxPoincare.Checked ) {
						double pointOfSection;
						if ( this.textBoxSectionPoint.Text.Contains ( "pi" ) ) {
							var param = this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble(b.Value.Text));
							Compilator c = new Compilator ( new Dictionary<string , string> { { "var" , this.textBoxSectionPoint.Text.Replace ( "pi" , "Math.PI" ) } } , param );
							pointOfSection= c.GetFuncs ()["var"].Invoke (0,new Dictionary<string,double>(),param);

						}
						else {
							pointOfSection=Convert.ToDouble(this.textBoxSectionPoint.Text);
						}

						integrParam.PoincareParameters = new PoincareSectionParameters {
							VariableForSection = this.comboBoxVarForPoincare.SelectedItem.ToString () ,
							HitPointsCount = Convert.ToInt32 ( this.textBoxHitCount.Text ) ,
							ThicknessOfLayer = Convert.ToDouble ( this.textBoxThicknessOfLayer.Text ),
							PointOfSection = pointOfSection,
							TimePeriodSection = this.checkBoxTime.Checked
						};
					}
					else {
						integrParam.PoincareParameters = null;
					}
					this.graphSystemBehavior1.IntergrationParameters = integrParam;
					this.graphSystemBehavior1.InitFunctionsD ( Eques , parameters );
					this.graphSystemBehavior1.SetAxisToShow ( this.listBoxX.SelectedItem.ToString () , this.listBoxY.SelectedItem.ToString () );
					this.graphSystemBehavior1.f0 = initials;
					this.graphSystemBehavior1.t0 = Convert.ToDouble ( this.textBoxt0.Text );

					if ( !this.initedgraphGrabli ) {

						this.graphSystemBehavior1.setData ( 1 , 0 , 1 , 0 );
						this.graphSystemBehavior1.zoom100Percent ();
						this.initedgraphGrabli = true;
					}
					else {
						this.graphSystemBehavior1.Redraw ();
					}
				}
				catch(FormatException ex){
					MessageBox.Show ("Input is invalid");
					throw;
				}catch(IncorrectInputException ex){
					MessageBox.Show ( ex.ErrorMessage );
				}
				

				//----test
				
			}
			catch(Exception ex)
			{
				if ( ex is DynamicCompilationException ) {
					MessageBox.Show ( ( ex as DynamicCompilationException ).Message );
				}
				else
				{
				MessageBox.Show (ex.Message);
				}
			}
			
			
		}
		public void buttonDelEquation_Click ( object sender , EventArgs e ) {
			string kk =this.DeleteButtons.Where ( a => a.Value == ( sender as Button ) ).First().Key;
			DelEquation ( kk );
			//this.Controls.Remove ( ( sender as Button ) );

		}

		private void listBoxSystemName_DoubleClick ( object sender , EventArgs e ) {
			switch ( ( sender as ListBox ).SelectedItem.ToString() ) {
				//case "Complex System Slides":
				//	if ( this.Equations.Count >= 2 ) {
				//		this.Equations.ElementAt ( 0 ).Value.Text = "a";
				//	}
				//	break;
				case "Harmonic oscillator":
					if ( this.Equations.Count < 2 ) {
						AddEquation ();				
					}
					this.Equations.ElementAt(0).Value.Text = "p";
					this.Equations.ElementAt(1).Value.Text = "-q";
					this.Variables.ElementAt ( 0 ).Value.Text = "q";
					this.Variables.ElementAt ( 1 ).Value.Text = "p";
					this.Initials.ElementAt ( 0 ).Value.Text = "2";
					this.Initials.ElementAt ( 1 ).Value.Text = "1";
					this.textBoxHamiltonian.Text = "(p*p+q*q)/2";
					this.checkBoxHDet.Checked = false;
					this.checkBoxPoincare.Checked = false;
					break;
				case "Henon-Heiles":
					if ( this.Equations.Count < 4 ) {
						while ( this.Equations.Count != 4 ) {
							AddEquation ();
						}
					}
					this.Equations.ElementAt ( 0 ).Value.Text = "px";
					this.Equations.ElementAt ( 1 ).Value.Text = "py";
					this.Variables.ElementAt ( 0 ).Value.Text = "x";
					this.Variables.ElementAt ( 1 ).Value.Text = "y";
					this.Initials.ElementAt ( 0 ).Value.Text = "0";
					this.Initials.ElementAt ( 1 ).Value.Text = "0";

					this.Equations.ElementAt ( 2 ).Value.Text = "-C*x-A*2*x*y";
					this.Equations.ElementAt ( 3 ).Value.Text = "-C*y-x*x+y*y*B";
					this.Variables.ElementAt ( 2 ).Value.Text = "px";
					this.Variables.ElementAt ( 3 ).Value.Text = "py";
					this.Initials.ElementAt ( 2 ).Value.Text = "0.288790581";
					this.Initials.ElementAt ( 3 ).Value.Text = "0";
					this.textBoxHamiltonian.Text = "(px*px+py*py)/2+C*(x*x+y*y)/2+A*x*x*y-y*y*y*B/3.0";
					this.comboBoxVarForDetH.SelectedIndex = 2;
					this.textBoxVarEquation.Text = "Math.Sqrt(2*H-(py*py+C*(x*x+y*y)+2*A*x*x*y-2*y*y*y*B/3.0))";
					this.ParamterTextBoxes["A"].Text = "1";
					this.ParamterTextBoxes["B"].Text = "1";
					this.ParamterTextBoxes["C"].Text = "1";
					this.checkBoxHDet.Checked = true;
					this.checkBoxPoincare.Checked = true;
					break;
				//case "WikipediaRungeSample":
				//	if ( this.Equations.Count < 2 ) {
				//		AddEquation ();
				//	}
				//	this.Equations.ElementAt ( 0 ).Value.Text = "y";
				//	this.Equations.ElementAt ( 1 ).Value.Text = "cos(3*t)-4*dy";
				//	this.Variables.ElementAt ( 0 ).Value.Text = "dy";
				//	this.Variables.ElementAt ( 1 ).Value.Text = "y";
				//	this.Initials.ElementAt ( 0 ).Value.Text = "2";
				//	this.Initials.ElementAt ( 1 ).Value.Text = "0.8";
				//	this.textBoxHamiltonian.Text = "y+dy";
					
				//	this.checkBoxHDet.Checked = false;
				//	this.checkBoxPoincare.Checked = false;
				//	break;
				case "Lorenz Equation":
					if ( this.Equations.Count < 3 ) {
						AddEquation ();
						AddEquation ();
					}

					this.Variables.ElementAt ( 0 ).Value.Text = "x";
					this.Equations.ElementAt ( 0 ).Value.Text = "A*(y - x)";
					this.Initials.ElementAt ( 0 ).Value.Text = "0";

					this.Variables.ElementAt ( 1 ).Value.Text = "y";
					this.Equations.ElementAt ( 1 ).Value.Text = "B*x - y -x*z";
					this.Initials.ElementAt ( 1 ).Value.Text = "1";

					this.Variables.ElementAt ( 2 ).Value.Text = "z";
					this.Equations.ElementAt ( 2 ).Value.Text = "x*y - C*z";
					this.Initials.ElementAt ( 2 ).Value.Text = "0";

					this.ParamterTextBoxes["A"].Text = "10";
					this.ParamterTextBoxes["B"].Text = "28";
					this.ParamterTextBoxes["C"].Text = "2.6666";
					
					this.checkBoxHDet.Checked = false;
					this.checkBoxPoincare.Checked = false;
					break;
				//case"Henon Map":
				//	if ( this.Equations.Count < 2 ) {
				//		AddEquation ();
				//	}
				//	this.Variables.ElementAt ( 0 ).Value.Text = "x";
				//	this.Equations.ElementAt ( 0 ).Value.Text = "1-A*x*x+y";
				//	this.Initials.ElementAt ( 0 ).Value.Text = "0.6313";

				//	this.Variables.ElementAt ( 1 ).Value.Text = "y";
				//	this.Equations.ElementAt ( 1 ).Value.Text = "B*x";
				//	this.Initials.ElementAt(1).Value.Text = "0.1894";

				//	this.ParamterTextBoxes["A"].Text = "1.4";
				//	this.ParamterTextBoxes["B"].Text = "0.3";
					
				//	this.checkBoxHDet.Checked = false;
				//	this.checkBoxPoincare.Checked = false;
				//	break;
				case "Lotka–Volterra":
					if ( this.Equations.Count < 2 ) {
						AddEquation ();
					}

					this.Variables.ElementAt ( 0 ).Value.Text = "x";
					this.Equations.ElementAt ( 0 ).Value.Text = "A*x-B*y*x";
					this.Initials.ElementAt ( 0 ).Value.Text = "0.5";

					this.Variables.ElementAt ( 1 ).Value.Text = "y";
					this.Equations.ElementAt ( 1 ).Value.Text = "B*x*y-C*y";
					this.Initials.ElementAt ( 1 ).Value.Text = "0.5";
					this.ParamterTextBoxes["A"].Text = "1";
					this.ParamterTextBoxes["B"].Text = "1";
					this.ParamterTextBoxes["C"].Text = "1";
					this.ParamterTextBoxes["D"].Text = "1";
					this.ParamterTextBoxes["E"].Text = "1";

					this.checkBoxHDet.Checked = false;
					this.checkBoxPoincare.Checked = false;
					break;
				default:
					var set = new Serializer ().DeSerializeObjectEquationsSet ( "EquationsSets/" + ( sender as ListBox ).SelectedItem.ToString () + ".equation" );
					LoadEquations ( set );
					break;
			}
			//this.graphSystemBehavior1.Redraw ();
		}

		private void LoadEquations ( EquationsSet set ) {
			foreach ( var param in set.Parameters ) {
				this.ParamterTextBoxes[param.Key].Text = param.Value.ToString ();
			}
			SetEquationsCount ( set.Equations.Count );
			for ( int i = 0 ; i < this.Equations.Count ; i++ ) {
				this.Equations.ElementAt ( i ).Value.Text = set.Equations.ElementAt ( i ).Value.Text;
				this.Initials.ElementAt ( i ).Value.Text = set.Equations.ElementAt ( i ).Value.Var0.ToString ();
				this.Variables.ElementAt ( i ).Value.Text = set.Equations.ElementAt ( i ).Value.Var.ToString ();
			}
			this.textBoxt0.Text = set.t0.ToString ();
			if ( set.IntegrationParameters.PoincareParameters != null ) {
				this.checkBoxPoincare.Checked = true;
				this.textBoxThicknessOfLayer.Text = set.IntegrationParameters.PoincareParameters.ThicknessOfLayer.ToString ();
				this.textBoxVarEquation.Text = set.IntegrationParameters.PoincareParameters.HForDetEquation;
				this.textBoxH.Text = set.IntegrationParameters.PoincareParameters.H;
				this.comboBoxVarForPoincare.SelectedIndex = this.comboBoxVarForPoincare.Items.IndexOf ( set.IntegrationParameters.PoincareParameters.VariableForSection );
				this.comboBoxVarForDetH.SelectedIndex = this.comboBoxVarForDetH.Items.IndexOf ( set.IntegrationParameters.PoincareParameters.HForDet );
				this.textBoxHitCount.Text = set.IntegrationParameters.PoincareParameters.HitPointsCount.ToString ();

				this.textBoxSectionPoint.Text = set.IntegrationParameters.PoincareParameters.TimePeriodSection ? set.IntegrationParameters.PoincareParameters.PointOfSectionString : set.IntegrationParameters.PoincareParameters.PointOfSection.ToString ();
				this.checkBoxTime.Checked = set.IntegrationParameters.PoincareParameters.TimePeriodSection;

				this.checkBoxHDet.Checked = set.IntegrationParameters.PoincareParameters.CheckDetH;
			}
			else {
				this.checkBoxPoincare.Checked = false;
				this.checkBoxHDet.Checked = false;
			}
			if ( set.SetOfInitials != null ) {
				this.SetOfInitials = set.SetOfInitials;
			}
			this.textBoxStep.Text = set.IntegrationParameters.Step.ToString ();
			this.textBoxIterations.Text = set.IntegrationParameters.IterationsCount.ToString ();
			this.checkBoxDirectionLeft.Checked = set.IntegrationParameters.LeftDirection;
			this.checkBoxDirectionRight.Checked = set.IntegrationParameters.RightDirection;
			this.textBoxHamiltonian.Text = set.Hamiltonian;
			this.ColorsOfInitials = set.Colors;
		}
		

		public void SetEquationsCount ( int count ) {
			if ( this.Equations.Count == count ) return;
			if ( this.Equations.Count > count ) {
				while ( this.Equations.Count != count ) {
					this.DelEquation ( this.Equations.Last ().Key );
				}
				return;
			}
			if ( this.Equations.Count < count ) {
				while ( this.Equations.Count != count ) {
					this.AddEquation ();
				}
				return;
			}
		}

		private void buttonParameterEdit_Click ( object sender , EventArgs e ) {
			
			string key = this.ParametersButtonsEdit.FirstOrDefault ( a => a.Value == ( sender as Button ) ).Key;
			EditPatameterValuesArea form = new EditPatameterValuesArea ( this.ParamterTrackBars[key] );
			form.Text = "Edit " + key;
			form.Show ();
			//form.numericUpDownMin.Value = this.ParamterTrackBars[key].Minimum / ( form.numericUpDownMin.DecimalPlaces * 10 );
			//form.numericUpDownMax.Value = this.ParamterTrackBars[key].Maximum / ( form.numericUpDownMax.DecimalPlaces * 10 );
			
			form.FormClosed += new FormClosedEventHandler ( ( sender1 , e1 ) => {
				this.WindowState = FormWindowState.Normal;
				
			} );
			
			this.WindowState = FormWindowState.Minimized;
			
		}
				
		private void trackBarParameter_Scroll ( object sender , EventArgs e ){
			
			string key = this.ParamterTrackBars.FirstOrDefault ( a => a.Value == ( sender as TrackBar ) ).Key;
			this.ParamterTextBoxes[key].Text = ((double)( sender as TrackBar ).Value/10000).ToString ();
			this.graphSystemBehavior1.Parameters = this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble ( b.Value.Text ) );
			this.graphSystemBehavior1.Redraw ();
			
		}

		private void radioButtonRungeKutta4_CheckedChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.IntegrationType = IntegrationType.RungeKutta4;
		}

		private void radioButton1_CheckedChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.IntegrationType = IntegrationType.EulerMethod;
		}

		private void radioButtonDormandPrince_CheckedChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.IntegrationType = IntegrationType.DormandPrince;
		}
		

		Color InvertColor ( Color ColorToInvert ) {
			int RGBMAX = 255;
			return Color.FromArgb ( RGBMAX - ColorToInvert.R ,
			  RGBMAX - ColorToInvert.G , RGBMAX - ColorToInvert.B );
		}
		private void buttonHamiltonian_Click ( object sender , EventArgs e ) {
			try {

				HamiltonianPlot form = new HamiltonianPlot ();
				form.graphSystemOscillogram1.setData ( 0 , 0 , 0 , 0 );
				form.Activate ();
				form.graphSystemOscillogram1.expotentialY = true;
				Dictionary<string , string> eques = new Dictionary<string , string> ();
				Dictionary<string , double> initials = new Dictionary<string , double> ();
				List<GraphData> data = new List<GraphData> ();
				double t0 = Convert.ToDouble ( this.textBoxt0.Text );
				//double Ap = Convert.ToDouble ( this.Initials["p"] );
				//double Aq = Convert.ToDouble ( this.Initials["q"] );
				//for ( int i = 0 ; i < this.Equations.Count ; i++ ) {
				//	eques.Add ( Variables[i].Text , Equations[i].Text );
				//}
				if ( this.textBoxHamiltonian.Text.Contains( "oscillator") ) {
					string variable = this.textBoxHamiltonian.Text.Split(' ')[1];
					
					double corrector = 1;
					foreach ( var sol in this.graphSystemBehavior1.Data ) {
						List<double> data_Y = new List<double> ();
						List<double> data_Y1 = new List<double> ();
						for ( int i = 0 ; i < sol.Solution["t"].Count ;i++ ) {
							//----absolete----
							if ( variable == "p" ) {
								data_Y.Add ( corrector*(sol.Solution[variable][i] - ( Math.Cos ( sol.Solution["t"][i]-t0 ) - 2 * Math.Sin ( sol.Solution["t"][i] -t0) )) );
							}
							else {
								if ( variable == "q" ) {
									data_Y.Add (corrector*Math.Abs(sol.Solution[variable][i]-( 2 * Math.Cos ( sol.Solution["t"][i] -t0) + Math.Sin ( sol.Solution["t"][i]-t0 ))) );
								}
								else {
									if ( variable == "pq" || variable == "qp" ) {
										data_Y.Add ( Math.Abs(sol.Solution["p"][i] - ( Math.Cos ( sol.Solution["t"][i] -t0) - 2 * Math.Sin ( sol.Solution["t"][i]-t0 )) ) );
										data_Y1.Add (-Math.Abs( sol.Solution["q"][i] - ( 2 * Math.Cos ( sol.Solution["t"][i]-t0 ) + Math.Sin ( sol.Solution["t"][i] -t0)) ) );
									}
									else {
										throw new Exception ();
									}
									
								}
							}

						}
						data.Add ( new GraphData {
							DataColor = sol.DataColor ,
							dataY = data_Y ,
							dataX = sol.Solution["t"]

						} );
						if ( data_Y1.Count != 0 ) {
							data.Add ( new GraphData {
								DataColor = Color.Blue ,
								dataY = data_Y1 ,
								dataX = sol.Solution["t"]

							} );
						}
						corrector *= -1;
					}
				}
				else {
					if ( this.textBoxHamiltonian.Text.Contains ( "error" ) ) {
						string variable = this.textBoxHamiltonian.Text.Split ( ' ' )[1];

						double corrector = 1;
						foreach ( var sol in this.graphSystemBehavior1.Data ) {
							List<double> data_Y = new List<double> ();
							List<double> data_Y1 = new List<double> ();
							for ( int i = 0 ; i < sol.Solution["t"].Count ; i++ ) {

								data_Y.Add ( Math.Abs ( sol.Solution[variable][i] - sol.Solution[variable][0] ) / sol.Solution[variable][0] );


							}
							data.Add ( new GraphData {
								DataColor = sol.DataColor ,
								dataY = data_Y ,
								dataX = sol.Solution["t"]

							} );
							if ( data_Y1.Count != 0 ) {
								data.Add ( new GraphData {
									DataColor = Color.Blue ,
									dataY = data_Y1 ,
									dataX = sol.Solution["t"]

								} );
							}
							corrector *= -1;
						}
					}
					else {
						form.graphSystemOscillogram1.AxisXlabel = "t";
						form.graphSystemOscillogram1.AxisYlabel = "%";
						foreach ( var key in this.Equations.Keys ) {
							eques.Add ( Variables[key].Text , Equations[key].Text );
						}
						Compilator compilator = new Compilator ( new Dictionary<string , string> { { "H" , this.textBoxHamiltonian.Text } } ,
																this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble ( b.Value.Text ) ) ,
																eques.Keys );
						var temt = compilator.GetFuncs ();


						foreach ( var sol in this.graphSystemBehavior1.Data ) {
							var dic = new Dictionary<string , List<double>> ();
							dic.Add ( "p" , new List<double> () );
							dic.Add ( "q" , new List<double> () );
							for ( int i = 0 ; i < sol.dataX.Count ; i++ ) {
								dic["p"].Add ( Math.Cos ( sol.Solution["t"][i] - t0 ) - 2 * Math.Sin ( sol.Solution["t"][i] - t0 ) );
								dic["q"].Add ( 2 * Math.Cos ( sol.Solution["t"][i] - t0 ) + Math.Sin ( sol.Solution["t"][i] - t0 ) );
							}
							dic.Add ( "t" , sol.Solution["t"] );
							//var uio = Hamiltonian.Calc ( temt , sol.Solution , this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble ( b.Value.Text ) ) );
							//var uio = Hamiltonian.Calc ( temt , dic , this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble ( b.Value.Text ) ) );
							//data.Add ( new GraphData {
							//	DataColor = sol.DataColor ,
							//	dataY = uio["H"] ,
							//	dataX = sol.Solution["t"]

							//} );
							var uio1 = Hamiltonian.Calc ( temt , sol.Solution , this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble ( b.Value.Text ) ) );
							double initial = uio1["H"][0];
							for ( int i = 0 ; i < uio1["H"].Count ; i++ ) {
								uio1["H"][i] = Math.Abs(uio1["H"][i]-initial) / initial;
							}
							data.Add ( new GraphData {
								DataColor = sol.DataColor ,
								dataY = uio1["H"] ,
								dataX = sol.Solution["t"]

							} );
						}
					}
				}
				form.graphSystemOscillogram1.setYdata ( data);
				//form.graphSystemOscillogram1.setXdata ( uio["t"] );
				form.graphSystemOscillogram1.zoom100Percent ();
				form.graphSystemOscillogram1.Refresh ();
				//form.graphSystemOscillogram1.yD
				form.Show ();
			}
			catch ( Exception ex) {
				if ( ex is DynamicCompilationException ) {
					MessageBox.Show (( ex as DynamicCompilationException ).Message);
				}else {
					MessageBox.Show ( ex.Message );
				}
				

			}
		}

		private void buttonLyapunov_Click ( object sender , EventArgs e ) {
			

			foreach ( var v in this.listBoxY.Items ) {
				if ( v.ToString() == "t" ) continue;

				LyapunovSpectrumPlot form = new LyapunovSpectrumPlot ();
				var parameters = this.ParamterTextBoxes.ToDictionary ( a => a.Key , b => Convert.ToDouble ( b.Value.Text ) );
				form.graphSystemOscillogram1.setYdata ( Lyapunov.Spectrum ( this.graphSystemBehavior1.Solutions , this.graphSystemBehavior1.functionsD , parameters )[v.ToString ()] , this.colorDialog1.Color );
				form.graphSystemOscillogram1.AxisXlabel = "t";
				form.graphSystemOscillogram1.AxisYlabel = "λ(" + v.ToString () + ")";
				form.Show ();
				form.graphSystemOscillogram1.zoom100Percent ();
			}
		}

		private void radioButtonIterativ_CheckedChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.IntegrationType = IntegrationType.Iterative;
		}

		private void buttonRedrawAxes_Click ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.RedrawWithChangeData ( this.listBoxX.SelectedItem.ToString () , this.listBoxY.SelectedItem.ToString () );

		}

		private void checkBoxPoincare_CheckedChanged ( object sender , EventArgs e ) {
			if ( this.checkBoxPoincare.Checked ) {

				this.textBoxThicknessOfLayer.Enabled = true;
				this.comboBoxVarForPoincare.Enabled = true;
				this.textBoxHitCount.Enabled = true;
				this.textBoxSectionPoint.Enabled = true;
				this.textBoxIterations.Enabled = false;

				this.checkBoxTime.Enabled = true;
			}
			else {
				this.textBoxThicknessOfLayer.Enabled = false;
				this.comboBoxVarForPoincare.Enabled = false;
				this.textBoxHitCount.Enabled = false;
				this.textBoxSectionPoint.Enabled = false;
				this.textBoxIterations.Enabled = true;

				this.checkBoxTime.Enabled = false;
			}
		}

		private void checkBoxHDet_CheckedChanged ( object sender , EventArgs e ) {
			if ( this.checkBoxHDet.Checked ) {
				this.textBoxH.Enabled = true;
				this.comboBoxVarForDetH.Enabled = true;
				this.textBoxVarEquation.Enabled = true;
			}
			else {
				this.textBoxH.Enabled = false;
				this.comboBoxVarForDetH.Enabled = false;
				this.textBoxVarEquation.Enabled = false;
				}
		}

		private void checkBoxAnimate_CheckedChanged ( object sender , EventArgs e ) {
			if ( this.checkBoxAnimate.Checked ) {
				this.graphSystemBehavior1.Animate = true;
				this.graphSystemBehavior1.AnimatePeriod = Convert.ToInt32 (this.textBoxAnimatePeriod.Text);
			}
			else {
				this.graphSystemBehavior1.Animate = false;
			}
		}

		private void buttonGif_Click ( object sender , EventArgs e ) {
			GifGenerator form = new GifGenerator (this);

			form.Show ();
		}

		private void radioButtonEulerSymplectic_CheckedChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.IntegrationType = IntegrationType.EulerMethodSymplectic;
		}

		private void graphSystemBehavior1_CalculationFinished ( object sender , CalculationFinishedEventArgs e ) {
			
			this.Invoke ( new Action ( () => {
				this.labelTimeElapsedResult.Text = e.TimeElapsed.ToString ();
				this.labelIterationsCountResult.Text = e.IterationsCount.ToString ();
				this.labelFunctionsInvocationsCountResult.Text = e.FuncInvoked.ToString ();
			} ) );
		}

		private void radioButtonHeuns_CheckedChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.IntegrationType = IntegrationType.Symplectic4;
		}

		private void radioButtonEulerImplicit_CheckedChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.IntegrationType = IntegrationType.EulerMethodImplicit;
		}

		private void buttonSaveSystem_Click ( object sender , EventArgs e ) {
			SaveSystemForm form = new SaveSystemForm ( this.PickEquationsData(),this );
			form.Show ();
		}

		private EquationsSet PickEquationsData () {
			Dictionary<string , Equation> eques = new Dictionary<string , Equation> ();
			IntegrationParameters param = new IntegrationParameters {
				Error = Convert.ToDouble ( this.textBoxError.Text ) ,
				IterationsCount = Convert.ToInt32 ( this.textBoxIterations.Text ) ,
				LeftDirection = this.checkBoxDirectionLeft.Checked ,
				RightDirection = this.checkBoxDirectionRight.Checked ,
				Step = Convert.ToDouble ( this.textBoxStep.Text ) ,
				PoincareParameters = this.checkBoxPoincare.Checked ? new PoincareSectionParameters {
					VariableForSection = this.comboBoxVarForPoincare.SelectedItem.ToString () ,
					HitPointsCount = Convert.ToInt32 ( this.textBoxHitCount.Text ) ,
					ThicknessOfLayer = Convert.ToDouble ( this.textBoxThicknessOfLayer.Text ) ,
					PointOfSection = this.checkBoxTime.Checked ? 0 : Convert.ToDouble ( this.textBoxSectionPoint.Text ) ,
					PointOfSectionString = this.checkBoxTime.Checked ? this.textBoxSectionPoint.Text : "" ,
					H = this.textBoxH.Text ,
					HForDet = this.comboBoxVarForDetH.Text ,
					HForDetEquation = this.textBoxVarEquation.Text ,
					CheckDetH = this.checkBoxHDet.Checked ,
					TimePeriodSection = this.checkBoxTime.Checked
				} : null
			};
			foreach ( var var in this.Variables ) {
				eques.Add ( var.Value.Text , new Equation {
					Text = this.Equations[var.Key].Text ,
					Var = var.Value.Text ,
					Var0 = Convert.ToDouble ( this.Initials[var.Key].Text )
				} );
			}
			EquationsSet set = new EquationsSet {
				t0 = Convert.ToDouble ( this.textBoxt0.Text ) ,
				Equations = eques ,
				Parameters = this.ParamterTextBoxes.ToDictionary ( a => a.Key , a => Convert.ToDouble ( a.Value.Text ) ) ,
				IntegrationParameters = param ,
				Hamiltonian = this.textBoxHamiltonian.Text ,
				SetOfInitials = this.SetOfInitials == null ? null : this.SetOfInitials ,
				Colors = this.ColorsOfInitials == null ? null : this.ColorsOfInitials
			};
			return set;
		}

		private void buttonFullScreen_Click ( object sender , EventArgs e ) {
			GraphFullScreen form = new GraphFullScreen ();
			form.graphDynamicType.setData (0,0,0,0);
			form.graphDynamicType.XMaxValue = this.graphSystemBehavior1.XMaxValue;
			form.graphDynamicType.XMinValue = this.graphSystemBehavior1.XMinValue;
			form.graphDynamicType.YMaxValue = this.graphSystemBehavior1.YMaxValue;
			form.graphDynamicType.YMinValue = this.graphSystemBehavior1.YMinValue;
			form.graphDynamicType.dataX = this.graphSystemBehavior1.dataX;
			form.graphDynamicType.dataY = this.graphSystemBehavior1.dataY;

			form.graphDynamicType.AxisXlabel = this.graphSystemBehavior1.AxisXlabel;
			form.graphDynamicType.AxisYlabel = this.graphSystemBehavior1.AxisYlabel;
			form.graphDynamicType.checkBoxScatter.Checked = this.graphSystemBehavior1.checkBoxScatter.Checked;
			form.graphDynamicType.checkBoxZoomrRecalc.Checked = this.graphSystemBehavior1.checkBoxZoomrRecalc.Checked;

			form.graphDynamicType.Data = this.graphSystemBehavior1.Data;

			form.graphDynamicType.BrushThickness = this.graphSystemBehavior1.BrushThickness;
			
			form.Show ();
			form.graphDynamicType.Redraw ();
		}

		private void checkBoxSavePastValues_CheckedChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.deletePastData = !this.checkBoxSavePastValues.Checked;
		}

		private void buttonColor_Click ( object sender , EventArgs e ) {
			if ( this.colorDialog1.ShowDialog () == System.Windows.Forms.DialogResult.OK ) {
				Color c = this.colorDialog1.Color;
				( sender as Button ).BackColor = c;
				this.graphSystemBehavior1.ColorForNewData = c;
			}
			
		}

		private void buttonSetOfInitials_Click ( object sender , EventArgs e ) {
			SetOfInitialsForm form = new SetOfInitialsForm ( this.Equations.Keys.ToList (),this.SetOfInitials,this.ColorsOfInitials);
			

			if ( form.ShowDialog () == System.Windows.Forms.DialogResult.OK ) {
				this.SetOfInitials = form.SetOfInitials;
				this.ColorsOfInitials = form.ColorButtons.Select ( a => a.BackColor ).ToList ();
				this.buttonCalcSet.Enabled = true;
			}
		}

		int current = 0;
		private void buttonCalcSet_Click ( object sender , EventArgs e ) {
			int current = 0;
			foreach ( var key in this.SetOfInitials.ElementAt ( current ).Keys ) {
				this.Initials[key].Text = this.SetOfInitials.ElementAt ( current )[key].ToString ();
			}
			
			this.graphSystemBehavior1.ColorForNewData = this.ColorsOfInitials.ElementAt ( current );
			current++;
			this.buttonCalc_Click ( null , null );
			this.graphSystemBehavior1.ImageCreated += graphSystemBehavior1_ImageCreated;

		}

		void graphSystemBehavior1_ImageCreated () {
			if ( current == this.SetOfInitials.Count ) {
				
				this.Invoke ( new Action ( () => {
					this.graphSystemBehavior1.ImageCreated -= this.graphSystemBehavior1_ImageCreated;
				} ) );
				return;

			}
			foreach ( var key in this.SetOfInitials.ElementAt ( current ).Keys ) {
				this.Invoke ( new Action ( () => {
					this.Initials[key].Text = this.SetOfInitials.ElementAt ( current )[key].ToString ();
				} ) );
			}
			this.graphSystemBehavior1.ColorForNewData = this.ColorsOfInitials.ElementAt ( current );
			current++;
			this.Invoke ( new Action ( () => {
				this.buttonCalc_Click ( null , null );
			} ) );
		}

		private void checkBoxTime_CheckedChanged ( object sender , EventArgs e ) {
			this.labelSectionPoint.Text = ( sender as CheckBox ).Checked ? "Period" : "Point for section";
		}

		private void buttonSaveData_Click ( object sender , EventArgs e ) {
			if ( this.saveFileDialog1.ShowDialog () == System.Windows.Forms.DialogResult.OK ) {
				Serializer ser = new Serializer ();
				var te = this.graphSystemBehavior1.Data;
				ser.SerializeObjectEquationsSetData ( this.saveFileDialog1.FileName , new EquationSetWithData {
					Data = this.graphSystemBehavior1.Data ,
					EqSet = this.PickEquationsData(),
					IntegrType = this.graphSystemBehavior1.IntegrationType
				} );

			}
		}

		private void button1_Click_1 ( object sender , EventArgs e ) {
			if ( this.openFileDialog1.ShowDialog () == System.Windows.Forms.DialogResult.OK ) {
				Serializer ser = new Serializer();
				var set = ser.DeSerializeObjectEquationsSetData ( this.openFileDialog1.FileName );
				this.LoadEquations ( set.EqSet );
				this.graphSystemBehavior1.Data = set.Data;
			}
		}

		

		private void buttonSaveData_Click_1 ( object sender , EventArgs e ) {
			if ( this.saveFileDialog1.ShowDialog () == System.Windows.Forms.DialogResult.OK ) {
				Serializer ser = new Serializer ();
				var te = this.graphSystemBehavior1.Data;
				ser.SerializeObjectEquationsSetData ( this.saveFileDialog1.FileName , new EquationSetWithData {
					Data = this.graphSystemBehavior1.Data ,
					EqSet = this.PickEquationsData () ,
					IntegrType = this.graphSystemBehavior1.IntegrationType
				} );

			}
		}

		private void buttonLoadData_Click ( object sender , EventArgs e ) {
			if ( this.openFileDialog1.ShowDialog () == System.Windows.Forms.DialogResult.OK ) {
				Serializer ser = new Serializer ();
				var set = ser.DeSerializeObjectEquationsSetData ( this.openFileDialog1.FileName );
				this.LoadEquations ( set.EqSet );
				if ( this.checkBox1.Checked ) {

					this.graphSystemBehavior1.Data.AddRange ( set.Data );
				}
				else {
					this.graphSystemBehavior1.Data =set.Data ;
					}
				this.graphSystemBehavior1.RedrawWithChangeData ( this.listBoxX.SelectedItem.ToString () , this.listBoxY.SelectedItem.ToString ());
			}
		}

		private void numericUpDownBrush_ValueChanged ( object sender , EventArgs e ) {
			this.graphSystemBehavior1.BrushThickness = (int)(( sender as NumericUpDown ).Value);
		}



	}
}
