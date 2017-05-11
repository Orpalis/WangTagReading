using System;
using System.IO;
using System.Windows.Forms;
using WangTiffToTiff.Properties;

namespace WangTiffToTiff
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// The constructor initializes members to their default values.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// The GdPicture license is initialized as soon as the form is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            GdPicture14.LicenseManager oLicenseManager = new GdPicture14.LicenseManager();
            oLicenseManager.RegisterKEY("XXX");
            //Please replace XXX by a valid commercial or demo key.
        }

        /// <summary>
        /// The settings are saved when the form is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }

        /// <summary>
        /// The conversion of a single file is achieved when the user presses the button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btConvertFile_Click(object sender, EventArgs e)
        {
            // Verify the parameters

            if (string.IsNullOrWhiteSpace(tbInputFilePath.Text))
            {
                MessageBox.Show("Please select an input file.", Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbOutputFilePath.Text))
            {
                MessageBox.Show("Please select an output file.", Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            // Run the conversion
            string errorMessage = "Success";
            Cursor.Current = Cursors.WaitCursor;
            bool bResult = WangTiffToGdPictureTiffConvert.Convert(tbOutputFilePath.Text, ref errorMessage, tbInputFilePath.Text);
            Cursor.Current = Cursors.Default;
            if (bResult)
            {
                MessageBox.Show("The file " + tbInputFilePath.Text +
                                " with its Wang annotations has been converted to " + tbOutputFilePath.Text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// The conversion is achieved when the user presses the button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btConvertFolder_Click(object sender, EventArgs e)
        {
            // Verify the parameters

            if (string.IsNullOrWhiteSpace(tbInputFolderPath.Text))
            {
                MessageBox.Show("Please select an input folder.", Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbOutputFolderPath.Text))
            {
                MessageBox.Show("Please select an output folder.", Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            // Run the conversion

            string[] files = Directory.GetFiles(tbInputFolderPath.Text, "*.tif", SearchOption.AllDirectories);

            foreach (string inputFilePath in files)
            {
                // Run the conversion
                string errorMessage = "Success";
                string outputFilePath = tbOutputFolderPath.Text + "\\" + Path.GetFileNameWithoutExtension(inputFilePath) +
                                        ".tif";

                Cursor.Current = Cursors.WaitCursor;
                bool bResult = WangTiffToGdPictureTiffConvert.Convert(outputFilePath, ref errorMessage, inputFilePath, _textExtension);
                Cursor.Current = Cursors.Default;
                if (!bResult)
                {
                    if (MessageBox.Show(
                        "An error occurred during the process of file " + inputFilePath + "\n" + errorMessage +
                        "\nContinue?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    {
                        break;
                    }
                }
            }

            MessageBox.Show("The files available in folder " + tbInputFolderPath.Text +
                            " with its Wang annotations have been converted.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// An open file dialog is displayed to the user so he can select the input file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBrowseInputFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "TIFF File (*.tif)|*.tif;*.tiff",
                FileName = ""
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbInputFilePath.Text = dlg.FileName;
            }
        }

        /// <summary>
        /// A save file dialog is displayed to the user so he can select the output file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBrowseOutputFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = "output",
                Filter = "TIFF File (*.tif)|*.tif;*.tiff"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbOutputFilePath.Text = dlg.FileName;
            }
        }

        /// <summary>
        /// An browse folder dialog is displayed to the user so he can select the input folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBrowseInputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbInputFolderPath.Text = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// An browse folder dialog is displayed to the user so he can select the output folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBrowseOutputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbOutputFolderPath.Text = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// The LoadSettings method loads the settings and updates the controls.
        /// </summary>
        private void LoadSettings()
        {
            tbInputFilePath.Text = Settings.Default.InputFilePath;
            tbOutputFilePath.Text = Settings.Default.OutputFilePath;
            tbInputFolderPath.Text = Settings.Default.InputFolderPath;
            tbOutputFolderPath.Text = Settings.Default.OutputFolderPath;
            _textExtension = Settings.Default.TextExtension;
            switch (Settings.Default.TextExtension)
            {
                case TextExtensionReference:
                    rbReferenceTextOutput.Checked = true;
                    break;

                case TextExtensionTest:
                    rbTestTextOutput.Checked = true;
                    break;

                case TextExtensionNoText:
                default:
                    rbNoTextOutput.Checked = true;
                    break;
            }
        }

        /// <summary>
        /// The SaveSettings method updates the settings with the content of the control and saves the settings.
        /// </summary>
        private void SaveSettings()
        {
            Settings.Default.InputFilePath = tbInputFilePath.Text;
            Settings.Default.OutputFilePath = tbOutputFilePath.Text;
            Settings.Default.InputFolderPath = tbInputFolderPath.Text;
            Settings.Default.OutputFolderPath = tbOutputFolderPath.Text;
            Settings.Default.TextExtension = _textExtension;
            Settings.Default.Save();
        }

        /// <summary>
        /// Update the text extension with the radio button selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonTestTextOutput_CheckedChanged(object sender, EventArgs e)
        {
            _textExtension = TextExtensionTest;
        }

        /// <summary>
        /// Update the text extension with the radio button selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonNoTextOutput_CheckedChanged(object sender, EventArgs e)
        {
            _textExtension = TextExtensionNoText;
        }

        /// <summary>
        /// Update the text extension with the radio button selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonReferenceTextOutput_CheckedChanged(object sender, EventArgs e)
        {
            _textExtension = TextExtensionReference;
        }

        /// <summary>
        /// The text extension.
        /// </summary>
        private string _textExtension;

        /// <summary>
        /// No text to create.
        /// </summary>
        private const string TextExtensionNoText = "";

        /// <summary>
        /// Test text to be created.
        /// </summary>
        private const string TextExtensionTest = ".test.txt";

        /// <summary>
        /// Reference text to be created.
        /// </summary>
        private const string TextExtensionReference = ".ref.txt";
    }
}
