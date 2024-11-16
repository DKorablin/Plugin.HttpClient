using System;
using Plugin.HttpClient.Project;
using AlphaOmega.Windows.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace Plugin.HttpClient.UI
{
	/// <summary>Узел дерева тестовой ноды проекта</summary>
	internal class ProjectTreeNode : TreeNode2<ProjectTreeNode.TagExtender>
	{
		/// <summary>Расширение объекта с тегом</summary>
		[Serializable]
		internal class TagExtender
		{
			/// <summary>Тестовая нода проекта</summary>
			public HttpProjectItem Project { get; set; }

			/// <summary>Последний ответ от теста</summary>
			public String HttpResponse { get; set; }
		}

		/// <summary>Родительский тест в дереве</summary>
		public new ProjectTreeNode Parent => (ProjectTreeNode)base.Parent;

		/// <summary>Индекс изображения статуса теста</summary>
		public new NodeStateEnum SelectedImageIndex
		{
			get => (NodeStateEnum)base.SelectedImageIndex;
			set => base.SelectedImageIndex = (Int32)value;
		}

		/// <summary>Индекс изображения статуса теста</summary>
		public new NodeStateEnum ImageIndex
		{
			get => (NodeStateEnum)base.ImageIndex;
			set => base.ImageIndex = (Int32)value;
		}

		/// <summary>Проект теста</summary>
		public HttpProjectItem Project
		{
			get => base.Tag.Project;
			set => base.Tag.Project = value;
		}

		/// <summary>Последний ответ на запуск теста</summary>
		public String HttpResponse
		{
			get => base.Tag.HttpResponse;
			set => base.Tag.HttpResponse = value;
		}

		/// <summary>Создать экземпляра узла дерева с указанием объекта теста</summary>
		/// <param name="item">Указатель на объект теста</param>
		public ProjectTreeNode(HttpProjectItem item)
			: this(item.AddressReal)
		{
			this.Project = item;

			foreach(HttpProjectItem child in item.Items)
			{
				ProjectTreeNode node = new ProjectTreeNode(child);
				base.Nodes.Add(node);
			}
		}

		/// <summary>Создать экзепляр узла дерева с ссылкой на сервер</summary>
		/// <param name="text">Ссылка на сервер для теста</param>
		public ProjectTreeNode(String text)
		{
			this.SetNodeText(text);
			base.Tag = new TagExtender();
			this.SetImageIndex(NodeStateEnum.New);
		}

		/// <summary>Обновить текст узла дерева из объекта теста</summary>
		public void UpdateNodeText()
			=> this.SetNodeText(this.Project.AddressReal);

		/// <summary>Пометить узел как пропущенный</summary>
		public void ToggleSkipImageIndex()
			=> this.SetImageIndex(this.ImageIndex == NodeStateEnum.Skip ? NodeStateEnum.New : NodeStateEnum.Skip);

		/// <summary>Установить индекс изображения</summary>
		/// <param name="icon">Тип картинки, который установить на узел</param>
		public void SetImageIndex(NodeStateEnum icon)
			=> this.SelectedImageIndex = this.ImageIndex = icon;

		private void SetNodeText(String text)
		{
			if(text?.Length > HttpTestsTreeView.MaxTextLength)
			{
				base.Text = text.Substring(0, HttpTestsTreeView.MaxTextLength);
				base.ForeColor = Color.Gray;
			} else
			{
				base.Text = text;
				base.ForeColor = Form.DefaultForeColor;
			}
		}
	}
}