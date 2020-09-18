using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models"), "Candle N130112.3DS", "plate.3DS", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            //sve sto ucitamo skaliramo tako da odgovara nasoj sceni
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5: this.Close(); break;
                case Key.T: if (m_world.RotationX > -85) m_world.RotationX -= 5.0f; break;
                case Key.G: if (m_world.RotationX < 85) m_world.RotationX += 5.0f; break;
                case Key.F: if (m_world.RotationY > -85) m_world.RotationY -= 5.0f; break;
                case Key.H: if (m_world.RotationY < 85) m_world.RotationY += 5.0f; break;
                case Key.Add: m_world.SceneDistance -= 50.0f; break;
                case Key.Subtract: m_world.SceneDistance += 50.0f; break;
                case Key.C: m_world.SetTimer1(); break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool)opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                        }
                    }
                    break;
            }
        }

        private void rotation_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                m_world.plateRotation = (float)Convert.ToDouble(rotation_value.Text);
            }
            catch (Exception)
            {
                rotation_value.Text = "0";
            }

        }
        
        private void diffuseR_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //crveno
            try
            {
                m_world.Diffuse0 = (float)Convert.ToDouble(diffuseR_slider.Value);
            }
            catch (Exception) { }

        }
        private void diffuseB_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //plavo
            try
            {
                m_world.Diffuse2 = (float)Convert.ToDouble(diffuseB_slider.Value);
            }
            catch (Exception) { }
        }

        private void diffuseG_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //zeleno
            try
            {
                m_world.Diffuse1 = (float)Convert.ToDouble(diffuseG_slider.Value);
            }
            catch (Exception) { }
        }

        private void scale_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                m_world.Scale_Plate_Slider = (float)Convert.ToDouble(scale_slider.Value);
            }
            catch (Exception) { }
        }
    }
}
