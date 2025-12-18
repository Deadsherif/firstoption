using AppSoftware.LicenceEngine.Common;
using AppSoftware.LicenceEngine.KeyVerification;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DrafterPerformance;
using System;
using System.Windows.Forms;
using WK.Libraries.TrialMakerNS;
using WK.Libraries.TrialMakerNS.Models;

namespace License
{
    public static class license
    {

        public static bool ValidateLicense()
        {
			try
			{

                SynchToDrive synchToDrive = new SynchToDrive();
                var validationCOde = synchToDrive.GetCodeByMachineId(TrialMaker.HardwareID);

               



                var keyByteSets = new[]
                {
                    new KeyByteSet(keyByteNumber: 1, keyByteA: 58, keyByteB: 6, keyByteC: 97),
                    new KeyByteSet(keyByteNumber: 5, keyByteA: 62, keyByteB: 4, keyByteC: 234),
                    new KeyByteSet(keyByteNumber: 8, keyByteA: 6, keyByteB: 88, keyByteC: 32)
            };
                var pkvKeyVerifier = new PkvKeyVerifier();
                var key = validationCOde;
                var pkvKeyVerificationResult = pkvKeyVerifier.VerifyKey(

                    key: key?.Trim(),
                    keyByteSetsToVerify: keyByteSets,

                    // The TOTAL number of KeyByteSets used to generate the licence key in SampleKeyGenerator

                    totalKeyByteSets: 8,

                    // Add blacklisted seeds here if required (these could be user IDs for example)

                    blackListedSeeds: null
                );


                if (pkvKeyVerificationResult == PkvKeyVerificationResult.KeyIsInvalid)
                {

                    ActivateForm activateForm = new ActivateForm();
                    activateForm.Show();

                    // [NB] You can display the list of errors to clients.
                    string errors = string.Join(",\n", TrialMaker.Instance.ValidationErrors);

                    return false;
                }
                else
                {

                    MessageBox.Show(
                        $"Thank you for purchasing our Product!");
                    return true;
                    //MainForm.Instance.UpdateTitles();

                }
            }
			catch (Exception)
			{
                return false;
			}
        }
    }
}
