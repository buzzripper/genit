using EdmlModelReader;


namespace WinFormsApp1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			lbxEntities.Items.Clear();

			// Read from file path
			var model = EdmlReader.Read(@"D:\Code\buzzripper\genit\.SolutionItems\App1Model.edml");

			//// Entities
			//foreach (var entity in model.Entities)
			//{
			//	lbxEntities.Items.Add($"{entity.Name} - InclRowVersion: {entity.InclRowVersion} - Auditable:{entity.Auditable} - Gen: {entity.GenerateCode}");
			//	foreach (var prop in entity.Properties)
			//	{
			//		lbxEntities.Items.Add($"   {prop.Name} ({prop.Type})");
			//	}
			//}

			// Enums
			foreach (var @enum in model.Enums)
			{
				lbxEntities.Items.Add($"{@enum.Name} - NS: {@enum.Namespace} - Gen: {@enum.GenerateCode} - Ext: {@enum.IsExternal} - Flags: {@enum.IsFlags}");
				foreach (var member in @enum.Members)
				{
					lbxEntities.Items.Add($"   {member}");
				}
			}

		}
	}
}
