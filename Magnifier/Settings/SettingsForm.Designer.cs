
using System.Drawing;
using System.Windows.Forms;

namespace Magnifier
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.StartPosition = FormStartPosition.CenterScreen; // Center the window
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // No resizing
            this.MaximizeBox = false; // Disable maximize button
            this.MinimizeBox = false; // Disable minimize button
            this.ControlBox = true; // Keep only the close button (X)

            this.Text = "Settings";
            this.Size = new System.Drawing.Size(400, 250);

            int labelX = 20;  // X position for labels
            int inputX = 150; // X position for inputs
            int rowHeight = 40; // Vertical space between rows
            int currentY = 20; // Starting Y position

            // FPS Label and Input
            var fpsLabel = new Label
            {
                Text = "FPS:",
                Location = new Point(labelX, currentY),
                AutoSize = true
            };

            var fpsInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 120,
                Value = parentMagnifier.FPS,
                Location = new Point(inputX, currentY - 3), // Align with label
                Width = 60
            };

            currentY += rowHeight;

            // Zoom Factor Label and Input
            var zoomLabel = new Label
            {
                Text = "Zoom Factor:",
                Location = new Point(labelX, currentY),
                AutoSize = true
            };

            var zoomInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 10,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Value = (decimal)parentMagnifier.ZoomFactor,
                Location = new Point(inputX, currentY - 3),
                Width = 60
            };

            currentY += rowHeight;

            // Transparency Key Label, Preview, and Button
            var transparencyLabel = new Label
            {
                Text = "Transparency Key:",
                Location = new Point(labelX, currentY),
                AutoSize = true
            };

            var colorPreview = new Panel
            {
                BackColor = selectedTransparencyKey,
                Location = new Point(inputX, currentY),
                Size = new Size(30, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            var colorButton = new Button
            {
                Text = "Select Color",
                Location = new Point(inputX + 40, currentY),
                AutoSize = true
            };

            colorButton.Click += (s, e) =>
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.Color = selectedTransparencyKey;
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedTransparencyKey = colorDialog.Color;
                        colorPreview.BackColor = selectedTransparencyKey; // Update preview
                    }
                }
            };

            currentY += rowHeight;

            var staticModeLabel = new Label
            {
                Text = "Static Magnification:",
                Location = new Point(labelX, currentY),
                AutoSize = true
            };

            var staticModeCheckbox = new CheckBox
            {
                Checked = parentMagnifier.IsStaticMagnification, // Reflect current state
                Location = new Point(inputX, currentY - 3)
            };

            currentY += rowHeight;

            // Save Button
            var saveButton = new Button
            {
                Text = "Save",
                Location = new Point(this.ClientSize.Width / 2 - 40, this.ClientSize.Height - 40), // Centered horizontally
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            saveButton.Click += (s, e) =>
            {
                parentMagnifier.FPS = (int)fpsInput.Value;
                parentMagnifier.ZoomFactor = (float)zoomInput.Value;
                parentMagnifier.IsStaticMagnification = staticModeCheckbox.Checked; // Save static magnification setting
                parentMagnifier.TransparencyKeyColor = selectedTransparencyKey; // Apply transparency key
                this.Close();
            };

            // Add controls to the form
            this.Controls.Add(fpsLabel);
            this.Controls.Add(fpsInput);
            this.Controls.Add(zoomLabel);
            this.Controls.Add(zoomInput);
            this.Controls.Add(transparencyLabel);
            this.Controls.Add(colorPreview);
            this.Controls.Add(colorButton);
            this.Controls.Add(staticModeLabel);
            this.Controls.Add(staticModeCheckbox);
            this.Controls.Add(saveButton);
        }

        #endregion
    }
}