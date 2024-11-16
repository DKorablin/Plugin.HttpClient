using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Test
{
	internal class RequestTemplateBuilder : TemplateEngine
	{
		private readonly TemplateItem[] _templateValues;

		public override System.String SelectedTemplate => Constant.Project.DefaultTemplateName;

		public override TemplateItem[] SelectedTemplateValues => this._templateValues;

		public RequestTemplateBuilder(TemplateItem[] templateValues)
			: base(null)
		{
			this._templateValues = templateValues;
		}
	}
}