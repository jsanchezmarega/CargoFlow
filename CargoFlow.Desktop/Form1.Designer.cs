namespace CargoFlow.Desktop;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        createButton = new Button();
        customerLabel = new Label();
        originLabel = new Label();
        destinationLabel = new Label();
        weightLabel = new Label();
        customerTextBox = new TextBox();
        originTextBox = new TextBox();
        destinationTextBox = new TextBox();
        weightTextBox = new TextBox();
        dataGridView1 = new DataGridView();
        idColumn = new DataGridViewTextBoxColumn();
        customerColumn = new DataGridViewTextBoxColumn();
        originColumn = new DataGridViewTextBoxColumn();
        destinationColumn = new DataGridViewTextBoxColumn();
        weightColumn = new DataGridViewTextBoxColumn();
        statusColumn = new DataGridViewTextBoxColumn();
        bindingSource1 = new BindingSource(components);
        startTransitButton = new Button();
        markAsDeliveredButton = new Button();
        cancelShipmentButton = new Button();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
        SuspendLayout();
        // 
        // createButton
        // 
        createButton.Location = new Point(153, 215);
        createButton.Name = "createButton";
        createButton.Size = new Size(75, 23);
        createButton.TabIndex = 0;
        createButton.Text = "Create";
        createButton.UseVisualStyleBackColor = true;
        createButton.Click += CreateButton_Click;
        // 
        // customerLabel
        // 
        customerLabel.AutoSize = true;
        customerLabel.Location = new Point(71, 73);
        customerLabel.Name = "customerLabel";
        customerLabel.Size = new Size(62, 15);
        customerLabel.TabIndex = 1;
        customerLabel.Text = "Customer:";
        // 
        // originLabel
        // 
        originLabel.AutoSize = true;
        originLabel.Location = new Point(90, 107);
        originLabel.Name = "originLabel";
        originLabel.Size = new Size(43, 15);
        originLabel.TabIndex = 3;
        originLabel.Text = "Origin:";
        // 
        // destinationLabel
        // 
        destinationLabel.AutoSize = true;
        destinationLabel.Location = new Point(61, 144);
        destinationLabel.Name = "destinationLabel";
        destinationLabel.Size = new Size(70, 15);
        destinationLabel.TabIndex = 5;
        destinationLabel.Text = "Destination:";
        // 
        // weightLabel
        // 
        weightLabel.AutoSize = true;
        weightLabel.Location = new Point(61, 179);
        weightLabel.Name = "weightLabel";
        weightLabel.Size = new Size(72, 15);
        weightLabel.TabIndex = 9;
        weightLabel.Text = "Weight (kg):";
        // 
        // customerTextBox
        // 
        customerTextBox.Location = new Point(153, 70);
        customerTextBox.Name = "customerTextBox";
        customerTextBox.Size = new Size(100, 23);
        customerTextBox.TabIndex = 2;
        // 
        // originTextBox
        // 
        originTextBox.Location = new Point(153, 104);
        originTextBox.Name = "originTextBox";
        originTextBox.Size = new Size(100, 23);
        originTextBox.TabIndex = 4;
        // 
        // destinationTextBox
        // 
        destinationTextBox.Location = new Point(153, 141);
        destinationTextBox.Name = "destinationTextBox";
        destinationTextBox.Size = new Size(100, 23);
        destinationTextBox.TabIndex = 8;
        // 
        // weightTextBox
        // 
        weightTextBox.Location = new Point(153, 176);
        weightTextBox.Name = "weightTextBox";
        weightTextBox.Size = new Size(100, 23);
        weightTextBox.TabIndex = 10;
        // 
        // dataGridView1
        // 
        dataGridView1.AllowUserToAddRows = false;
        dataGridView1.AutoGenerateColumns = false;
        dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView1.Columns.AddRange(new DataGridViewColumn[] { idColumn, customerColumn, originColumn, destinationColumn, weightColumn, statusColumn });
        dataGridView1.DataSource = bindingSource1;
        dataGridView1.Location = new Point(362, 70);
        dataGridView1.Name = "dataGridView1";
        dataGridView1.RowHeadersVisible = false;
        dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridView1.Size = new Size(605, 239);
        dataGridView1.TabIndex = 11;
        // 
        // idColumn
        // 
        idColumn.DataPropertyName = "Id";
        idColumn.HeaderText = "Id";
        idColumn.Name = "idColumn";
        // 
        // customerColumn
        // 
        customerColumn.DataPropertyName = "Customer";
        customerColumn.HeaderText = "Customer";
        customerColumn.Name = "customerColumn";
        // 
        // originColumn
        // 
        originColumn.DataPropertyName = "Origin";
        originColumn.HeaderText = "Origin";
        originColumn.Name = "originColumn";
        // 
        // destinationColumn
        // 
        destinationColumn.DataPropertyName = "Destination";
        destinationColumn.HeaderText = "Destination";
        destinationColumn.Name = "destinationColumn";
        // 
        // weightColumn
        // 
        weightColumn.DataPropertyName = "Weight";
        weightColumn.HeaderText = "Weight";
        weightColumn.Name = "weightColumn";
        // 
        // statusColumn
        // 
        statusColumn.DataPropertyName = "Status";
        statusColumn.HeaderText = "Status";
        statusColumn.Name = "statusColumn";
        // 
        // startTransitButton
        // 
        startTransitButton.Location = new Point(476, 348);
        startTransitButton.Name = "startTransitButton";
        startTransitButton.Size = new Size(75, 23);
        startTransitButton.TabIndex = 13;
        startTransitButton.Text = "Start transit";
        startTransitButton.UseVisualStyleBackColor = true;
        startTransitButton.Click += startTransitButton_Click;
        // 
        // markAsDeliveredButton
        // 
        markAsDeliveredButton.Location = new Point(584, 348);
        markAsDeliveredButton.Name = "markAsDeliveredButton";
        markAsDeliveredButton.Size = new Size(117, 23);
        markAsDeliveredButton.TabIndex = 14;
        markAsDeliveredButton.Text = "Mark as delivered";
        markAsDeliveredButton.UseVisualStyleBackColor = true;
        markAsDeliveredButton.Click += markAsDeliveredButton_Click;
        // 
        // cancelShipmentButton
        // 
        cancelShipmentButton.Location = new Point(729, 348);
        cancelShipmentButton.Name = "cancelShipmentButton";
        cancelShipmentButton.Size = new Size(115, 23);
        cancelShipmentButton.TabIndex = 15;
        cancelShipmentButton.Text = "Cancel shipment";
        cancelShipmentButton.UseVisualStyleBackColor = true;
        cancelShipmentButton.Click += cancelShipmentButton_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1072, 474);
        Controls.Add(cancelShipmentButton);
        Controls.Add(markAsDeliveredButton);
        Controls.Add(startTransitButton);
        Controls.Add(dataGridView1);
        Controls.Add(weightTextBox);
        Controls.Add(weightLabel);
        Controls.Add(destinationTextBox);
        Controls.Add(destinationLabel);
        Controls.Add(originTextBox);
        Controls.Add(originLabel);
        Controls.Add(customerTextBox);
        Controls.Add(customerLabel);
        Controls.Add(createButton);
        Name = "Form1";
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button createButton;
    private Label customerLabel;
    private TextBox customerTextBox;
    private TextBox originTextBox;
    private Label originLabel;
    private Label destinationLabel;
    private TextBox destinationTextBox;
    private TextBox weightTextBox;
    private Label weightLabel;
    private DataGridView dataGridView1;
    private BindingSource bindingSource1;
    private Button startTransitButton;
    private DataGridViewTextBoxColumn idColumn;
    private DataGridViewTextBoxColumn customerColumn;
    private DataGridViewTextBoxColumn originColumn;
    private DataGridViewTextBoxColumn destinationColumn;
    private DataGridViewTextBoxColumn weightColumn;
    private DataGridViewTextBoxColumn statusColumn;
    private Button markAsDeliveredButton;
    private Button cancelShipmentButton;
}
