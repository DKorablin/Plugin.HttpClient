namespace Plugin.HttpClient.UI
{
	/// <summary>Изображение узлов дерева</summary>
	internal enum NodeStateEnum
	{
		/// <summary>Новый узел по которому ещё тест ещё не произведён</summary>
		New = 0,
		/// <summary>В работе</summary>
		Running = 1,
		/// <summary>Тест закончился удачей</summary>
		Success = 2,
		/// <summary>Тест провален</summary>
		Failure = 3,
		/// <summary>Тест провален. Запрещено</summary>
		FailureForbidden = 4,
		/// <summary>Папки в тестах не реализованы</summary>
		Folder = 5,
		/// <summary>Пропуск теста при запуске тестирования потока или всех URL'ов</summary>
		Skip = 6,
	}
}