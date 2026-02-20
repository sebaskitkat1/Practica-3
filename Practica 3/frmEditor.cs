using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;

namespace Calculadora.Formularios
{
    public partial class frmEditor : Form
    {
        Boolean saved = false;
        string path = "";
        int contadorPalabras = 0;
        string texto = "";
        public frmEditor()
        {
            InitializeComponent();
        }

        private void rtbEditor_TextChanged(object sender, EventArgs e)
        {
            texto = rtbEditor.Text;
            string[] palabras = texto.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            contadorPalabras = palabras.Length;
            this.tssStatus.Text = $"Numero de palabras: {contadorPalabras}";
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.opfEditor.ShowDialog();
            if (File.Exists(opfEditor.FileName))
            {
                path = opfEditor.FileName; 
                rtbEditor.Text = File.ReadAllText(path);
                saved = true;
                buscarXml();
            }
        }

        private void buscarXml()
        {
            if (string.IsNullOrEmpty(path))
                return;

            string carpeta = Path.GetDirectoryName(path);
            string rutaXml = Path.Combine(carpeta, "Caracteristicas.xml");

            if (File.Exists(rutaXml))
            {
                try
                {
                    XElement xml = XElement.Load(rutaXml);
                    string fuenteTexto = xml.Element("Fuente")?.Value;
                    string color = xml.Element("Color")?.Value;
                    string colorDeFondo = xml.Element("ColorDeFondo")?.Value;

                    if (!string.IsNullOrEmpty(fuenteTexto))
                    {
                        string nombre = "Microsoft Sans Serif";
                        float tamano = 12.0f;

                        if (fuenteTexto.Contains("Name="))
                        {
                            int inicioNombre = fuenteTexto.IndexOf("Name=") + 5;
                            int finNombre = fuenteTexto.IndexOf(",", inicioNombre);
                            nombre = fuenteTexto.Substring(inicioNombre, finNombre - inicioNombre);
                        }

                        if (fuenteTexto.Contains("Size="))
                        {
                            int inicioSize = fuenteTexto.IndexOf("Size=") + 5;
                            int finSize = fuenteTexto.IndexOf(",", inicioSize);
                            string valorSize = fuenteTexto.Substring(inicioSize, finSize - inicioSize);
                            tamano = float.Parse(valorSize);
                        }

                        rtbEditor.Font = new Font(nombre, tamano);
                    }

                    if (!string.IsNullOrEmpty(color))
                    {
                        int argbColor = int.Parse(color);
                        rtbEditor.ForeColor = Color.FromArgb(argbColor);
                    }

                    if (!string.IsNullOrEmpty(colorDeFondo))
                    {
                        int argbColorDeFondo = int.Parse(colorDeFondo);
                        rtbEditor.BackColor = Color.FromArgb(argbColorDeFondo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar el archivo XML: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!saved)
            {
                guardar();
                saved = true;
            }
            else
            {
                using (StreamWriter archivo = new StreamWriter(path))
                {
                    archivo.Write(rtbEditor.Text);
                    guardarXml();
                }
            }
        }

        private void guardarXml()
        {
            string carpeta = Path.GetDirectoryName(path);
            string rutaXml = Path.Combine(carpeta, "Caracteristicas.xml");
            XElement xml = new XElement("Caracteristicas");
            xml.Add(new XElement("Caracteristicas"),
            new XElement("FuenteNombre", rtbEditor.Font.Name),
            new XElement("FuenteTamaño", rtbEditor.Font.Size),
            new XElement("Color", rtbEditor.ForeColor.ToArgb().ToString()),
            new XElement("ColorDeFondo", rtbEditor.BackColor.ToArgb().ToString())
        );
            try
            {
                xml.Save(rutaXml);
                MessageBox.Show("Archivo guardado correctamente.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void guardar()
        {
            if (this.sfdEditor.ShowDialog() == DialogResult.OK)
            {
                path = sfdEditor.FileName;
                using (StreamWriter archivo = new StreamWriter(path))
                {
                    archivo.Write(rtbEditor.Text);
                }
                guardarXml();
            }
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guardar();
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbEditor.Clear();
            path = "";
            saved = false;
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void stsEditor_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tssStatus_Click(object sender, EventArgs e)
        {
            string[] palabras = texto.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string[] parrafos = texto.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            MessageBox.Show("Estadisicas: \n\nPalabras: " + contadorPalabras + "\nCaracteres(con espacio): " + texto.Length.ToString()
                + "\nParrafos: " + parrafos.Length.ToString(),
                "Contador de palabras", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void fuenteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ftdEditor.ShowDialog() == DialogResult.OK)
            {
                rtbEditor.Font = ftdEditor.Font;
            }
        }

        private void mnsEditor_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void fontDialog1_Apply(object sender, EventArgs e)
        {

        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cldEditor.ShowDialog() == DialogResult.OK)
            {
                rtbEditor.ForeColor = cldEditor.Color;
            }
        }

        private void colorDeFondoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cldEditor.ShowDialog() == DialogResult.OK)
            {
                rtbEditor.BackColor = cldEditor.Color;
            }
        }
    }
}
