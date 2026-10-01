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
        createButton = new Button();
        customerLabel = new Label();
        originLabel = new Label();
        destinationLabel = new Label();
        weightLabel = new Label();
        originTextBox = new TextBox();
        destinationTextBox = new TextBox();
        weightTextBox = new TextBox();
        startTransitButton = new Button();
        markAsDeliveredButton = new Button();
        cancelShipmentButton = new Button();
        createCustomerButton = new Button();
        customerNameLabel = new Label();
        customerNameTextBox = new TextBox();
        customerComboBox = new ComboBox();
        shipmentGridControl = new DevExpress.XtraGrid.GridControl();
        shipmentGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
        mainTableLayoutPanel = new TableLayoutPanel();
        shipmentsGroupBox = new GroupBox();
        shipmentsTableLayoutPanel = new TableLayoutPanel();
        shipmentActionsFlowLayoutPanel = new FlowLayoutPanel();
        customersGroupBox = new GroupBox();
        newShipmentGroupBox = new GroupBox();
        idColumn = new DevExpress.XtraGrid.Columns.GridColumn();
        customerColumn = new DevExpress.XtraGrid.Columns.GridColumn();
        originColumn = new DevExpress.XtraGrid.Columns.GridColumn();
        destinationColumn = new DevExpress.XtraGrid.Columns.GridColumn();
        weightColumn = new DevExpress.XtraGrid.Columns.GridColumn();
        statusColumn = new DevExpress.XtraGrid.Columns.GridColumn();
        ((System.ComponentModel.ISupportInitialize)shipmentGridControl).BeginInit();
        ((System.ComponentModel.ISupportInitialize)shipmentGridView).BeginInit();
        mainTableLayoutPanel.SuspendLayout();
        shipmentsGroupBox.SuspendLayout();
        shipmentsTableLayoutPanel.SuspendLayout();
        shipmentActionsFlowLayoutPanel.SuspendLayout();
        customersGroupBox.SuspendLayout();
        newShipmentGroupBox.SuspendLayout();
        SuspendLayout();
        // 
        // createButton
        // 
        createButton.Location = new Point(6, 212);
        createButton.Name = "createButton";
        createButton.Size = new Size(132, 23);
        createButton.TabIndex = 4;
        createButton.Text = "Create shipment";
        createButton.UseVisualStyleBackColor = true;
        createButton.Click += CreateButton_Click;
        // 
        // customerLabel
        // 
        customerLabel.AutoSize = true;
        customerLabel.Location = new Point(6, 33);
        customerLabel.Name = "customerLabel";
        customerLabel.Size = new Size(62, 15);
        customerLabel.TabIndex = 1;
        customerLabel.Text = "Customer:";
        // 
        // originLabel
        // 
        originLabel.AutoSize = true;
        originLabel.Location = new Point(6, 77);
        originLabel.Name = "originLabel";
        originLabel.Size = new Size(43, 15);
        originLabel.TabIndex = 3;
        originLabel.Text = "Origin:";
        // 
        // destinationLabel
        // 
        destinationLabel.AutoSize = true;
        destinationLabel.Location = new Point(6, 121);
        destinationLabel.Name = "destinationLabel";
        destinationLabel.Size = new Size(70, 15);
        destinationLabel.TabIndex = 5;
        destinationLabel.Text = "Destination:";
        // 
        // weightLabel
        // 
        weightLabel.AutoSize = true;
        weightLabel.Location = new Point(6, 165);
        weightLabel.Name = "weightLabel";
        weightLabel.Size = new Size(72, 15);
        weightLabel.TabIndex = 9;
        weightLabel.Text = "Weight (kg):";
        // 
        // originTextBox
        // 
        originTextBox.Location = new Point(6, 95);
        originTextBox.Name = "originTextBox";
        originTextBox.Size = new Size(195, 23);
        originTextBox.TabIndex = 1;
        // 
        // destinationTextBox
        // 
        destinationTextBox.Location = new Point(6, 139);
        destinationTextBox.Name = "destinationTextBox";
        destinationTextBox.Size = new Size(195, 23);
        destinationTextBox.TabIndex = 2;
        // 
        // weightTextBox
        // 
        weightTextBox.Location = new Point(6, 183);
        weightTextBox.Name = "weightTextBox";
        weightTextBox.Size = new Size(195, 23);
        weightTextBox.TabIndex = 3;
        // 
        // startTransitButton
        // 
        startTransitButton.Location = new Point(292, 3);
        startTransitButton.Name = "startTransitButton";
        startTransitButton.Size = new Size(75, 23);
        startTransitButton.TabIndex = 0;
        startTransitButton.Text = "Start transit";
        startTransitButton.UseVisualStyleBackColor = true;
        startTransitButton.Click += startTransitButton_Click;
        // 
        // markAsDeliveredButton
        // 
        markAsDeliveredButton.Location = new Point(373, 3);
        markAsDeliveredButton.Name = "markAsDeliveredButton";
        markAsDeliveredButton.Size = new Size(117, 23);
        markAsDeliveredButton.TabIndex = 1;
        markAsDeliveredButton.Text = "Mark as delivered";
        markAsDeliveredButton.UseVisualStyleBackColor = true;
        markAsDeliveredButton.Click += markAsDeliveredButton_Click;
        // 
        // cancelShipmentButton
        // 
        cancelShipmentButton.Location = new Point(496, 3);
        cancelShipmentButton.Name = "cancelShipmentButton";
        cancelShipmentButton.Size = new Size(115, 23);
        cancelShipmentButton.TabIndex = 2;
        cancelShipmentButton.Text = "Cancel shipment";
        cancelShipmentButton.UseVisualStyleBackColor = true;
        cancelShipmentButton.Click += cancelShipmentButton_Click;
        // 
        // createCustomerButton
        // 
        createCustomerButton.Location = new Point(6, 80);
        createCustomerButton.Name = "createCustomerButton";
        createCustomerButton.Size = new Size(107, 23);
        createCustomerButton.TabIndex = 1;
        createCustomerButton.Text = "Create customer";
        createCustomerButton.UseVisualStyleBackColor = true;
        createCustomerButton.Click += createCustomerButton_Click;
        // 
        // customerNameLabel
        // 
        customerNameLabel.AutoSize = true;
        customerNameLabel.Location = new Point(6, 33);
        customerNameLabel.Name = "customerNameLabel";
        customerNameLabel.Size = new Size(95, 15);
        customerNameLabel.TabIndex = 17;
        customerNameLabel.Text = "Customer name:";
        // 
        // customerNameTextBox
        // 
        customerNameTextBox.Location = new Point(6, 51);
        customerNameTextBox.Name = "customerNameTextBox";
        customerNameTextBox.Size = new Size(195, 23);
        customerNameTextBox.TabIndex = 0;
        // 
        // customerComboBox
        // 
        customerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        customerComboBox.FormattingEnabled = true;
        customerComboBox.Location = new Point(6, 51);
        customerComboBox.Name = "customerComboBox";
        customerComboBox.Size = new Size(195, 23);
        customerComboBox.TabIndex = 0;
        // 
        // shipmentGridControl
        // 
        shipmentGridControl.Dock = DockStyle.Fill;
        shipmentGridControl.Location = new Point(3, 3);
        shipmentGridControl.MainView = shipmentGridView;
        shipmentGridControl.Name = "shipmentGridControl";
        shipmentGridControl.Size = new Size(614, 502);
        shipmentGridControl.TabIndex = 0;
        shipmentGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { shipmentGridView });
        // 
        // shipmentGridView
        // 
        shipmentGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { idColumn, customerColumn, originColumn, destinationColumn, weightColumn, statusColumn });
        shipmentGridView.GridControl = shipmentGridControl;
        shipmentGridView.Name = "shipmentGridView";
        shipmentGridView.OptionsBehavior.Editable = false;
        shipmentGridView.OptionsFind.AlwaysVisible = true;
        shipmentGridView.OptionsSelection.MultiSelect = true;
        shipmentGridView.OptionsView.ShowAutoFilterRow = true;
        shipmentGridView.OptionsView.ShowFooter = true;
        // 
        // mainTableLayoutPanel
        // 
        mainTableLayoutPanel.ColumnCount = 3;
        mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
        mainTableLayoutPanel.Controls.Add(shipmentsGroupBox, 2, 0);
        mainTableLayoutPanel.Controls.Add(customersGroupBox, 0, 0);
        mainTableLayoutPanel.Controls.Add(newShipmentGroupBox, 1, 0);
        mainTableLayoutPanel.Dock = DockStyle.Fill;
        mainTableLayoutPanel.Location = new Point(0, 0);
        mainTableLayoutPanel.Name = "mainTableLayoutPanel";
        mainTableLayoutPanel.Padding = new Padding(12);
        mainTableLayoutPanel.RowCount = 1;
        mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainTableLayoutPanel.Size = new Size(1184, 611);
        mainTableLayoutPanel.TabIndex = 21;
        // 
        // shipmentsGroupBox
        // 
        shipmentsGroupBox.Controls.Add(shipmentsTableLayoutPanel);
        shipmentsGroupBox.Dock = DockStyle.Fill;
        shipmentsGroupBox.Location = new Point(540, 18);
        shipmentsGroupBox.Margin = new Padding(6);
        shipmentsGroupBox.Name = "shipmentsGroupBox";
        shipmentsGroupBox.Size = new Size(626, 575);
        shipmentsGroupBox.TabIndex = 0;
        shipmentsGroupBox.TabStop = false;
        shipmentsGroupBox.Text = "Shipments";
        // 
        // shipmentsTableLayoutPanel
        // 
        shipmentsTableLayoutPanel.ColumnCount = 1;
        shipmentsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        shipmentsTableLayoutPanel.Controls.Add(shipmentGridControl, 0, 0);
        shipmentsTableLayoutPanel.Controls.Add(shipmentActionsFlowLayoutPanel, 0, 1);
        shipmentsTableLayoutPanel.Dock = DockStyle.Fill;
        shipmentsTableLayoutPanel.Location = new Point(3, 19);
        shipmentsTableLayoutPanel.Name = "shipmentsTableLayoutPanel";
        shipmentsTableLayoutPanel.RowCount = 2;
        shipmentsTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        shipmentsTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        shipmentsTableLayoutPanel.Size = new Size(620, 553);
        shipmentsTableLayoutPanel.TabIndex = 21;
        // 
        // shipmentActionsFlowLayoutPanel
        // 
        shipmentActionsFlowLayoutPanel.Controls.Add(cancelShipmentButton);
        shipmentActionsFlowLayoutPanel.Controls.Add(markAsDeliveredButton);
        shipmentActionsFlowLayoutPanel.Controls.Add(startTransitButton);
        shipmentActionsFlowLayoutPanel.Dock = DockStyle.Fill;
        shipmentActionsFlowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
        shipmentActionsFlowLayoutPanel.Location = new Point(3, 511);
        shipmentActionsFlowLayoutPanel.Name = "shipmentActionsFlowLayoutPanel";
        shipmentActionsFlowLayoutPanel.Size = new Size(614, 39);
        shipmentActionsFlowLayoutPanel.TabIndex = 21;
        shipmentActionsFlowLayoutPanel.WrapContents = false;
        // 
        // customersGroupBox
        // 
        customersGroupBox.Controls.Add(customerNameTextBox);
        customersGroupBox.Controls.Add(createCustomerButton);
        customersGroupBox.Controls.Add(customerNameLabel);
        customersGroupBox.Dock = DockStyle.Fill;
        customersGroupBox.Location = new Point(18, 18);
        customersGroupBox.Margin = new Padding(6);
        customersGroupBox.Name = "customersGroupBox";
        customersGroupBox.Size = new Size(220, 575);
        customersGroupBox.TabIndex = 21;
        customersGroupBox.TabStop = false;
        customersGroupBox.Text = "Customers";
        // 
        // newShipmentGroupBox
        // 
        newShipmentGroupBox.Controls.Add(customerComboBox);
        newShipmentGroupBox.Controls.Add(weightTextBox);
        newShipmentGroupBox.Controls.Add(createButton);
        newShipmentGroupBox.Controls.Add(weightLabel);
        newShipmentGroupBox.Controls.Add(customerLabel);
        newShipmentGroupBox.Controls.Add(destinationTextBox);
        newShipmentGroupBox.Controls.Add(originLabel);
        newShipmentGroupBox.Controls.Add(destinationLabel);
        newShipmentGroupBox.Controls.Add(originTextBox);
        newShipmentGroupBox.Dock = DockStyle.Fill;
        newShipmentGroupBox.Location = new Point(250, 18);
        newShipmentGroupBox.Margin = new Padding(6);
        newShipmentGroupBox.Name = "newShipmentGroupBox";
        newShipmentGroupBox.Size = new Size(278, 575);
        newShipmentGroupBox.TabIndex = 22;
        newShipmentGroupBox.TabStop = false;
        newShipmentGroupBox.Text = "New Shipment";
        // 
        // idColumn
        // 
        idColumn.Caption = "ID";
        idColumn.FieldName = "Id";
        idColumn.Name = "idColumn";
        idColumn.Visible = true;
        idColumn.VisibleIndex = 0;
        // 
        // customerColumn
        // 
        customerColumn.Caption = "Customer";
        customerColumn.FieldName = "Customer";
        customerColumn.Name = "customerColumn";
        customerColumn.Visible = true;
        customerColumn.VisibleIndex = 1;
        // 
        // originColumn
        // 
        originColumn.Caption = "Origin";
        originColumn.FieldName = "Origin";
        originColumn.Name = "originColumn";
        originColumn.Visible = true;
        originColumn.VisibleIndex = 2;
        // 
        // destinationColumn
        // 
        destinationColumn.Caption = "Destination";
        destinationColumn.FieldName = "Destination";
        destinationColumn.Name = "destinationColumn";
        destinationColumn.Visible = true;
        destinationColumn.VisibleIndex = 3;
        // 
        // weightColumn
        // 
        weightColumn.Caption = "Weight";
        weightColumn.FieldName = "Weight";
        weightColumn.Name = "weightColumn";
        weightColumn.Visible = true;
        weightColumn.VisibleIndex = 4;
        // 
        // statusColumn
        // 
        statusColumn.Caption = "Status";
        statusColumn.FieldName = "Status";
        statusColumn.Name = "statusColumn";
        statusColumn.Visible = true;
        statusColumn.VisibleIndex = 5;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 611);
        Controls.Add(mainTableLayoutPanel);
        MinimumSize = new Size(1000, 550);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "CargoFlow";
        ((System.ComponentModel.ISupportInitialize)shipmentGridControl).EndInit();
        ((System.ComponentModel.ISupportInitialize)shipmentGridView).EndInit();
        mainTableLayoutPanel.ResumeLayout(false);
        shipmentsGroupBox.ResumeLayout(false);
        shipmentsTableLayoutPanel.ResumeLayout(false);
        shipmentActionsFlowLayoutPanel.ResumeLayout(false);
        customersGroupBox.ResumeLayout(false);
        customersGroupBox.PerformLayout();
        newShipmentGroupBox.ResumeLayout(false);
        newShipmentGroupBox.PerformLayout();
        ResumeLayout(false);
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
    private Button startTransitButton;
    private Button markAsDeliveredButton;
    private Button cancelShipmentButton;
    private Button createCustomerButton;
    private Label customerNameLabel;
    private TextBox customerNameTextBox;
    private ComboBox customerComboBox;
    private DevExpress.XtraGrid.GridControl shipmentGridControl;
    private DevExpress.XtraGrid.Views.Grid.GridView shipmentGridView;
    private TableLayoutPanel mainTableLayoutPanel;
    private GroupBox customersGroupBox;
    private GroupBox newShipmentGroupBox;
    private GroupBox shipmentsGroupBox;
    private TableLayoutPanel shipmentsTableLayoutPanel;
    private FlowLayoutPanel shipmentActionsFlowLayoutPanel;
    private DevExpress.XtraGrid.Columns.GridColumn idColumn;
    private DevExpress.XtraGrid.Columns.GridColumn customerColumn;
    private DevExpress.XtraGrid.Columns.GridColumn originColumn;
    private DevExpress.XtraGrid.Columns.GridColumn destinationColumn;
    private DevExpress.XtraGrid.Columns.GridColumn weightColumn;
    private DevExpress.XtraGrid.Columns.GridColumn statusColumn;
}
