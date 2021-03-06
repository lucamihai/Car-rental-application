﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Car_Rental_Application.Forms
{
    public partial class FormConfirmation : Form
    {
        public FormConfirmation(string action = null, string consequence = null)
        {
            InitializeComponent();

            UpdateLanguage();

            if (action == null)
            {
                labelAreYouSure.Text = string.Empty;
            }

            if (action != null)
            {
                string translatedAction = Program.Language.Translate(action);
                labelAreYouSure.Text = string.Format("{0}: {1}", Program.Language.Translate("Action"), translatedAction);
            }

            if (consequence != null)
            {
                string translatedConsequence = Program.Language.Translate(consequence);
                labelAreYouSure.Text += string.Format("\r\n{0}: {1}", Program.Language.Translate("Consequence"), translatedConsequence);
            }
        }

        void UpdateLanguage()
        {
            this.Text = string.Format("{0}?", Program.Language.Translate("Are you sure"));
            buttonConfirm.Text = Program.Language.Translate("Confirm");
            buttonCancel.Text = Program.Language.Translate("Cancel");
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
