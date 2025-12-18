using System;
using System.Drawing;
using System.Windows.Forms;
using AppSoftware.LicenceEngine.Common;
using AppSoftware.LicenceEngine.KeyVerification;
using DrafterPerformance;
using WK.Libraries.TrialMakerNS;
using WK.Libraries.TrialMakerNS.Models;

namespace License
{

    public partial class ActivateForm : Form
    {
        public bool Valid { get; set; } = false;
        #region Constructor

        public ActivateForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        //private void ShowBusy()
        //{
        //    btnActivate.Text = "Activating...";
        //    Cursor = Cursors.WaitCursor;
        //    btnActivate.Enabled = false;
        //}

        //private void HideBusy()
        //{

        //    btnActivate.Enabled = true;
        //    Cursor = Cursors.Default;
        //    btnActivate.Text = "Activate";
        //}

        #endregion

        #region Events

        private void Activate_Load(object sender, EventArgs e)
        {
            txtHardwareID.Text = TrialMaker.HardwareID;
        }

        //private void BtnActivate_Click(object sender, EventArgs e)
        //{
        //    SynchToDrive synchToDrive = new SynchToDrive();
        //    var validationCOde = synchToDrive.GetCodeByMachineId(TrialMaker.HardwareID);



        //    ShowBusy();

        //    var keyByteSets = new[]
        //    {
        //            new KeyByteSet(keyByteNumber: 1, keyByteA: 58, keyByteB: 6, keyByteC: 97),
        //            new KeyByteSet(keyByteNumber: 5, keyByteA: 62, keyByteB: 4, keyByteC: 234),
        //            new KeyByteSet(keyByteNumber: 8, keyByteA: 6, keyByteB: 88, keyByteC: 32)
        //    };
        //    var pkvKeyVerifier = new PkvKeyVerifier();
        //    var key = txtLicense.Text;
        //    var pkvKeyVerificationResult = pkvKeyVerifier.VerifyKey(

        //        key: key?.Trim(),
        //        keyByteSetsToVerify: keyByteSets,

        //        // The TOTAL number of KeyByteSets used to generate the licence key in SampleKeyGenerator

        //        totalKeyByteSets: 8,

        //        // Add blacklisted seeds here if required (these could be user IDs for example)

        //        blackListedSeeds: null
        //    );
           

        //    if (pkvKeyVerificationResult == PkvKeyVerificationResult.KeyIsInvalid)
        //    {
        //        HideBusy();

        //        MessageBox.Show("The license is invalid.");

        //        // [NB] You can display the list of errors to clients.
        //        string errors = string.Join(",\n", TrialMaker.Instance.ValidationErrors);
        //        MessageBox.Show(errors);
        //    }
        //    else
        //    {
        //        HideBusy();
        //        Valid = true;
        //        MessageBox.Show(
        //            $"Thank you  for purchasing our Product!");

        //        //MainForm.Instance.UpdateTitles();

        //        Close();
        //    }
        //}

        private void BtnCopyCode_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtHardwareID.Text);
        }

        #endregion

        private void txtHardwareID_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
