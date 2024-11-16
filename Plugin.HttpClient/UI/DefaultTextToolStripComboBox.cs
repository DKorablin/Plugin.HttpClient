using System;
using System.Drawing;
using System.Windows.Forms;

namespace Plugin.HttpClient.UI
{
	internal class DefaultTextToolStripComboBox : ToolStripComboBox
	{
		public static readonly Color DefaultColor = Color.Gray;
		private String _defaultText;
		private String _placeHolderText;

		public String DefaultText
		{
			get => this._defaultText;
			set => this.Text = this._defaultText = value;
		}

		public String PlaceHolderText
		{
			get => this._placeHolderText;
			set => this.DefaultText = this._placeHolderText = value;
		}

		public override String Text
		{
			get => base.Text == this.PlaceHolderText ? String.Empty : base.Text;
			set
			{
				base.Text = value;
				if(base.Text.Length == 0 && this.PlaceHolderText != null)
					base.Text = this.PlaceHolderText;

				this.SetDefaultColor();
			}
		}

		public Boolean IsEmpty
		{
			get
			{
				if(base.ForeColor == DefaultTextToolStripComboBox.DefaultColor)
					return true;
				else if(base.Focused && base.Text.Length == 0)
					return true;
				else if(base.Text == this.PlaceHolderText)
					return true;
				else return false;
			}
		}

		public DefaultTextToolStripComboBox()
			=> base.MaxLength = Int32.MaxValue;

		protected override void OnLostFocus(EventArgs e)
		{
			if(base.Text.Length == 0 && this.PlaceHolderText != null)
				base.Text = this.PlaceHolderText;

			this.SetDefaultColor();
			base.OnLostFocus(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.ForeColor = Color.Empty;
			if(base.Text == this.PlaceHolderText)
				base.Text = String.Empty;

			base.OnGotFocus(e);
		}

		private void SetDefaultColor()
		{
			base.ForeColor = base.Text == this.PlaceHolderText
				? DefaultTextToolStripComboBox.DefaultColor
				: Color.Empty;
		}
	}
}