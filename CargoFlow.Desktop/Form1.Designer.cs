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
        originTextBox = new TextBox();
        destinationTextBox = new TextBox();
        weightTextBox = new TextBox();
        dataGridView1 = new DataGridView();
        bindingSource1 = new BindingSource(components);
        startTransitButton = new Button();
        markAsDeliveredButton = new Button();
        cancelShipmentButton = new Button();
        createCustomerButton = new Button();
        customerNameLabel = new Label();
        customerNameTextBox = new TextBox();
        customerComboBox = new ComboBox();
        customerColumn = new DataGridViewTextBoxColumn();
        idColumn = new DataGridViewTextBoxColumn();
        originColumn = new DataGridViewTextBoxColumn();
        destinationColumn = new DataGridViewTextBoxColumn();
        weightColumn = new DataGridViewTextBoxColumn();
        statusColumn = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
        SuspendLayout();
        // 
        // createButton
        // 
        createButton.Location = new Point(132, 403);
        createButton.Name = "createButton";
        createButton.Size = new Size(132, 23);
        createButton.TabIndex = 0;
        createButton.Text = "Create shipment";
        createButton.UseVisualStyleBackColor = true;
        createButton.Click += CreateButton_Click;
        // 
        // customerLabel
        // 
        customerLabel.AutoSize = true;
        customerLabel.Location = new Point(67, 250);
        customerLabel.Name = "customerLabel";
        customerLabel.Size = new Size(62, 15);
        customerLabel.TabIndex = 1;
        customerLabel.Text = "Customer:";
        // 
        // originLabel
        // 
        originLabel.AutoSize = true;
        originLabel.Location = new Point(86, 284);
        originLabel.Name = "originLabel";
        originLabel.Size = new Size(43, 15);
        originLabel.TabIndex = 3;
        originLabel.Text = "Origin:";
        // 
        // destinationLabel
        // 
        destinationLabel.AutoSize = true;
        destinationLabel.Location = new Point(57, 321);
        destinationLabel.Name = "destinationLabel";
        destinationLabel.Size = new Size(70, 15);
        destinationLabel.TabIndex = 5;
        destinationLabel.Text = "Destination:";
        // 
        // weightLabel
        // 
        weightLabel.AutoSize = true;
        weightLabel.Location = new Point(57, 356);
        weightLabel.Name = "weightLabel";
        weightLabel.Size = new Size(72, 15);
        weightLabel.TabIndex = 9;
        weightLabel.Text = "Weight (kg):";
        // 
        // originTextBox
        // 
        originTextBox.Location = new Point(149, 281);
        originTextBox.Name = "originTextBox";
        originTextBox.Size = new Size(100, 23);
        originTextBox.TabIndex = 4;
        // 
        // destinationTextBox
        // 
        destinationTextBox.Location = new Point(149, 318);
        destinationTextBox.Name = "destinationTextBox";
        destinationTextBox.Size = new Size(100, 23);
        destinationTextBox.TabIndex = 8;
        // 
        // weightTextBox
        // 
        weightTextBox.Location = new Point(149, 353);
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
        // createCustomerButton
        // 
        createCustomerButton.Location = new Point(164, 129);
        createCustomerButton.Name = "createCustomerButton";
        createCustomerButton.Size = new Size(107, 23);
        createCustomerButton.TabIndex = 16;
        createCustomerButton.Text = "Create customer";
        createCustomerButton.UseVisualStyleBackColor = true;
        createCustomerButton.Click += createCustomerButton_Click;
        // 
        // customerNameLabel
        // 
        customerNameLabel.AutoSize = true;
        customerNameLabel.Location = new Point(51, 92);
        customerNameLabel.Name = "customerNameLabel";
        customerNameLabel.Size = new Size(95, 15);
        customerNameLabel.TabIndex = 17;
        customerNameLabel.Text = "Customer name:";
        // 
        // customerNameTextBox
        // 
        customerNameTextBox.Location = new Point(164, 89);
        customerNameTextBox.Name = "customerNameTextBox";
        customerNameTextBox.Size = new Size(100, 23);
        customerNameTextBox.TabIndex = 18;
        // 
        // customerComboBox
        // 
        customerComboBox.FormattingEnabled = true;
        customerComboBox.Location = new Point(150, 242);
        customerComboBox.Name = "customerComboBox";
        customerComboBox.Size = new Size(121, 23);
        customerComboBox.TabIndex = 19;
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
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1162, 507);
        Controls.Add(customerComboBox);
        Controls.Add(customerNameTextBox);
        Controls.Add(customerNameLabel);
        Controls.Add(createCustomerButton);
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
    private TextBox originTextBox;
    private Label originLabel;
    private Label destinationLabel;
    private TextBox destinationTextBox;
    private TextBox weightTextBox;
    private Label weightLabel;
    private DataGridView dataGridView1;
    private BindingSource bindingSource1;
    private Button startTransitButton;
    private Button markAsDeliveredButton;
    private Button cancelShipmentButton;
    private Button createCustomerButton;
    private Label customerNameLabel;
    private TextBox customerNameTextBox;
    private ComboBox customerComboBox;
    private DataGridViewTextBoxColumn customerColumn;
    private DataGridViewTextBoxColumn idColumn;
    private DataGridViewTextBoxColumn originColumn;
    private DataGridViewTextBoxColumn destinationColumn;
    private DataGridViewTextBoxColumn weightColumn;
    private DataGridViewTextBoxColumn statusColumn;
}
