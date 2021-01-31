using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Product.Common;

namespace Product.SettingsUi {
	public class AttachmentViewModel : INotifyPropertyChanged {

		private Regex acceptableMask = new Regex(@"[0-9+\-\(\)A-Za-z\s]");

		private string _appTitleMask;
		public string AppTitleMask {
			get => _appTitleMask;
			set {
				if (value != _appTitleMask) {
					var matches = acceptableMask.Matches(value);
					var cleanedValue = string.Join("", matches.Cast<Match>().Select(x => x.Value));
					_appTitleMask = cleanedValue;
					RaisePropertyChanged(x => x.AppTitleMask);
				}
			}
		}

		private string _languageOrLayoutId;
		public string LanguageOrLayoutId {
			get => _languageOrLayoutId;
			set {
				if (value != _languageOrLayoutId) {
					_languageOrLayoutId = value;
					RaisePropertyChanged(x => x.LanguageOrLayoutId);
				}
			}
		}

		private IList<LanguageOrLayoutViewModel> _languageOrLayoutList;
		public IList<LanguageOrLayoutViewModel> LanguageOrLayoutList {
			get => _languageOrLayoutList;
			set {
				if (value != _languageOrLayoutList) {
					_languageOrLayoutList = value;
					RaisePropertyChanged(x => x.LanguageOrLayoutList);
				}
			}
		}

		public AttachmentViewModel(string mask, string languageOrLayoutId) {
			_appTitleMask = mask;
			_languageOrLayoutId = languageOrLayoutId;
			var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
			_languageOrLayoutList = languageService
				.GetInputLayouts()
				.Aggregate(new List<LanguageOrLayoutViewModel>(), (List<LanguageOrLayoutViewModel> acc, InputLayout layout) => {
					if (acc.FirstOrDefault(x => x.LanguageOrLayoutId == layout.LanguageId) == null) {
						acc.Add(new LanguageOrLayoutViewModel((InputLanguage) layout));
					}
					acc.Add(new LanguageOrLayoutViewModel(layout));
					return acc;
				})
				.ToList();
		}

		private void RaisePropertyChanged<T1>(Expression<Func<AttachmentViewModel, T1>> expression) {
			RaisePropertyChanged(ExpressionUtils.GetMemberName(expression));
		}

		private void RaisePropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
