using System;
using System.Collections.Generic;

namespace Product {
	public interface ILanguageService {
		InputLayout GetCurrentLayout();
		IntPtr GetCurrentLayoutHandle();
		IList<InputLayout> GetInputLayouts();

		void ConductSwitch(KeyboardSwitch keyboardSwitch);
		void SetCurrentLayout(InputLayout layout);
		void SetCurrentLanguage(string languageId, bool restoreLastUsedLayout);
	}
}